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
	public class busResourcesLookup : busMainBase
	{
		private Collection<busResources> _iclbLookupResult;
		public Collection<busResources> iclbLookupResult
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

		public void LoadResources(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busResources>(adtbSearchResult, "icdoResources");
		}
	}
}
