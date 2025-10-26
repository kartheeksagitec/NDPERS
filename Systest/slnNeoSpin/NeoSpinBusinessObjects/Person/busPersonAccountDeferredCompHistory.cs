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
	public class busPersonAccountDeferredCompHistory : busPersonAccountDeferredCompHistoryGen
	{
        public void Set457Limit(DateTime adtEffectiveDate)
        {
           /* PROD PIR 4014 and as per Maik's mail 
             * Regular – Current date is outside Catch-Up date range and member age is below 50
                Catch-Up – Current date is within Catch-Up date range and member age is below 50
                50+ – Member age on current date is above 50 regardless of catch-up date range
             * Changed on 20 Oct 2010
             */
            //Prod PIR 4366
            //457 limit should not be set for Plan Other 457
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.FindPersonAccount(icdoPersonAccountDeferredCompHistory.person_account_id);
            if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                //new logic as per Sharmain & Maik 
                //Prod PIR 4457
                if ((icdoPersonAccountDeferredCompHistory.catch_up_start_date != DateTime.MinValue) && (icdoPersonAccountDeferredCompHistory.catch_up_end_date != DateTime.MinValue) &&
                    busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, new DateTime(icdoPersonAccountDeferredCompHistory.catch_up_start_date.Year, 1, 1),
                    new DateTime(icdoPersonAccountDeferredCompHistory.catch_up_end_date.Year, 12, 31)))
                {
                    icdoPersonAccountDeferredCompHistory.limit_457_value = busConstant.PersonAccount457LimitCatchUp;
                }
                else if (CalculateDeffCompEnrollingPersonAge(adtEffectiveDate) >= 50)
                {
                    icdoPersonAccountDeferredCompHistory.limit_457_value = busConstant.PersonAccount457Limit50;
                }
                else
                {
                    icdoPersonAccountDeferredCompHistory.limit_457_value = busConstant.PersonAccount457LimitRegular;
                }
            }
            else
            {
                icdoPersonAccountDeferredCompHistory.limit_457_value = null;                
            }
        }

        public int CalculateDeffCompEnrollingPersonAge(DateTime adtEffectiveDate)
        {
            // get the difference in years
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icdoPerson.person_id > 0)
            {
                DateTime ldtPersonDateOfBirth = ibusPersonAccount.ibusPerson.icdoPerson.date_of_birth;
                DateTime ldtCalculationDate = new DateTime(adtEffectiveDate.Year, 12, 31);
                return busGlobalFunctions.CalulateAge(ldtPersonDateOfBirth, ldtCalculationDate);
            }
            return 0;
        }
	}
}
