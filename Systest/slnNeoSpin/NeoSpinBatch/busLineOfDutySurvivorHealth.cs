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
    public class busLineOfDutySurvivorHealth : busNeoSpinBatch
    {
        public void GenerateWorkflowForLineOfDutySurvivours()
        {
            istrProcessName = "Line Of Duty Survivors";
            idlgUpdateProcessLog("Loading All Line of Duty Survivor for Health plan", "INFO", istrProcessName);
            DataTable ldtbAllLineOfSurvivor = busNeoSpinBase.Select("entPersonAccountGhdv.LoadAllLineOfSurvivorHealth", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

            foreach (DataRow dr in ldtbAllLineOfSurvivor.Rows)
            {
                InitializeWorkFlow(Convert.ToInt32(dr["person_id"]));
            }
        }

        private void InitializeWorkFlow(int aintPersonID)
        {
            //cdoWorkflowRequest lcdoWorkflowRequest = new cdoWorkflowRequest();
            //lcdoWorkflowRequest.process_id = busConstant.MapMaintainLineOfDutySurvivor;
            //lcdoWorkflowRequest.reference_id = aintPersonID;
            //lcdoWorkflowRequest.person_id = aintPersonID;
            //lcdoWorkflowRequest.status_value = busConstant.WorkflowProcessStatus_UnProcessed;
            //lcdoWorkflowRequest.source_value = busConstant.WorkflowProcessSource_Batch;
            //lcdoWorkflowRequest.Insert();
            busWorkflowHelper.InitiateBpmRequest(busConstant.MapMaintainLineOfDutySurvivor, aintPersonID, 0, aintPersonID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);

        }
    }
}
