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
    public class busPersonAccountDeferredCompHistoryGen : busExtendBase
    {
		public busPersonAccountDeferredCompHistoryGen()
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
		private cdoPersonAccountDeferredCompHistory _icdoPersonAccountDeferredCompHistory;
		public cdoPersonAccountDeferredCompHistory icdoPersonAccountDeferredCompHistory
		{
			get
			{
				return _icdoPersonAccountDeferredCompHistory;
			}
			set
			{
				_icdoPersonAccountDeferredCompHistory = value;
			}
		}

		public bool FindPersonAccountDeferredCompHistory(int Aintpersonaccountdeferredcomphistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountDeferredCompHistory == null)
			{
				_icdoPersonAccountDeferredCompHistory = new cdoPersonAccountDeferredCompHistory();
			}
			if (_icdoPersonAccountDeferredCompHistory.SelectRow(new object[1] { Aintpersonaccountdeferredcomphistoryid }))
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
            _ibusPersonAccount.FindPersonAccount(_icdoPersonAccountDeferredCompHistory.person_account_id);
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_ibusPersonAccount.icdoPersonAccount.plan_id);
        }
	}
}
