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
    /// Class NeoSpin.BusinessObjects.busPlanMemberTypeLookup:
    /// Inherited from busPlanMemberTypeLookupGen, this class is used to customize the lookup business object busPlanMemberTypeLookupGen. 
    /// </summary>
    [Serializable]
    public class busPlanMemberTypeLookup : busPlanMemberTypeLookupGen
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busPlanMemberTypeCrossref lbusPlanMemberTypeCrossref = (busPlanMemberTypeCrossref)aobjBus;
            lbusPlanMemberTypeCrossref.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusPlanMemberTypeCrossref.ibusPlan.icdoPlan.LoadData(adtrRow);
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
    }
}
