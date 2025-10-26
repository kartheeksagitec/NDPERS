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
    public class busPersonAccountTFFRTIAA : busPersonAccount
    {
        private busPersonAccountTFFRTIAAHistory _ibusHistory;
        public busPersonAccountTFFRTIAAHistory ibusHistory
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

        private Collection<busPersonAccountTFFRTIAAHistory> _iclbTFFTIAAHistory;
        public Collection<busPersonAccountTFFRTIAAHistory> iclbTFFTIAAHistory
        {
            get { return _iclbTFFTIAAHistory; }
            set { _iclbTFFTIAAHistory = value; }
        }

        public void LoadTFFRTIAAHistory()
        {
            DataTable ldtbTFFTIAAHistories = Select<cdoPersonAccountTffrtiaaHistory>(new string[1] { "person_account_id" },
                                            new object[1] { icdoPersonAccount.person_account_id } , null, "person_account_tffrtiaa_history_id DESC");
            _iclbTFFTIAAHistory = GetCollection<busPersonAccountTFFRTIAAHistory>(ldtbTFFTIAAHistories, "icdoPersonAccountTffrtiaaHistory");
            foreach (busPersonAccountTFFRTIAAHistory lobjHistory in _iclbTFFTIAAHistory)
            {
                lobjHistory.LoadPersonAccount();
                lobjHistory.LoadPlan();
            }
        }
        public void InsertHistory()
        {
            cdoPersonAccountTffrtiaaHistory lobjtffrtiaaHistory = new cdoPersonAccountTffrtiaaHistory();
            lobjtffrtiaaHistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjtffrtiaaHistory.start_date = icdoPersonAccount.history_change_date;
            lobjtffrtiaaHistory.end_date = icdoPersonAccount.end_date;
            lobjtffrtiaaHistory.provider_org_id = icdoPersonAccount.provider_org_id;
            lobjtffrtiaaHistory.plan_participation_status_id = icdoPersonAccount.plan_participation_status_id;
            lobjtffrtiaaHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjtffrtiaaHistory.status_id = icdoPersonAccount.status_id;
            lobjtffrtiaaHistory.status_value = icdoPersonAccount.status_value;
            lobjtffrtiaaHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjtffrtiaaHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;            
            lobjtffrtiaaHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjtffrtiaaHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjtffrtiaaHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjtffrtiaaHistory.Insert();
        }

        public void ProcessHistory()
        {
            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired))
            {
                if (_ibusHistory == null)
                    LoadPreviousHistory();

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.

                if ((ibusHistory.icdoPersonAccountTffrtiaaHistory.end_date == DateTime.MinValue) &&
                    (ibusHistory.icdoPersonAccountTffrtiaaHistory.person_account_tffrtiaa_history_id > 0))
                {
                    if (icdoPersonAccount.end_date != DateTime.MinValue)
                    {
                        ibusHistory.icdoPersonAccountTffrtiaaHistory.end_date = icdoPersonAccount.end_date;
                    }
                    else
                    {
                        if (ibusHistory.icdoPersonAccountTffrtiaaHistory.start_date == icdoPersonAccount.history_change_date)
                            ibusHistory.icdoPersonAccountTffrtiaaHistory.end_date = icdoPersonAccount.history_change_date;
                        else
                            ibusHistory.icdoPersonAccountTffrtiaaHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                    }
                    ibusHistory.icdoPersonAccountTffrtiaaHistory.Update();
                }

                if (icdoPersonAccount.end_date == DateTime.MinValue)
                {
                    InsertHistory();
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusHistory == null)
                LoadPreviousHistory();

            SetHistoryEntryRequiredOrNot();
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                if (ibusPersonEmploymentDetail != null)
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id != 0)
                    {
                        LoadOrgPlan();
                    }
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
            {
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;
            }
            else
            {
                icdoPersonAccount.suppress_warnings_by = string.Empty;
            }

            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
            }
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ProcessHistory();
            LoadTFFRTIAAHistory();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
               SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadPreviousHistory();
        }

        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountTFFRTIAAHistory();
                _ibusHistory.icdoPersonAccountTffrtiaaHistory = new cdoPersonAccountTffrtiaaHistory();
            }
            DataTable ldtbHistory = Select("cdoPersonAccount.GetLatestTFFRTIAAHistoryRecord", new object[1] { icdoPersonAccount.person_account_id });
            if (ldtbHistory.Rows.Count > 0)
            {
                _ibusHistory.icdoPersonAccountTffrtiaaHistory.LoadData(ldtbHistory.Rows[0]);
            }
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_ibusHistory == null)
                LoadPreviousHistory();

            if ((icdoPersonAccount.plan_participation_status_value != ibusHistory.icdoPersonAccountTffrtiaaHistory.plan_participation_status_value) ||
            (icdoPersonAccount.current_plan_start_date != ibusHistory.icdoPersonAccountTffrtiaaHistory.start_date) ||
            (icdoPersonAccount.end_date != ibusHistory.icdoPersonAccountTffrtiaaHistory.end_date) ||
            (icdoPersonAccount.provider_org_id != ibusHistory.icdoPersonAccountTffrtiaaHistory.provider_org_id))
            {
                IsHistoryEntryRequired = true;
            }
            else
            {
                IsHistoryEntryRequired = false;
            }
        }
    }
}
