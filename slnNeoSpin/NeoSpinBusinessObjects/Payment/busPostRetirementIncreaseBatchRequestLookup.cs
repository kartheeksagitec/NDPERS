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
	/// Class NeoSpin.BusinessObjects.busPostRetirementIncreaseBatchRequestLookup:
	/// Inherited from busPostRetirementIncreaseBatchRequestLookupGen, this class is used to customize the lookup business object busPostRetirementIncreaseBatchRequestLookupGen. 
	/// </summary>
	[Serializable]
	public class busPostRetirementIncreaseBatchRequestLookup : busPostRetirementIncreaseBatchRequestLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busPostRetirementIncreaseBatchRequest lobjPostRetirmentIncreaseBatchRequest = (busPostRetirementIncreaseBatchRequest)aobjBus;

            //lobjPostRetirmentIncreaseBatchRequest.ibusPlan = new busPlan {icdoPlan = new cdoPlan() };
            //lobjPostRetirmentIncreaseBatchRequest.ibusPlan.icdoPlan.LoadData(adtrRow);           
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            if (ahstParam["astr_post_retirment_increase_type"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(8000, "");
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }
	}
}
