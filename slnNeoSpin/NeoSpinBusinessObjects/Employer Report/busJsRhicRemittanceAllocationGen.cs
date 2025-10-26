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
	public partial class busJsRhicRemittanceAllocation : busExtendBase
    {
		public busJsRhicRemittanceAllocation()
		{

		} 

		private cdoJsRhicRemittanceAllocation _icdoJsRhicRemittanceAllocation;
		public cdoJsRhicRemittanceAllocation icdoJsRhicRemittanceAllocation
		{
			get
			{
				return _icdoJsRhicRemittanceAllocation;
			}

			set
			{
				_icdoJsRhicRemittanceAllocation = value;
			}
		}

		public bool FindJsRhicRemittanceAllocation(int Aintrhicremittanceallocationid)
		{
			bool lblnResult = false;
			if (_icdoJsRhicRemittanceAllocation == null)
			{
				_icdoJsRhicRemittanceAllocation = new cdoJsRhicRemittanceAllocation();
			}
			if (_icdoJsRhicRemittanceAllocation.SelectRow(new object[1] { Aintrhicremittanceallocationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
