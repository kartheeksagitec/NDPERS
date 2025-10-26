#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busHelpIndexLookup : busMainBase
	{
		private Collection<busHelpIndex> _iclbLookupResult;
		public Collection<busHelpIndex> iclbLookupResult
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

		public void LoadHelpIndexs(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busHelpIndex>(adtbSearchResult, "icdoHelpIndex");
		}
	}
}
