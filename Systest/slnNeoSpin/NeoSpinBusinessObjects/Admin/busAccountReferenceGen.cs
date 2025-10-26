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
	public partial class busAccountReference : busExtendBase
    {
		public busAccountReference()
		{

		} 

		private cdoAccountReference _icdoAccountReference;
		public cdoAccountReference icdoAccountReference
		{
			get
			{
				return _icdoAccountReference;
			}

			set
			{
				_icdoAccountReference = value;
			}
		}

		public bool FindAccountReference(int Aintaccountreferenceid)
		{
			bool lblnResult = false;
			if (_icdoAccountReference == null)
			{
				_icdoAccountReference = new cdoAccountReference();
			}
			if (_icdoAccountReference.SelectRow(new object[1] { Aintaccountreferenceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
