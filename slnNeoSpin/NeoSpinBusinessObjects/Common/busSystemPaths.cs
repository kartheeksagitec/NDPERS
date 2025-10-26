#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busSystemPaths : busExtendBase
    {
		public busSystemPaths()
		{
		}

		private cdoSystemPaths _icdoSystemPaths;
		public cdoSystemPaths icdoSystemPaths
		{
			get
			{
				return _icdoSystemPaths;
			}

			set
			{
				_icdoSystemPaths = value;
			}
		}

        public bool FindPath(int aintPathId)
		{
			bool lblnResult = false;
			if (_icdoSystemPaths == null)
			{
				_icdoSystemPaths = new cdoSystemPaths();
			}
            if (_icdoSystemPaths.SelectRow(new object[1] { aintPathId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
