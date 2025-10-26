#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busProviderReportDataBatchRequestGen : busExtendBase
    {
		public busProviderReportDataBatchRequestGen()
		{

		}

		private cdoProviderReportDataBatchRequest _icdoProviderReportDataBatchRequest;
        public cdoProviderReportDataBatchRequest icdoProviderReportDataBatchRequest
		{
			get
			{
				return _icdoProviderReportDataBatchRequest;
			}
			set
			{
				_icdoProviderReportDataBatchRequest = value;
			}
		}

		public bool FindProviderReportDataBatchRequest(int Aintproviderreportdatabatchrequestid)
		{
			bool lblnResult = false;
			if (_icdoProviderReportDataBatchRequest == null)
			{
				_icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest();
			}
			if (_icdoProviderReportDataBatchRequest.SelectRow(new object[1] { Aintproviderreportdatabatchrequestid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
