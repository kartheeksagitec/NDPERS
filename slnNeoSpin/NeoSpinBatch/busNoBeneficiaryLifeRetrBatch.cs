using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;

namespace NeoSpinBatch
{
    class busNoBeneficiaryLifeRetrBatch : busNeoSpinBatch
    {
        public DataTable idtbNoBeneficiaryLifeRetrByPerson { get; set; }
        public void CreateNoBeneficiaryLifeRetrCorrespondence()
        {
            istrProcessName = "No Beneficiary Batch";
            idlgUpdateProcessLog("Loading All Members Having No Beneficiary", "INFO", istrProcessName);
            DataTable ldtbNoBeneficiaryMembers = busNeoSpinBase.Select("cdoPerson.NoBeneficiaryLifeRetrBatch", new object[0] { });
            idtbNoBeneficiaryLifeRetrByPerson = busNeoSpinBase.Select("cdoPerson.NoBeneficiaryLifeRetrByPerson", new object[0] { });
            //utlPassInfo.iobjPassInfo.idictParams[utlConstants.istrProcessAuditLogSync] = true;
            foreach (DataRow ldrRow in ldtbNoBeneficiaryMembers.Rows)
            {
                busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPerson.icdoPerson.LoadData(ldrRow);
                try
                {
                    idlgUpdateProcessLog("Generating No Beneficiary Letter for Person ID " + lbusPerson.icdoPerson.person_id.ToString(), "INFO", istrProcessName);
                    utlPassInfo.iobjPassInfo.BeginTransaction();
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lbusPerson);
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("sfwCallingForm", "Batch");
                    CreateCorrespondence("PER-0004", lbusPerson, lshtTemp);
                    UpdatePersonAccountNoBeneFlag(lbusPerson);
                    utlPassInfo.iobjPassInfo.Commit();
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                    utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("No Beneficiary Letter Failed for Person ID " + lbusPerson.icdoPerson.person_id.ToString() + " " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
            //utlPassInfo.iobjPassInfo.idictParams.Remove(utlConstants.istrProcessAuditLogSync);
            idlgUpdateProcessLog("No Beneficiary Batch Ended.", "INFO", istrProcessName);
        }

        private void UpdatePersonAccountNoBeneFlag(busPerson abusPerson)
        {
           DataRow[] ldrRetrLifePersonAccounts = idtbNoBeneficiaryLifeRetrByPerson.FilterTable(busConstant.DataType.Numeric, "PERSON_ID", abusPerson.icdoPerson.person_id);
           foreach (DataRow ldrRetrOrLifeAcct in ldrRetrLifePersonAccounts)
           {
               cdoPersonAccount lcdoPersonAccount = new cdoPersonAccount();
               lcdoPersonAccount.LoadData(ldrRetrOrLifeAcct);
               if ((abusPerson.icdoPerson.life_bene_count > 0 && lcdoPersonAccount.plan_id == busConstant.PlanIdGroupLife) || (abusPerson.icdoPerson.retr_bene_count > 0 && lcdoPersonAccount.plan_id != busConstant.PlanIdGroupLife))
               {
                   lcdoPersonAccount.no_bene_sent = busConstant.Flag_Yes;
                   lcdoPersonAccount.Update();
               }
           }
        }
    }
}
