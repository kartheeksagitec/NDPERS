#region Using directives

using System;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;
using System.Data;

#endregion

namespace NeoSpin.CustomDataObjects
{
    /// <summary>
    /// Class NeoSpin.CustomDataObjects.cdoWssBenApp:
    /// Inherited from doWssBenApp, the class is used to customize the database object doWssBenApp.
    /// </summary>
    [Serializable]
    public class cdoWssBenApp : doWssBenApp
    {
        public int wss_health_person_account_enrollment_request_id { get; set; }

        //public int member_age_year_part { get; set; }

        public string ben_elected_value { get; set; }

        public cdoWssBenApp() : base()
        {
        }
        public string istrDeferralValue { get; set; }
        public string istrRetirementDate { get; set; }

        public string istrDefferalDate { get; set; }
        public DateTime payment_date { get; set; }
        public string ref_dist_description
        {
            get
            {
                if (ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly)
                    return "Refund To Me Directly";
                else if (ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
                    return "Rollover part or all of my refund";
                else if (ref_dist_value == busConstant.RefundDistributionRolloverPartOfMyRefund)
                    return "Rollover Part Of My Refund";
                return string.Empty;
            }
        }
        public string sick_leave_flag_desc
        {
            get
            {
                if (sick_leave_purchase_indicated_flag == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string plso_requested_flag_desc
        {
            get
            {
                if (plso_requested_flag == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string rollover_plso_flag_desc
        {
            get
            {
                if (rollover_plso_flag == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string ref_state_tax_not_withhold_desc
        {
            get
            {
                if (ref_state_tax_not_withhold == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string is_social_security_applied_desc
        {
            get
            {
                if (is_social_security_applied == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string is_worker_comp_benefit_applied_desc
        {
            get
            {
                if (is_worker_comp_benefit_applied == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string continue_flex_med_spending_desc
        {
            get
            {
                if (continue_flex_med_spending == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string rhic_option_null_description
        {
            get
            {
                if (string.IsNullOrEmpty(rhic_option_value))
                    return busGlobalFunctions.GetData3ByCodeValue(1905, busConstant.RHICOptionStandard, iobjPassInfo);
                else
                    return busGlobalFunctions.GetData3ByCodeValue(1905, rhic_option_value, iobjPassInfo);
            }
        }
        public string pay_premium_pre_tax_desc
        {
            get
            {
                if (pay_premium_pre_tax == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }
        public string pay_premium_post_tax_desc
        {
            get
            {
                if (pay_premium_post_tax == busConstant.Flag_Yes)
                    return busConstant.Flag_Yes_Value;
                return busConstant.Flag_No_Value;
            }
        }

        public DateTime normal_retr_date { get; set; }
        public string sub_type_value { get; set; }
        public bool iblnIsEligibleForRtmt { get; set; }

        public string dep_medicare_plan_option_desc
        {
            get
            {
                DataTable ldtbCodeDesc = iobjPassInfo.isrvDBCache.GetCodeDescription(6003, dep_medicare_plan_option_value);
                return (ldtbCodeDesc.IsNotNull() && ldtbCodeDesc.Rows.Count > 0 && ldtbCodeDesc.Rows[0]["DESCRIPTION"] != DBNull.Value) ? 
                    Convert.ToString(ldtbCodeDesc.Rows[0]["DESCRIPTION"]) : string.Empty; 
            }
        }

        public string Plan_Name { get; set; } //PIR 18493 - To Display Plan Name on MSS - View App Status

        public string Full_Name { get; set; }//PIR 18493 - To Display Person Full Name on wfmWssBenefitApplicationLookupLOB
        public string mss_display_status { get; set; }
    }
}
