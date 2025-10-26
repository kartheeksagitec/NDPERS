using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSBenefitPlan : busPersonAccount
    {
        public busMSSBenefitPlan()
        {
        }
        public busMSSGHDVWeb ibusMSSGHDVWeb { get; set; }
        public busMSSLifeWeb ibusMSSLifeWeb { get; set; }
        public busMSSLTCWeb ibusMSSLTCWeb { get; set; }
        public busMSSMedicarePartDWeb ibusMSSMedicarePartDWeb { get; set; }//PIR 15870

        //PIR 18503 - Wizard changes - Visibility Rule - Update Payment Method 
        public bool IsRetireePlanEnrolledOrIsCobra()
        {
            bool lblnResult = false;
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                // Check for GHDV Plans 
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision ||
                    lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                        lbusPersonAccount.LoadPersonAccountGHDV();

                    if ((lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                    {
                        busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(DateTime.Now);
                        if (lobjGHDVHistory.IsNotNull() && lobjGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
                //Medicare
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    lblnResult = true;
                    break;
                }

                // Check for Life Insurance Plan
                if (lbusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (lbusPersonAccount.ibusPersonAccountLife.IsNull())
                        lbusPersonAccount.LoadPersonAccountLife();
                    if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
                    {
                        lbusPersonAccount.ibusPersonAccountLife.LoadHistory();
                        if (lbusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                            o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date) &&
                                            o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }
    }
}
