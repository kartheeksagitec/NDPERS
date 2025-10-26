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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busVoyaFileOut : busFileBaseOut
    {
        private Collection<busVoyaPortFile> _iclbLifeInsurancePortibility;
        public Collection<busVoyaPortFile> iclbLifeInsurancePortibility
        {
            get { return _iclbLifeInsurancePortibility; }
            set { _iclbLifeInsurancePortibility = value; }
        }

        private int _iintRecordCount;
        public int iintRecordCount
        {
            get { return _iintRecordCount; }
            set { _iintRecordCount = value; }
        }


        private DateTime _idtCurrentDate;
        public DateTime idtCurrentDate
        {
            get { return _idtCurrentDate; }
            set { _idtCurrentDate = value; }
        }


        public override void InitializeFile()
        {
            istrFileName = "673897_NDPERS_PC_" + DateTime.Now.ToString(busConstant.DateFormatMMDDYYYY) + busConstant.FileFormatcsv;
        }

        public void LoadLifeInsurancePlanPortibility(DataTable adtbLifeInsurancePortibility)
        {
            adtbLifeInsurancePortibility = new DataTable();
            _iclbLifeInsurancePortibility = new Collection<busVoyaPortFile>();
            adtbLifeInsurancePortibility = busBase.Select("cdoPerson.LoadLifeInsurancePortability", new object[] { });
            
            foreach (DataRow dr in adtbLifeInsurancePortibility.Rows)
            {
                busVoyaPortFile lobjLifeInsurancePortability = new busVoyaPortFile();

                if (!Convert.IsDBNull(dr["ssn"]))
                    lobjLifeInsurancePortability.ssn = dr["ssn"].ToString();

                StringBuilder sbSSN = new StringBuilder();
                sbSSN.Append("\"");
                foreach (char nextChar in lobjLifeInsurancePortability.ssn)
                {
                    sbSSN.Append(nextChar);
                    if (nextChar == '"')
                        sbSSN.Append("\"");
                }
                sbSSN.Append("\"");

                lobjLifeInsurancePortability.ssn = sbSSN.ToString();

                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjLifeInsurancePortability.first_name = dr["first_name"].ToString();

                if (!Convert.IsDBNull(dr["middle_name"]))
                    lobjLifeInsurancePortability.middle_name = dr["middle_name"].ToString();

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjLifeInsurancePortability.last_name = dr["last_name"].ToString();

                if (!Convert.IsDBNull(dr["date_of_birth"]))
                    lobjLifeInsurancePortability.date_of_birth = Convert.ToDateTime(dr["date_of_birth"]);

                if (!Convert.IsDBNull(dr["addr_line_1"]))
                    lobjLifeInsurancePortability.address_line_1 = dr["addr_line_1"].ToString();

                StringBuilder sbAddr1 = new StringBuilder();
                sbAddr1.Append("\"");
                foreach (char nextChar in lobjLifeInsurancePortability.address_line_1)
                {
                    sbAddr1.Append(nextChar);
                    if (nextChar == '"')
                        sbAddr1.Append("\"");
                }
                sbAddr1.Append("\"");

                lobjLifeInsurancePortability.address_line_1 = sbAddr1.ToString();

                if (!Convert.IsDBNull(dr["addr_line_2"]))
                    lobjLifeInsurancePortability.address_line_2 = dr["addr_line_2"].ToString();

                StringBuilder sbAddr2 = new StringBuilder();
                sbAddr2.Append("\"");
                foreach (char nextChar in lobjLifeInsurancePortability.address_line_2)
                {
                    sbAddr2.Append(nextChar);
                    if (nextChar == '"')
                        sbAddr2.Append("\"");
                }
                sbAddr2.Append("\"");

                lobjLifeInsurancePortability.address_line_2 = sbAddr2.ToString();

                if (!Convert.IsDBNull(dr["addr_city"]))
                    lobjLifeInsurancePortability.city = dr["addr_city"].ToString();

                if (!Convert.IsDBNull(dr["addr_state_value"]))
                    lobjLifeInsurancePortability.state = dr["addr_state_value"].ToString();

                if (!Convert.IsDBNull(dr["zip_code"]))
                    lobjLifeInsurancePortability.zip_code = dr["zip_code"].ToString();

                if (!Convert.IsDBNull(dr["zip_code_4"]))
                    lobjLifeInsurancePortability.zip_code_4 = dr["zip_code_4"].ToString();

                if (!Convert.IsDBNull(dr["date_of_hire"]))
                    lobjLifeInsurancePortability.date_of_hire = Convert.ToDateTime(dr["date_of_hire"]);

                if (!Convert.IsDBNull(dr["annual_salary_amount"]))
                    lobjLifeInsurancePortability.annual_salary_amount = (dr["annual_salary_amount"]).ToString();

                if (!Convert.IsDBNull(dr["group_name"]))
                    lobjLifeInsurancePortability.group_name = dr["group_name"].ToString();

                if (!Convert.IsDBNull(dr["group_number"]))
                    lobjLifeInsurancePortability.group_number = Convert.ToInt32(dr["group_number"]);

                if (!Convert.IsDBNull(dr["account_number"]))
                    lobjLifeInsurancePortability.account_number = Convert.ToInt32(dr["account_number"]);

                if (!Convert.IsDBNull(dr["ee_class"]))
                    lobjLifeInsurancePortability.ee_class = dr["ee_class"].ToString();

                if (!Convert.IsDBNull(dr["tobacco_status"]))
                    lobjLifeInsurancePortability.tobacco_status = dr["tobacco_status"].ToString();

                if (!Convert.IsDBNull(dr["home_phone"]))
                    lobjLifeInsurancePortability.home_phone = dr["home_phone"].ToString();

                if (!Convert.IsDBNull(dr["employment_termination_date"]))
                    lobjLifeInsurancePortability.employment_termination_date = Convert.ToDateTime(dr["employment_termination_date"]);

                if (!Convert.IsDBNull(dr["insurance_termination_date"]))
                    lobjLifeInsurancePortability.insurance_termination_date = Convert.ToDateTime(dr["insurance_termination_date"]);

                if (!Convert.IsDBNull(dr["additional_comment"]))
                    lobjLifeInsurancePortability.additional_comment = dr["additional_comment"].ToString();

                if (!Convert.IsDBNull(dr["reason_for_termination"]))
                    lobjLifeInsurancePortability.reason_for_termination = dr["reason_for_termination"].ToString();

                if (!Convert.IsDBNull(dr["employee_basic_coverage_amount"]))
                    lobjLifeInsurancePortability.employee_basic_coverage_amount = Convert.ToDecimal(dr["employee_basic_coverage_amount"]);

                if (!Convert.IsDBNull(dr["employee_supplemental_coverage_amount"]))
                    lobjLifeInsurancePortability.employee_supplemental_coverage_amount = Convert.ToDecimal(dr["employee_supplemental_coverage_amount"]);

                if (!Convert.IsDBNull(dr["employee_basic_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.employee_basic_ad_d_coverage_amount = Convert.ToDecimal(dr["employee_basic_ad_d_coverage_amount"]);

                if (!Convert.IsDBNull(dr["employee_supplemental_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.employee_supplemental_ad_d_coverage_amount = Convert.ToDecimal(dr["employee_supplemental_ad_d_coverage_amount"]);

                if (!Convert.IsDBNull(dr["additional_coverage_amount_ee"]))
                    lobjLifeInsurancePortability.additional_coverage_amount_ee = (dr["additional_coverage_amount_ee"]).ToString();

                if (!Convert.IsDBNull(dr["additional_coverage_amount_fam"]))
                    lobjLifeInsurancePortability.additional_coverage_amount_fam = (dr["additional_coverage_amount_fam"]).ToString();

                if (!Convert.IsDBNull(dr["sp_last_name"]))
                    lobjLifeInsurancePortability.sp_last_name = dr["sp_last_name"].ToString();

                if (!Convert.IsDBNull(dr["sp_first_name"]))
                    lobjLifeInsurancePortability.sp_first_name = dr["sp_first_name"].ToString();

                if (!Convert.IsDBNull(dr["sp_ssn"]))
                    lobjLifeInsurancePortability.sp_ssn = dr["sp_ssn"].ToString();

                if (!Convert.IsDBNull(dr["sp_date_of_birth"]))
                    lobjLifeInsurancePortability.sp_date_of_birth = Convert.ToDateTime(dr["sp_date_of_birth"]);

                if (!Convert.IsDBNull(dr["sp_tobacco_status"]))
                    lobjLifeInsurancePortability.sp_tobacco_status = dr["sp_tobacco_status"].ToString();

                if (!Convert.IsDBNull(dr["spouse_basic_coverage_amount"]))
                    lobjLifeInsurancePortability.spouse_basic_coverage_amount = (dr["spouse_basic_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["spouse_supplemental_coverage_amount"]))
                    lobjLifeInsurancePortability.spouse_supplemental_coverage_amount = Convert.ToDecimal(dr["spouse_supplemental_coverage_amount"]);

                if (!Convert.IsDBNull(dr["spouse_basic_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.spouse_basic_ad_d_coverage_amount = (dr["spouse_basic_ad_d_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["spouse_supplemental_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.spouse_supplemental_ad_d_coverage_amount = (dr["spouse_supplemental_ad_d_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["child_basic_coverage_amount"]))
                    lobjLifeInsurancePortability.child_basic_coverage_amount = (dr["child_basic_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["child_supplemental_coverage_amount"]))
                    lobjLifeInsurancePortability.child_supplemental_coverage_amount = Convert.ToDecimal(dr["child_supplemental_coverage_amount"]);

                if (!Convert.IsDBNull(dr["child_basic_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.child_basic_ad_d_coverage_amount = (dr["child_basic_ad_d_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["child_supplemental_ad_d_coverage_amount"]))
                    lobjLifeInsurancePortability.child_supplemental_ad_d_coverage_amount = (dr["child_supplemental_ad_d_coverage_amount"].ToString());

                if (!Convert.IsDBNull(dr["additional_info_field_1"]))
                    lobjLifeInsurancePortability.additional_info_field_1 = dr["additional_info_field_1"].ToString();

                if (!Convert.IsDBNull(dr["additional_info_field_2"]))
                    lobjLifeInsurancePortability.additional_info_field_2 = dr["additional_info_field_2"].ToString();

                _iclbLifeInsurancePortibility.Add(lobjLifeInsurancePortability);
            }

            iintRecordCount = adtbLifeInsurancePortibility.Rows.Count;

            idtCurrentDate = DateTime.Now;
            
        }

        public override void FinalizeFile()
        {
            DBFunction.DBNonQuery("cdoPerson.UpdateVoyaSentFlagForTerminatedMembers", new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            DBFunction.DBNonQuery("cdoPerson.UpdateRetVoyaSentFlagForRetiredMembers", new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
 	        base.FinalizeFile();
        }
    }
}
