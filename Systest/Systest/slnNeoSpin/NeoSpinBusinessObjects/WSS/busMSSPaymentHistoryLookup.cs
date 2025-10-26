#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busMSSPaymentHistoryLookup:
	/// Inherited from busMSSPaymentHistoryLookupGen, this class is used to customize the lookup business object busMSSPaymentHistoryLookupGen. 
	/// </summary>
	[Serializable]
	public class busMSSPaymentHistoryLookup : busMSSPaymentHistoryLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPaymentHistoryHeader)
            {
                busPaymentHistoryHeader lobjPaymentHistoryHeader = (busPaymentHistoryHeader)aobjBus;
                if (!Convert.IsDBNull(adtrRow["gross_amount"]))
                {
                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.gross_amount = Convert.ToDecimal(adtrRow["gross_amount"]);
                }
                if (lobjPaymentHistoryHeader.ibusPlan == null)
                    lobjPaymentHistoryHeader.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                if (!Convert.IsDBNull(adtrRow["plan_name"]))
                {
                    lobjPaymentHistoryHeader.ibusPlan.icdoPlan.plan_name = adtrRow["plan_name"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["org_code"]))
                {
                    lobjPaymentHistoryHeader.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lobjPaymentHistoryHeader.ibusOrganization.icdoOrganization.org_code = adtrRow["org_code"].ToString();
                }
            }
        }
	}
}
