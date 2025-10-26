using System;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;
using System.Linq;

namespace NeoSpinBatch
{
    class busLifeAgeChangeBatch : busNeoSpinBatch
    {
        DateTime idtBatchRunDate;
        DateTime idtEffectiveStartDate;
        public DataTable idtbCachedLifeRate { get; set; }

        public void LoadLifeAgeChangeMembers()
        {
            idtBatchRunDate = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(1).Year, 1, 1);
            idtEffectiveStartDate = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.AddYears(1).Year, 1, 1);
            istrProcessName = "Life Age Change Batch";
            idlgUpdateProcessLog("Loading all Life Age Change members", "INFO", istrProcessName);
            DataTable ldtbLifeAgeChange = busBase.Select("cdoPersonAccount.LifeAgeChangeForBenefitEnrollment",
                new object[1] { idtBatchRunDate });

            foreach (DataRow ldrRow in ldtbLifeAgeChange.Rows)
            {
                try
                {
                    busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
                    lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

                    decimal ldecADAndDBasicRate = 0.0000M;
                    decimal ldecADAndDSupplementalRate = 0.0000M;
                    decimal ldecEmployerPremiumAmount = 0.0M;
                    decimal ldecEmployeePremiumAmount = 0.0M;

                    busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife();
                    lobjPersonAccountLife = new busPersonAccountLife() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
                    lobjPersonAccountLife.FindPersonAccountLife(Convert.ToInt32(ldrRow["PERSON_ACCOUNT_ID"]));
                    lobjPersonAccountLife.FindPersonAccount(Convert.ToInt32(ldrRow["PERSON_ACCOUNT_ID"]));

                    lobjPersonAccountLife.LoadPlanEffectiveDate();
                    if (lobjPersonAccountLife.iclbPersonAccountLifeHistory == null)
                        lobjPersonAccountLife.LoadHistory();

                    if (lobjPersonAccountLife.iclbLifeOption == null)
                        lobjPersonAccountLife.LoadLifeOptionData();

                    lobjPersonAccountLife.LoadPersonEmployment();

                    lobjPersonAccountLife.LoadOrgPlan(idtBatchRunDate, lobjPersonAccountLife.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id);
                    lobjPersonAccountLife.LoadProviderOrgPlan(idtBatchRunDate);


                    if (lobjPersonAccountLife.iclbLifeOption.Count > 0)
                    {
                        foreach (busPersonAccountLifeOption lobjLifeOption in lobjPersonAccountLife.iclbLifeOption)
                        {
                            busPersonAccountLifeHistory lobjLifeHistory = lobjPersonAccountLife.LoadHistoryByDate(lobjLifeOption, idtBatchRunDate);

                            if (lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                            {

                                DateTime ldtDOB = DateTime.MinValue;

                                if (!Convert.IsDBNull(ldrRow["DATE_OF_BIRTH"]))
                                    ldtDOB = Convert.ToDateTime(ldrRow["DATE_OF_BIRTH"]);

                                ldecEmployeePremiumAmount = busRateHelper.GetLifePremiumAmount(Convert.ToString(ldrRow["LIFE_INSURANCE_TYPE_VALUE"]),
                                                lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value, lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount,
                                                busGlobalFunctions.CalulateAge(ldtDOB, idtBatchRunDate),
                                                lobjPersonAccountLife.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, idtBatchRunDate, ref ldecEmployerPremiumAmount,
                                                idtbCachedLifeRate, iobjPassInfo, ref ldecADAndDBasicRate, ref ldecADAndDSupplementalRate);

                                lobjEnrollmentData.icdoEnrollmentData.source_id = lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id;
                                lobjEnrollmentData.icdoEnrollmentData.plan_id = busConstant.PlanIdGroupLife;
                                lobjEnrollmentData.icdoEnrollmentData.ssn = Convert.ToString(ldrRow["SSN"]);
                                lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = Convert.ToInt32(ldrRow["PERSON_ID"]);
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = Convert.ToString(ldrRow["PEOPLESOFT_ID"]);
                                lobjEnrollmentData.icdoEnrollmentData.employer_org_id = lobjPersonAccountLife.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                lobjEnrollmentData.icdoEnrollmentData.employment_type_value = lobjPersonAccountLife.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                                lobjEnrollmentData.icdoEnrollmentData.plan_status_value = Convert.ToString(ldrRow["PLAN_PARTICIPATION_STATUS_VALUE"]);
                                lobjEnrollmentData.icdoEnrollmentData.change_reason_value = busConstant.ChangeReasonLifeAgeUpdate;
                                lobjEnrollmentData.icdoEnrollmentData.person_account_id = lobjPersonAccountLife.icdoPersonAccount.person_account_id;
                                lobjEnrollmentData.icdoEnrollmentData.start_date = idtEffectiveStartDate;
                                lobjEnrollmentData.icdoEnrollmentData.end_date = lobjLifeHistory.icdoPersonAccountLifeHistory.effective_end_date;
                                lobjEnrollmentData.icdoEnrollmentData.provider_org_id = lobjLifeHistory.icdoPersonAccountLifeHistory.provider_org_id;
                                lobjEnrollmentData.icdoEnrollmentData.level_of_coverage_value = lobjLifeHistory.icdoPersonAccountLifeHistory.level_of_coverage_value;
                                lobjEnrollmentData.icdoEnrollmentData.coverage_amount = lobjLifeHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                lobjEnrollmentData.icdoEnrollmentData.monthly_premium = ldecEmployeePremiumAmount + ldecEmployerPremiumAmount;
                                lobjEnrollmentData.icdoEnrollmentData.pretaxed_premiums = Convert.ToString(ldrRow["PREMIUM_CONVERSION_INDICATOR_FLAG"]);
                                lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
                                lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes; //This record should not be picked in the Daily Peoplesoft batch.  
                                lobjEnrollmentData.icdoEnrollmentData.Insert();
                            }
                        }

                    }

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }
}
