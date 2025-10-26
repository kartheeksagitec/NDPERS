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
	public class busPersonAccountFlexCompHistoryGen : busExtendBase
    {
		public busPersonAccountFlexCompHistoryGen()
		{

		}
        private busPlan _ibusPlan;

        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busPersonAccount _ibusPersonAccount;

        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }
         public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountFlexCompHistory.person_account_id);
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(ibusPersonAccount.icdoPersonAccount.plan_id);
        }
		private cdoPersonAccountFlexCompHistory _icdoPersonAccountFlexCompHistory;
		public cdoPersonAccountFlexCompHistory icdoPersonAccountFlexCompHistory
		{
			get
			{
				return _icdoPersonAccountFlexCompHistory;
			}
			set
			{
				_icdoPersonAccountFlexCompHistory = value;
			}
		}

		public bool FindPersonAccountFlexCompHistory(int Aintpersonaccountflexcomphistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountFlexCompHistory == null)
			{
				_icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory();
			}
			if (_icdoPersonAccountFlexCompHistory.SelectRow(new object[1] { Aintpersonaccountflexcomphistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
