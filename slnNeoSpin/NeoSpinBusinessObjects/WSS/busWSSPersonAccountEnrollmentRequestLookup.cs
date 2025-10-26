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
	/// Class NeoSpin.BusinessObjects.busWSSPersonAccountEnrollmentRequestLookup:
	/// Inherited from busWSSPersonAccountEnrollmentRequestLookupGen, this class is used to customize the lookup business object busWSSPersonAccountEnrollmentRequestLookupGen. 
	/// </summary>
	[Serializable]
	public class busWSSPersonAccountEnrollmentRequestLookup : busWSSPersonAccountEnrollmentRequestLookupGen
	{
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busWssPersonAccountEnrollmentRequest lobjEnrollmentrequest = (busWssPersonAccountEnrollmentRequest)aobjBus;
            lobjEnrollmentrequest.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjEnrollmentrequest.ibusPerson.icdoPerson.LoadData(adtrRow);
            lobjEnrollmentrequest.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lobjEnrollmentrequest.ibusPlan.icdoPlan.LoadData(adtrRow);
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
	}
}
