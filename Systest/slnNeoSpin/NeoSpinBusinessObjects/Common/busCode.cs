#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCode : busExtendBase
    {
		public busCode()
		{
		}

		private cdoCode _icdoCode;
		public cdoCode icdoCode
		{
			get
			{
				return _icdoCode;
			}

			set
			{
				_icdoCode = value;
			}
		}

		private Collection<busCodeValue> _iclbCodeValue;
		public Collection<busCodeValue> iclbCodeValue
		{
			get
			{
				return _iclbCodeValue;
			}

			set
			{
				_iclbCodeValue = value;
			}
		}

		public bool FindCode(int aintCodeId)
		{
			bool lblnResult = false;
			if (_icdoCode == null)
			{
				_icdoCode = new cdoCode();
			}
			if (_icdoCode.SelectRow(new object[1] { aintCodeId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadCodeValues()
		{
			DataTable ldtbList = Select<cdoCodeValue>( new string[1] { "code_id" },
				new object[1] { _icdoCode.code_id }, null, "code_value_order, description");
			_iclbCodeValue = GetCollection<busCodeValue>(ldtbList, "icdoCodeValue");
		}
	}
}
