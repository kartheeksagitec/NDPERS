using NeoSpin.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;

namespace NeoSpinBatch
{
    public class busWaiverOfLifeExpiringBatch : busNeoSpinBatch
    {
        // public busWaiverOfLifeExpiringBatch() { }
        public void ProcessRecordsForWaiverOfLifeExpiring()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog("Loading members for Waiver Of Life Expiring.", "INFO", iobjBatchSchedule.step_name);
            DataTable ldtbMembersForWaiverOfLifeExpiring = busNeoSpinBase.Select("cdoPersonAccountLife.LoadMembersForWaiverOfLifeExpiring", new object[0] { });
            if (ldtbMembersForWaiverOfLifeExpiring.Rows.Count > 0)
            {
                foreach (DataRow drWaiverOfLifeExpiring in ldtbMembersForWaiverOfLifeExpiring.Rows)
                {

                    busPersonAccountLife lobjbusPersonAccountLife = new busPersonAccountLife();
                    lobjbusPersonAccountLife.icdoPersonAccountLife = new cdoPersonAccountLife();
                    lobjbusPersonAccountLife.icdoPersonAccount = new cdoPersonAccount();
                    lobjbusPersonAccountLife.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjbusPersonAccountLife.ibusPersonAccount = new busPersonAccount();
                    lobjbusPersonAccountLife.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                    try
                    {
                        lobjbusPersonAccountLife.icdoPersonAccountLife.LoadData(drWaiverOfLifeExpiring);
                        lobjbusPersonAccountLife.icdoPersonAccount.LoadData(drWaiverOfLifeExpiring);
                        lobjbusPersonAccountLife.ibusPersonAccount.icdoPersonAccount.LoadData(drWaiverOfLifeExpiring);
                        lobjbusPersonAccountLife.ibusPerson.icdoPerson.LoadData(drWaiverOfLifeExpiring);
                        lobjbusPersonAccountLife.LoadPersonAccountLifeOptions();
                        lobjbusPersonAccountLife.LoadPreviousHistory();
                        ProcessRecord(lobjbusPersonAccountLife);
                        //CreateCorrespondence(lobjbusPersonAccountLife);
                    }
                    catch (Exception ex)
                    {
                        idlgUpdateProcessLog("Exception occurred while processing record of Person id " + lobjbusPersonAccountLife.icdoPersonAccount.person_id + ", Exception : " + ex.Message, "ERR", istrProcessName);
                    }
                }
            }
            else
            {
                idlgUpdateProcessLog("No Members For Waiver Of Life Expiring.", "INFO", istrProcessName);
            }
        }

        private void ProcessRecord(busPersonAccountLife aobjbusPersonAccountLife)
        {
            int lintTempYear = aobjbusPersonAccountLife.ibusPerson.icdoPerson.date_of_birth.AddMonths(780).Year;
            int lintTempMonth = aobjbusPersonAccountLife.ibusPerson.icdoPerson.date_of_birth.AddMonths(780).Month;
            DateTime lDateOnAge65 = new DateTime(lintTempYear, lintTempMonth, 01);
            aobjbusPersonAccountLife.idtCoverageEndDate = lDateOnAge65.AddDays(-1);
            aobjbusPersonAccountLife.idtCoverageEndDatePlus31Days = aobjbusPersonAccountLife.idtCoverageEndDate.AddDays(31);
            aobjbusPersonAccountLife.icdoPersonAccount.history_change_date = lDateOnAge65;
            aobjbusPersonAccountLife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
            aobjbusPersonAccountLife.icdoPersonAccount.end_date = lDateOnAge65.AddDays(-1);
            if (aobjbusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeActiveMember)
                aobjbusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = "N";
            if (aobjbusPersonAccountLife.iclbLifeOption == null)
                aobjbusPersonAccountLife.LoadLifeOptionData();
            foreach (busPersonAccountLifeOption lobjPAOption in aobjbusPersonAccountLife.iclbLifeOption)
            {
                if (lobjPAOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)
                {
                    lobjPAOption.icdoPersonAccountLifeOption.coverage_amount = 0;
                    lobjPAOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                    lobjPAOption.icdoPersonAccountLifeOption.effective_end_date = aobjbusPersonAccountLife.idtCoverageEndDate;
                }
            }
            aobjbusPersonAccountLife.LoadPaymentElection();
            aobjbusPersonAccountLife.LoadOrgPlan();
            aobjbusPersonAccountLife.LoadProviderOrgPlan();
            aobjbusPersonAccountLife.BeforeValidate(utlPageMode.All);
            aobjbusPersonAccountLife.ValidateHardErrors(utlPageMode.All);
            if (aobjbusPersonAccountLife.iarrErrors.Count == 0)
            {
                idlgUpdateProcessLog("Processing for Person ID : " + aobjbusPersonAccountLife.icdoPersonAccount.person_id, "INFO", istrProcessName);
                aobjbusPersonAccountLife.BeforePersistChanges();
                aobjbusPersonAccountLife.PersistChanges();
                aobjbusPersonAccountLife.AfterPersistChanges();
                CreateCorrespondence(aobjbusPersonAccountLife);
                idlgUpdateProcessLog("process executed sucessfully for Person ID : " + aobjbusPersonAccountLife.icdoPersonAccount.person_id, "INFO", istrProcessName);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (utlError lobjutlError in aobjbusPersonAccountLife.iarrErrors)
                {
                    sb.Append(lobjutlError.istrDisplayMessage);
                    sb.Append(";");
                }
                idlgUpdateProcessLog(sb.ToString(), "INFO", istrProcessName);
                idlgUpdateProcessLog("Could not process for Person ID : " + aobjbusPersonAccountLife.icdoPersonAccount.person_id, "INFO", istrProcessName);

            }
        }

        private void CreateCorrespondence(busPersonAccountLife aobjbusPersonAccountLife)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjbusPersonAccountLife);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PER-0161", aobjbusPersonAccountLife, lhstDummyTable);
        }
    }
}
