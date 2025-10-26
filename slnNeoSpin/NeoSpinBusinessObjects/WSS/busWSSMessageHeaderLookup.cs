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
	/// Class NeoSpin.BusinessObjects.busWSSMessageHeaderLookup:
	/// Inherited from busWSSMessageHeaderLookupGen, this class is used to customize the lookup business object busWSSMessageHeaderLookupGen. 
	/// </summary>
	[Serializable]
	public class busWSSMessageHeaderLookup : busWSSMessageHeaderLookupGen
	{
        //WSS message request PIR-17066
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            if ((Convert.ToString(ahstParam["astrMemberType"]) == string.Empty) || (ahstParam["astrMemberType"]) == "All")
            {
                utlError lobjutlError = null;
                lobjutlError = AddError(1132, "");
                larrErrors.Add(lobjutlError);
            }
            return larrErrors;
        }
	}
}
