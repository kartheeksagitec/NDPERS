using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using NeoSpin.Common;
using System.Collections.Generic;
using Sagitec.Bpm;
using Sagitec.Common;

namespace NeoSpinBatch
{
    class busSeminarWorkflowReminderBatch : busNeoSpinBatch
    {

        public void ProcessSeminarWorkflow()
        {
            istrProcessName = "Initialize Or Resume Suspended Seminar";
            DataTable ldtbContactTicketList = busBase.Select("cdoContactTicket.SeminarWorkflowReminderBatchQuery", new object[] { });
            foreach (DataRow dr in ldtbContactTicketList.Rows)
            {
                int lintPersonId = 0;
                int lintOrgId = 0;
                int lintContactTicketId = 0;
                int lintActivityInstanceID = 0;
                string astrOrgCodeId = string.Empty;

                if (dr["person_id"] != DBNull.Value)
                    lintPersonId = (int)dr["person_id"];
                lintContactTicketId = (int)dr["contact_ticket_id"];
                lintActivityInstanceID = (int)dr["activity_instance_id"];
                if (dr["org_id"] != DBNull.Value)
                {
                    lintOrgId = (int)dr["org_id"];
                    astrOrgCodeId = busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgId);
                }

                //if (lintPersonId > 0 || lintOrgId > 0)
                //{
                if (lintActivityInstanceID > 0)
                {
                    if (lintPersonId > 0)
                    {
                        idlgUpdateProcessLog("Resuming Workflow for Person id " + lintPersonId + " contact ticket id " + lintContactTicketId, "INFO", istrProcessName);
                    }
                    else
                    {
                        idlgUpdateProcessLog("Resuming Workflow for Org id " + lintOrgId + " contact ticket id " + lintContactTicketId, "INFO", istrProcessName);
                    }
                    busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
                    busBpmActivityInstance lbusActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(lintActivityInstanceID);

                    string lstrUserID = iobjPassInfo.istrUserID;
                    iobjPassInfo.istrUserID = lbusActivityInstance.icdoBpmActivityInstance.checked_out_user;

                    busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lbusActivityInstance, iobjPassInfo);
                    iobjPassInfo.istrUserID = lstrUserID;
                }
                else
                {
                    if (lintPersonId > 0)
                    {
                        idlgUpdateProcessLog("Initializing Workflow for Person id " + lintPersonId + " contact ticket id " + lintContactTicketId, "INFO", istrProcessName);
                    }
                    else
                    {
                        idlgUpdateProcessLog("Initializing Workflow for Org id " + lintOrgId + " contact ticket id " + lintContactTicketId, "INFO", istrProcessName);
                    }


                    Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(astrOrgCodeId))
                        ldctParams["org_code"] = astrOrgCodeId;
                    busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Schedule_Seminar, lintPersonId, 0, lintContactTicketId, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, ldctParams);

                    //InitializeWorkFlow(busConstant.Map_Schedule_Seminar, lintPersonId, lintContactTicketId,
                    //    busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                }
                //}
            }
        }

    }
}
