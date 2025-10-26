using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountHMOFileOut : busFileBaseOut
    {
        private Collection<busPersonAccountHMOFile> _iclbHMO;
        public Collection<busPersonAccountHMOFile> iclbHMO
        {
            get { return _iclbHMO; }
            set { _iclbHMO = value; }
        }

        public void LoadHMOMmebersAndDependents(DataTable ldtbHMOMembers)
        {
            _iclbHMO = new Collection<busPersonAccountHMOFile>();
            ldtbHMOMembers= busBase.Select("cdoPersonAccountGhdv.NDPERS_HMO_Enrollment", new object[] { });
            foreach (DataRow dr in ldtbHMOMembers.Rows)
            {
                /// Load HMO Member
                busPersonAccountHMOFile lobjHMOMember = new busPersonAccountHMOFile();
                busPersonAccountGhdv lobjGHDV = new busPersonAccountGhdv();
                lobjGHDV.ibusPerson = new busPerson();
                lobjGHDV.ibusPlan = new busPlan();
                lobjGHDV.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
                lobjGHDV.icdoPersonAccount = new cdoPersonAccount();
                lobjGHDV.ibusPerson.icdoPerson = new cdoPerson();
                lobjGHDV.ibusPlan.icdoPlan = new cdoPlan();
                lobjGHDV.icdoPersonAccount.LoadData(dr);
                lobjGHDV.icdoPersonAccountGhdv.LoadData(dr);
                lobjGHDV.ibusPerson.icdoPerson.LoadData(dr);
                lobjGHDV.ibusPlan.icdoPlan.LoadData(dr);

                // Employee First & Last Name
                lobjHMOMember.first_name = lobjGHDV.ibusPerson.icdoPerson.first_name;
                lobjHMOMember.last_name = lobjGHDV.ibusPerson.icdoPerson.last_name;

                // Benefit Plan
                lobjHMOMember.benefit_plan = lobjGHDV.icdoPersonAccountGhdv.hmo_insurance_type_description;


                // Deduction Begin Date
                DateTime ldteMaxContribitionDate = Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPersonAccountGhdv.GetMaximumContributionDate", new object[1]{
                                                    lobjGHDV.icdoPersonAccount.person_account_id}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                lobjHMOMember.deduction_begin_date = ldteMaxContribitionDate;

                if (lobjGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    // Coverage Election Value
                    lobjHMOMember.coverage_election = "E";

                    if (lobjGHDV.ibusHistory.IsNull())
                        lobjGHDV.LoadPreviousHistory();

                    // Coverage Begin & End Date
                    lobjHMOMember.coverage_begin_date = lobjGHDV.ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                    lobjHMOMember.coverage_end_date = lobjGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date;
                }
                else if ((lobjGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended) ||
                    (lobjGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled))
                {
                    // Coverage Election Value
                    lobjHMOMember.coverage_election = "T";

                    if (lobjGHDV.ibusEnrollmentHistory.IsNull())
                        lobjGHDV.LoadPreviousEnrolledHistory();

                    // Coverage Begin & End Date
                    lobjHMOMember.coverage_begin_date = lobjGHDV.ibusEnrollmentHistory.icdoPersonAccountGhdvHistory.start_date;
                    lobjHMOMember.coverage_end_date = lobjGHDV.ibusEnrollmentHistory.icdoPersonAccountGhdvHistory.end_date;
                    // Suspendend records needs to send once to the file.
                    lobjGHDV.icdoPersonAccountGhdv.included_to_hmo_file_flag = busConstant.Flag_Yes;
                    lobjGHDV.icdoPersonAccountGhdv.Update();
                }

                // Level of Coverage value
                lobjHMOMember.level_of_coverage = lobjGHDV.icdoPersonAccountGhdv.level_of_coverage_description;

                // Monthly Premium Amount
                lobjGHDV.icdoPersonAccount.person_employment_dtl_id = lobjGHDV.GetEmploymentDetailID(lobjGHDV.icdoPersonAccount.start_date);
                if (lobjGHDV.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lobjGHDV.LoadPersonEmploymentDetail();
                    lobjGHDV.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lobjGHDV.LoadOrgPlan();
                    lobjGHDV.LoadProviderOrgPlan();
                    lobjGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lobjGHDV.ibusOrgPlan.ibusOrganization = lobjGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
                }                
                else
                {
                    lobjGHDV.LoadActiveProviderOrgPlan(lobjGHDV.icdoPersonAccount.start_date);
                }
                lobjGHDV.GetMonthlyPremiumAmount();
                lobjHMOMember.premium_amount = Convert.ToString(lobjGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount);

                // Employee SSN
                lobjHMOMember.employee_ssn = lobjGHDV.ibusPerson.icdoPerson.ssn;
                _iclbHMO.Add(lobjHMOMember);

                /* LOAD HMO DEPENDENTS */
                DataTable ldtbHMODependents = busBase.Select<cdoPersonAccountDependent>(new string[1] { "person_account_id" },
                                      new object[1] { lobjGHDV.icdoPersonAccount.person_account_id }, null, null);
                foreach (DataRow ldrDependent in ldtbHMODependents.Rows)
                {
                    busPersonAccountHMOFile lobjHMODependent = new busPersonAccountHMOFile();
                    busPersonAccountDependent lobjPADependent = new busPersonAccountDependent();
                    lobjPADependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                    lobjPADependent.icdoPersonAccountDependent.LoadData(ldrDependent);
                    lobjPADependent.FindPersonDependent(lobjPADependent.icdoPersonAccountDependent.person_dependent_id);
                    lobjPADependent.LoadDependentInfo();
                    // Load to Collection
                    lobjHMODependent.first_name = lobjPADependent.icdoPersonDependent.dependent_first_name;
                    lobjHMODependent.last_name = lobjPADependent.icdoPersonDependent.dependent_last_name;
                    lobjHMODependent.benefit_plan = busConstant.NotApplicable;
                    lobjHMODependent.coverage_begin_date = lobjPADependent.icdoPersonAccountDependent.start_date;
                    lobjHMODependent.coverage_end_date = lobjPADependent.icdoPersonAccountDependent.end_date;
                    lobjHMOMember.deduction_begin_date = ldteMaxContribitionDate;
                    lobjHMODependent.coverage_election = busConstant.NotApplicable;
                    lobjHMODependent.level_of_coverage = busConstant.NotApplicable;
                    lobjHMODependent.premium_amount = busConstant.NotApplicable;
                    lobjHMODependent.employee_ssn = lobjGHDV.ibusPerson.icdoPerson.ssn;
                    // If End-dated send only once to the file and stop sending further.
                    if (lobjPADependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                    {
                        _iclbHMO.Add(lobjHMODependent);
                    }
                    else
                    {
                        if (lobjPADependent.icdoPersonAccountDependent.included_to_hmo_file_flag != busConstant.Flag_Yes)
                        {
                            _iclbHMO.Add(lobjHMODependent);
                            lobjPADependent.icdoPersonAccountDependent.included_to_hmo_file_flag = busConstant.Flag_Yes;
                            lobjPADependent.icdoPersonAccountDependent.Update();
                        }
                    }
                }
            }
        }
    }
}
