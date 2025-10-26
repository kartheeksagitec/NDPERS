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
	public class busMessagesLookup : busMainBase
	{
		private Collection<busMessages> _iclbLookupResult;
		public Collection<busMessages> iclbLookupResult
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

		public void LoadMessages(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busMessages>(adtbSearchResult, "icdoMessages");
		}
	}
}
