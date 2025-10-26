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
	public partial class busEmployerPayrollDetailError : busExtendBase
    {
		public busEmployerPayrollDetailError()
		{

		} 

		private cdoEmployerPayrollDetailError _icdoEmployerPayrollDetailError;
		public cdoEmployerPayrollDetailError icdoEmployerPayrollDetailError
		{
			get
			{
				return _icdoEmployerPayrollDetailError;
			}

			set
			{
				_icdoEmployerPayrollDetailError = value;
			}
		}

		public bool FindEmployerPayrollDetailError(int Aintemployerpayrolldetailerrorid)
		{
			bool lblnResult = false;
			if (_icdoEmployerPayrollDetailError == null)
			{
				_icdoEmployerPayrollDetailError = new cdoEmployerPayrollDetailError();
			}
			if (_icdoEmployerPayrollDetailError.SelectRow(new object[1] { Aintemployerpayrolldetailerrorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
