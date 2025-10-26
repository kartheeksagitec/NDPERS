#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busHelpIndex : busExtendBase
    {
		public busHelpIndex()
		{
		}

		private cdoHelpIndex _icdoHelpIndex;
        public cdoHelpIndex icdoHelpIndex
		{
			get
			{
                return _icdoHelpIndex;
			}

			set
			{
                _icdoHelpIndex = value;
			}
		}
	}
}
