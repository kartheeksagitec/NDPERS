using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busSystemManagement : busExtendBase
    {
		public busSystemManagement()
		{
		}

		private cdoSystemManagement _icdoSystemManagement;
		public cdoSystemManagement icdoSystemManagement
		{
			get
			{
				return _icdoSystemManagement;
			}

			set
			{
				_icdoSystemManagement = value;
			}
		}

		public bool FindSystemManagement()
		{
			bool lblnResult = false;
			if (_icdoSystemManagement == null)
			{
				_icdoSystemManagement = new cdoSystemManagement();
			}
			if (_icdoSystemManagement.SelectRow(new object[1] { 1 }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
