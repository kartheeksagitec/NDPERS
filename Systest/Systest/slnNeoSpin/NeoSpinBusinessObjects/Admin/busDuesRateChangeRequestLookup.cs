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
	/// Class NeoSpin.BusinessObjects.busDuesRateChangeRequestLookup:
	/// Inherited from busDuesRateChangeRequestLookupGen, this class is used to customize the lookup business object busDuesRateChangeRequestLookupGen. 
	/// </summary>
	[Serializable]
	public class busDuesRateChangeRequestLookup : busDuesRateChangeRequestLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busDuesRateChangeRequest lobjDuesRateChange = (busDuesRateChangeRequest)aobjBus;

            lobjDuesRateChange.ibusVendor = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjDuesRateChange.ibusVendor.icdoOrganization.LoadData(adtrRow);
        }
	}
}
