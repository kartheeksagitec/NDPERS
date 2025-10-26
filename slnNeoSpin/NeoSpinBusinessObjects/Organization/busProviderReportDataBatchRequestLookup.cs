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
	public class busProviderReportDataBatchRequestLookup : busProviderReportDataBatchRequestLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busProviderReportDataBatchRequest lbusProviderReportDataBatchRequest = (busProviderReportDataBatchRequest)aobjBus;
            lbusProviderReportDataBatchRequest.ibusOrganization = new busOrganization();
            lbusProviderReportDataBatchRequest.ibusOrganization.icdoOrganization = new cdoOrganization();
            lbusProviderReportDataBatchRequest.ibusOrganization.icdoOrganization.LoadData(adtrRow);
            lbusProviderReportDataBatchRequest.ibusPlan = new busPlan();
            lbusProviderReportDataBatchRequest.ibusPlan.icdoPlan = new cdoPlan();
            lbusProviderReportDataBatchRequest.ibusPlan.icdoPlan.LoadData(adtrRow);
        }
	}
}
