#region Using directives

using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpinBatch
{
    class busIntiatingRetirementWorkflow : busNeoSpinBatch
    {
        public void LoadPersonToInitiateWorkflow()
        {
            istrProcessName = "Initiating Retirement Workflow";
            DataTable ldtbGetPersonAccount = busBase.Select("cdoBenefitApplication.InitiateRetirementWorkflow", new object[] { });
            DateTime ldtPERSLinkGoLiveDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52,busConstant.PERSLinkGoLiveDate,iobjPassInfo));
            foreach (DataRow dr in ldtbGetPersonAccount.Rows)
            {
                busPersonAccount lobjPersonAccount = new busPersonAccount();
                lobjPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                lobjPersonAccount.icdoPersonAccount.LoadData(dr);

                busPerson lobjPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lobjPerson.icdoPerson.LoadData(dr);

                busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
                lobjPlan.icdoPlan.LoadData(dr);

                if (lobjPerson.icdoPerson.date_of_death == DateTime.MinValue)
                {
                    DataTable ldtbEmployment = busBase.Select<cdoPersonEmployment>(
                                                    new string[1] { "person_id" },
                                                    new object[1] { lobjPersonAccount.icdoPersonAccount.person_id }, null, null);
                    //PROD pir 4537
                    //employment ended after perslink go live date
                    if (!ldtbEmployment.AsEnumerable().Where(o => o.Field<DateTime?>("end_date") == null).Any() &&
                        ldtbEmployment.AsEnumerable().Where(o => o.Field<DateTime>("end_date") >= ldtPERSLinkGoLiveDate).Any())
                    {
                        DataTable ldtGetBenefitApplication = busBase.Select<cdoBenefitApplication>(
                                                                    new string[2] { "member_person_id", "plan_id" },
                                                                    new object[2] { lobjPersonAccount.icdoPersonAccount.person_id,
                                                                                lobjPersonAccount.icdoPersonAccount.plan_id }, null, null);
                        if (ldtGetBenefitApplication.Rows.Count == 0)
                        {
                            //PROD pir 4537
                            //checking whether member is vested or not
                            busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
                            lobjPersonAccountRetirement.FindPersonAccountRetirement(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lobjPersonAccountRetirement.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                            lobjPersonAccountRetirement.ibusPerson = lobjPerson;
                            lobjPersonAccountRetirement.ibusPlan = lobjPlan;
                            lobjPersonAccountRetirement.LoadTotalVSC();
                            lobjPersonAccountRetirement.SetVestingProperty();
                            if (lobjPersonAccountRetirement.istrIsPersonVested == busConstant.Flag_Yes)
                            {
                                //Create a record in workflow
                                InitializeWorkFlow(lobjPersonAccount.icdoPersonAccount.person_id);
                                idlgUpdateProcessLog("Workflow initiated Successfully for Person id: " + lobjPersonAccount.icdoPersonAccount.person_id, "INFO", istrProcessName);
                            }
                        }
                    }
                }
            }
        }

        private void InitializeWorkFlow(int aintPersonID)
        {

            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Initialize_Retirement_Workflow, aintPersonID, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
        }
    }
}
