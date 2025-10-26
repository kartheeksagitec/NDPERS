#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCorTemplatesLookup : busMainBase
	{

		private Collection<busCorTemplates> _iclbLookupResult;
		public Collection<busCorTemplates> iclbLookupResult
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

		public void LoadCorTemplates(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busCorTemplates>(adtbSearchResult, "icdoCorTemplates");
		}



	}
}
