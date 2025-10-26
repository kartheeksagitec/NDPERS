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
	/// Class NeoSpin.BusinessObjects.busRateChangeLetterRequestLookup:
	/// Inherited from busRateChangeLetterRequestLookupGen, this class is used to customize the lookup business object busRateChangeLetterRequestLookupGen. 
	/// </summary>
	[Serializable]
	public class busRateChangeLetterRequestLookup : busRateChangeLetterRequestLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busRateChangeLetterRequest lobjRateChangeLetterRequest = (busRateChangeLetterRequest)aobjBus;

            lobjRateChangeLetterRequest.ibusProviderOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjRateChangeLetterRequest.ibusProviderOrganization.icdoOrganization.LoadData(adtrRow);                
        }
	}
}
