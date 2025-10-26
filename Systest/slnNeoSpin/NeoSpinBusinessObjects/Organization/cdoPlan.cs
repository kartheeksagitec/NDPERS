#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPlan : doPlan
    {
        public cdoPlan()
            : base()
        {
        }

        /// In Correspondence
        public string istr_PLAN_NAME
        {
            get
            {
                if (!string.IsNullOrEmpty(plan_name))
                    return plan_name.ToUpper();
                else
                    return plan_name;
            }
        }

        /// In Correspondence
        public string istr_BENEFIT_TYPE_DESCRIPTION
        {
            get
            {
                if (!string.IsNullOrEmpty(benefit_type_description))
                    return benefit_type_description.ToUpper();
                else
                    return benefit_type_description;
            }
        }

        public bool IsPlanJobService
        {
            get
            {
                if (plan_id == BusinessObjects.busConstant.PlanIdJobService)
                    return true;
                else
                    return false;
            }
        }

        public int job_service_sort_order
        {
            get
            {
                if (plan_id == BusinessObjects.busConstant.PlanIdJobService)
                {
                    return 1;
                }
                return 2;
            }
        }

        public string full_plan_name
        {
            get
            {
                if (plan_id == BusinessObjects.busConstant.PlanIdEAP)
                    return "Employee Assistance Program";
                if (plan_id == BusinessObjects.busConstant.PlanIdLTC)
                    return "Long Term Care";
                if (plan_id == BusinessObjects.busConstant.PlanIdFlex)
                    return "FlexComp";
                if (plan_id == BusinessObjects.busConstant.PlanIdGroupHealth ||
                    plan_id == BusinessObjects.busConstant.PlanIdDental ||
                    plan_id == BusinessObjects.busConstant.PlanIdVision ||
                    plan_id == BusinessObjects.busConstant.PlanIdGroupLife)
                    return plan_name + " Insurance";
                return plan_name;
            }
        }

        // PROD PIR ID 6964
        public string mss_plan_name
        {
            get
            {
                //PIR 25920  New DC plan
                if (retirement_type_value == BusinessObjects.busConstant.PlanRetirementTypeValueDB || retirement_type_value == BusinessObjects.busConstant.PlanRetirementTypeValueHB ||
                    retirement_type_value == BusinessObjects.busConstant.PlanRetirementTypeValueDC)
                    return plan_name + " " + benefit_type_description;
                else
                    return full_plan_name;
            }
        }
    }
}
