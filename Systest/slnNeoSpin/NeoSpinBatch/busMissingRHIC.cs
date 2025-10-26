#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Linq;
#endregion



namespace NeoSpinBatch

{
    class busMissingRHIC : busNeoSpinBatch
    {
        private Collection<busPayeeAccount> _iclbMissingRhicPayeeAccounts;
        public Collection<busPayeeAccount> iclbMissingRhicPayeeAccounts
        {
            get { return _iclbMissingRhicPayeeAccounts; }
            set { _iclbMissingRhicPayeeAccounts = value; }
        }
        public void GenerateWorkflowForProcessMissingRHICRecord()
        {
            istrProcessName = "Missing RHIC Batch";
            idlgUpdateProcessLog("Loading all payee accounts having Rhic amount greater than zero and having Approved/Receiving status", "INFO", istrProcessName);
            DataTable ldbtPayeeAccount = busNeoSpinBase.Select("entPayeeAccount.LoadRHICRecord", null);
            
            if ((ldbtPayeeAccount.Rows.Count > 0))
            {
                busBase lobjBase= new busBase();
                iclbMissingRhicPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldbtPayeeAccount, "icdoPayeeAccount");
                foreach (busPayeeAccount lobjPayeeAccount in iclbMissingRhicPayeeAccounts)
                {
                    {
                        idlgUpdateProcessLog("Initiating BPM", "INFO", istrProcessName);
                        busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Missing_RHIC_Record, lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, 0, lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, iobjPassInfo);
                        idlgUpdateProcessLog("BPM Initiated", "INFO", istrProcessName);
                    }
                }
            }
        }
        
    }
}