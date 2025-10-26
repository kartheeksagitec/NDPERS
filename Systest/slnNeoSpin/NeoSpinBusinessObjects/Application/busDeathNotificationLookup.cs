#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busDeathNotificationLookup : busDeathNotificationLookupGen
	{

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busDeathNotification lobjDeathNotification = (busDeathNotification)aobjBus;
            lobjDeathNotification.ibusPerson = new busPerson();
            lobjDeathNotification.ibusPerson.icdoPerson = new cdoPerson();
            lobjDeathNotification.ibusPerson.icdoPerson.LoadData(adtrRow);
        }
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            if (ahstParam["person_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(176, "");
                larrErrors.Add(lobjError);
            }

            int lintPersonId = 0;
            if (int.TryParse(ahstParam["person_id"].ToString(), out lintPersonId))
            {
                busPerson lbusPerson = new busPerson();
                if (!lbusPerson.FindPerson(lintPersonId))
                {
                    utlError lobjError = null;
                    lobjError = AddError(2009, "");
                    larrErrors.Add(lobjError);
                }
            }
            return larrErrors;
        }
    }
}
