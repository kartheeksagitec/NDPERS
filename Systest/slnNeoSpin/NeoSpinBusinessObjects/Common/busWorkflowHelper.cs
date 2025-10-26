using System;
using System.Collections;
using NeoSpin.Common;
using NeoSpin.Interface;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System.Data;
using Sagitec.DBUtility;
using Sagitec.Bpm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoSpin.CustomDataObjects;
using System.Collections.ObjectModel;

namespace NeoSpin.BusinessObjects
{
    public class WorkflowProcessInfo
    {
        public int process_id;
        public string description;
        public string name;
        public List<WorkflowActivity> Activities = null;
        public string case_name
        {
            get
            {
                if (!string.IsNullOrEmpty(name))
                {
                    return name.Replace("nfm", "sbp");
                }
                return string.Empty;
            }
        }
    }

    public class WorkflowActivity
    {
        public int ACTIVITY_ID;
        public string NAME;
        public string DISPLAY_NAME;
    }
    public static class busWorkflowHelper
    {
        public static busBpmActivityInstance GetActivityInstance(int aintActivityInstanceId)
        {
            busBpmCaseInstance lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
            return lbusBpmCaseInstance.LoadWithActivityInst(aintActivityInstanceId);
        }
        public static List<WorkflowProcessInfo> iclbWorkflowProcessInfos { get; set; }
        static busWorkflowHelper()
        {
            LoadWorkflowProcessInfo();
        }
        public static WorkflowProcessInfo GetCaseMappingByWorkflowProcessId(int aintWorkFlowProcessId)
        {
            return iclbWorkflowProcessInfos.Where(wi => wi.process_id == aintWorkFlowProcessId).FirstOrDefault();
        }

        public static WorkflowProcessInfo GetCaseMappingByBpmProcessName(string astrBpmProcessName)
        {
            return iclbWorkflowProcessInfos.Where(wi => wi.description == astrBpmProcessName).FirstOrDefault();
        }

        public static int GetWorkflowProcessIdByBpmProcessName(string astrBpmProcessName)
        {
            WorkflowProcessInfo lworkflowProcessInfo = GetCaseMappingByBpmProcessName(astrBpmProcessName);

            return lworkflowProcessInfo != null ? lworkflowProcessInfo.process_id : 0;
        }
        public static int GetWorkflowActivityIdByBpmActivityName(string astrProcessDescription, string astrBpmActivityName)
        {
            WorkflowProcessInfo lobjWorkflowProcessInfo = GetCaseMappingByBpmProcessName(astrProcessDescription);
            if (lobjWorkflowProcessInfo != null)
            {
                WorkflowActivity activity = lobjWorkflowProcessInfo.Activities.Where(act => act.NAME.Equals(astrBpmActivityName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (activity != null)
                    return activity.ACTIVITY_ID;
            }
            return 0;
        }
        public static busBpmRequest InitiateBpmRequest(int aintWorkflowProcessId, int aintPersonId, int aintOrgId, int aintReferenceId, utlPassInfo aobjPassInfo, string astrSource = BpmRequestSource.Online, Dictionary<string, object> adictInstanceParameters = null)
        {
            WorkflowProcessInfo lWorkflowProcessInfo = GetCaseMappingByWorkflowProcessId(aintWorkflowProcessId);
            if (aintOrgId == 0 && adictInstanceParameters != null && adictInstanceParameters.ContainsKey("org_code") && adictInstanceParameters["org_code"].IsNotNull())
            {
                aintOrgId = busGlobalFunctions.GetOrgIdFromOrgCode(Convert.ToString(adictInstanceParameters["org_code"]));
            }
            bool lblnCheckForExistingInstance = true;
            if (adictInstanceParameters != null && adictInstanceParameters.ContainsKey("check_for_existing_inst") && adictInstanceParameters["check_for_existing_inst"].IsNotNull())
            {
                lblnCheckForExistingInstance = Convert.ToString(adictInstanceParameters["check_for_existing_inst"]) == busConstant.Flag_Yes ? true : false;
            }
            if (lWorkflowProcessInfo.IsNotNull())
            {
                ArrayList larrResult = BpmHelper.InitiateRequest(lWorkflowProcessInfo.case_name, lWorkflowProcessInfo.description, aintPersonId, aintOrgId, aintReferenceId, aobjPassInfo, astrRequestSource: astrSource, adctRequestParameters: adictInstanceParameters, ablnCheckForExistingInstance: lblnCheckForExistingInstance);


                if (larrResult.Count > 0 && larrResult[0] is busBpmRequest)
                {
                    //PIR 25255 ‘Process Online Service Purchase Request’ need to be always initiated no matter even there are already existing in progress instances
                    busBpmRequest lbpmRequest = larrResult[0] as busBpmRequest;
                    if (lbpmRequest.IsNotNull() && lbpmRequest?.icdoBpmRequest?.process_id > 0)
                    {
                        lbpmRequest.LoadBpmProcess();
                        //PIR 26149 ‘Process Online Contact Ticket’ BPM's not creating when there are multiple MSS online tickets submitted the same day
                        if (lbpmRequest.ibusBpmProcess.icdoBpmProcess.name == "Process Online Service Purchase Request" ||
                            lbpmRequest.ibusBpmProcess.icdoBpmProcess.name == "Process Online Contact Ticket")
                        {
                            lbpmRequest.icdoBpmRequest.check_for_existing_inst = busConstant.Flag_No;
                            lbpmRequest.icdoBpmRequest.Update();
                        }
                    }
                    return (busBpmRequest)larrResult[0];
                }
            }
            return null;
        }
        public static busBpmProcess GetBpmProcessDetails(string astrCaseName, string astrProcessName)
        {
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
            lobjParameter.ParameterName = "@NAME";
            lobjParameter.Value = astrCaseName;
            lobjParameter.DbType = DbType.String;
            lcolParameters.Add(lobjParameter);

            busBpmCase lbusBpmCase = busBpmCase.GetEffectiveBpmCase(astrCaseName, DateTimeExtensions.ApplicationDateTime.Date);//busBpmCase.GetBpmCase((int)ldtCase.Rows[0]["CASE_ID"]);
            if (lbusBpmCase == null)
            {
                throw new Exception($"Unable to start bpm case '{astrCaseName}' as it is not found.");
            }
            if (lbusBpmCase.icdoBpmCase.status_value == BpmCaseStatus.InActive)
            {
                throw new Exception($"Unable to start bpm case '{astrCaseName}' as status is inactive.");

            }
            if (lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.name == astrProcessName).Count() > 1)
            {
                throw new Exception($"Unable to start bpm process '{astrProcessName}' as more than one process with same name is found for case {astrCaseName}.");
            }
            busBpmProcess lbusBpmProcess = lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.name == astrProcessName).FirstOrDefault();
            if (lbusBpmProcess == null)
            {
                throw new Exception($"Unable to start bpm process '{astrProcessName}' as it is not found for case '{astrCaseName}'.");
            }
            return lbusBpmProcess;
        }
        public static busBpmRequest InitiateLinkedBpmRequest(busBpmRequest aobjParentRequest, int aintWorkflowProcessId)
        {
            WorkflowProcessInfo lWorkflowProcessInfo = GetCaseMappingByWorkflowProcessId(aintWorkflowProcessId);
            busBpmRequest lobjBpmRequest = new busBpmRequest();
            lobjBpmRequest.icdoBpmRequest.person_id = aobjParentRequest.icdoBpmRequest.person_id;
            lobjBpmRequest.icdoBpmRequest.org_id = aobjParentRequest.icdoBpmRequest.org_id;
            lobjBpmRequest.icdoBpmRequest.reference_id = aobjParentRequest.icdoBpmRequest.reference_id;
            lobjBpmRequest.icdoBpmRequest.parent_request_id = aobjParentRequest.icdoBpmRequest.request_id;
            if(lWorkflowProcessInfo != null)
            {
                busBpmProcess lobjBpmProcess = busWorkflowHelper.GetBpmProcessDetails(lWorkflowProcessInfo.case_name, lWorkflowProcessInfo.description);
                if (lobjBpmProcess != null)
                {
                    lobjBpmRequest.icdoBpmRequest.process_id = lobjBpmProcess.icdoBpmProcess.process_id;
                }
            }
            lobjBpmRequest.iarrChangeLog.Add(lobjBpmRequest.icdoBpmRequest);

            if (aobjParentRequest.iclbBpmRequestParameter != null && aobjParentRequest.iclbBpmRequestParameter.Count > 0)
            {
                busBpmRequestParameter lobjParameter = null;
                if (lobjBpmRequest.iclbBpmRequestParameter == null)
                    lobjBpmRequest.iclbBpmRequestParameter = new Collection<busBpmRequestParameter>();
                foreach (busBpmRequestParameter lobjParentParameter in aobjParentRequest.iclbBpmRequestParameter)
                {
                    lobjParameter = new busBpmRequestParameter();
                    lobjParameter.icdoBpmRequestParameter.parameter_name = lobjParentParameter.icdoBpmRequestParameter.parameter_name;
                    lobjParameter.icdoBpmRequestParameter.parameter_value = lobjParentParameter.icdoBpmRequestParameter.parameter_value;
                    lobjBpmRequest.iclbBpmRequestParameter.Add(lobjParameter);
                    lobjBpmRequest.iarrChangeLog.Add(lobjParameter.icdoBpmRequestParameter);
                }
            }
            lobjBpmRequest.PersistChanges();
            return lobjBpmRequest;
        }
        public static ArrayList UpdateWorkflowActivityByStatus(string astrStatus, busMainBase abusActivityInstance, utlPassInfo aobjPassInfo)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;
            utlPassInfo.iobjPassInfo = aobjPassInfo;

            busBpmActivityInstance lobjActivityInstance = (busBpmActivityInstance)abusActivityInstance;
            if (lobjActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
            {
                lobjActivityInstance.UpdateBpmActivityInstanceByStatus(astrStatus, true);

                //try
                //{
                //    //Update the Status, UserId of Activity Instance                    
                //    lobjActivityInstance.icdoBpmActivityInstance.status_value = astrStatus;
                //    lobjActivityInstance.icdoBpmActivityInstance.checked_out_user = String.Empty;
                //    if (astrStatus == busConstant.ActivityStatusSuspended)
                //    {
                //        lobjActivityInstance.icdoActivityInstance.suspension_end_date = lobjActivityInstance.icdoActivityInstance.suspension_end_date;
                //        lobjActivityInstance.icdoActivityInstance.suspension_start_date = DateTime.Now;
                //        lobjActivityInstance.icdoActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                //        lobjActivityInstance.icdoActivityInstance.resume_action_value = lobjActivityInstance.icdoActivityInstance.resume_action_value;
                //    }
                //    else if ((astrStatus == busConstant.ActivityStatusInProcess) || (astrStatus == busConstant.ActivityStatusResumed))
                //    {
                //        lobjActivityInstance.icdoActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                //    }
                //    lobjActivityInstance.icdoActivityInstance.Update();

                //    //PROD PIR : 4060 Release the Workflow to Workflow If the resumed User Role is expired.
                //    if (astrStatus == busConstant.ActivityStatusResumed)
                //    {
                //        busUser lbusCheckedoutUser = new busUser();
                //        lbusCheckedoutUser.FindUserByUserName(lobjActivityInstance.icdoActivityInstance.checked_out_user);
                //        if (lobjActivityInstance.ibusActivity == null)
                //            lobjActivityInstance.LoadActivity();
                //        if (!lbusCheckedoutUser.IsMemberActiveInRole(lobjActivityInstance.ibusActivity.icdoActivity.role_id, DateTime.Now))
                //        {
                //            larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusReleased, lobjActivityInstance, aobjPassInfo);
                //        }
                //    }
                //}
                //catch (Exception Ex)
                //{
                //    lobjError = new utlError();
                //    lobjError.istrErrorMessage = "Could not checkout the activity.: " + Ex.Message;

                //    larrResult.Add(lobjError);
                //}
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Workflow instance not found.";

                larrResult.Add(lobjError);
            }

            return larrResult;
        }
        public static ArrayList UpdateWorkflowActivityByEvent(busBase abusObject, enmNextAction anmAction, string astrStatus, utlPassInfo aobjPassInfo)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;
            utlPassInfo.iobjPassInfo = aobjPassInfo;
            busBpmActivityInstance lobjActivityInstance = null;
            if (abusObject is busBpmActivityInstance)
            {
                lobjActivityInstance = (busBpmActivityInstance)abusObject;
            }
            else
            {
                lobjActivityInstance = (busBpmActivityInstance)abusObject.ibusBaseActivityInstance;
            }
            if (lobjActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
            {
                //Update SEQ Fix : To Avoid Record Changed Since last modified.
                //object lobjUpdateSeq = DBFunction.DBExecuteScalar("select update_seq from sgw_activity_instance where activity_instance_id = " +
                //    lobjActivityInstance.icdoActivityInstance.activity_instance_id.ToString(), aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
                //if (lobjUpdateSeq is Int32 && lobjActivityInstance.icdoActivityInstance.update_seq < (Int32)lobjUpdateSeq)
                //{
                //    lobjActivityInstance.icdoActivityInstance.Select();
                //}
                lobjActivityInstance.icdoBpmActivityInstance.Select();

                //lobjActivityInstance.icdoBpmActivityInstance.busObject = abusObject;
                lobjActivityInstance.icdoBpmActivityInstance.checked_out_user = aobjPassInfo.istrUserID;
                //lobjActivityInstance.icdoBpmActivityInstance.UserId = aobjPassInfo.istrUserID;
                //lobjActivityInstance.icdoBpmActivityInstance.UserSerialId = aobjPassInfo.iintUserSerialID;
                lobjActivityInstance.icdoBpmActivityInstance.status_value = astrStatus;
                //if (astrStatus == busConstant.ActivityStatusReturned)
                //{
                //    lobjActivityInstance.icdoBpmActivityInstance.return_from_audit_flag = busConstant.Return_From_Audit_Flag_Yes;
                //}
                //else if (astrStatus == busConstant.ActivityStatusReturnedToAudit)
                //{
                //    lobjActivityInstance.icdoBpmActivityInstance.return_from_audit_flag = busConstant.Return_From_Audit_Flag_No;
                //}

                //ActivityInstanceEventArgs aiEventArgs = new ActivityInstanceEventArgs();
                //aiEventArgs.icdoActivityInstance = lobjActivityInstance.icdoActivityInstance;
                //aiEventArgs.ienmNextAction = anmAction;//TODO: Refactor it later to decide when and how we should use it
                //if (lobjActivityInstance.ibusProcessInstance.IsNull())
                //    lobjActivityInstance.LoadProcessInstance();
                //larrResult = RaiseWorkflowEvent(lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.workflow_instance_guid, aiEventArgs);
                lobjActivityInstance.UpdateBpmActivityInstanceByEvent(enmBpmNextAction.Next, astrStatus);
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Workflow instance not found.";

                larrResult.Add(lobjError);
            }

            return larrResult;
        }
        public static void LoadWorkflowProcessInfo()
        {
            iclbWorkflowProcessInfos = new List<WorkflowProcessInfo>();
            var assembly = typeof(WorkflowProcessInfo).Assembly;
            //var resourceName = "NeoSpin.BusinessObjects.BPM.Extension.WorkflowProcessInfo.json";
            var resourceName = "NeoSpin.BusinessObjects.BPM.Extension.WorkflowProcessAndActivities.json";
            
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                iclbWorkflowProcessInfos = HelperFunction.JsonDeserialize(result, iclbWorkflowProcessInfos.GetType()) as List<WorkflowProcessInfo>;
            }
        }
        /// <summary>
        /// Method to check whether any running instance is available for person and process
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>bool value</returns>
        public static bool IsActiveInstanceAvailable(int aintPersonID, int aintProcessID)
        {
            bool lblnResult = false;

            DataTable ldtRunningInstance = new DataTable();
            ldtRunningInstance = LoadRunningInstancesByPersonAndProcess(aintPersonID, aintProcessID);
            if (ldtRunningInstance.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to load all running instance for person and process
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>Data table</returns>
        public static DataTable LoadRunningInstancesByPersonAndProcess(int aintPersonID, int aintProcessID)
        {
            return busBase.Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonAndProcess",
                                    new object[2] { aintPersonID, aintProcessID });
        }

        /// <summary>
        /// Method to check whether any running instance is available for org and process
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>bool value</returns>
        public static bool IsActiveInstanceAvailableForOrg(int aintOrgID, int aintProcessID)
        {
            bool lblnResult = false;

            DataTable ldtRunningInstance = new DataTable();
            ldtRunningInstance = LoadRunningInstancesByOrgAndProcess(aintOrgID, aintProcessID);
            if (ldtRunningInstance.Rows.Count > 0)
                lblnResult = true;
            return lblnResult;
        }

        /// <summary>
        /// Method to load all running instance for org and process
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>Data table</returns>
        public static DataTable LoadRunningInstancesByOrgAndProcess(int aintOrgID, int aintProcessID)
        {
            return busBase.Select("entSolBpmActivityInstance.LoadRunningInstancesByOrgAndProcess",
                                    new object[2] { aintOrgID, aintProcessID });
        }

        /// <summary>
        /// Method to load all unprocessed workflows for org and process
        /// PIR 19345 - For Some Reason, if workflow service is not picking unprocessed 
        /// workflows and not creating process instance and activity instance, there will be multiple
        /// workflows created for the same org and same process, this method checks if there are any unprocessed 
        /// workflow requests by org and process, and if there are, this method is used not to create a duplicate workflow. 
        /// </summary>
        /// <param name="aintOrgID">Org ID</param>
        /// <param name="aintProcessID">Process ID</param>
        /// <returns>Data table</returns>
        public static DataTable LoadUnProcessedWorkflowRequestsByOrgAndProcess(int aintProcessID, string astrOrgCode)
        {
            return busBase.Select("entActivityInstance.LoadUnProcessedWorkflowRequestsByOrgAndProcess",
                                    new object[2] { aintProcessID, astrOrgCode });
        }

        public static DataTable LoadRunningInstancesByFirstActivityofProcess(int aintFirstActivityID) 
        {
            return busBase.Select("cdoBpmActivityInstance.LoadRunningInstancesByFirstActivityofProcess",
                                    new object[1] { aintFirstActivityID });
        }
        public static bool IsActiveInstanceNotAvailableForFirstActivityofProcess(int aintFirstActivityID)
        { 
            DataTable ldtRunningInstance = LoadRunningInstancesByFirstActivityofProcess(aintFirstActivityID);
            if (ldtRunningInstance.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtRunningInstance.Rows[0]["TOTALCOUNT"]);
                if (lintCount > 0)
                    return false;
            }
            return true;
        }
        public static DataTable LoadNotProcessedWorkflowRequestsByProcessID(int aintProcessID)
        {
            return busBase.Select("cdoBpmRequest.LoadNotProcessedWorkflowRequestsByProcessID",
                                    new object[1] { aintProcessID });
        }
        public static bool IsWorkflowRequestNotProcessedForProcessID(int aintProcessID)
        {
            DataTable ldtRunningInstance = LoadNotProcessedWorkflowRequestsByProcessID(aintProcessID);
            if (ldtRunningInstance.Rows.Count > 0)
            {
                int lintCount = Convert.ToInt32(ldtRunningInstance.Rows[0]["TOTALCOUNT"]);
                if (lintCount > 0)
                    return false;
            }
            return true;
        }

        //modify query - venkat
        public static void UpdateSuspendedInstancestoResumed(int aintPersonID)
        {
            DataTable ldtSuspendedInstance = busBase.Select("entSolBpmActivityInstance.LoadSuspendedInstancesByPersonWhenEmploymentEnded",
                                    new object[1] { aintPersonID });

            foreach (DataRow dr in ldtSuspendedInstance.Rows)
            {
                busSolBpmCaseInstance lbusbpmCaseInstance = new busSolBpmCaseInstance();
                busBpmActivityInstance lbusBpmActivityInstance = lbusbpmCaseInstance.LoadWithActivityInst(Convert.ToInt32(dr["ACTIVITY_INSTANCE_ID"]));
                lbusBpmActivityInstance.Resume();
            }
        }
    }
}
