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
	public partial class busChartOfAccount : busExtendBase
    {
		public busChartOfAccount()
		{

		} 

		private cdoChartOfAccount _icdoChartOfAccount;
		public cdoChartOfAccount icdoChartOfAccount
		{
			get
			{
				return _icdoChartOfAccount;
			}

			set
			{
				_icdoChartOfAccount = value;
			}
		}

		public bool FindChartOfAccount(int Aintchartofaccountid)
		{
			bool lblnResult = false;
			if (_icdoChartOfAccount == null)
			{
				_icdoChartOfAccount = new cdoChartOfAccount();
			}
			if (_icdoChartOfAccount.SelectRow(new object[1] { Aintchartofaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
