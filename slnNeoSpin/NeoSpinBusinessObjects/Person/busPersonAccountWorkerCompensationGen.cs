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
	public class busPersonAccountWorkerCompensationGen : busPersonAccount
	{
		public busPersonAccountWorkerCompensationGen()
		{

		}

		private cdoPersonAccountWorkerCompensation _icdoPersonAccountWorkerCompensation;
		public cdoPersonAccountWorkerCompensation icdoPersonAccountWorkerCompensation
		{
			get
			{
				return _icdoPersonAccountWorkerCompensation;
			}
			set
			{
				_icdoPersonAccountWorkerCompensation = value;
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

		public bool FindPersonAccountWorkerCompensation(int Aintaccountworkercompid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountWorkerCompensation == null)
			{
				_icdoPersonAccountWorkerCompensation = new cdoPersonAccountWorkerCompensation();
			}
			if (_icdoPersonAccountWorkerCompensation.SelectRow(new object[1] { Aintaccountworkercompid }))
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
			//_ibusPersonAccount.ibusPersonAccountWorkerCompensation = this;
			_ibusPersonAccount.FindPersonAccount(_icdoPersonAccountWorkerCompensation.person_account_id);
		}

	}
}
