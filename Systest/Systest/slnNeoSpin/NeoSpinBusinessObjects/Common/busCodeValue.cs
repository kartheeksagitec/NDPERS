#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCodeValue : busExtendBase
    {
		public busCodeValue()
		{
		}

		private busCode _ibusCode;
		public busCode ibusCode
		{
			get
			{
				return _ibusCode;
			}

			set
			{
				_ibusCode = value;
			}
		}

		private cdoCodeValue _icdoCodeValue;
		public cdoCodeValue icdoCodeValue
		{
			get
			{
				return _icdoCodeValue;
			}

			set
			{
				_icdoCodeValue = value;
			}
		}

		public bool FindCodeValue(int aintCodeSerialId)
		{
			bool lblnResult = false;
			if (_icdoCodeValue == null)
			{
				_icdoCodeValue = new cdoCodeValue();
			}
			if (_icdoCodeValue.SelectRow(new object[1] { aintCodeSerialId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadCode()
		{
			if (_ibusCode == null)
			{
				_ibusCode = new busCode();
			}
			_ibusCode.FindCode(_icdoCodeValue.code_id);
		}
	}
}
