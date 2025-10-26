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
	public partial class busPersonAddress : busExtendBase
	{
		public busPersonAddress()
		{

		} 

		private cdoPersonAddress _icdoPersonAddress;
		public cdoPersonAddress icdoPersonAddress
		{
			get
			{
				return _icdoPersonAddress;
			}

			set
			{
				_icdoPersonAddress = value;
			}
		}

		public bool FindPersonAddress(int Aintpersonaddressid)
		{
			bool lblnResult = false;
			if (_icdoPersonAddress == null)
			{
				_icdoPersonAddress = new cdoPersonAddress();
			}
			if (_icdoPersonAddress.SelectRow(new object[1] { Aintpersonaddressid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
