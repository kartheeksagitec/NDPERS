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
	public partial class busEmployerPayrollHeaderError : busExtendBase
    {
		public busEmployerPayrollHeaderError()
		{

		} 

		private cdoEmployerPayrollHeaderError _icdoEmployerPayrollHeaderError;
		public cdoEmployerPayrollHeaderError icdoEmployerPayrollHeaderError
		{
			get
			{
				return _icdoEmployerPayrollHeaderError;
			}

			set
			{
				_icdoEmployerPayrollHeaderError = value;
			}
		}

		public bool FindEmployerPayrollHeaderError(int Aintemployerpayrollheadererrorid)
		{
			bool lblnResult = false;
			if (_icdoEmployerPayrollHeaderError == null)
			{
				_icdoEmployerPayrollHeaderError = new cdoEmployerPayrollHeaderError();
			}
			if (_icdoEmployerPayrollHeaderError.SelectRow(new object[1] { Aintemployerpayrollheadererrorid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
