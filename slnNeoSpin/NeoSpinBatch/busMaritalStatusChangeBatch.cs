using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
namespace NeoSpinBatch
{
    class busMaritalStatusChangeBatch : busNeoSpinBatch
    {
       
        public void GenerateCorrepondenceForMaritalStatusChange()
        {
            istrProcessName = "Marital Status Change Batch ";
            idlgUpdateProcessLog("Loading all Persons whose marital status has been changed to divorced", "INFO", istrProcessName);
            DataTable ldtbPerson = busBase.Select("cdoPerson.MaritalStatusChangeBatch", new object[0] {  });
            Collection<busPerson> iclbPerson = new Collection<busPerson>();
            if (ldtbPerson.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbPerson.Rows)
                {
                    idlgUpdateProcessLog("Processing Persons who have changed their marital status", "INFO", istrProcessName);
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.icdoPerson = new cdoPerson();
                    lobjPerson.icdoPerson.LoadData(dr);
                    idlgUpdateProcessLog("Create correspondence for the PERSLink ID : " + lobjPerson.icdoPerson.person_id, "INFO", istrProcessName);
                    //lobjPerson.LoadBeneficiary();
                    //lobjPerson.iclbPersonBeneficiary = lobjPerson.iclbPersonBeneficiary.OrderByDescending(bene => bene.ibusPersonAccountBeneficiary.istrPlanNameCaps).ThenByDescending(n => n.ibusPersonAccountBeneficiary.istrBeneficiaryTypeCaps).ToList().ToCollection(); 
                    CreateCorrespondence(lobjPerson);
                    //if(lobjPerson.icdoPerson.person_id > 0)
                    //    InitializedBPM(lobjPerson.icdoPerson.person_id);
                    lobjPerson.icdoPerson.ms_change_batch_flag = busConstant.Flag_No;
                    lobjPerson.icdoPerson.Update();
                }
            }

            DataTable ldtbPersonBPM = busBase.Select("cdoPerson.MaritalStatusChangeBatchBPM", new object[0] { });
            if (ldtbPersonBPM.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbPersonBPM.Rows)
                {
                    idlgUpdateProcessLog("Processing Persons who have changed their marital status", "INFO", istrProcessName);
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.icdoPerson = new cdoPerson();
                    lobjPerson.icdoPerson.LoadData(dr);
                    idlgUpdateProcessLog("Create correspondence for the PERSLink ID : " + lobjPerson.icdoPerson.person_id, "INFO", istrProcessName);
                    //lobjPerson.LoadBeneficiary();
                    //lobjPerson.iclbPersonBeneficiary = lobjPerson.iclbPersonBeneficiary.OrderByDescending(bene => bene.ibusPersonAccountBeneficiary.istrPlanNameCaps).ThenByDescending(n => n.ibusPersonAccountBeneficiary.istrBeneficiaryTypeCaps).ToList().ToCollection();
                    CreateCorrespondence(lobjPerson);
                    if (lobjPerson.icdoPerson.person_id > 0)
                        InitializedBPM(lobjPerson.icdoPerson.person_id);
                    lobjPerson.icdoPerson.ms_change_batch_flag = busConstant.Flag_No;
                    lobjPerson.icdoPerson.Update();
                }
            }
         }

        private void CreateCorrespondence(busPerson abusPerson)
        {
            // Generate Correspondence
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(abusPerson);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("PER-0055", abusPerson, lhstDummyTable);
        }
        public void InitializedBPM(int aintPersonID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, busConstant.Map_Process_Marital_Status_Change))
            {
                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Marital_Status_Change, aintPersonID, 0, aintPersonID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
            }
        }
    }
}
