#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Data;
#endregion

namespace NeoSpin.CustomDataObjects
{
    /// <summary>
    /// Class NeoSpin.CustomDataObjects.cdoActuaryFilePensionDetail:
    /// Inherited from doActuaryFilePensionDetail, the class is used to customize the database object doActuaryFilePensionDetail.
    /// </summary>
    [Serializable]
    public class cdoActuaryFilePensionDetail : doActuaryFilePensionDetail
    {
        public cdoActuaryFilePensionDetail()
            : base()
        {
        }
        public DateTime date_of_birth { get; set; }
        public string ssn { get; set; }
        public string plan_name { get; set; }
        public string plan_name_formatted
        {
            get
            {
                if (!string.IsNullOrEmpty(plan_name) && plan_name.Length > 10)
                    return plan_name.Substring(0, 10);
                else
                    return plan_name;
            }
        }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string employmenttype { get; set; }
        public string hourly { get; set; }
        public string seasonal { get; set; }
        public string MemberGender { get; set; }
        public string SpouseGender { get; set; }
        public int plan_id { get; set; }
        public string org_code { get; set; }
        public string MemberGenderCode { get; set; }
        public string SpouseGenderCode { get; set; }
        public string current_plan_participation_code_value { get; set; }
        public string current_employment_status_value { get; set; }
        public string current_payee_status_value { get; set; }
        public string previous_plan_participation_code_value { get; set; }
        public string previous_employment_status_value { get; set; }
        public string previous_payee_status_value { get; set; }
        public DateTime date_of_decrement { get; set; }
        public string decrement_reason { get; set; }
        public DateTime employment_start_date { get; set; }
        public DateTime employment_end_date { get; set; }
        public DateTime joint_annuitant_dob_formatted
        {
            get
            {
                if(account_relation_value=="MEMB")
                    return joint_annuitant_dob;
                else
                    return DateTime.MinValue;
            }
        }
        public string   payee_status_data2 { get; set; }
        public string employment_status_value_formatted { get; set; }
        public string plan_participation_value_formatted { get; set; }
        public string current_plan_participation_value_formatted { get; set; }
        public string previous_plan_participation_value_formatted { get; set; }
        public string current_payee_status_data2 { get; set; }
        public string previous_payee_status_data2 { get; set; }
        public string payee_acct_exists { get; set; }
        public int totctr { get; set; }
        public int ehctr { get; set; }
        public int edctr { get; set; }
        public decimal tffr_tiaa_service { get; set; }
        public string emp_dt_chk { get; set; }
        public int ben_acc_owner_perslinkID { get; set; }
        public string graduated_benefit_option_percentage { get; set; }

        //public string current_plan_participation_value_formatted
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(current_plan_participation_code_value))
        //        {
        //            DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
        //                current_plan_participation_code_value);
        //            return ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
        //        }
        //        return string.Empty;
        //    }
        //}
        //public string previous_plan_participation_value_formatted
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(previous_plan_participation_code_value))
        //        {
        //            DataTable ldtbDataTable = iobjPassInfo.isrvDBCache.GetCodeDescription(337,
        //                previous_plan_participation_code_value);
        //            return ldtbDataTable.Rows.Count > 0 ? ldtbDataTable.Rows[0]["data2"].ToString() : string.Empty;
        //        }
        //        return string.Empty;
        //    }
        //}
    }
}