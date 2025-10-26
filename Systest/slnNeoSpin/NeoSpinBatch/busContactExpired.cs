using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using Sagitec.Bpm;
using Sagitec.DBUtility;


namespace NeoSpinBatch
{
    class busContactExpired : busNeoSpinBatch
    {
        public void ContactExpiring()
        {
            DataTable ldtbExpiringContacts =  DBFunction.DBSelect("entPersonContact.ContactExpiring", 
                                                                   new object[1] { busGlobalFunctions.GetSysManagementBatchDate().AddMonths(1) }, 
                                                                   iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbExpiringContacts.Rows.Count > 0)
            {
                istrProcessName = "Contact Expiring";
                idlgUpdateProcessLog("Initiating BPM", "INFO", istrProcessName);
                foreach (DataRow ldr in ldtbExpiringContacts.Rows)
                {
                    busPersonContact lobjPersonContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                    lobjPersonContact.icdoPersonContact.LoadData(ldr);
                    InitializeWorkFlow(lobjPersonContact.icdoPersonContact.person_id);
                }


            }
        }
        private void InitializeWorkFlow(int aintPersonID)
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Person_Contact_Expiring, aintPersonID, 0, aintPersonID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
        }
    }
}
