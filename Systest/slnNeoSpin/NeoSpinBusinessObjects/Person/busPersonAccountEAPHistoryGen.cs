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
	public class busPersonAccountEAPHistoryGen : busExtendBase
    {
		public busPersonAccountEAPHistoryGen()
		{

		}
        private busOrganization _ibusProvider;

        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
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

		private cdoPersonAccountEapHistory _icdoPersonAccountEapHistory;
		public cdoPersonAccountEapHistory icdoPersonAccountEapHistory
		{
			get
			{
				return _icdoPersonAccountEapHistory;
			}
			set
			{
				_icdoPersonAccountEapHistory = value;
			}
		}

		public bool FindPersonAccountEAPHistory(int Aintpersonaccounteaphistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountEapHistory == null)
			{
				_icdoPersonAccountEapHistory = new cdoPersonAccountEapHistory();
			}
			if (_icdoPersonAccountEapHistory.SelectRow(new object[1] { Aintpersonaccounteaphistoryid }))
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
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountEapHistory.person_account_id);
        }
        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(ibusPersonAccount.icdoPersonAccount.plan_id);
        }
        public void LoadProvider()
        {
            if (_ibusProvider == null)
            {
                _ibusProvider = new busOrganization();
            }
            _ibusProvider.FindOrganization(_icdoPersonAccountEapHistory.provider_org_id);
        }

	}
}
