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
	public class busProviderReportDataDCGen : busExtendBase
    {
		public busProviderReportDataDCGen()
		{

		}

		private cdoProviderReportDataDc _icdoProviderReportDataDc;
		public cdoProviderReportDataDc icdoProviderReportDataDc
		{
			get
			{
				return _icdoProviderReportDataDc;
			}
			set
			{
				_icdoProviderReportDataDc = value;
			}
		}

		public bool FindProviderReportDataDC(int Aintproviderreportdatadcid)
		{
			bool lblnResult = false;
			if (_icdoProviderReportDataDc == null)
			{
				_icdoProviderReportDataDc = new cdoProviderReportDataDc();
			}
			if (_icdoProviderReportDataDc.SelectRow(new object[1] { Aintproviderreportdatadcid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
