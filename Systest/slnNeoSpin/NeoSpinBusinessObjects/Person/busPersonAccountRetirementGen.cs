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
    public class busPersonAccountRetirementGen : busPersonAccount
    {
        public busPersonAccountRetirementGen()
        {

        }
        public string IsFYTD
        {
            get { return busConstant.Flag_Yes; }
        }

        private cdoPersonAccountRetirement _icdoPersonAccountRetirement;
        public cdoPersonAccountRetirement icdoPersonAccountRetirement
        {
            get
            {
                return _icdoPersonAccountRetirement;
            }
            set
            {
                _icdoPersonAccountRetirement = value;
            }
        }
        private Collection<busPersonAccountRetirementHistory> _icolPersonAccountRetirementHistory;
        public Collection<busPersonAccountRetirementHistory> icolPersonAccountRetirementHistory
        {
            get
            {
                return _icolPersonAccountRetirementHistory;
            }

            set
            {
                _icolPersonAccountRetirementHistory = value;
            }
        }


        //private Collection<busPersonAccountRetirementContribution> _iclbMissedDeposits;

        //public Collection<busPersonAccountRetirementContribution> iclbMissedDeposits
        //{
        //    get { return _iclbMissedDeposits; }
        //    set { _iclbMissedDeposits = value; }
        //}


        public bool FindPersonAccountRetirement(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountRetirement>(
                new string[1] { "person_account_id" },
                new object[1] { Aintpersonaccountid }, null, null);
            if (_icdoPersonAccountRetirement == null)
            {
                _icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
            }
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPersonAccountRetirement.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else if (ldtbList.Rows.Count > 1)
            {
                throw new Exception("FindPersonAccountRetirement method : Multiple records returned for given Person Account ID : " +
                    Aintpersonaccountid);
            };

            //Loading the Person Account too
            if (icdoPersonAccount == null)
                icdoPersonAccount = new cdoPersonAccount();

            if (icdoPersonAccount.person_account_id == 0)
                lblnResult = FindPersonAccount(Aintpersonaccountid);

            return lblnResult;
        }

        public void LoadPersonAccountRetirementHistory()
        {
            LoadPersonAccountRetirementHistory(true);
        }

        public void LoadPersonAccountRetirementHistory(bool ablnLoadOtherObjects)
        {
            DataTable ldtbList = Select<cdoPersonAccountRetirementHistory>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccountRetirement.person_account_id }, null, "case when end_date is null then 0 else 1 end, start_date DESC, end_date DESC");
            _icolPersonAccountRetirementHistory = GetCollection<busPersonAccountRetirementHistory>(ldtbList, "icdoPersonAccountRetirementHistory");

            if (ablnLoadOtherObjects)
            {
                foreach (busPersonAccountRetirementHistory lobjRetrHistory in _icolPersonAccountRetirementHistory)
                {
                    lobjRetrHistory.LoadPersonAccount();
                    lobjRetrHistory.LoadPlan();
                    DataTable ldtLatestHistory = Select<cdoPersonAccountRetirementHistory>(new string[1] { "person_account_retirement_history_id" },
                                       new object[1] { lobjRetrHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id }, null, null);
                    if (ldtLatestHistory.IsNotNull() && ldtLatestHistory.Rows.Count > 0 && Convert.ToString(ldtLatestHistory.Rows[0]["addl_ee_contribution_percent"]).IsNullOrEmpty())
                    {
                        lobjRetrHistory.iblnShowAddlEEContributionPercent = false;
                    }
                    else
                    {
                        lobjRetrHistory.iblnShowAddlEEContributionPercent = true;
                        //iintESSAddlEEContributionPercent = ibusESSPersonAccountRetirement.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                    }
                }
            }
        }
    }
}
