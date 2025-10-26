#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPersonEmploymentDetail : doPersonEmploymentDetail
    {
        public cdoPersonEmploymentDetail()
            : base()
        {
        }
        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                else
                    return end_date;
            }
        }

        //Date of last paycheck 'PIR-8298'
        //public DateTime date_of_last_paycheck { get; set; } 

        public bool is_recertified
        {
            get
            {
                if (recertified_date == DateTime.MinValue)
                {
                    return false;
                }
                return true;
            }
        }

        public string istrSuppressWarnings { get; set; }

        public string start_long_date
        {
            get
            {
                return start_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public string derived_member_type_value { get; set; }
        public string istrBenefitPlanForRetr { get; set; }
        public string istrRHICBenefitPlanForRetr { get; set; }

        public string derived_member_type_description { get; set; }

        public string end_date_long_date
        {
            get
            {
                if (end_date != DateTime.MinValue)
                {
                    return end_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
                }
                else
                    return string.Empty;
            }
        }

        public string new_job_class_value { get; set; }

        public string new_official_list_value { get; set; }

        public string new_type_value { get; set; }

        public string new_status_value { get; set; }

        public int person_id { get; set; }

        public int org_id { get; set; }
        public override int Insert()
        {
            int lintReturnValue = base.Insert();
            //If Employment ID is in ACA Cert and new and previous Employment Details are Temp – Update PER_EMP_DTL_ID to new Emp Dtl ID (all information carries forward as is)
            if (lintReturnValue > 0 && type_value == NeoSpin.BusinessObjects.busConstant.PersonJobTypeTemporary && person_employment_id > 0 && person_employment_dtl_id > 0)
            {
                System.Data.DataTable ldtbPreviousEmpDetail = Sagitec.BusinessObjects.busBase.Select("entPersonEmploymentDetail.LoadPreviousEmploymentDetailWithinSameOrg", new object[2] { person_employment_id, person_employment_dtl_id });
                if (ldtbPreviousEmpDetail.Rows.Count > 0)
                {
                    cdoPersonEmploymentDetail lcdoPreviousEmpDetail = new cdoPersonEmploymentDetail();
                    lcdoPreviousEmpDetail.LoadData(ldtbPreviousEmpDetail.Rows[0]);
                    if (lcdoPreviousEmpDetail.type_value == NeoSpin.BusinessObjects.busConstant.PersonJobTypeTemporary)
                    {
                        System.Data.DataTable ldtACAEligibilityCertification = Sagitec.BusinessObjects.busBase.Select<cdoWssEmploymentAcaCert>(new string[2] { enmWssEmploymentAcaCert.person_employment_id.ToString(), enmWssEmploymentAcaCert.per_emp_dtl_id.ToString() },
                                                                        new object[2] { person_employment_id, lcdoPreviousEmpDetail.person_employment_dtl_id }, null, "wss_employment_aca_cert_id desc");
                        if (ldtACAEligibilityCertification.Rows.Count > 0)
                        {
                            cdoWssEmploymentAcaCert lcdoWssEmploymentAcaCert = new cdoWssEmploymentAcaCert();
                            lcdoWssEmploymentAcaCert.LoadData(ldtACAEligibilityCertification.Rows[0]);
                            if (lcdoWssEmploymentAcaCert.per_emp_dtl_id != person_employment_dtl_id)
                            {
                                lcdoWssEmploymentAcaCert.per_emp_dtl_id = person_employment_dtl_id;
                                lcdoWssEmploymentAcaCert.Update();
                            }
                        }
                    }
                }
            }
            if (lintReturnValue > 0 && person_employment_id > 0 && person_employment_dtl_id > 0)
            {
                DataTable ldtbList = busBase.Select("entServicePurchaseHeader.ServicePurchasePaymentInstallments",
                                        new object[1] { person_id });
                if (ldtbList.Rows.Count > 0)
                {
                    DataTable ldtbPreviousEmplDetail = busBase.Select("entPersonEmploymentDetail.LoadPreviousEmploymentDetail", new object[1] { person_employment_id });
                    if (ldtbPreviousEmplDetail.Rows.Count > 0)
                    {
                        cdoPersonEmploymentDetail lcdoPreviousEmpDetail = new cdoPersonEmploymentDetail();
                        lcdoPreviousEmpDetail.LoadData(ldtbPreviousEmplDetail.Rows[0]);
                        if (lcdoPreviousEmpDetail.IsNotNull() &&
                            (type_value == busConstant.PersonJobTypeTemporary || status_value == busConstant.EmploymentStatusNonContributing))
                        {
                            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = this };
                            if (lobjPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                                lobjPersonEmploymentDetail.LoadPersonEmployment();

                            lobjPersonEmploymentDetail.InitiateWorkFlowForServicePurchasePaymentInstallmentsEmploymentChange(lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id);
                        }
                    }
                }               
            }
            return lintReturnValue;
        }
    }
}
