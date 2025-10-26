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
	public class busBatchScheduleLookup : busMainBase
	{
		private Collection<busBatchSchedule> _iclbLookupResult;
		public Collection<busBatchSchedule> iclbLookupResult
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

		public void LoadBatchSchedule(DataTable adtbSearchResult)
		{
			_iclbLookupResult = GetCollection<busBatchSchedule>(adtbSearchResult, "icdoBatchSchedule");
		}


	}
}
