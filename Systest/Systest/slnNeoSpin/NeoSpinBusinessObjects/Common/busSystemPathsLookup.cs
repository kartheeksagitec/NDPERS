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
	public class busSystemPathsLookup : busMainBase
	{
		private Collection<busSystemPaths> _iclbLookupResult;
		public Collection<busSystemPaths> iclbLookupResult
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

		public void LoadSystemPaths(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busSystemPaths>(adtbSearchResult, "icdoSystemPaths");
		}
	}
}
