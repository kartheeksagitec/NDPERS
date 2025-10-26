using Sagitec.Bpm;
using NeoSpin.BusinessObjects;
using System.Data;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using NeoBase.BPM;

namespace NeoSpin.BusinessTier
{
  
    /// <summary>
    /// Developer : Tanaji Biradar
    /// Release : Iteration-8
    /// Date : 23rd December 2020
    /// Comments : Applied new AppBase-App changes for Service classes.
    /// </summary>
    public class srvBPMN : srvNeoSpin
    {
        public srvBPMN()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public busBpmEscalationLookup LoadBpmEscalations(DataTable adtbSearchResult)
        {
            busBpmEscalationLookup lobjBpmEscalationLookup = new busBpmEscalationLookup();
            lobjBpmEscalationLookup.LoadBpmEscalations(adtbSearchResult);
            return lobjBpmEscalationLookup;
        }
        public busBpmEscalation FindBpmEscalation(int aintEscalationId)
        {
            busBpmEscalation lbusBpmEscalation = new busBpmEscalation();
            lbusBpmEscalation.FindByPrimaryKey(aintEscalationId);
            lbusBpmEscalation.LoadBpmEscalationRecipients();
            lbusBpmEscalation.LoadBpmActivity();
            lbusBpmEscalation.LoadBpmEscalationAdditionalRecipients();
            if (aintEscalationId == 0)
            {
                lbusBpmEscalation.icdoBpmEscalation.lapse_type_value = BpmEscalationLapseTypes.BeforeExpiration;
                lbusBpmEscalation.icdoBpmEscalation.recur_type_value = BpmEscalationRecurTypes.None;
            }
            return lbusBpmEscalation;
        }

        public busBpmProcess FindBpmProcess(int aintProcessId)
        {
            busBpmProcess lbusBpmProcess = new busBpmProcess();
            busBpmCase lbusBpmCase = null;
            if (lbusBpmProcess.FindByPrimaryKey(aintProcessId))
            {
                lbusBpmCase = busBpmCase.GetBpmCase(lbusBpmProcess.icdoBpmProcess.case_id);
            }
            if (lbusBpmCase != null)
            {
                lbusBpmProcess = lbusBpmCase.GetProcess(aintProcessId);
                lbusBpmProcess.LoadRestrictNotifyConfigurationForParentProcess(lbusBpmProcess.icdoBpmProcess.process_id);
                lbusBpmProcess.iclbParentProcessRestrictNotify.ForEach(processRestrictNotifyXr => processRestrictNotifyXr.FindDependentProcess());

                // below line is to load the DependentCase with the newly added method FindCaseOfDependentProcess.
                lbusBpmProcess.iclbParentProcessRestrictNotify.ForEach(processRestrictNotifyXr => processRestrictNotifyXr.FindCaseOfDependentProcess());
                lbusBpmProcess.LoadBpmProcessEventXrs();
                lbusBpmProcess.iclbBpmProcessEventXr.ForEach(x => x.ibusBpmProcess = lbusBpmProcess);
                lbusBpmProcess.iclbBpmProcessEventXr.ForEach(x => x.LoadBpmEvent());
                return lbusBpmProcess;
            }
            return null;
        }

        public busBpmCase FindBpmCase(int aintCaseId)
        {
            busBpmCase lbusBpmCase = busBpmCase.GetBpmCase(aintCaseId);
            lbusBpmCase.GetCaseInstancesCountGroppedByStatus();
            return lbusBpmCase;
        }

        public busWFMimicIndexing FindWFMimicIndexing(int aintWFImageDataFilenetID)
        {
            busWFMimicIndexing lobjWFMimicIndexing = new busWFMimicIndexing();
            if (lobjWFMimicIndexing.FindWFMimicIndexing(aintWFImageDataFilenetID))
            {

            }
            return lobjWFMimicIndexing;
        }

        public busSolBpmActivityInstance BPMGetUserActivities(int aintActivityInstanceID)
        {
            busSolBpmActivityInstance lbusBpmActivityInstance = new busSolBpmActivityInstance();
            lbusBpmActivityInstance.LoadCenterleftObjects(aintActivityInstanceID);
            if (lbusBpmActivityInstance.icdoBpmActivityInstance != null && lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
            {
                lbusBpmActivityInstance.LoadBpmActivity();
                lbusBpmActivityInstance.LoadBpmProcessInstance();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmProcess();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadBpmCaseInstance();
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadPerson();
                lbusBpmActivityInstance.ibusBpmProcessInstance.LoadOrganization();
                lbusBpmActivityInstance.LoadBpmActivityInstanceChecklist();
                lbusBpmActivityInstance.LoadProcessInstanceNotes();
                lbusBpmActivityInstance.EvaluateInitialLoadRules();

            }
            return lbusBpmActivityInstance;
        }

        /// <summary>
        /// Fetches the BpmActivity Instance object for supplied Activity Instance Id
        /// </summary>
        /// <param name="aintActivityInstanceId">Activity Instance Id</param>
        /// <returns></returns>
        public busSolBpmActivityInstance FindBpmActivityInstance(int aintactivityinstanceid)
        {
            busSolBpmCaseInstance lbusBpmCaseInstance = new busSolBpmCaseInstance();
            busBpmActivityInstance lbusAI = lbusBpmCaseInstance.LoadWithActivityInst(aintactivityinstanceid);
            lbusAI.istrResumeActionValue = lbusAI.icdoBpmActivityInstance.resume_action_value;
            if (lbusAI != null)
                lbusAI.EvaluateInitialLoadRules();
            
            busSolBpmActivityInstance lbusActivityInstance = lbusAI as busSolBpmActivityInstance;
            if (lbusActivityInstance != null)
            {
                lbusActivityInstance.LoadProcessCheckList();
                utlBPMElement lutlBPMElement = lbusAI.ibusBpmActivity.GetBpmActivityDetails();
                utlBPMTask lutlBPMTask = lutlBPMElement as utlBPMTask;
                if (lutlBPMTask != null)
                {
                    lbusActivityInstance.iblnIsApprovalActivity = lutlBPMTask.iblnIsApprovalActivity;
                    if (lutlBPMTask.iobjPerformers != null && lutlBPMTask.iobjPerformers.icolDefaultPreconditions != null && lutlBPMTask.iobjPerformers.icolDefaultPreconditions.Count > 0)
                    {
                        if (lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].iblnIsDefaultCondition)
                        {
                            if (!string.IsNullOrEmpty(lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria[0].istrRole))
                            {
                                lbusActivityInstance.ibusBpmActivity.role_id = int.Parse(lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria[0].istrRole);
                                lbusActivityInstance.ibusBpmActivity.LoadRoles();
                                lbusActivityInstance.istrRoleDescription = ((busRoles)lbusActivityInstance.ibusBpmActivity.ibusRoles).icdoRoles.role_description;
                            }
                        }
                    }
                    foreach (utlBpmForm lobjForm in lutlBPMTask.icolForms)
                    {
                        if (lobjForm.ienmMode == utlPageMode.New)
                        {
                            lbusActivityInstance.istrNewModeScreen = lobjForm.istrFormName;
                        }
                        else if (lobjForm.ienmMode == utlPageMode.Update)
                        {
                            lbusActivityInstance.istrUpdateModeScreen = lobjForm.istrFormName;
                        }
                    }
                }
                    lbusActivityInstance.LoadProcessInstanceHistory();
                    lbusActivityInstance.LoadBpmProcessInstanceDetails();
                    lbusActivityInstance.LoadDocumentUpload();
                    lbusActivityInstance.LoadImageDataByProcessInstance();
                
            }
            
            return lbusActivityInstance;
        }

        public busBpmMyBasket SearchAndLoadMyBasket()
        {
            busBpmMyBasket lobjMyBasket = null;
            try
            {
                lobjMyBasket = iobjPassInfo.iutlObjectStore.GetObjectFromDB("wfmBPMMyBasketMaintenance", 0) as busBpmMyBasket; //Maintain my basket page state per session
            }
            catch { }
            if (lobjMyBasket.IsNull())
                lobjMyBasket = new busBpmMyBasket();
            //Set the Default Filter As Work Pool
            if (string.IsNullOrEmpty(lobjMyBasket.istrMyBasketFilter)) //PROD PIR 22963 - My Basket Filter Should Be Retained
                lobjMyBasket.istrMyBasketFilter = busConstant.MyBasketFilter_WorkPool;

            lobjMyBasket.SearchAndLoadMyBasket();
            return lobjMyBasket;
        }

        protected override void InitializeNewChildObject(object aobjParentObject, busBase aobjChildObject)
        {
            if(aobjChildObject is busBpmProcessEventXr && aobjParentObject is busBpmProcess)
            {
                ((busBpmProcessEventXr)aobjChildObject).ibusBpmProcess = (busBpmProcess)aobjParentObject;
            }

            base.InitializeNewChildObject(aobjParentObject, aobjChildObject);
        }
        public busBpmSupervisor LoadSupervisorDashboardData()
        {
            busBpmSupervisor lbusBpmSupervisor = new busBpmSupervisor();
            lbusBpmSupervisor.LoadBpmSupervisorDashboardData();
            return lbusBpmSupervisor;
        }
    }
}
    