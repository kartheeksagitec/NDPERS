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
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitGhdvDeduction : busBenefitGhdvDeductionGen
    {
        public busPersonAccountGhdv ibusPersonAccountGHDV { get; set; }
        public busOrgPlan ibusOrgPlan { get; set; }
        public void LoadGHDVObjectFromDeduction()
        {
            if (ibusPersonAccountGHDV == null)
            {
                ibusPersonAccountGHDV = new busPersonAccountGhdv();
                ibusPersonAccountGHDV.icdoPersonAccount = new cdoPersonAccount();
                ibusPersonAccountGHDV.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
            }
            ibusPersonAccountGHDV.icdoPersonAccount.plan_id = busConstant.PlanIdGroupHealth;
            ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = icdoBenefitGhdvDeduction.health_insurance_type_value;
            ibusPersonAccountGHDV.icdoPersonAccountGhdv.low_income_credit = icdoBenefitGhdvDeduction.low_income_credit;
            ibusPersonAccountGHDV.icdoPersonAccountGhdv.plan_option_value = icdoBenefitGhdvDeduction.plan_option_value;
            ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = icdoBenefitGhdvDeduction.cobra_type_value;
            ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = icdoBenefitGhdvDeduction.coverage_code;
            ibusPersonAccountGHDV.ibusOrgPlan = ibusOrgPlan;
            ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(DateTime.Now);
        }
    }
}
