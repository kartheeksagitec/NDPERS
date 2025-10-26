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
	public class busProviderReportDataDeffCompGen : busExtendBase
    {
		public busProviderReportDataDeffCompGen()
		{

		}

		private cdoProviderReportDataDeffComp _icdoProviderReportDataDeffComp;
		public cdoProviderReportDataDeffComp icdoProviderReportDataDeffComp
		{
			get
			{
				return _icdoProviderReportDataDeffComp;
			}
			set
			{
				_icdoProviderReportDataDeffComp = value;
			}
		}

		public bool FindProviderReportDataDeffComp(int Aintproviderreportdatadeffcompid)
		{
			bool lblnResult = false;
			if (_icdoProviderReportDataDeffComp == null)
			{
				_icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp();
			}
			if (_icdoProviderReportDataDeffComp.SelectRow(new object[1] { Aintproviderreportdatadeffcompid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        //property to load provider details
        public busOrganization ibusProvider { get; set; }
        //Load Provider Details
        public void LoadProvider()
        {
            if (ibusProvider == null)
                ibusProvider = new busOrganization();
            ibusProvider.FindOrganization(icdoProviderReportDataDeffComp.provider_org_id);
        }
	}
}
