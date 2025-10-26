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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busUpdateOrgPlanProvider:
	/// Inherited from busUpdateOrgPlanProviderGen, the class is used to customize the business object busUpdateOrgPlanProviderGen.
	/// </summary>
	[Serializable]
	public class busUpdateOrgPlanProvider : busUpdateOrgPlanProviderGen
	{
        public busPlan ibusPlan { get; set; }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull()) ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            ibusPlan.FindPlan(icdoUpdateOrgPlanProvider.plan_id);
        }

        public busOrganization ibusEmployerOrg { get; set; }

        public void LoadEmployerOrg()
        {
            if (ibusEmployerOrg.IsNull()) ibusEmployerOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            ibusEmployerOrg.FindOrganization(icdoUpdateOrgPlanProvider.employer_org_id);
        }

        public busOrganization ibusFromProviderOrg { get; set; }

        public void LoadFromProvider()
        {
            if (ibusFromProviderOrg.IsNull()) ibusFromProviderOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            ibusFromProviderOrg.FindOrganization(icdoUpdateOrgPlanProvider.from_provider_org_id);
        }

        public busOrganization ibusToProviderOrg { get; set; }

        public void LoadToProvider()
        {
            if (ibusToProviderOrg.IsNull()) ibusToProviderOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            ibusToProviderOrg.FindOrganization(icdoUpdateOrgPlanProvider.to_provider_org_id);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            icdoUpdateOrgPlanProvider.employer_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoUpdateOrgPlanProvider.employer_org_code);
            icdoUpdateOrgPlanProvider.from_provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoUpdateOrgPlanProvider.from_provider_org_code);
            icdoUpdateOrgPlanProvider.to_provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoUpdateOrgPlanProvider.to_provider_org_code);

            LoadEmployerOrg();
            LoadFromProvider();
            LoadToProvider();
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            icdoUpdateOrgPlanProvider.employer_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoUpdateOrgPlanProvider.employer_org_id);
            icdoUpdateOrgPlanProvider.from_provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoUpdateOrgPlanProvider.from_provider_org_id);
            icdoUpdateOrgPlanProvider.to_provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoUpdateOrgPlanProvider.to_provider_org_id);
        }

        public ArrayList btnUpdateOrgPlanProvider()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusEmployerOrg.IsNull()) LoadEmployerOrg();
            if (ibusFromProviderOrg.IsNull()) LoadFromProvider();
            if (ibusToProviderOrg.IsNull()) LoadToProvider();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                DataTable ldtbResults = Select("cdoUpdateOrgPlanProvider.GetAllEAPAccounts", new object[3]{ busConstant.PlanIdEAP,
                                        icdoUpdateOrgPlanProvider.from_provider_org_id,icdoUpdateOrgPlanProvider.employer_org_id});
                foreach (DataRow ldtrRow in ldtbResults.Rows)
                {
                    busPersonAccountEAP lobjEAPAccount = new busPersonAccountEAP
                    {
                        icdoPersonAccount = new cdoPersonAccount(),
                        ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                        ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                        
                    };
                    lobjEAPAccount.icdoPersonAccount.LoadData(ldtrRow);
                    lobjEAPAccount.ibusPerson.icdoPerson.LoadData(ldtrRow);
                    lobjEAPAccount.ibusPlan = ibusPlan;

                    lobjEAPAccount.LoadEAPHistory(false);
                    lobjEAPAccount.LoadPreviousHistory();

                    if (icdoUpdateOrgPlanProvider.effective_date < lobjEAPAccount.ibusHistory.icdoPersonAccountEapHistory.start_date)
                    {
                        // Future dated scenario
                        foreach (busPersonAccountEAPHistory lobjHistory in lobjEAPAccount.iclbEAPHistory)
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(icdoUpdateOrgPlanProvider.effective_date,
                                lobjHistory.icdoPersonAccountEapHistory.start_date, lobjHistory.icdoPersonAccountEapHistory.end_date))
                            {
                                // Update the existing History
                                DateTime ldteEndDate = lobjHistory.icdoPersonAccountEapHistory.end_date;
                                lobjHistory.icdoPersonAccountEapHistory.end_date = icdoUpdateOrgPlanProvider.effective_date.AddMonths(-1).GetLastDayofMonth();
                                lobjHistory.icdoPersonAccountEapHistory.Update();

                                // Insert new history for the new provider
                                busPersonAccountEAPHistory ibusEAPHistory = new busPersonAccountEAPHistory
                                {
                                    icdoPersonAccountEapHistory = new cdoPersonAccountEapHistory
                                        {
                                            person_account_id = lobjHistory.icdoPersonAccountEapHistory.person_account_id,
                                            plan_participation_status_id = lobjHistory.icdoPersonAccountEapHistory.plan_participation_status_id,
                                            plan_participation_status_value = lobjHistory.icdoPersonAccountEapHistory.plan_participation_status_value,
                                            status_id = lobjHistory.icdoPersonAccountEapHistory.status_id,
                                            status_value = lobjHistory.icdoPersonAccountEapHistory.status_value,
                                            from_person_account_id = lobjHistory.icdoPersonAccountEapHistory.from_person_account_id,
                                            to_person_account_id = lobjHistory.icdoPersonAccountEapHistory.to_person_account_id,
                                            suppress_warnings_by = lobjHistory.icdoPersonAccountEapHistory.suppress_warnings_by,
                                            suppress_warnings_flag = lobjHistory.icdoPersonAccountEapHistory.suppress_warnings_flag,
                                            suppress_warnings_date = lobjHistory.icdoPersonAccountEapHistory.suppress_warnings_date,

                                            provider_org_id = icdoUpdateOrgPlanProvider.to_provider_org_id,
                                            start_date = icdoUpdateOrgPlanProvider.effective_date,
                                            end_date = ldteEndDate
                                        }
                                };             
                                ibusEAPHistory.icdoPersonAccountEapHistory.Insert();

                            }
                        }
                    }
                    else
                    {
                        lobjEAPAccount.icdoPersonAccount.history_change_date = icdoUpdateOrgPlanProvider.effective_date;
                        lobjEAPAccount.icdoPersonAccount.provider_org_id = icdoUpdateOrgPlanProvider.to_provider_org_id;
                        lobjEAPAccount.icdoPersonAccount.ienuObjectState = ObjectState.Update;

                        lobjEAPAccount.iarrChangeLog.Add(lobjEAPAccount.icdoPersonAccount);
                        lobjEAPAccount.BeforeValidate(utlPageMode.All);
                        lobjEAPAccount.BeforePersistChanges();
                        lobjEAPAccount.PersistChanges();
                        lobjEAPAccount.AfterPersistChanges();
                    }
                }
                icdoUpdateOrgPlanProvider.status_value = busConstant.Vendor_Payment_Status_Processed;
                icdoUpdateOrgPlanProvider.Update();

                icdoUpdateOrgPlanProvider.Select();
                alReturn.Add(this);
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        public bool IsGivenInfoValid()
        {
            if (ibusEmployerOrg.IsNull()) LoadEmployerOrg();
            if (ibusEmployerOrg.iclbOrgPlan.IsNull()) ibusEmployerOrg.LoadOrgPlan();
            bool ablnFromProviderFlag = true; bool ablnToProviderFlag = true;
            foreach (busOrgPlan lobjOrgPlan in ibusEmployerOrg.iclbOrgPlan)
            {
                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdEAP)
                {
                    lobjOrgPlan.LoadOrgPlanProviders();
                    if (lobjOrgPlan.icdoOrgPlan.participation_end_date != DateTime.MinValue)
                    {
                        // From Provider should be ended.
                        if (lobjOrgPlan.iclbOrgPlanProvider.Where(lobj => lobj.icdoOrgPlanProvider.provider_org_id == icdoUpdateOrgPlanProvider.from_provider_org_id).Any())
                            ablnFromProviderFlag = false;
                    }
                    else
                    {
                        // To Provider should be open dated.
                        if (lobjOrgPlan.iclbOrgPlanProvider.Where(lobj => lobj.icdoOrgPlanProvider.provider_org_id == icdoUpdateOrgPlanProvider.to_provider_org_id).Any())
                            ablnToProviderFlag = false;
                    }
                }
            }

            if (ablnFromProviderFlag || ablnToProviderFlag)
                return false;
            return true;
        }
	}
}
