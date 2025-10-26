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
    public partial class busPlan : busExtendBase
    {
        private Collection<busPlanRetirementRate> _iclbRetirementRate;
        public Collection<busPlanRetirementRate> iclbRetirementRate
        {
            get { return _iclbRetirementRate; }
            set { _iclbRetirementRate = value; }
        }
        public void LoadRetirementRates()
        {
            DataTable ldtbList = Select<cdoPlanRetirementRate>(
                  new string[1] { "plan_id" },
                  new object[1] { icdoPlan.plan_id }, null, "effective_date desc");
            _iclbRetirementRate = GetCollection<busPlanRetirementRate>(ldtbList, "icdoPlanRetirementRate");
        }

        public bool FindPlanByPlanCode(string astrPlanCode)
        {
            if (icdoPlan == null)
            {
                icdoPlan = new cdoPlan();
            }
            DataTable ldtbList = Select<cdoPlan>(new string[1] { "plan_code" },
                  new object[1] { astrPlanCode }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoPlan.LoadData(ldtbList.Rows[0]);
                return true;
            }
            return false;
        }

        public bool IsInsurancePlan()
        {
            return (icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance);
        }

        public bool IsRetirementPlan()
        {
            return (icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement);
        }

        public bool IsDeferredCompPlan()
        {
            return (icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp);
        }

        public bool IsDBRetirementPlan()
        {
            return ((icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement) &&
                (icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)); 
        }
        public bool IsHBRetirementPlan()
        {
            return ((icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement) &&
                   (icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB));  //PIR 25920  New DC plan
        }
        public bool IsDCRetirementPlan()
        {
            return ((icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement) &&
                (icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC));
        }

        public bool IsJobServicePlan()
        {
            // PROD PIR ID 5522
            // Even for Plan Job service 3rd party payer, the Applied amount should be consider as JS RHIC amount
            if (icdoPlan.plan_id == busConstant.PlanIdJobService || icdoPlan.plan_id == busConstant.PlanIdJobService3rdPartyPayor)
                return true;
            return false;
        }

        public bool IsNDPERSPlan()
        {
            if ((icdoPlan.plan_id == busConstant.PlanIdMain) ||
                (icdoPlan.plan_id == busConstant.PlanIdMain2020) || //PIR 20232
                (icdoPlan.plan_id == busConstant.PlanIdLE) ||
                (icdoPlan.plan_id == busConstant.PlanIdLEWithoutPS) ||
                (icdoPlan.plan_id == busConstant.PlanIdHP) ||
                (icdoPlan.plan_id == busConstant.PlanIdJudges) ||
                (icdoPlan.plan_id == busConstant.PlanIdNG) ||
                (icdoPlan.plan_id == busConstant.PlanIdBCILawEnf) || //pir 7943
                (icdoPlan.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                return true;
            else
                return false;
        }

        public bool IsGHDVPlan()
        {
            if ((icdoPlan.plan_id == busConstant.PlanIdGroupHealth) ||
                        (icdoPlan.plan_id == busConstant.PlanIdHMO) || (icdoPlan.plan_id == busConstant.PlanIdDental) ||
                        (icdoPlan.plan_id == busConstant.PlanIdVision))
            {
                return true;
            }
            return false;
        }
    }
}
