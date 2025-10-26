using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using Sagitec.CustomDataObjects;
using System.Collections.ObjectModel;
using System.Linq;
using NeoSpin.DataObjects;
using Microsoft.VisualBasic;
using Sagitec.DBUtility;


namespace NeoSpinBatch
{
    class busDC25ReminderLetterBatch : busNeoSpinBatch
    {
        public void ProcessDC25ReminderLetterBatch()
        {
            istrProcessName = "DC25 Reminder Letter Batch";
            idlgUpdateProcessLog("Reminder batch letter to initiate 14 days after DC2025 enrollment.", "INFO", istrProcessName);
            DataTable ldtbReminderBatchLetter = busBase.Select("entPersonAccount.DC25ReminderLetter", new object[0] { });
            foreach (DataRow ldrRow in ldtbReminderBatchLetter.Rows)
            {
                busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);
                if (lbusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    CreateCorrespondence(lbusPersonAccount);
                    lbusPersonAccount.icdoPersonAccount.dc25_reminder_letter_flag = busConstant.Flag_Yes;
                    lbusPersonAccount.icdoPersonAccount.Update();
                }
                else
                {
                    idlgUpdateProcessLog("No Correspondence Created", "INFO", istrProcessName);
                }
            }
        }        
        private void CreateCorrespondence(busPersonAccount abusPersonAccount)
        {
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("ENR-5955", abusPersonAccount, lhstDummyTable);
            idlgUpdateProcessLog("Correspondence Created successfully for the PERSLink ID : " + abusPersonAccount.icdoPersonAccount.person_id, "INFO", istrProcessName);
        }

    }
}
