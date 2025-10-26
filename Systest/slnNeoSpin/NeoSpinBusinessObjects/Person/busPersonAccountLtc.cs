#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountLtc : busPersonAccount
    {
        private bool _iblnMemberEnrolled;
        public bool iblnMemberEnrolled
        {
            get
            {
                DataTable ldtbMember = busNeoSpinBase.Select<cdoPersonAccountLtcOption>(new string[2] { "person_account_id", "ltc_relationship_value" },
                                        new object[2] { icdoPersonAccount.person_account_id, busConstant.PersonAccountLtcRelationShipMember }, null, null);
                if (ldtbMember.Rows.Count > 0)
                {
                    _iblnMemberEnrolled = true;
                }
                else
                {
                    _iblnMemberEnrolled = false;
                }
                return _iblnMemberEnrolled;
            }
            set { _iblnMemberEnrolled = value; }
        }

        private decimal _idecTotalMonthlyPremium;
        public decimal idecTotalMonthlyPremium
        {
            get { return _idecTotalMonthlyPremium; }
            set { _idecTotalMonthlyPremium = value; }
        }

        private busPerson _ibusSpousePerson;
        public busPerson ibusSpousePerson
        {
            get { return _ibusSpousePerson; }
            set { _ibusSpousePerson = value; }
        }

        private int _iintMemberAge;
        public int iintMemberAge
        {
            get { return _iintMemberAge; }
            set { _iintMemberAge = value; }
        }

        private int _iintSpousePERSLinkID;
        public int iintSpousePERSLinkID
        {
            get { return _iintSpousePERSLinkID; }
            set { _iintSpousePERSLinkID = value; }
        }

        private int _iintSpouseAge;
        public int iintSpouseAge
        {
            get { return _iintSpouseAge; }
            set { _iintSpouseAge = value; }
        }

        private Collection<busPersonAccountLtcOption> _iclbLtcOptionMember;
        public Collection<busPersonAccountLtcOption> iclbLtcOptionMember
        {
            get { return _iclbLtcOptionMember; }
            set { _iclbLtcOptionMember = value; }
        }

        private Collection<busPersonAccountLtcOption> _iclbLtcOptionSpouse;
        public Collection<busPersonAccountLtcOption> iclbLtcOptionSpouse
        {
            get { return _iclbLtcOptionSpouse; }
            set { _iclbLtcOptionSpouse = value; }
        }

        private Collection<busPersonAccountLtcOptionHistory> _iclbLtcHistory;
        public Collection<busPersonAccountLtcOptionHistory> iclbLtcHistory
        {
            get { return _iclbLtcHistory; }
            set { _iclbLtcHistory = value; }
        }

        private Collection<busPersonAccountLtcOptionHistory> _iclbPreviousHistory;
        public Collection<busPersonAccountLtcOptionHistory> iclbPreviousHistory
        {
            get { return _iclbPreviousHistory; }
            set { _iclbPreviousHistory = value; }
        }

        private Collection<busPersonAccountLtcOption> _iclbLtcOptionModified;
        public Collection<busPersonAccountLtcOption> iclbLtcOptionModified
        {
            get { return _iclbLtcOptionModified; }
            set { _iclbLtcOptionModified = value; }
        }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        private bool iblnSpouseEnrollment { get; set; }//PIR 7408
        private bool iblnMemberEnrollment { get; set; }//PIR 7408
        public DataTable idtbCachedLtcRate { get; set; }
        public int iintOldPayeeAccountID { get; set; }
        public string istrAllowOverlapHistory { get; set; }
        public void LoadLtcOptionHistory(bool ablnLoadOtherObjects = true)
        {
            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountLtcOptionHistory.LoadLtcHistory",
             new object[1] { icdoPersonAccount.person_account_id });
            _iclbLtcHistory = GetCollection<busPersonAccountLtcOptionHistory>(ldtbltc, "icdoPersonAccountLtcOptionHistory");
            if (ablnLoadOtherObjects)
            {
                foreach (busPersonAccountLtcOptionHistory lobjLtcHistory in _iclbLtcHistory)
                {
                    lobjLtcHistory.LoadLtcOption();
                    lobjLtcHistory.LoadPersonAccount();
                    lobjLtcHistory.LoadPlan();
                }
            }
        }

        public void LoadLtcOptionNewMember()
        {

            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadLtcOptionNewMember",
             new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionMember = GetCollection<busPersonAccountLtcOption>(ldtbltc, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                lobjLtcOption.icdoPersonAccountLtcOption.person_id = icdoPersonAccount.person_id;
            }
        }

        public void LoadLtcOptionNewSpouse()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusSpouse == null)
                ibusPerson.LoadSpouse();
            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadLtcOptionNewSpouse",
                                                                                    new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionSpouse = GetCollection<busPersonAccountLtcOption>(ldtbltc, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
            {
                lobjLtcOption.icdoPersonAccountLtcOption.person_id = ibusPerson.ibusSpouse.icdoPerson.person_id;
                _iintSpousePERSLinkID = lobjLtcOption.icdoPersonAccountLtcOption.person_id;
            }
        }

        public void LoadLtcOptionUpdateMember()
        {
            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadLtcOptionUpdateMember",
             new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionMember = GetCollection<busPersonAccountLtcOption>(ldtbltc, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                if (lobjLtcOption.icdoPersonAccountLtcOption.person_id == 0)
                    lobjLtcOption.icdoPersonAccountLtcOption.person_id = icdoPersonAccount.person_id;
            }
        }

        public void LoadLtcOptionUpdateMemberFromHistory(DateTime adtEffectiveDate)
        {
            if (iclbLtcOptionMember == null)
                LoadLtcOptionUpdateMember();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory(false);

            var lenuList = iclbLtcHistory.Where(i => i.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipMember);

            foreach (busPersonAccountLtcOptionHistory lbusHistory in lenuList)
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    if ((lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value) &&
                        (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date,
                        lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)) &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date) //ignore same dated records
                    {
                        lobjLtcOption.icdoPersonAccountLtcOption.person_account_id = icdoPersonAccount.person_account_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_id = lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value = lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_id = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_id = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_id = lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value = lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date = lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date;
                        lobjLtcOption.icdoPersonAccountLtcOption.effective_end_date = lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date;
                        lobjLtcOption.icdoPersonAccountLtcOption.person_id = lbusHistory.icdoPersonAccountLtcOptionHistory.person_id;
                        break;
                    }
                }
            }
        }

        public void LoadLtcOptionUpdateSpouse()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusSpouse == null)
                ibusPerson.LoadSpouse();

            DataTable ldtbltc = busNeoSpinBase.Select("cdoPersonAccountLtcOption.LoadLtcOptionUpdateSpouse",
             new object[1] { icdoPersonAccount.person_account_id });
            iclbLtcOptionSpouse = GetCollection<busPersonAccountLtcOption>(ldtbltc, "icdoPersonAccountLtcOption");
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
            {
                lobjLtcOption.icdoPersonAccountLtcOption.person_id = ibusPerson.ibusSpouse.icdoPerson.person_id;
                _iintSpousePERSLinkID = lobjLtcOption.icdoPersonAccountLtcOption.person_id;
            }
        }

        public void LoadLtcOptionUpdateSpouseFromHistory(DateTime adtEffectiveDate)
        {
            if (iclbLtcOptionSpouse == null)
                LoadLtcOptionUpdateSpouse();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory(false);

            var lenuList = iclbLtcHistory.Where(i => i.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse);

            foreach (busPersonAccountLtcOptionHistory lbusHistory in lenuList)
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                {
                    if ((lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value) &&
                        (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date,
                        lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)) &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled &&
                        lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date) //ignore same dated records
                    {
                        lobjLtcOption.icdoPersonAccountLtcOption.person_account_id = icdoPersonAccount.person_account_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_id = lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value = lbusHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_id = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_id = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value = lbusHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_id = lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_id;
                        lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value = lbusHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value;
                        lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date = lbusHistory.icdoPersonAccountLtcOptionHistory.effective_start_date;
                        lobjLtcOption.icdoPersonAccountLtcOption.effective_end_date = lbusHistory.icdoPersonAccountLtcOptionHistory.effective_end_date;
                        lobjLtcOption.icdoPersonAccountLtcOption.person_id = lbusHistory.icdoPersonAccountLtcOptionHistory.person_id;
                        break;
                    }
                }
            }
        }

        public void LoadSpouse(int aintPersonID)
        {
            if (ibusSpousePerson == null)
            {
                ibusSpousePerson = new busPerson();
            }
            ibusSpousePerson.FindPerson(aintPersonID);
        }

        public void LoadMemberAge()
        {
            LoadMemberAge(DateTime.Now);
        }

        public void LoadMemberAge(DateTime adtEffectiveDate)
        {
            if (ibusPerson == null)
                LoadPerson();

            _iintMemberAge = busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, adtEffectiveDate);
        }

        public void LoadSpouseAge(int aintPersonID)
        {
            LoadSpouseAge(aintPersonID, DateTime.Now);
        }

        public void LoadSpouseAge(int aintPersonID, DateTime adtEffectiveDate)
        {
            if (ibusSpousePerson == null)
                LoadSpouse(aintPersonID);

            _iintSpouseAge = busGlobalFunctions.CalulateAge(ibusSpousePerson.icdoPerson.date_of_birth, adtEffectiveDate);
        }

        //PIR 7408 the old contents of this method are avaialble in IsMemberEnrollment() and IsSpouseEnrollment()
        public bool CheckAtleastOneLevelOfCoverageSelectedForMember()
        {
            if (IsMemberEnrollment() || IsSpouseEnrollment())
                return true;
            return false;
        }

        public bool IsMandatoryFieldsEnteredForLoc()
        {
            if (!CheckAllLOCSelectedForMemberOrSpouse())
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived)
                        {
                            if (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value == null)
                            {
                                return true;
                            }
                        }
                    }
                }
                if (iblnMemberEnrolled)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                        {
                            if (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != busConstant.PlanOptionStatusValueWaived)
                            {
                                if (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value == null)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool IsStartDateNotEnteredForLoc()
        {
            if (!CheckAllLOCSelectedForMemberOrSpouse())
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    if ((lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value != null) ||
                        (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != null))
                    {
                        if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date == DateTime.MinValue)
                        {
                            return true;
                        }
                    }
                }
                if (iblnMemberEnrolled)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                    {
                        if ((lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value != null) ||
                            (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != null))
                        {
                            if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date == DateTime.MinValue)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckAllLOCSelectedForMemberOrSpouse()
        {
            int lintLOCCountMember = 0, lintLOCCountSpouse = 0, lintLOCCount = 2;
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                if ((lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue) ||
                    (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value != null) ||
                    (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != null))
                {
                    lintLOCCountMember++;
                }
            }
            if (iblnMemberEnrolled)
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                {
                    if ((lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue) ||
                        (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value != null) ||
                        (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != null))
                    {
                        lintLOCCountSpouse++;
                    }
                }
            }
            if ((lintLOCCountMember == lintLOCCount) || (lintLOCCountSpouse == lintLOCCount))
            {
                return true;
            }
            return false;
        }

        //systest PIR - 2055
        public bool IsSouseNotHavingPerslinkID()
        {
            var lintSpouseWithPerslinkCount = iclbLtcOptionSpouse.Where(lobjLTCOption => lobjLTCOption.icdoPersonAccountLtcOption.person_id == 0
                && lobjLTCOption.icdoPersonAccountLtcOption.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse
                && lobjLTCOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue).Count();
            if (lintSpouseWithPerslinkCount > 0)
                return true;
            return false;
        }

        private void InsertHistory(busPersonAccountLtcOption aobjLtcOption, bool ablnIsOptionEnteredFirstTime)
        {
            cdoPersonAccountLtcOptionHistory lobjcdoLtcOptionhistory = new cdoPersonAccountLtcOptionHistory();
            lobjcdoLtcOptionhistory.person_account_ltc_option_id = aobjLtcOption.icdoPersonAccountLtcOption.person_account_ltc_option_id;
            lobjcdoLtcOptionhistory.plan_option_status_id = aobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_id;
            // PIR 9115
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                DBFunction.DBNonQuery("cdoPersonAccountLtcOptionHistory.UpdateReportGeneratedFlag", new object[1] { aobjLtcOption.icdoPersonAccountLtcOption.person_account_ltc_option_id },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            lobjcdoLtcOptionhistory.plan_option_status_value = aobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value;
            lobjcdoLtcOptionhistory.ltc_relationship_id = aobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_id;
            lobjcdoLtcOptionhistory.ltc_relationship_value = aobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value;
            lobjcdoLtcOptionhistory.level_of_coverage_id = aobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_id;
            lobjcdoLtcOptionhistory.level_of_coverage_value = aobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value;
            lobjcdoLtcOptionhistory.ltc_insurance_type_id = aobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_id;
            lobjcdoLtcOptionhistory.ltc_insurance_type_value = aobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value;
            if (ablnIsOptionEnteredFirstTime)
                lobjcdoLtcOptionhistory.effective_start_date = aobjLtcOption.icdoPersonAccountLtcOption.effective_start_date;
            else
                lobjcdoLtcOptionhistory.effective_start_date = icdoPersonAccount.history_change_date;

            lobjcdoLtcOptionhistory.person_id = aobjLtcOption.icdoPersonAccountLtcOption.person_id;
            lobjcdoLtcOptionhistory.start_date = icdoPersonAccount.history_change_date;
            lobjcdoLtcOptionhistory.plan_participation_status_id = icdoPersonAccount.plan_participation_status_id;
            lobjcdoLtcOptionhistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjcdoLtcOptionhistory.status_id = icdoPersonAccount.status_id;
            lobjcdoLtcOptionhistory.status_value = icdoPersonAccount.status_value;
            lobjcdoLtcOptionhistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjcdoLtcOptionhistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjcdoLtcOptionhistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjcdoLtcOptionhistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjcdoLtcOptionhistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjcdoLtcOptionhistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjcdoLtcOptionhistory.reason_id = icdoPersonAccount.reason_id;
            lobjcdoLtcOptionhistory.reason_value = icdoPersonAccount.reason_value;
            lobjcdoLtcOptionhistory.Insert();
        }

        public void ProcessHistory()
        {
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
            {
                //Remove the Overlapping History
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountLtcOptionHistory lbusPALtcHistory in iclbOverlappingHistory)
                    {
                        lbusPALtcHistory.Delete();
                    }
                }

                if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.

                if (_iclbLtcOptionModified != null)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in _iclbLtcOptionModified)
                    {
                        busPersonAccountLtcOptionHistory lobjOptionHistory = GetPreviousHistoryForOption(lobjLtcOption);

                        if (lobjOptionHistory.icdoPersonAccountLtcOptionHistory.person_account_ltc_option_history_id > 0)
                        {
                            if (lobjOptionHistory.icdoPersonAccountLtcOptionHistory.effective_start_date == icdoPersonAccount.history_change_date)
                            {
                                lobjOptionHistory.icdoPersonAccountLtcOptionHistory.effective_end_date = icdoPersonAccount.history_change_date;
                                // Set flag to 'Y' so that ESS Benefit Enrollment report will ignore these records
                                lobjOptionHistory.icdoPersonAccountLtcOptionHistory.is_enrollment_report_generated = busConstant.Flag_Yes;
                            }
                            else
                            {
                                lobjOptionHistory.icdoPersonAccountLtcOptionHistory.effective_end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                            }
                            lobjOptionHistory.icdoPersonAccountLtcOptionHistory.Update();
                        }

                        if (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value.IsNotNullOrEmpty())
                        {
                            bool lblnIsOptionEnteredFirstTime = false;
                            if (lobjOptionHistory.icdoPersonAccountLtcOptionHistory.person_account_ltc_option_history_id == 0)
                                lblnIsOptionEnteredFirstTime = true;
                            InsertHistory(lobjLtcOption, lblnIsOptionEnteredFirstTime);
                        }

                        // PIR 9115
                        busPersonAccountLtcOptionHistory lobjOptionHistoryOther = GetPreviousHistoryForOptionNotModified(lobjLtcOption);
                        if (lobjOptionHistoryOther.icdoPersonAccountLtcOptionHistory.person_account_ltc_option_history_id > 0)
                        {
                            lobjOptionHistoryOther.icdoPersonAccountLtcOptionHistory.is_enrollment_report_generated = busConstant.Flag_No;
                            lobjOptionHistoryOther.icdoPersonAccountLtcOptionHistory.Update();
                        }
                    }

                    //PIR 1729 :Update the End Date if all the records are end dated and plan participation status is not enrolled
                    icdoPersonAccount.end_date = DateTime.MinValue;
                    if (icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        icdoPersonAccount.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                    }
                    icdoPersonAccount.Update();
                }
            }
        }

        public void GetMonthlyPremiumAmount(DateTime adtEffectiveDate)
        {
            GetMonthlyPremiumAmount(adtEffectiveDate, ibusProviderOrgPlan.icdoOrgPlan.org_plan_id);
        }

        public void GetMonthlyPremiumAmount(DateTime adtEffectiveDate, int aintOrgPlanID)
        {
            idecTotalMonthlyPremium = 0.00M;
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                busPersonAccountLtcOptionHistory lobjPALtcHistory = LoadHistoryByDate(lobjLtcOption, adtEffectiveDate);

                if ((lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value.IsNotNullOrEmpty()) &&
                    (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                {
                    //UAT CRITICAL PIR : 2484 Load the Age by Option Effective Start Date -- Temp Fix as satya told.
                    LoadMemberAge(lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date);

                    lobjLtcOption.idecMonthlyPremium =
                    busRateHelper.GetLTCPremiumAmount(aintOrgPlanID, lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value,
                                                      lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value,
                                                      adtEffectiveDate, _iintMemberAge, idtbCachedLtcRate, iobjPassInfo);
                    idecTotalMonthlyPremium += lobjLtcOption.idecMonthlyPremium;
                }
            }

            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
            {
                busPersonAccountLtcOptionHistory lobjPALtcHistory = LoadHistoryByDate(lobjLtcOption, adtEffectiveDate);

                if ((lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value.IsNotNullOrEmpty()) &&
                    (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                {
                    //UAT CRITICAL PIR :2484 Load the Age by Option Effective Start Date -- Temp Fix as satya told.
                    if (iintSpousePERSLinkID != 0)
                    {
                        LoadSpouseAge(iintSpousePERSLinkID, lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date);
                    }

                    lobjLtcOption.idecMonthlyPremium =
                    busRateHelper.GetLTCPremiumAmount(aintOrgPlanID, lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value,
                                                      lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value,
                                                      adtEffectiveDate, iintSpouseAge, idtbCachedLtcRate, iobjPassInfo);
                    idecTotalMonthlyPremium += lobjLtcOption.idecMonthlyPremium;
                }
            }
        }

        public void GetMonthlyPremiumAmount()
        {
            idecTotalMonthlyPremium = 0.00M;
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();
            GetMonthlyPremiumAmount(idtPlanEffectiveDate);
        }

        public Collection<busPersonAccountLtcOptionHistory> iclbOverlappingHistory { get; set; }
        private Collection<busPersonAccountLtcOptionHistory> LoadOverlappingHistoryForOption(busPersonAccountLtcOption aobjOption)
        {
            if (iclbLtcHistory == null)
                LoadLtcOptionHistory();
            Collection<busPersonAccountLtcOptionHistory> lclbPALtcHistory = new Collection<busPersonAccountLtcOptionHistory>();
            var lenuList = iclbLtcHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountLtcOptionHistory.effective_start_date, i.icdoPersonAccountLtcOptionHistory.effective_end_date)
                || i.icdoPersonAccountLtcOptionHistory.effective_start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountLtcOptionHistory lobjHistory in lenuList)
            {
                if ((lobjHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == aobjOption.icdoPersonAccountLtcOption.level_of_coverage_value) &&
                    (lobjHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == aobjOption.icdoPersonAccountLtcOption.ltc_relationship_value))
                {
                    if (lobjHistory.icdoPersonAccountLtcOptionHistory.effective_start_date >= icdoPersonAccount.history_change_date)
                    {
                        lclbPALtcHistory.Add(lobjHistory);
                    }
                    else if (lobjHistory.icdoPersonAccountLtcOptionHistory.effective_start_date == lobjHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                    {
                        lclbPALtcHistory.Add(lobjHistory);
                    }
                    else if (lobjHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lobjHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                    {
                        break;
                    }
                }
            }
            return lclbPALtcHistory;
        }

        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    foreach (busPersonAccountLtcOption lbusOption in iclbLtcOptionMember)
                    {
                        var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lbusOption.icdoPersonAccountLtcOption.level_of_coverage_value &&
                            i.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == lbusOption.icdoPersonAccountLtcOption.ltc_relationship_value &&
                            i.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            i.icdoPersonAccountLtcOptionHistory.start_date != i.icdoPersonAccountLtcOptionHistory.end_date);
                        if ((lenuList != null) && (lenuList.Count() > 1))
                            return true;
                    }

                    foreach (busPersonAccountLtcOption lbusOption in _iclbLtcOptionSpouse)
                    {
                        var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lbusOption.icdoPersonAccountLtcOption.level_of_coverage_value &&
                            i.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == lbusOption.icdoPersonAccountLtcOption.ltc_relationship_value &&
                            i.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            i.icdoPersonAccountLtcOptionHistory.start_date != i.icdoPersonAccountLtcOptionHistory.end_date);
                        if ((lenuList != null) && (lenuList.Count() > 1))
                            return true;
                    }
                }
            }
            return false;
        }
        
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //PROD Logic changes. - Delete the Previous open History lines if the Allow Overlap Sets true
            bool lblnReloadPreviousHistory = false;
            iclbOverlappingHistory = new Collection<busPersonAccountLtcOptionHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                //Reload the History Always...
                LoadLtcOptionHistory();
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    lobjLtcOption.iblnOverlapHistoryFound = false;
                    Collection<busPersonAccountLtcOptionHistory> lclbOpenHistory = LoadOverlappingHistoryForOption(lobjLtcOption);
                    if (lclbOpenHistory.Count > 0)
                    {
                        foreach (busPersonAccountLtcOptionHistory lbusPALtcHistory in lclbOpenHistory)
                        {
                            iclbLtcHistory.Remove(lbusPALtcHistory);
                            iclbOverlappingHistory.Add(lbusPALtcHistory);
                            lobjLtcOption.iblnOverlapHistoryFound = true;
                            lblnReloadPreviousHistory = true;
                        }
                    }
                }

                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                {
                    lobjLtcOption.iblnOverlapHistoryFound = false;
                    Collection<busPersonAccountLtcOptionHistory> lclbOpenHistory = LoadOverlappingHistoryForOption(lobjLtcOption);
                    if (lclbOpenHistory.Count > 0)
                    {
                        foreach (busPersonAccountLtcOptionHistory lbusPALtcHistory in lclbOpenHistory)
                        {
                            iclbLtcHistory.Remove(lbusPALtcHistory);
                            iclbOverlappingHistory.Add(lbusPALtcHistory);
                            lobjLtcOption.iblnOverlapHistoryFound = true;
                            lblnReloadPreviousHistory = true;
                        }
                    }
                }
            }

            if (lblnReloadPreviousHistory)
            {
                LoadPreviousHistory();
            }

            LoadPlanEffectiveDate();

            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;

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
            iblnMemberEnrollment = IsMemberEnrollment();//PIR 7408
            iblnSpouseEnrollment = IsSpouseEnrollment();//PIR 7408
            if (icdoPersonAccount.start_date == DateTime.MinValue)
                GetPlanStartDate();

            if (iclbPreviousHistory == null)
                LoadPreviousHistory();

            SetHistoryEntryRequiredOrNot();
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                iblnIsNewMode = true;
                if (ibusPersonEmploymentDetail != null)
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        LoadOrgPlan();
                        LoadProviderOrgPlan();
                    }
            }
            //Function to set bool variable based on any value change in Payment election
            //SetPaymentElectionChangedOrNot();
            base.BeforeValidate(aenmPageMode);
        }
        
        //PIR 7408- Check if the spouse enrollment is valid
        private bool IsSpouseEnrollment()
        {
            bool iblnSpouse = false;
            foreach (busPersonAccountLtcOption lobjLtcOptionSpouce in iclbLtcOptionSpouse)
            {
                if (lobjLtcOptionSpouce.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue &&
                        lobjLtcOptionSpouce.icdoPersonAccountLtcOption.ltc_insurance_type_value != null &&
                        lobjLtcOptionSpouce.icdoPersonAccountLtcOption.plan_option_status_value != null)
                {
                    {
                        iblnSpouse= true;
                        break;
                    }
                }
            }
            if (iblnSpouse && !iblnMemberEnrollment)
            {
                ibusPerson.LoadActivePersonEmployment();
                foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.iclbActivePersonEmployment)
                {
                    lobjPersonEmployment.LoadLatestPersonEmploymentDetail();
                    if (lobjPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //PIR 7408- Check if this is a member enrollment
        private bool IsMemberEnrollment()
        {            
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                if ((lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue) &&
                    (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value != null) &&
                    (lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value != null))
                {
                    return true;
                }
            }
            return false;
        }

        public override void BeforePersistChanges()
        {
            if (ibusProviderOrgPlan != null)
            {
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
                iintOldPayeeAccountID = Convert.ToInt32(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);

            //UAT PIR 2373 people soft file changes
            //--Start--//
            if (icdoPersonAccount.ihstOldValues.Count > 0
                && Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]) != icdoPersonAccount.plan_participation_status_value
                && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
            {
                SetPersonAcccountForTeminationChange();
                SetOptionForTerminationChange();
            }
            else
            {
                SetPersonAccountForEnrollmentChange();
            }
            //--End--//

            if (icdoPersonAccount.ihstOldValues.Count > 0)
                istrPreviousPlanParticipationStatus = Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]);
        }

        /// <summary>
        /// prod pir 4861 : method to set peoplesoft flag when termination happens
        /// </summary>
        private void SetOptionForTerminationChange()
        {
            if (iclbLtcOptionMember == null)
                LoadLtcOptionUpdateMember();
            if (iclbLtcOptionSpouse == null)
                LoadLtcOptionUpdateSpouse();

            foreach (busPersonAccountLtcOption lobjLTCOption in iclbLtcOptionMember)
            {
                if (lobjLTCOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    lobjLTCOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
            }

            foreach (busPersonAccountLtcOption lobjLTCOption in iclbLtcOptionSpouse)
            {
                if (lobjLTCOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    lobjLTCOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
            }
        }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (iclbPreviousHistory == null)
                LoadPreviousHistory();
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
            {
                busPersonAccountLtcOptionHistory lobjPALOH = iclbPreviousHistory
                                    .Where(o => o.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value &&
                                        o.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value)
                                        .FirstOrDefault();
                if (IsHistoryEntryRequired && lobjPALOH != null)
                {
                    //PROD PIR 4586
                    //prod pir 4861 : removal of annual enrollment logic
                    // PROD PIR 7705: Added Annual enrollment logic
                    if (icdoPersonAccount.reason_value == busConstant.ChangeReasonAnnualEnrollment
                        
                        )
                    {
                        icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                        break;
                    }
                    else if (lobjPALOH.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                        icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                        icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                        lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    }
                    else if (lobjPALOH.icdoPersonAccountLtcOptionHistory.effective_start_date != lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date ||
                        lobjPALOH.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value != lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value ||
                        lobjPALOH.icdoPersonAccountLtcOptionHistory.plan_option_status_value != lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value)
                    {
                        icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                        icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                        lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    }
                }
            }
            foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
            {
                busPersonAccountLtcOptionHistory lobjPALOH = iclbPreviousHistory
                                    .Where(o => o.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value &&
                                        o.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value)
                                        .FirstOrDefault();
                if (IsHistoryEntryRequired && lobjPALOH != null)
                {
                    if (lobjPALOH.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                        icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                        icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                        lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    }
                    else if (lobjPALOH.icdoPersonAccountLtcOptionHistory.effective_start_date != lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date ||
                        lobjPALOH.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value != lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value ||
                        lobjPALOH.icdoPersonAccountLtcOptionHistory.plan_option_status_value != lobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value)
                    {
                        icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                        icdoPersonAccount.ps_file_change_event_value = busConstant.LevelOfCoverageChange;
                        lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    }
                }
            }
        }

        public override int PersistChanges()
        {
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                //PROD PIR 4586
                //prod pir 4861 : removal of annual enrollment logic
                //PIR 7987
                if (icdoPersonAccount.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                }
                else
                {
                    //UAT PIR 2373 people soft file changes
                    //--Start--//
                    icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    //--End--//
                }
                
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                icdoPersonAccount.Insert();
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    lobjLtcOption.icdoPersonAccountLtcOption.person_account_id = icdoPersonAccount.person_account_id;
                    lobjLtcOption.icdoPersonAccountLtcOption.Insert();
                }
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                {
                    lobjLtcOption.icdoPersonAccountLtcOption.people_soft_file_sent_flag = busConstant.Flag_No;
                    lobjLtcOption.icdoPersonAccountLtcOption.person_account_id = icdoPersonAccount.person_account_id;
                    lobjLtcOption.icdoPersonAccountLtcOption.Insert();
                }
            }
            else
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    {
                        lobjLtcOption.icdoPersonAccountLtcOption.Update();
                    }
                }
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                {
                    if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    {
                        lobjLtcOption.icdoPersonAccountLtcOption.Update();
                    }
                }
                //PIR 7987
                if (icdoPersonAccount.reason_value == busConstant.ChangeReasonAnnualEnrollment
                    && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollment;
                    else//PIR 7987
                        icdoPersonAccount.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                }
            }
            //Payment election may not be inserted in New mode, so we need to insert in update mode too.
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                ibusPaymentElection.icdoPersonAccountPaymentElection.Update();
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.person_account_id = icdoPersonAccount.person_account_id;
                ibusPaymentElection.icdoPersonAccountPaymentElection.Insert();
            }
            return base.PersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ProcessHistory();

            // PROD PIR ID 7408 -- The User clicks save twice with Suppress warning in second time. The options collection was not refreshed earlier.
            LoadLtcOptionUpdateMember(); 
            LoadLtcOptionUpdateSpouse();

            LoadLtcOptionHistory();
            GetMonthlyPremiumAmount();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadPreviousHistory();

            //Creating Payroll / IBS Adjustment Record If Enrollment Changes Occurs
            if (IsHistoryEntryRequired && icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
            {
                CreateAdjustmentPayrollForEnrollmentHistoryClose();
                //PIR : 2029 Do not create Positive Adjustment for Suspended Status
                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    CreateAdjustmentPayrollForEnrollmentHistoryAdd();
            }

            // PROD PIR ID 4735
            //PIR 7535
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled &&
               istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                ibusPaymentElection.EndPAPITEntriesForSuspendedAccount(icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date);
            }
            //PAPIT entries should only be updated when the IBS Adjustment is posted. //PIR 16393 - Wrong insurance deduction
            else if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                    //prod pir 5538 : need to recalculate premium as of latest date
                    //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));
                        LoadProviderOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));
                    }
                    else
                    {
                        LoadActiveProviderOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));
                    }
                    //Recalculate the Premium Based on the New Effective Date
                    GetMonthlyPremiumAmount(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));

                    // Change 1: UAT 1452 : need to end date PAPIT entries if at all in review status as we are saving payment election
                    // Change 2: Modified by Elayaraja - PIR 1488 deduction amount should be equal to premium amount - RHIC benefit amount
                    //             *** BR-074-12 *** System must transfer insurance premium information to the Payee Account's deduction list,
                    //             when insurance premium information is updated by the enrollment
                    // Change 3: Earlier this will update only if there exists changes in Payment Election fields. Modified now to update by all time.
                    if (iintOldPayeeAccountID != 0 && iintOldPayeeAccountID != ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id)
                    {
                        ibusPaymentElection.ManagePayeeAccountPaymentItemType(ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                                iintOldPayeeAccountID, idecTotalMonthlyPremium, icdoPersonAccount.plan_id,
                                                GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                                idtNextBenefiPaymentDate), ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, ibusProviderOrgPlan.icdoOrgPlan.org_id, ablnIsPayeeAccountChanged: true);
                    }
                //prod pir 5538 : need to recalculate premium as on latest date
                RecalculatePremiumBasedOnPlanEffectiveDate();
            }
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            LoadAllPersonEmploymentDetails();

            //uat pir 2220
            if ((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid) && (IsHistoryEntryRequired))
            {
                PostESSMessage();
            }
        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
            icdoPersonAccount.reason_value = string.Empty; // UAT PIR ID 1043
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryClose()
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = icdoPersonAccount.history_change_date;
            DateTime ldatEndDate = icdoPersonAccount.history_change_date.AddDays(-1);
            decimal ldecOldPremium = 0.00M;
            decimal ldecOldMember3YrsPremium = 0.00M;
            decimal ldecOldMember5YrsPremium = 0.00M;
            decimal ldecOldSpouse3YrsPremium = 0.00M;
            decimal ldecOldSpouse5YrsPremium = 0.00M;
            //ucs - 038 addendum : new col. for paid premium amount
            decimal ldecPaidPremiumAmount = 0.0M;
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuIBSContributionByMonth =
                iclbInsuranceContributionAll.Where(
                    i =>
                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit).
                    GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());
            if (lenuIBSContributionByMonth != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuIBSContributionByMonth)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    {
                        if ((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit) &&
                            ((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount > 0) ||
                             (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount > 0) ||
                             (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount > 0))
                            //Some times full RHIC goes to premium so due premium amount is zero but we need to create nagative entries when the RHIC Changes happens
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

                            //For Negative Adjustment Premoum Amount, we should not only consider Regular.. we also need to consider other adjustment too
                            //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                            //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.                            
                            var lclbIBSFilterdContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                            //ucs - 038 addendum, new col. for paid premium amount
                            var lclbIBSFilterdPaidContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                            ldecOldMember3YrsPremium = lclbIBSFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_member_three_yrs_premium_amount);
                            ldecOldMember5YrsPremium = lclbIBSFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_member_five_yrs_premium_amount);
                            ldecOldSpouse3YrsPremium = lclbIBSFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_spouse_three_yrs_premium_amount);
                            ldecOldSpouse5YrsPremium = lclbIBSFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_spouse_five_yrs_premium_amount);
                            ldecOldPremium = ldecOldMember3YrsPremium + ldecOldMember5YrsPremium + ldecOldSpouse3YrsPremium + ldecOldSpouse5YrsPremium;
                            ldecPaidPremiumAmount = 0.0M;

                            //ucs - 038 addendum: added new col. paid premium amount
                            if (lclbIBSFilterdPaidContribution != null)
                                ldecPaidPremiumAmount = lclbIBSFilterdPaidContribution.Sum(o => o.icdoPersonAccountInsuranceContribution.paid_premium_amount);
                            //uat pir 1461 --//start
                            string lstrPaymentMethod = string.Empty;
                            //uat pir : 2374 - as part of removal of ibs_effective_Date in payment election
                            //if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date > lbusIBSHdr.icdoIbsHeader.billing_month_and_year)
                            //{
                            //    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                            //}
                            //else 
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
                            //uat pir 1461 --//end
                            if (ldecOldPremium != 0)
                                lbusIBSHdr.AddIBSDetailForLTC(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id,
                                    icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                    lstrPaymentMethod,
                                -1M * ldecOldPremium, -1M * ldecOldMember3YrsPremium, -1M * ldecOldMember5YrsPremium, -1M * ldecOldSpouse3YrsPremium, -1M * ldecOldSpouse5YrsPremium,
                                lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, adecPaidPremiumAmount: -1M * ldecPaidPremiumAmount);//ucs - 038 addendum : added new col. paid premium amount

                        }
                    }
                }
            }
            //prod pir 5831 : need to group IBS and employer reporting as separate
            var lenuPayrollContributionByMonth =
               iclbInsuranceContributionAll.Where(
                   i =>
                   i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting).
                   GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuPayrollContributionByMonth != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuPayrollContributionByMonth)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    {
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
                                //IBS Org ID is not applicable for LTC
                                if (lintOrgID == 0)
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

                            if (lintOrgID > 0)
                            {
                                //For Negative Adjustment Premoum Amount, we should not only consider Regular.. we also need to consider other adjustment too
                                //Example Scenario : Member was in Single First and Then changed Family and then suspended. This case Single - Family Change
                                //Could have created the adjustment record which also we need to consider while creating Neg Adjustment for suspended case.                        
                                var lclbFilterdPayrollContribution = _iclbInsuranceContributionAll.Where(
                                        i =>
                                        i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                                        i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                ldecOldMember3YrsPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_member_three_yrs_premium_amount);
                                ldecOldMember5YrsPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_member_five_yrs_premium_amount);
                                ldecOldSpouse3YrsPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_spouse_three_yrs_premium_amount);
                                ldecOldSpouse5YrsPremium = lclbFilterdPayrollContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.ltc_spouse_five_yrs_premium_amount);
                                ldecOldPremium = ldecOldMember3YrsPremium + ldecOldMember5YrsPremium + ldecOldSpouse3YrsPremium + ldecOldSpouse5YrsPremium;

                                if (ldecOldPremium > 0)
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                        ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium,
                                        lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                        adecLtcMember3YrsPremium: ldecOldMember3YrsPremium, adecLtcMember5YrsPremium: ldecOldMember5YrsPremium,
                                        adecLtcSpouse3YrsPremium: ldecOldSpouse3YrsPremium, adecLtcSpouse5YrsPremium: ldecOldSpouse5YrsPremium);
                                else if (ldecOldPremium < 0)
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                    ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecOldPremium * -1,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                    adecLtcMember3YrsPremium: ldecOldMember3YrsPremium * -1, adecLtcMember5YrsPremium: ldecOldMember5YrsPremium * -1,
                                    adecLtcSpouse3YrsPremium: ldecOldSpouse3YrsPremium * -1, adecLtcSpouse5YrsPremium: ldecOldSpouse5YrsPremium * -1);
                            }
                        }
                    }
                }
            }
            // Update changes
            if ((lbusIBSHdr != null) && (lbusIBSHdr.icolIbsDetail != null) && (lbusIBSHdr.icolIbsDetail.Count > 0))
            {
                lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                foreach (busIbsDetail lbusIBSDtl in lbusIBSHdr.icolIbsDetail)
                {
                    lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0))
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }

        private void CreateAdjustmentPayrollForEnrollmentHistoryAdd()
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = icdoPersonAccount.history_change_date;
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            decimal ldecOldPremium = 0.00M;
            decimal ldecOldMember3YrsPremium = 0.00M;
            decimal ldecOldMember5YrsPremium = 0.00M;
            decimal ldecOldSpouse3YrsPremium = 0.00M;
            decimal ldecOldSpouse5YrsPremium = 0.00M;
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

            //If no contribution record, we must load the current pay period from payroll header / IBS header.
            if (ldatCurrentPayPeriod == DateTime.MinValue)
            {
                //Discussion with Satya : If the new Members enrolled, there wont be any contribution record. To handle such scenario also,
                //we are now taking the current pay pareiod from payroll header instead of last pay period from contrbution table.

                //Pure IBS Member
                if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                   (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0)) //?? For life there may be a split between supplemental / basic
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
                }
                else
                {
                    if (lintOrgID > 0)
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
                }
            }

            if (ldatCurrentPayPeriod == DateTime.MinValue)
            {
                if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                   (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0))
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastIBSPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }
                else
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastEmployerPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }
            }

            while (ldatEffectedDate <= ldatCurrentPayPeriod)
            {
                //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
                if (icdoPersonAccount.person_employment_dtl_id > 0)
                {
                    LoadOrgPlan(ldatEffectedDate);
                    LoadProviderOrgPlan(ldatEffectedDate);
                }
                else
                {
                    LoadActiveProviderOrgPlan(ldatEffectedDate);
                }
                //Recalculate the Premium Based on the New Effective Date
                GetMonthlyPremiumAmount(ldatEffectedDate);

                if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0))
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

                    ldecOldPremium = 0.00M;
                    ldecOldMember3YrsPremium = 0.00M;
                    ldecOldMember5YrsPremium = 0.00M;
                    ldecOldSpouse3YrsPremium = 0.00M;
                    ldecOldSpouse5YrsPremium = 0.00M;
                    GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                 ref ldecOldMember3YrsPremium, ref ldecOldMember5YrsPremium,
                                                 ref ldecOldSpouse3YrsPremium, ref ldecOldSpouse5YrsPremium);
                    //uat pir 1461 --//start
                    string lstrPaymentMethod = string.Empty;
                    //uat pir : 2374 - as part of removal of ibs_effective_Date in payment election
                    //if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date > lbusIBSHdr.icdoIbsHeader.billing_month_and_year)
                    //{
                    //    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    //}
                    //else
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
                    //uat pir 1461 --//end
                    if (ldecOldPremium != 0)
                        lbusIBSHdr.AddIBSDetailForLTC(icdoPersonAccount.person_account_id, icdoPersonAccount.person_id, icdoPersonAccount.plan_id,
                            ldatEffectedDate, lstrPaymentMethod,
                            ldecOldPremium, ldecOldMember3YrsPremium, ldecOldMember5YrsPremium, ldecOldSpouse3YrsPremium, ldecOldSpouse5YrsPremium,
                            ibusProviderOrgPlan.icdoOrgPlan.org_id);
                }
                else
                {
                    // Health/D/V/Life
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
                    if (lintOrgID > 0)
                    {
                        ldecOldPremium = 0.00M;
                        ldecOldMember3YrsPremium = 0.00M;
                        ldecOldMember5YrsPremium = 0.00M;
                        ldecOldSpouse3YrsPremium = 0.00M;
                        ldecOldSpouse5YrsPremium = 0.00M;
                        GetPremiumSplitForHistoryAdd(ref ldecOldPremium,
                                                     ref ldecOldMember3YrsPremium, ref ldecOldMember5YrsPremium,
                                                     ref ldecOldSpouse3YrsPremium, ref ldecOldSpouse5YrsPremium);

                        if (ldecOldPremium > 0)
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                ldatEffectedDate, ldecOldPremium, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                adecLtcMember3YrsPremium: ldecOldMember3YrsPremium, adecLtcMember5YrsPremium: ldecOldMember5YrsPremium,
                                adecLtcSpouse3YrsPremium: ldecOldSpouse3YrsPremium, adecLtcSpouse5YrsPremium: ldecOldSpouse5YrsPremium);
                        else if (ldecOldPremium < 0)
                            lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                                ibusPerson.icdoPerson.last_name, ibusPerson.icdoPerson.ssn, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                ldatEffectedDate, ldecOldPremium * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                adecLtcMember3YrsPremium: ldecOldMember3YrsPremium * -1, adecLtcMember5YrsPremium: ldecOldMember5YrsPremium * -1,
                                adecLtcSpouse3YrsPremium: ldecOldSpouse3YrsPremium * -1, adecLtcSpouse5YrsPremium: ldecOldSpouse5YrsPremium * -1);
                    }
                }
                ldatEffectedDate = ldatEffectedDate.AddMonths(1);
            }
            // Update changes
            if ((lbusIBSHdr != null) && (lbusIBSHdr.icolIbsDetail != null) && (lbusIBSHdr.icolIbsDetail.Count > 0))
            {
                lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                foreach (busIbsDetail lbusIBSDtl in lbusIBSHdr.icolIbsDetail)
                {
                    lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0))
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
            //prod pir 5538 : need to recalculate premium as of latest date
            RecalculatePremiumBasedOnPlanEffectiveDate();
        }

        private void GetPremiumSplitForHistoryAdd(ref decimal adecTotalPremium, ref decimal adecMember3YrsPremium, ref decimal adecMember5YrsPremium,
                                                  ref decimal adecSpouse3YrsPremium, ref decimal adecSpouse5YrsPremium)
        {
            foreach (var lbusLtcOption in iclbLtcOptionMember)
            {
                if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                {
                    adecMember3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                    adecTotalPremium += adecMember3YrsPremium;
                }
                else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                {
                    adecMember5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                    adecTotalPremium += adecMember5YrsPremium;
                }
            }

            foreach (var lbusLtcOption in iclbLtcOptionSpouse)
            {
                if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS)
                {
                    adecSpouse3YrsPremium = lbusLtcOption.idecMonthlyPremium;
                    adecTotalPremium += adecSpouse3YrsPremium;
                }
                else if (lbusLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                {
                    adecSpouse5YrsPremium = lbusLtcOption.idecMonthlyPremium;
                    adecTotalPremium += adecSpouse5YrsPremium;
                }
            }
        }

        //PIR 7408 Proper Date caclulated for spouse enrollment and member enrollment
        private void GetPlanStartDate()
        {
            DateTime ldtStartDate = DateTime.MinValue;
            if (iblnMemberEnrollment)
            {
                foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                {
                    if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    {
                        if (ldtStartDate == DateTime.MinValue)
                        {
                            ldtStartDate = lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date;
                        }
                        else if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date < ldtStartDate)
                        {
                            ldtStartDate = lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date;
                        }
                    }
                }
            }
            else if (iblnSpouseEnrollment)
            {
                foreach (busPersonAccountLtcOption lobjLtcOptionSpouse in iclbLtcOptionSpouse)
                {
                    if (lobjLtcOptionSpouse.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                    {
                        if (ldtStartDate == DateTime.MinValue)
                        {
                            ldtStartDate = lobjLtcOptionSpouse.icdoPersonAccountLtcOption.effective_start_date;
                        }
                        else if (lobjLtcOptionSpouse.icdoPersonAccountLtcOption.effective_start_date < ldtStartDate)
                        {
                            ldtStartDate = lobjLtcOptionSpouse.icdoPersonAccountLtcOption.effective_start_date;
                        }
                    }
                }
            }
            icdoPersonAccount.start_date = ldtStartDate;
        }

        public void LoadPreviousHistory()
        {
            if (_iclbPreviousHistory == null)
                _iclbPreviousHistory = new Collection<busPersonAccountLtcOptionHistory>();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory();

            bool lblnMemberRel3Yrs = false;
            bool lblnMemberRel5Yrs = false;
            bool lblnSpouseRel3Yrs = false;
            bool lblnSpouseRel5Yrs = false;

            foreach (busPersonAccountLtcOptionHistory lbusLtcHistory in iclbLtcHistory)
            {
                if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS) &&
                    (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipMember))
                {
                    if (!lblnMemberRel3Yrs)
                    {
                        iclbPreviousHistory.Add(lbusLtcHistory);
                        lblnMemberRel3Yrs = true;
                    }
                }
                else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS) &&
                   (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipMember))
                {
                    if (!lblnMemberRel5Yrs)
                    {
                        iclbPreviousHistory.Add(lbusLtcHistory);
                        lblnMemberRel5Yrs = true;
                    }
                }
                else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS) &&
                    (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse))
                {
                    if (!lblnSpouseRel3Yrs)
                    {
                        iclbPreviousHistory.Add(lbusLtcHistory);
                        lblnSpouseRel3Yrs = true;
                    }
                }
                else if ((lbusLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS) &&
                   (lbusLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == busConstant.PersonAccountLtcRelationShipSpouse))
                {
                    if (!lblnSpouseRel5Yrs)
                    {
                        iclbPreviousHistory.Add(lbusLtcHistory);
                        lblnSpouseRel5Yrs = true;
                    }
                }

                if (lblnMemberRel3Yrs && lblnMemberRel5Yrs && lblnSpouseRel3Yrs && lblnSpouseRel5Yrs) break;
            }
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_iclbPreviousHistory == null)
                LoadPreviousHistory();

            IsHistoryEntryRequired = false;

            //Clear the Collection If Exists
            _iclbLtcOptionModified = new Collection<busPersonAccountLtcOption>();
            SetIsHistoryRequiredByType(iclbLtcOptionMember);
            SetIsHistoryRequiredByType(iclbLtcOptionSpouse);
        }

        private void SetIsHistoryRequiredByType(Collection<busPersonAccountLtcOption> aclbLtcOption)
        {
            foreach (busPersonAccountLtcOption lobjLtcOption in aclbLtcOption)
            {
                //If Data Entered
                if (lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue)
                {
                    //Check If the History Exists for this Option
                    busPersonAccountLtcOptionHistory lobjPreviousOptionHistory = GetPreviousHistoryForOption(lobjLtcOption);
                    if (lobjPreviousOptionHistory.icdoPersonAccountLtcOptionHistory.person_account_ltc_option_history_id == 0)
                    {
                        IsHistoryEntryRequired = true;
                        _iclbLtcOptionModified.Add(lobjLtcOption);
                        continue;
                    }

                    if (IsMandatoryFieldChanged(lobjLtcOption, lobjPreviousOptionHistory))
                    {
                        IsHistoryEntryRequired = true;
                        _iclbLtcOptionModified.Add(lobjLtcOption);
                        continue;
                    }
                }
            }
        }

        private busPersonAccountLtcOptionHistory GetPreviousHistoryForOption(busPersonAccountLtcOption aobjLtcOption)
        {
            busPersonAccountLtcOptionHistory lobjOptionHistory = new busPersonAccountLtcOptionHistory();
            lobjOptionHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();

            foreach (busPersonAccountLtcOptionHistory lobjLtcHistory in iclbPreviousHistory)
            {
                if ((lobjLtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == aobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value) &&
                   (lobjLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == aobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value))
                {
                    lobjOptionHistory = lobjLtcHistory;
                    break;
                }
            }
            return lobjOptionHistory;
        }

        // PIR 9115
        private busPersonAccountLtcOptionHistory GetPreviousHistoryForOptionNotModified(busPersonAccountLtcOption aobjLtcOption)
        {
            busPersonAccountLtcOptionHistory lobjOptionHistory = new busPersonAccountLtcOptionHistory();
            lobjOptionHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();

            foreach (busPersonAccountLtcOptionHistory lobjLtcHistory in iclbPreviousHistory)
            {
                if (lobjLtcHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value != aobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value)
                {
                    lobjOptionHistory = lobjLtcHistory;
                    break;
                }
            }
            return lobjOptionHistory;
        }

        private bool IsMandatoryFieldChanged(busPersonAccountLtcOption aobjLtcOption, busPersonAccountLtcOptionHistory aobjOptionHistory)
        {
            bool lblnResult = false;

            if ((aobjOptionHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value != icdoPersonAccount.plan_participation_status_value) ||
              (aobjOptionHistory.icdoPersonAccountLtcOptionHistory.plan_option_status_value != aobjLtcOption.icdoPersonAccountLtcOption.plan_option_status_value) ||
               (aobjOptionHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value != aobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value) ||
               (aobjOptionHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value != aobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value)
               )
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsHistoryChangeDateLessThanLastChangeDate()
        {
            bool lblnResult = false;

            if (IsHistoryEntryRequired)
            {
                if (_iclbPreviousHistory == null)
                    LoadPreviousHistory();

                foreach (busPersonAccountLtcOption lobjLtcOption in _iclbLtcOptionModified)
                {
                    if (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value.IsNotNullOrEmpty())
                    {
                        busPersonAccountLtcOptionHistory lobjPreviousOptionHistory = GetPreviousHistoryForOption(lobjLtcOption);
                        if (lobjPreviousOptionHistory.icdoPersonAccountLtcOptionHistory.person_account_ltc_option_history_id > 0)
                        {
                            if (lobjPreviousOptionHistory.icdoPersonAccountLtcOptionHistory.effective_end_date != DateTime.MinValue)
                            {
                                if (icdoPersonAccount.history_change_date < lobjPreviousOptionHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                                {
                                    lblnResult = true;
                                    break;
                                }
                            }
                            else if (icdoPersonAccount.history_change_date < lobjPreviousOptionHistory.icdoPersonAccountLtcOptionHistory.effective_start_date)
                            {
                                lblnResult = true;
                                break;
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }

        public bool IsMemberDecreasingLevelOfCoverage()
        {
            foreach (object lobjtemp in iarrChangeLog)
            {
                if (lobjtemp is cdoPersonAccountLtcOption)
                {
                    foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionMember)
                    {
                        if ((lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue) &&
                            (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value.IsNotNullOrEmpty()))
                        {
                            if (CheckLevelDecreased(lobjLtcOption))
                            {
                                return true;
                            }
                        }
                    }
                    foreach (busPersonAccountLtcOption lobjLtcOption in iclbLtcOptionSpouse)
                    {
                        if ((lobjLtcOption.icdoPersonAccountLtcOption.effective_start_date != DateTime.MinValue) &&
                            (lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value.IsNotNullOrEmpty()))
                        {
                            if (CheckLevelDecreased(lobjLtcOption))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckLevelDecreased(busPersonAccountLtcOption lobjLtcOption)
        {
            bool lblLevelDecreasedFlag = false;
            if (iclbPreviousHistory == null)
                LoadPreviousHistory();
            foreach (busPersonAccountLtcOptionHistory lobjHistory in iclbPreviousHistory)
            {
                if (lobjHistory.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == lobjLtcOption.icdoPersonAccountLtcOption.ltc_relationship_value)
                {
                    if ((lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == busConstant.LTCLevelOfCoverage3YRS) &&
                        (lobjHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == busConstant.LTCLevelOfCoverage5YRS))
                    {
                        lblLevelDecreasedFlag = true;
                    }
                    else if (lobjLtcOption.icdoPersonAccountLtcOption.level_of_coverage_value == lobjHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value)
                    {
                        string lstrCurrentLtcTypeValue = busGlobalFunctions.GetData1ByCodeValue(339, lobjLtcOption.icdoPersonAccountLtcOption.ltc_insurance_type_value, iobjPassInfo);
                        string lstrHistoryLtcTypeValue = busGlobalFunctions.GetData1ByCodeValue(339, lobjHistory.icdoPersonAccountLtcOptionHistory.ltc_insurance_type_value, iobjPassInfo);
                        if (Convert.ToInt32(lstrCurrentLtcTypeValue) < Convert.ToInt32(lstrHistoryLtcTypeValue))
                        {
                            lblLevelDecreasedFlag = true;
                        }
                    }
                }
            }
            return lblLevelDecreasedFlag;
        }

        public busPersonAccountLtcOptionHistory LoadHistoryByDate(busPersonAccountLtcOption aobjPALtcOption, DateTime adtGivenDate)
        {
            busPersonAccountLtcOptionHistory lobjPersonAccountLtcHistory = new busPersonAccountLtcOptionHistory();
            lobjPersonAccountLtcHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory();

            var lenuList = iclbLtcHistory.Where(i => i.icdoPersonAccountLtcOptionHistory.ltc_relationship_value == aobjPALtcOption.icdoPersonAccountLtcOption.ltc_relationship_value);

            foreach (busPersonAccountLtcOptionHistory lobjPALtcHistory in lenuList)
            {
                if (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.level_of_coverage_value == aobjPALtcOption.icdoPersonAccountLtcOption.level_of_coverage_value)
                {
                    //Ignore the Same Start Date and End Date Records
                    if (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date,
                            lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date))
                        {
                            lobjPersonAccountLtcHistory = lobjPALtcHistory;
                            break;
                        }
                    }
                }
            }
            return lobjPersonAccountLtcHistory;
        }

        public busPersonAccountLtcOptionHistory LoadHistoryByDateWithOutOption(DateTime adtGivenDate)
        {
            busPersonAccountLtcOptionHistory lobjPersonAccountLtcHistory = new busPersonAccountLtcOptionHistory();
            lobjPersonAccountLtcHistory.icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();

            if (iclbLtcHistory == null)
                LoadLtcOptionHistory(false);

            foreach (busPersonAccountLtcOptionHistory lobjPALtcHistory in iclbLtcHistory)
            {
                if (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != DateTime.MinValue)
                {
                    //Ignore the Same Start Date and End Date Records
                    if (lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date != lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_start_date,
                            lobjPALtcHistory.icdoPersonAccountLtcOptionHistory.effective_end_date))
                        {
                            lobjPersonAccountLtcHistory = lobjPALtcHistory;
                            break;
                        }
                    }
                }
            }
            return lobjPersonAccountLtcHistory;
        }

        //Logic of Date to Calculate Premium Amount has been changed and the details are mailed on 3/18/2009 after the discussion with RAJ.
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
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (_iclbLtcHistory == null)
                    LoadLtcOptionHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountLtcOptionHistory lbusPersonAccountLtcOptionHistory in _iclbLtcHistory)
                {
                    if (lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        public string istrLTCLevelOfCoverageForMember { get; set; }
        public string istrLTCLevelOfCoverageValueForMember { get; set; }
        public decimal idecNewIBSLTCPremiumForMember { get; set; }
        public decimal idecCurrentIBSLTCPremiumForMember { get; set; }

        public string istrLTCLevelOfCoverageForSpouse { get; set; }
        public string istrLTCLevelOfCoverageValueForSpouse { get; set; }
        public decimal idecNewIBSLTCPremiumForSpouse { get; set; }
        public decimal idecCurrentIBSLTCPremiumForSpouse { get; set; }
        public decimal idecCurrentIBSPremium { get; set; }
        public decimal idecNewIBSPremium { get; set; }

        # region UCS 22 correspondence
        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPerson.istrLTCCarrierName.IsNullOrEmpty())
                ibusPerson.LoadLTCProviderName();

            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
        }

        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            ibusPersonAccount.icdoPersonAccount = this.icdoPersonAccount;
        }
        public void LoadCancelledEndDate(ref DateTime adtCancelledEffectiveDate)
        {
            if (_iclbPreviousHistory.IsNull())
                LoadPreviousHistory();
            if (_iclbPreviousHistory.Count > 0)
            {
                foreach (busPersonAccountLtcOptionHistory lbusPersonAccountLtcOptionHistory in _iclbPreviousHistory)
                {
                    if (lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled)
                    {
                        adtCancelledEffectiveDate = lbusPersonAccountLtcOptionHistory.icdoPersonAccountLtcOptionHistory.start_date;
                        break;
                    }
                }
            }
        }

        public override busBase GetCorPerson()
        {
            return base.GetCorPerson();
        }
        # endregion

        //UAT PIR 2220
        private void PostESSMessage()
        {
            string lstrPrioityValue = string.Empty;
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (icdoPersonAccount.person_employment_dtl_id <= 0)
                    icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                // post message to employer
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                if (ibusPlan == null)
                    LoadPlan();

                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    GetMonthlyPremiumAmount();
                    string lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(29, iobjPassInfo, ref lstrPrioityValue),
                        ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, idecTotalMonthlyPremium, icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, lstrMessage, lstrPrioityValue, aintPlanID: busConstant.PlanIdLTC, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                        else
                        {
                            DataTable ldtPersonEmploymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                                                    new object[1] { ibusPerson.icdoPerson.person_id }
                                                    , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            // PIR 11309 - dual employment scenario
                            if (ldtPersonEmploymentCount.Rows.Count > 1)
                            {
                                int lintOrgId = 0;
                                Collection<busPersonEmployment> lclbPersonEmployment = new Collection<busPersonEmployment>();

                                lclbPersonEmployment = GetCollection<busPersonEmployment>(ldtPersonEmploymentCount, "icdoPersonEmployment");
                                lclbPersonEmployment = busGlobalFunctions.Sort<busPersonEmployment>("icdoPersonEmployment.start_date desc", lclbPersonEmployment);
                                foreach (busPersonEmployment lobjPersonEmployment in lclbPersonEmployment)
                                {
                                    if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date, lobjPersonEmployment.icdoPersonEmployment.start_date,
                                        lobjPersonEmployment.icdoPersonEmployment.end_date))
                                    {
                                        lintOrgId = lobjPersonEmployment.icdoPersonEmployment.org_id;
                                        break;
                                    }
                                }
                                busGlobalFunctions.PostESSMessage(lintOrgId, icdoPersonAccount.plan_id, iobjPassInfo);
                            }
                            else
                            {
                                busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                busConstant.PlanIdLTC, iobjPassInfo);
                            }
                        }
                    }
                }
                else
                {
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(25, iobjPassInfo, ref lstrPrioityValue),
                                ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, ibusPlan.icdoPlan.plan_name, icdoPersonAccount.history_change_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))),
                                lstrPrioityValue, aintPlanID: icdoPersonAccount.plan_id, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                    astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                        else
                        {
                            busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                            icdoPersonAccount.plan_id, iobjPassInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PROD pir 5538 : need to recalculate premium based on plan effective date
        /// </summary>
        private void RecalculatePremiumBasedOnPlanEffectiveDate()
        {
            //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                LoadOrgPlan(idtPlanEffectiveDate);
                LoadProviderOrgPlan(idtPlanEffectiveDate);
            }
            else
            {
                LoadActiveProviderOrgPlan(idtPlanEffectiveDate);
            }
            //Recalculate the Premium with Default History Change Date
            GetMonthlyPremiumAmount(idtPlanEffectiveDate);
        }
        //pir 7817
        public bool IsACHDetailWithNoEndDateExists()
        {
            bool lblnResult = false;
            if (iclbPersonAccountAchDetail == null)
                LoadPersonAccountAchDetail();

            lblnResult = iclbPersonAccountAchDetail.Where(i => i.icdoPersonAccountAchDetail.ach_end_date.IsNull() || i.icdoPersonAccountAchDetail.ach_end_date.Equals(DateTime.MinValue)).Any();
            return lblnResult;
        }
    }
}
