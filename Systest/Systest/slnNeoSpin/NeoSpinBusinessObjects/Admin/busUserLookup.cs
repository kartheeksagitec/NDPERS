#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busUserLookup : busMainBase
	{
		private Collection<busUser> _iclbLookupResult;
		public Collection<busUser> iclbLookupResult
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

		public void LoadUsers(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busUser>(adtbSearchResult, "icdoUser");
		}
	}
}
