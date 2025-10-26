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
    class busDeferredCompBatchReport : busNeoSpinBatch
    {
        DataTable ldtbDeferredCompAgentSeminar = new DataTable();       
        public busDeferredCompBatchReport()
        {
 
        }       

        private Collection<busContact> _iclbContact;
        public Collection<busContact> iclbContact
        {
            get
            {
                return _iclbContact;
            }

            set
            {
                _iclbContact = value;
            }
        }

        public void CreateCorrespondenceForDeferredCompAgentSeminar(bool ablnIsNewDefCompAgent = false)
        {
            busContact lobjContact = new busContact { icdoContact = new cdoContact() };
            if (_iclbContact.IsNull())
                _iclbContact = new Collection<busContact>();
            istrProcessName = "Invitation for Deferred Compensation Seminar";
            idlgUpdateProcessLog("Inviting Deferred Compensation Agents", "INFO", istrProcessName);
            DateTime ldteBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;

            if (ablnIsNewDefCompAgent)
                ldtbDeferredCompAgentSeminar = busNeoSpinBase.Select("cdoSeminarSchedule.ListofNewDefCompAgentsHavingNOTraining", new object[1] { ldteBatchDate });
            else
                ldtbDeferredCompAgentSeminar = busNeoSpinBase.Select("cdoSeminarSchedule.ListofDefCompAgentsHavingActiveTraining", new object[1] { ldteBatchDate });
            _iclbContact = lobjContact.GetCollection<busContact>(ldtbDeferredCompAgentSeminar, "icdoContact");
            lobjContact.istrIsNewDefCompAgent = (ablnIsNewDefCompAgent ? busConstant.Flag_Yes : busConstant.Flag_No);
            lobjContact.LoadDeferredCompSeminarShcedule();
            foreach (busContact lobjTempContact in _iclbContact)
            {
                try
                {
                    idlgUpdateProcessLog("Loading Primary Address", "INFO", istrProcessName);
                    lobjTempContact.LoadContactAddressForDeferredCompAgent(lobjTempContact.icdoContact.contact_id);
                    lobjTempContact.iclbDeferredCompSeminarSchedule = lobjContact.iclbDeferredCompSeminarSchedule;
                    lobjTempContact.istrIsNewDefCompAgent = lobjContact.istrIsNewDefCompAgent;

                    //ArrayList larrlist = new ArrayList();
                    //larrlist.Add(lobjTempContact);
                    idlgUpdateProcessLog("Creating Correspondence for Contact", "INFO", istrProcessName);
                    Hashtable lhstDummyTable = new Hashtable();
                    lhstDummyTable.Add("sfwCallingForm", "Batch");
                    CreateCorrespondence("EVT-3100", lobjTempContact, lhstDummyTable);
                }
                catch (Exception _exc)
                {
                    idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                }
            }
        }
    }
}
