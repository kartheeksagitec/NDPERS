#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;


#endregion
namespace NeoSpinBatch
{
    class busPreRetirmentDeathWorkflowBatch : busNeoSpinBatch
    {
        private Collection<busDeathNotification> _iclbListOfDeceased;
        public Collection<busDeathNotification> iclbListOfDeceased
        {
            get { return _iclbListOfDeceased; }
            set { _iclbListOfDeceased = value; }
        }


        public void CreateWorkflowForPreRetirement()
        {
            istrProcessName = "Initiating Pre Retirement Death Workflow";
            //get all death notification whose recieved date is within the date of 

            idlgUpdateProcessLog("Getting all records having death certified", "INFO", istrProcessName);

            DataTable ldtbGetCertifiedDeceasedList = busBase.Select<cdoDeathNotification>(new string[1] { "DEATH_CERTIFIED_FLAG" }, new object[1] { busConstant.Flag_Yes }, null, null);
            busBase lobjBase = new busBase();
            _iclbListOfDeceased = lobjBase.GetCollection<busDeathNotification>(ldtbGetCertifiedDeceasedList, "icdoDeathNotification");

            foreach (busDeathNotification lobjdeathNotification in _iclbListOfDeceased)
            {
                if (lobjdeathNotification.icdoDeathNotification.date_of_death_certified_on != DateTime.MinValue)
                {
                    if ((lobjdeathNotification.icdoDeathNotification.action_status_value != busConstant.DeathNotificationActionStatusCancelled)
                        && (lobjdeathNotification.icdoDeathNotification.workflow_initiated_flag != busConstant.Flag_Yes))
                    {
                        DateTime ldtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();

                        if (busGlobalFunctions.DateDiffInDays(lobjdeathNotification.icdoDeathNotification.date_of_death_certified_on, ldtBatchDate) > 30)
                        {
                            if (lobjdeathNotification.ibusPerson == null)
                                lobjdeathNotification.LoadPerson();
                            if (lobjdeathNotification.ibusPerson.icolPersonAccount == null)
                                lobjdeathNotification.ibusPerson.LoadPersonAccount();
                            foreach (busPersonAccount lobjPersonAccount in lobjdeathNotification.ibusPerson.icolPersonAccount)
                            {
                                if (lobjPersonAccount.ibusPlan == null)
                                    lobjPersonAccount.LoadPlan();
                                if (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                                {
                                    if (((lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                                        || (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)))
                                    {
                                        if (lobjdeathNotification.ibusPerson.iclbPersonBeneficiary == null)
                                            lobjdeathNotification.ibusPerson.LoadBeneficiary();
                                        foreach (busPersonBeneficiary lobjPersonBeneficiary in lobjdeathNotification.ibusPerson.iclbPersonBeneficiary)
                                        {
                                            if (lobjPersonBeneficiary.iclbPersonAccountBeneficiary == null)
                                                lobjPersonBeneficiary.LoadPersonAccountBeneficiaryData();
                                            if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                                            {
                                                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount == null)
                                                    lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();
                                                if (lobjPersonAccount.icdoPersonAccount.plan_id == lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id)
                                                {
                                                    busPerson lobjPerson = new busPerson();
                                                    lobjPerson.FindPerson(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);
                                                    if (lobjPerson.icdoPerson.date_of_death == DateTime.MinValue)
                                                    {
                                                        bool lblnProceed = false;
                                                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue)
                                                        {
                                                            if ((lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date <= ldtBatchDate)
                                                                && (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= lobjdeathNotification.icdoDeathNotification.date_of_death)) // PIR - 1288
                                                            {
                                                                lblnProceed = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            lblnProceed = true;
                                                        }
                                                        if (lblnProceed)
                                                        {
                                                            DataTable ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "member_person_id", "recipient_person_id", "benefit_account_type_value", "plan_id" },
                                                                                        new object[4] { lobjPersonBeneficiary.icdoPersonBeneficiary.person_id, 
                                                                                 lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id , busConstant.ApplicationBenefitTypePreRetirementDeath,
                                                                                 lobjPersonAccount.icdoPersonAccount.plan_id}, null, null);
                                                            if (ldtbApplication.Rows.Count > 0)
                                                            {
                                                                busBenefitApplication lobjBenefitApplication = new busBenefitApplication();
                                                                lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                                                                lobjBenefitApplication.icdoBenefitApplication.LoadData(ldtbApplication.Rows[0]);

                                                                if ((lobjBenefitApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusCancelled)
                                                                    || (lobjBenefitApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusDenied))
                                                                {
                                                                    idlgUpdateProcessLog("Initiating Workflow - Pre retirement Death for Person Id - " + lobjBenefitApplication.icdoBenefitApplication.member_person_id
                                                                        + " and beneficiary id - " + lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                                                                    + " and Plan " + lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id, "INFO", istrProcessName);

                                                                    InitializeWorkFlow(lobjBenefitApplication.icdoBenefitApplication.member_person_id);

                                                                    // set workflow as yes
                                                                    lobjdeathNotification.icdoDeathNotification.workflow_initiated_flag = busConstant.Flag_Yes;
                                                                    lobjdeathNotification.icdoDeathNotification.Update();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                idlgUpdateProcessLog("Initiating Workflow for Pre retirement Death for Person Id - " + lobjPersonBeneficiary.icdoPersonBeneficiary.person_id
                                                                  + " and beneficiary id - " + lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                                                                    + " and Plan " + lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id, "INFO", istrProcessName);

                                                                InitializeWorkFlow(lobjPersonBeneficiary.icdoPersonBeneficiary.person_id);

                                                                // set workflow as yes
                                                                lobjdeathNotification.icdoDeathNotification.workflow_initiated_flag = busConstant.Flag_Yes;
                                                                lobjdeathNotification.icdoDeathNotification.Update();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitializeWorkFlow(int aintPersonID)
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Initialize_PreRetirement_Death_Workflow, aintPersonID, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);

        }
    }
}
