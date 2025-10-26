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
	public class busProviderReportDataBatchRequestLookupGen : busMainBase
	{
		private Collection<busProviderReportDataBatchRequest> _iclbProviderReportDataBatchRequest;
		public Collection<busProviderReportDataBatchRequest> iclbProviderReportDataBatchRequest
		{
			get
			{
				return _iclbProviderReportDataBatchRequest;
			}
			set
			{
				_iclbProviderReportDataBatchRequest = value;
			}
		}

		public void LoadProviderReportDataBatchRequests(DataTable adtbSearchResult)
		{
			_iclbProviderReportDataBatchRequest = GetCollection<busProviderReportDataBatchRequest>(adtbSearchResult, "icdoProviderReportDataBatchRequest");
		}
	}
}
