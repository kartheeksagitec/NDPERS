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
	/// Class NeoSpin.BusinessObjects.busMASBatchRequestLookup:
	/// Inherited from busMASBatchRequestLookupGen, this class is used to customize the lookup business object busMASBatchRequestLookupGen. 
	/// </summary>
	[Serializable]
	public class busMASBatchRequestLookup : busMASBatchRequestLookupGen
	{
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            string lstrBatchRequestType=string.Empty;
            // Batch Request Type is not entered
            if (Convert.ToString(ahstParam["astr_batch_request_type"]) == string.Empty)
            {
                utlError lobjError = null;
                lobjError = AddError(4814, string.Empty);
                larrErrors.Add(lobjError);            
            }

            //Group Type is not entered

            if (Convert.ToString(ahstParam["astr_group_type_value"]) == string.Empty)
            {
                utlError lobjError = null;
                lobjError = AddError(4815, string.Empty);
                larrErrors.Add(lobjError);
            }


            return larrErrors;
        }
      
	}
}
