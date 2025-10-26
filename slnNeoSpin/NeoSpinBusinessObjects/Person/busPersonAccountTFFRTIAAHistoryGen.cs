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
	public class busPersonAccountTFFRTIAAHistoryGen : busExtendBase
    {
        public busPersonAccountTFFRTIAAHistoryGen()
		{

		}

		private cdoPersonAccountTffrtiaaHistory _icdoPersonAccountTffrtiaaHistory;
		public cdoPersonAccountTffrtiaaHistory icdoPersonAccountTffrtiaaHistory
		{
			get
			{
				return _icdoPersonAccountTffrtiaaHistory;
			}
			set
			{
				_icdoPersonAccountTffrtiaaHistory = value;
			}
		}

		public bool FindPersonAccountTffrtiaaHistory(int Aintpersonaccounttffrtiaahistoryid)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountTffrtiaaHistory == null)
			{
				_icdoPersonAccountTffrtiaaHistory = new cdoPersonAccountTffrtiaaHistory();
			}
			if (_icdoPersonAccountTffrtiaaHistory.SelectRow(new object[1] { Aintpersonaccounttffrtiaahistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
