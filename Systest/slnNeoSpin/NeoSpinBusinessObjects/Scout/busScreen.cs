#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Data.Common;


#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busScreen : busExtendBase
    {
		public busScreen()
		{
		}

		private string _istrScreenName;
		public string istrScreenName
		{
			get
			{
				return _istrScreenName;
			}

			set
			{
				_istrScreenName = value;
			}
		}

	}
}
