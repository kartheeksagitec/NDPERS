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
	public class busPersonAccountLtcOptionGen : busExtendBase
    {
		public busPersonAccountLtcOptionGen()
		{

		}
        private decimal _idecMonthlyPremium;

        public decimal idecMonthlyPremium
        {
            get { return _idecMonthlyPremium; }
            set { _idecMonthlyPremium = value; }
        }       
		private cdoPersonAccountLtcOption _icdoPersonAccountLtcOption;
		public cdoPersonAccountLtcOption icdoPersonAccountLtcOption
		{
			get
			{
				return _icdoPersonAccountLtcOption;
			}
			set
			{
				_icdoPersonAccountLtcOption = value;
			}
		}

		private busPersonAccount _ibusPersonAccount;
		public busPersonAccount ibusPersonAccount
		{
			get
			{
				return _ibusPersonAccount;
			}
			set
			{
				_ibusPersonAccount = value;
			}
		}

		public bool FindPersonAccountLtcOption(int Aintpersonaccountltcoptionid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountLtcOption == null)
			{
				_icdoPersonAccountLtcOption = new cdoPersonAccountLtcOption();
			}
			if (_icdoPersonAccountLtcOption.SelectRow(new object[1] { Aintpersonaccountltcoptionid }))
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
		//	_ibusPersonAccount.ibusPersonAccountLtcOption = this;
			_ibusPersonAccount.FindPersonAccount(_icdoPersonAccountLtcOption.person_account_ltc_option_id);
		}

	}
}
