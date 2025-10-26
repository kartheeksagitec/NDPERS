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
	public class busPersonAccountLtcOptionHistoryGen : busExtendBase
    {
		public busPersonAccountLtcOptionHistoryGen()
		{

		}
        private busPlan _ibusPlan;

        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }
        private busPersonAccountLtcOption _ibusLtcOption;

        public busPersonAccountLtcOption ibusLtcOption
        {
            get { return _ibusLtcOption; }
            set { _ibusLtcOption = value; }
        }

        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }
        public void LoadLtcOption()
        {
            if (_ibusLtcOption == null)
            {
                _ibusLtcOption = new busPersonAccountLtcOption();
            }
            _ibusLtcOption.FindPersonAccountLtcOption(icdoPersonAccountLtcOptionHistory.person_account_ltc_option_id);
        }
        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(ibusLtcOption.icdoPersonAccountLtcOption.person_account_id);
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(ibusPersonAccount.icdoPersonAccount.plan_id);
        }
		private cdoPersonAccountLtcOptionHistory _icdoPersonAccountLtcOptionHistory;
		public cdoPersonAccountLtcOptionHistory icdoPersonAccountLtcOptionHistory
		{
			get
			{
				return _icdoPersonAccountLtcOptionHistory;
			}
			set
			{
				_icdoPersonAccountLtcOptionHistory = value;
			}
		}

		public bool FindPersonAccountLtcOptionHistory(int Aintpersonaccountltcoptionhistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountLtcOptionHistory == null)
			{
				_icdoPersonAccountLtcOptionHistory = new cdoPersonAccountLtcOptionHistory();
			}
			if (_icdoPersonAccountLtcOptionHistory.SelectRow(new object[1] { Aintpersonaccountltcoptionhistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
