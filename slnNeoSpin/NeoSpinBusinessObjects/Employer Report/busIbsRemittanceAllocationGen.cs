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
	public partial class busIbsRemittanceAllocation : busExtendBase
    {
		public busIbsRemittanceAllocation()
		{

		} 

		private cdoIbsRemittanceAllocation _icdoIbsRemittanceAllocation;
		public cdoIbsRemittanceAllocation icdoIbsRemittanceAllocation
		{
			get
			{
				return _icdoIbsRemittanceAllocation;
			}

			set
			{
				_icdoIbsRemittanceAllocation = value;
			}
		}

		public bool FindIbsRemittanceAllocation(int Aintibsremittanceallocationid)
		{
			bool lblnResult = false;
			if (_icdoIbsRemittanceAllocation == null)
			{
				_icdoIbsRemittanceAllocation = new cdoIbsRemittanceAllocation();
			}
			if (_icdoIbsRemittanceAllocation.SelectRow(new object[1] { Aintibsremittanceallocationid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
