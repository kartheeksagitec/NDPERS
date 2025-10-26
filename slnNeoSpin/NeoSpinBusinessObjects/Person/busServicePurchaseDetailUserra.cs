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
    public class busServicePurchaseDetailUserra : busServicePurchaseDetailUserraGen
    {
        private busServicePurchaseDetail _ibusServicePurchaseDetail;
        public busServicePurchaseDetail ibusServicePurchaseDetail
        {
            get
            {
                return _ibusServicePurchaseDetail;
            }
            set
            {
                _ibusServicePurchaseDetail = value;
            }
        }

        public void LoadServicePurchaseDetail()
        {

            DataTable ldtbList = Select<cdoServicePurchaseDetail>(
             new string[1] { "service_purchase_detail_id" },
             new object[1] { icdoServicePurchaseDetailUserra.service_purchase_detail_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ibusServicePurchaseDetail = new busServicePurchaseDetail();
                ibusServicePurchaseDetail.icdoServicePurchaseDetail = new cdoServicePurchaseDetail();
                ibusServicePurchaseDetail.icdoServicePurchaseDetail.LoadData(ldtbList.Rows[0]);
            }
        }

        public override void BeforePersistChanges()
        {
            CalculateContributionAmounts();
            base.BeforePersistChanges();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!String.IsNullOrEmpty(icdoServicePurchaseDetailUserra.idtMissingMonthFormatted))
            {
                icdoServicePurchaseDetailUserra.missed_salary_month = Convert.ToDateTime(icdoServicePurchaseDetailUserra.idtMissingMonthFormatted.ToString());
            }
            else
            {
                icdoServicePurchaseDetailUserra.idtMissingMonthFormatted = String.Empty;
            }
            base.BeforeValidate(aenmPageMode);
        }
        public void CalculateContributionAmounts()
        {
            // Before the value is getting stored in the database, we need to calculate the Employee Contribution, Employee Contribution and RHIC Contribution
            // Do the calc only when missed_salary_amount is entered.
            // Refer to UCS-044 BR-044-33
            if (icdoServicePurchaseDetailUserra.missed_salary_amount > 0)
            {
                cdoPlanRetirementRate lobjcdoPlanRetirementRate =
                    busGlobalFunctions.GetRetirementRateForPlanDateCombination(
                        ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id,
                        icdoServicePurchaseDetailUserra.missed_salary_month,
                        ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.member_type_value
                        );
                //TODO check with Jeeva whether the value will be stored as /100 or multiplied by 100
                // TODO check with Jeeva whether the calculation should be done for only specific plans ?? as mentioned in the Use Case
                if (lobjcdoPlanRetirementRate != null && lobjcdoPlanRetirementRate.plan_rate_id > 0)
                {

                    decimal ldecEmployeeRate = ((lobjcdoPlanRetirementRate.ee_pre_tax / 100) +
                                                (lobjcdoPlanRetirementRate.ee_post_tax / 100));
                                                //+(lobjcdoPlanRetirementRate.ee_emp_pickup / 100)); //pir 8728
                    decimal ldecEmployeePickup = (lobjcdoPlanRetirementRate.ee_emp_pickup / 100);//pir 8728
                    decimal ldecEmployerRate = lobjcdoPlanRetirementRate.er_post_tax / 100;

                    // as per BR-044-63, there is no employee RHIC portion under USERRA
                    decimal ldecRHICRate = (lobjcdoPlanRetirementRate.er_rhic / 100);
                    //PIR - 14656  - Start - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)
                    if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.IsNull()) ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
                    if (!string.IsNullOrEmpty(ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib) &&
                        ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes)
                    {
                        ldecEmployeeRate = (((lobjcdoPlanRetirementRate.ee_pre_tax + lobjcdoPlanRetirementRate.addl_ee_pre_tax) / 100) +
                                                ((lobjcdoPlanRetirementRate.ee_post_tax + lobjcdoPlanRetirementRate.addl_ee_post_tax) / 100));
                        ldecEmployeePickup = ((lobjcdoPlanRetirementRate.ee_emp_pickup + lobjcdoPlanRetirementRate.addl_ee_emp_pickup) / 100);
                    }
                    //PIR - 14656 - End - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)

                    icdoServicePurchaseDetailUserra.employer_contribution =
                        Math.Round(icdoServicePurchaseDetailUserra.missed_salary_amount * ldecEmployerRate, 2, MidpointRounding.AwayFromZero);

                    icdoServicePurchaseDetailUserra.employee_contribution =
                         Math.Round(icdoServicePurchaseDetailUserra.missed_salary_amount * ldecEmployeeRate, 2, MidpointRounding.AwayFromZero);
                    //pir 8728
                    icdoServicePurchaseDetailUserra.employee_pickup =
                         Math.Round(icdoServicePurchaseDetailUserra.missed_salary_amount * ldecEmployeePickup, 2, MidpointRounding.AwayFromZero);
                    icdoServicePurchaseDetailUserra.rhic_contribution =
                         Math.Round(icdoServicePurchaseDetailUserra.missed_salary_amount * ldecRHICRate, 2, MidpointRounding.AwayFromZero);
                }
            }
        }

        // get member's actual age
        public int GetMemberAge()
        {
            return ibusServicePurchaseDetail.iintMemberAge(icdoServicePurchaseDetailUserra.missed_salary_month);

        }

        //UAT PIR 1018
        public bool IsMissedSalaryOverlapWithActiveDutyDate()
        {
            bool lblnResult = false;
            DateTime ldtStartDate =
                new DateTime(ibusServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_start_date.Year,
                             ibusServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_start_date.Month, 1);

            DateTime ldtEndDate =
                new DateTime(ibusServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_end_date.Year,
                             ibusServicePurchaseDetail.icdoServicePurchaseDetail.userra_active_duty_end_date.Month, 1).
                    AddMonths(1).AddDays(-1);

            if (busGlobalFunctions.CheckDateOverlapping(icdoServicePurchaseDetailUserra.missed_salary_month, ldtStartDate, ldtEndDate))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public override void AfterPersistChanges()
        {
            //PIR 712: Recompute the Header Level Fields (Update the Total Contract Amount and etc..)
            ibusServicePurchaseDetail.RecomputeCalculatedFields();
            ibusServicePurchaseDetail.icdoServicePurchaseDetail.Update();

            //Assigning this Detail Object to Header Primary Detail Object So that Calculation takes the latest values
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail = ibusServicePurchaseDetail;

            if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments != 0)
            {
                //PIR 913 : Every time when we add / modify the amount, we need to repopulate the expected payment amount
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount = 0;
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadAmortizationSchedule();
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.iblnIsPaymentElectionChanged = true;
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.UpdatePaymentElection();
            }

            //Update the Total Contract Amount            
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.CalculateTotalContractAmount();
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.Update();

            base.AfterPersistChanges();
        }
    }
}
