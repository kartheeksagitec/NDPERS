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
	public class busPersonAccountMissedDepositGen : busExtendBase
    {
		public busPersonAccountMissedDepositGen()
		{

		}

		private cdoPersonAccountMissedDeposit _icdoPersonAccountMissedDeposit;
		public cdoPersonAccountMissedDeposit icdoPersonAccountMissedDeposit
		{
			get
			{
				return _icdoPersonAccountMissedDeposit;
			}
			set
			{
				_icdoPersonAccountMissedDeposit = value;
			}
		}

		public bool FindPersonAccountMissedDeposit(int Aintpersonaccountmisseddepositid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountMissedDeposit == null)
			{
				_icdoPersonAccountMissedDeposit = new cdoPersonAccountMissedDeposit();
			}
			if (_icdoPersonAccountMissedDeposit.SelectRow(new object[1] { Aintpersonaccountmisseddepositid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
