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
    public class busPersonAccountRetirementHistoryGen : busExtendBase
    {
		public busPersonAccountRetirementHistoryGen()
		{

		}
        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }
        private busPlan _ibusPlan;

        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

		private cdoPersonAccountRetirementHistory _icdoPersonAccountRetirementHistory;
		public cdoPersonAccountRetirementHistory icdoPersonAccountRetirementHistory
		{
			get
			{
				return _icdoPersonAccountRetirementHistory;
			}
			set
			{
				_icdoPersonAccountRetirementHistory = value;
			}
		}

		public bool FindPersonAccountRetirementHistory(int Aintpersonaccountretirementhistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountRetirementHistory == null)
			{
				_icdoPersonAccountRetirementHistory = new cdoPersonAccountRetirementHistory();
			}
			if (_icdoPersonAccountRetirementHistory.SelectRow(new object[1] { Aintpersonaccountretirementhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(_icdoPersonAccountRetirementHistory.person_account_id);
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_ibusPersonAccount.icdoPersonAccount.plan_id);
        }
        //PIR 25920 DC 2025 changes
        public bool iblnShowAddlEEContributionPercent { get; set; }
    }
}
