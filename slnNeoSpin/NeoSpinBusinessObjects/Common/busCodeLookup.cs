#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCodeLookup : busMainBase
	{
		private Collection<busCode> _iclbLookupResult;
		public Collection<busCode> iclbLookupResult
		{
			get
			{
				return _iclbLookupResult;
			}

			set
			{
				_iclbLookupResult = value;
			}
		}

		public void LoadCodes(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busCode>(adtbSearchResult, "icdoCode");
		}
	}
}
