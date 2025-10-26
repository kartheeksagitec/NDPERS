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
	public class busProviderReportDataInsuranceGen : busExtendBase
    {
		public busProviderReportDataInsuranceGen()
		{

		}

		private cdoProviderReportDataInsurance _icdoProviderReportDataInsurance;
		public cdoProviderReportDataInsurance icdoProviderReportDataInsurance
		{
			get
			{
				return _icdoProviderReportDataInsurance;
			}
			set
			{
				_icdoProviderReportDataInsurance = value;
			}
		}

		public bool FindProviderReportDataInsurance(int Aintproviderreportdatadeffcompid)
		{
			bool lblnResult = false;
			if (_icdoProviderReportDataInsurance == null)
			{
				_icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
			}
			if (_icdoProviderReportDataInsurance.SelectRow(new object[1] { Aintproviderreportdatadeffcompid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
