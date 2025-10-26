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
    public class busBenefitCalculationLookup : busBenefitCalculationLookupGen
    {
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            string lstrBenefitType = string.Empty;
            int lintPersonID = 0;
            int lintPlanID = 0;
            // PERSLink ID not entered
            if (Convert.ToString(ahstParam["person_id"]) == string.Empty)
            {
                utlError lobjError = null;
                lobjError = AddError(1902, string.Empty);
                larrErrors.Add(lobjError);
            }
            // Plan ID not entered
            if (Convert.ToString(ahstParam["plan_id"]) == string.Empty)
            {
                utlError lobjError = null;
                lobjError = AddError(1057, string.Empty);
                larrErrors.Add(lobjError);
            }
            // Benefit Type not entered
            if (Convert.ToString(ahstParam["benefit_type"]) == string.Empty)
            {
                utlError lobjError = null;
                lobjError = AddError(1901, string.Empty);
                larrErrors.Add(lobjError);
            }
            else
            {
                // Estimate can be done only for Benefit Type of 'Retirement' and 'Disability'
                lstrBenefitType = Convert.ToString(ahstParam["benefit_type"]);
                if ((lstrBenefitType == busConstant.ApplicationBenefitTypePostRetirementDeath) ||
                    (lstrBenefitType == busConstant.ApplicationBenefitTypeRefund))
                {
                    utlError lobjError = null;
                    lobjError = AddError(1968, string.Empty);
                    larrErrors.Add(lobjError);
                }
            }
            if ((Convert.ToString(ahstParam["person_id"]) != string.Empty) &&
                (Convert.ToString(ahstParam["plan_id"]) != string.Empty))
            {
                lintPersonID = Convert.ToInt32(Convert.ToString(ahstParam["person_id"]));
                lintPlanID = Convert.ToInt32(Convert.ToString(ahstParam["plan_id"]));
                busPerson lbusPerson = new busPerson();
                lbusPerson.FindPerson(lintPersonID);
                busPersonAccount lobjPersonAccount = lbusPerson.LoadActivePersonAccountByPlan(lintPlanID);
                // No Person Account for Given Person and Plan
                if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == string.Empty)
                {
                    utlError lobjError = null;
                    lobjError = AddError(1914, string.Empty);
                    larrErrors.Add(lobjError);
                }
                // Check Person is Enrolled or Suspendend in the given Plan
                else if ((lobjPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled) &&
                        (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended))
                {
                    utlError lobjError = null;
                    lobjError = AddError(1967, string.Empty);
                    larrErrors.Add(lobjError);
                }
            }
            // UCS-060 BR-060-14
            if (lstrBenefitType == busConstant.ApplicationBenefitTypeDisability)
            {
                // 1. Current benefit type is Disability
                int lintPre_RTWPayeeAccountID = 0;
                bool lblnIsRTWMember = false;

                // 2. Determine whether Member Return to work (RTW)
                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                if (lobjPerson.FindPerson(lintPersonID))
                {
                    lblnIsRTWMember = lobjPerson.IsRTWMember(lintPlanID, busConstant.PayeeStatusForRTW.SuspendedOnly, ref lintPre_RTWPayeeAccountID);
                    if (lblnIsRTWMember)
                    {
                        if (lintPre_RTWPayeeAccountID > 0)
                        {
                            // 3. RTW Payee account is created for Benefit Type Retirement
                            busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                            lobjPayeeAccount.FindPayeeAccount(lintPre_RTWPayeeAccountID);
                            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                            {
                                if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO) ||
                                    (lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal))
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(7661, "");
                                    larrErrors.Add(lobjError);
                                }
                            }
                        }
                    }
                }
            }
            return larrErrors;
        }
    }
}
