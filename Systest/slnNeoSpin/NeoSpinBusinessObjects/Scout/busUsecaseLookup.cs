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
	public class busUsecaseLookup : busMainBase
	{
		private Collection<busUsecase> _iclbLookupResult;
		public Collection<busUsecase> iclbLookupResult
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

		public void LoadUsecases(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busUsecase>(adtbSearchResult, "icdoUsecase");
		}

		protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
		//This is fired for every datarow found in the search result. 
		//Handle is returned to this method include the datarow and busObject being created
		//
		{
		}

	}
}
