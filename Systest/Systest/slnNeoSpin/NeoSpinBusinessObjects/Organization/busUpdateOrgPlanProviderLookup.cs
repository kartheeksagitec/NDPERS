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
	/// Class NeoSpin.BusinessObjects.busUpdateOrgPlanProviderLookup:
	/// Inherited from busUpdateOrgPlanProviderLookupGen, this class is used to customize the lookup business object busUpdateOrgPlanProviderLookupGen. 
	/// </summary>
	[Serializable]
	public class busUpdateOrgPlanProviderLookup : busUpdateOrgPlanProviderLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busUpdateOrgPlanProvider)
            {
                busUpdateOrgPlanProvider lobjUpdateOrgPlanProvider = (busUpdateOrgPlanProvider)aobjBus;
                lobjUpdateOrgPlanProvider.LoadEmployerOrg();
                lobjUpdateOrgPlanProvider.LoadFromProvider();
                lobjUpdateOrgPlanProvider.LoadToProvider();
                lobjUpdateOrgPlanProvider.LoadPlan();
            }
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
	}
}
