using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using System.Linq;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    public static class busPersonAccountHelper
    {
        public static DataTable DeterminePlan(string astrJobClass, string astrJobType)
        {
            DataTable ldtbPlan = busNeoSpinBase.Select("cdoPersonAccount.DeterminePlan",
                                                                new object[2] { astrJobType, astrJobClass });
            return ldtbPlan;
        }

        /// <summary>
        /// Returns Member Account ID if Member is active for the plan.
        /// </summary>
        /// <param name="AintPlanID">Plan ID</param>
        /// <param name="AintPersonID">Person ID</param>
        /// <returns>Personc Account ID</returns>
        public static int GetPersonAccountID(int AintPlanID, int AintPersonID)
        {
            int lintPersonAccountID = 0;
            DataTable ldtbGetPersonAccountID = busBase.Select("cdoPersonAccount.GetActivePersonAccount", new object[2] { AintPersonID, AintPlanID });
            if (ldtbGetPersonAccountID.Rows.Count > 0)
            {
                lintPersonAccountID = Convert.ToInt32(ldtbGetPersonAccountID.Rows[0]["person_account_id"]);
            }
            return lintPersonAccountID;
        }        
    }
}
