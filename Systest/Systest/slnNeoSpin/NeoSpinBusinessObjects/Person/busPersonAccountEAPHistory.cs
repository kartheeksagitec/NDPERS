#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountEAPHistory : busPersonAccountEAPHistoryGen
    {
        /// <summary>
        /// This Method Gets the EAP objects as param and returns the same with old value. 
        /// It needs the object in param because of the start date.
        /// </summary>
        /// <param name="abusPersonAccountEap"></param>
        /// <returns></returns>
        public busPersonAccountEAP LoadEAPObject(busPersonAccountEAP abusPersonAccountEAP)
        {
            if (abusPersonAccountEAP.icdoPersonAccount.start_date == DateTime.MinValue)
                abusPersonAccountEAP.icdoPersonAccount.start_date = icdoPersonAccountEapHistory.start_date;
            abusPersonAccountEAP.icdoPersonAccount.history_change_date = icdoPersonAccountEapHistory.start_date;
            abusPersonAccountEAP.icdoPersonAccount.end_date = icdoPersonAccountEapHistory.end_date;
            abusPersonAccountEAP.icdoPersonAccount.provider_org_id = icdoPersonAccountEapHistory.provider_org_id;
            abusPersonAccountEAP.icdoPersonAccount.plan_participation_status_value =
                icdoPersonAccountEapHistory.plan_participation_status_value;
            abusPersonAccountEAP.icdoPersonAccount.status_value = icdoPersonAccountEapHistory.status_value;
            abusPersonAccountEAP.icdoPersonAccount.from_person_account_id =
                icdoPersonAccountEapHistory.from_person_account_id;
            abusPersonAccountEAP.icdoPersonAccount.to_person_account_id =
                icdoPersonAccountEapHistory.to_person_account_id;
            return abusPersonAccountEAP;
        }
    }
}
