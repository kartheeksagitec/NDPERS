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
	/// Class NeoSpin.BusinessObjects.busWssAcknowledgementLookup:
	/// Inherited from busWssAcknowledgementLookupGen, this class is used to customize the lookup business object busWssAcknowledgementLookupGen. 
	/// </summary>
	[Serializable]
	public class busWssAcknowledgementLookup : busWssAcknowledgementLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            base.LoadOtherObjects(adtrRow, aobjBus);
            if (aobjBus is busWssAcknowledgement)
            {
                if (adtrRow["screen_step_value"] != DBNull.Value)
                {
                    busWssAcknowledgement lobjAck = (busWssAcknowledgement)aobjBus;
                    lobjAck.icdoWssAcknowledgement.screen_step_description = busGlobalFunctions.GetCodeValueDetails(6000, adtrRow["screen_step_value"].ToString()).description;
                }
            }
        }
	}
}
