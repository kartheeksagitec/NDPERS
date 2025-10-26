#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;

#endregion

namespace NeoSpinBatch
{
    class busVestedMembershipLetterBatch : busNeoSpinBatch
    {
        public void CreateCorrespondenceForVestedMembership()
        {
            istrProcessName = "Vesting Threshold Batch";
            idlgUpdateProcessLog("Creating correspondence for Vesting Threshold Batch", "INFO", istrProcessName);
            DataTable ldtbLists = busBase.Select("cdoPersonAccountRetirement.VestedMembershipLetterBatch", new object[] { });
            DataTable ldtDBCacheBenefitProvision = iobjPassInfo.isrvDBCache.GetCacheData("sgt_benefit_provision_eligibility", null);
            foreach (DataRow dr in ldtbLists.Rows)
            {
                // Initialize and load objects
                busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement
                {
                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement(),
                    icdoPersonAccount = new cdoPersonAccount(),
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() },
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                };
                lobjRetirement.icdoPersonAccountRetirement.LoadData(dr);
                lobjRetirement.icdoPersonAccount.LoadData(dr);
                lobjRetirement.ibusPerson.icdoPerson.LoadData(dr);
                lobjRetirement.ibusPlan.icdoPlan.LoadData(dr);

                idlgUpdateProcessLog("PERSLink ID :" + lobjRetirement.ibusPerson.icdoPerson.person_id.ToString(), "INFO", istrProcessName);
                lobjRetirement.ibusPersonAccount.icdoPersonAccount = lobjRetirement.icdoPersonAccount;
                //lobjRetirement.LoadTotalVSC();
                //PIR 26345
                decimal ldecTotTVSCIncludingTentativeService = Math.Round(lobjRetirement.ibusPerson.GetTotalVSCForPerson(lobjRetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService, DateTime.MinValue,
                                                                                             true, false, iintBenefitPlanId: lobjRetirement.ibusPlan.icdoPlan.plan_id), 4, MidpointRounding.AwayFromZero);

                DateTime ldtTerminationDate = DateTime.MinValue;
                lobjRetirement.GetOrgIdAsLatestEmploymentOrgId(lobjRetirement.ibusPersonAccount, busConstant.ApplicationBenefitTypeRetirement, ref ldtTerminationDate);

                // UAT PIR ID 1567 -- Earlier Requirement as per PIR
                DataTable ldtbResult = ldtDBCacheBenefitProvision.AsEnumerable().Where(ldtr =>
                                                ldtr.Field<string>("BENEFIT_ACCOUNT_TYPE_VALUE") == busConstant.ApplicationBenefitTypeRetirement &&
                                                ldtr.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityEarly &&
                                                ldtr.Field<int>("BENEFIT_PROVISION_ID") == lobjRetirement.ibusPlan.icdoPlan.benefit_provision_id).AsDataTable();
                if (ldtbResult.Rows.Count > 0)
                {
                    cdoBenefitProvisionEligibility lcdoProvisionEligiblity = new cdoBenefitProvisionEligibility();
                    lcdoProvisionEligiblity.LoadData(ldtbResult.Rows[0]);
                    lobjRetirement.idecEarlyRetirementAge = lcdoProvisionEligiblity.age;
                }
                if (ldecTotTVSCIncludingTentativeService >= lobjRetirement.ibusPlan.icdoPlan.vsc_threshold_months)
                {
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lobjRetirement);
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("FormTable", "Batch");
                    CreateCorrespondence("PER-0200", lobjRetirement, lshtTemp);
                }
                // Creating Contact Ticket
                cdoContactTicket lobjContactTicket = new cdoContactTicket();
                CreateContactTicket(lobjRetirement.ibusPerson.icdoPerson.person_id, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
                lobjContactTicket = null;

                // Updating Vesting Letter Sent Flag
                lobjRetirement.icdoPersonAccountRetirement.vesting_letter_sent_flag = busConstant.Flag_Yes;
                lobjRetirement.icdoPersonAccountRetirement.Update();
                lobjRetirement = null;
            }
            if (ldtbLists.Rows.Count == 0)
                idlgUpdateProcessLog("No Records to create Correspondence Letter.", "INFO", istrProcessName);
            else
                idlgUpdateProcessLog("Correspondence Created Successfully", "INFO", istrProcessName);
        }
    }
}
