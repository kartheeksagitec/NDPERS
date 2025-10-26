using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.BusinessObjects;
using NeoSpin.Common;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using NeoSpinBatch;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace NeoBPMN.Service
{
    public class ExtendedBPMService : BPMService
    {
        //PIR 18493 - MSS Ben App supporting documents upload processing paths
        private string istrDocUploadedPath;
        private string istrDocImagedPath;
        private string istrDocPurgedPath;
        private string istrPurgedPath;
        private string istrGeneratedPath;
        private string istrImagedPath;
        private string istrImagingPrinterName;
        private bool iblnInitializeBatch = false;

        public static void Initialize(string[] args)
        {
            string lstrServerName = "BPM Service";
            string lstrUserId = "BPM Service";
            int lintBpmQueueInterval = 500;
            int lintMaxConcurrentQueueRequests = 10;
            int lintDefaultTimerInterval = 300000;
            string[] details;
            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    details = arg.Split('=');
                    switch (details[0])
                    {
                        case "ServerName":
                            lstrServerName = details[1];
                            break;
                        case "UserId":
                            lstrUserId = details[1];
                            break;
                        case "DeQueueInterval":
                            lintBpmQueueInterval = Convert.ToInt32(details[1]);
                            break;
                        case "ConcurrentQueueRequests":
                            lintMaxConcurrentQueueRequests = Convert.ToInt32(details[1]); ;
                            break;
                        case "DefaultTimerInterval":
                            lintDefaultTimerInterval = Convert.ToInt32(details[1]); ;
                            break;
                    }
                }
            }

            // Developer : Rahul Mane
            // Iteration : 8.2
            // Date : 06_14_2021
            // Comment - Change Related to Override busBPMService method at Solution side /Application side

            Type type = ClassMapper.GetSolutionSideDerivedType(typeof(ExtendedBPMService));
            if (type != null)
            {
                _instance = type.GetConstructors()[0].Invoke(new object[] { lstrServerName, lstrUserId, lintBpmQueueInterval, lintMaxConcurrentQueueRequests, lintDefaultTimerInterval }) as ExtendedBPMService;
            }
            if (_instance == null)

            { _instance = new ExtendedBPMService(lstrServerName, lstrUserId, lintBpmQueueInterval, lintMaxConcurrentQueueRequests, lintDefaultTimerInterval); }


        }
        static ExtendedBPMService _instance;
        public static ExtendedBPMService Instance
        {
            get
            {
                return _instance;
            }
        }
        static ExtendedBPMService()
        {
            ilstActionMethods = new utlActionMethods();
            GetActionMethods(typeof(ExtendedBPMService));
        }
        protected ExtendedBPMService(string astrServerName, string astrUserId, int aintBpmQueueInterval = 500, int aintMaxConcurrentQueueRequests = 10, int aintDefaultTimerInterval = 300000) : base(astrServerName, astrUserId, aintBpmQueueInterval, aintMaxConcurrentQueueRequests, aintDefaultTimerInterval, false)
        {
            this.iintQueueItemReprocessingInterval = ConfigurationManager.AppSettings["QUEUE_ITEM_REPROCESSING_INTERVAL"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["QUEUE_ITEM_REPROCESSING_INTERVAL"]) : 5;
            this.iintActivityInstanceReprocessingInterval = ConfigurationManager.AppSettings["ACTIVTY_INSTANCE_REPROCESSING_INTERVAL"] != null ?
            Convert.ToInt32(ConfigurationManager.AppSettings["ACTIVTY_INSTANCE_REPROCESSING_INTERVAL"]) : 5;
        }
        protected override void InitializeDocumentExceptionProcess(busBpmRequest abusBpmRequest)
        {
            //CreateWorkflow(abusWorkflowRequest, abusWorkflowRequest.ibusDocument.icdoDocument.document_id,
            //        busConstant.Map_Resolve_Incoming_Mail, ref lstrWorkflowProcessStatus, ref lblnInitiateGenericWorkflow);

            busWorkflowHelper.InitiateLinkedBpmRequest(abusBpmRequest, busConstant.Map_Resolve_Incoming_Mail);
        }
        protected void HandleDocumentRequestFW(busBpmRequest abusBpmRequest)
        {
            busBpmEvent lbusEvent = ClassMapper.GetObject<busBpmEvent>();
            if (abusBpmRequest.icdoBpmRequest.doc_type != null && lbusEvent.FindBpmEventByDocTypeAndDocClass(abusBpmRequest.icdoBpmRequest.doc_type,
                abusBpmRequest.icdoBpmRequest.doc_class, true))
            {
                //Get list of all the processes to which this event is attached to.
                //If abusBpmRequest.icdoBpmRequest.process_id > 0 fetch process event xr's specific to that process. Else fetch all process event xr's for doc_type.
                lbusEvent.LoadBpmProcessEventXrsByProcessId(abusBpmRequest.icdoBpmRequest.process_id);
                //Process request for each process event xr
                foreach (busBpmProcessEventXr lbusBpmProcessEventXr in lbusEvent.iclbBpmProcessEventXr)
                {
                    lbusBpmProcessEventXr.LoadBpmProcess();
                    lbusBpmProcessEventXr.LoadBpmEvent();
                    lbusBpmProcessEventXr.ibusBpmProcess.LoadBpmCase();
                    //lbusBpmProcessEventXr.ibusBpmProcess.ibusBpmCase = busBpmCase.GetBpmCase(lbusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.case_id);
                    Collection<busBpmActivityInstance> lclbBpmActivityInstance = GetActivityInstancesToResume(lbusBpmProcessEventXr, abusBpmRequest);
                    if (lclbBpmActivityInstance?.Count > 0)
                    {
                        //activities found to resume
                        foreach (busBpmActivityInstance lbusBpmActivityInstanceToResume in lclbBpmActivityInstance)
                        {
                            ResumeActivityInstance(abusBpmRequest, lbusBpmActivityInstanceToResume, lbusBpmProcessEventXr);
                            if (busBpmRequestAction.iblnRequestAction)
                            {
                                //update all these activity in request action - sandip -vvimp check flag 
                                busBpmRequestAction lobjbusBpmRequestAction = new busBpmRequestAction();
                                lobjbusBpmRequestAction.ResumeRequestAction(lbusBpmActivityInstanceToResume, abusBpmRequest, lbusBpmProcessEventXr);
                            }
                        }
                    }
                    else
                    {
                        //PIR 27111 - BPM Event/Activity Mapping - Initiate new BPM for Events where current BPM process is past linked
                        //Check For In-Progress Instances
                        HandleInprogressInstances(abusBpmRequest, lbusBpmProcessEventXr);
                        //no activity found to resume
                        switch (lbusBpmProcessEventXr.icdoBpmProcessEventXr.action_value)
                        {
                            case BpmResumeAction.InitiateNew:
                                // Initiate new case instance
                                bool lblnResetFlag = abusBpmRequest.icdoBpmRequest.check_for_existing_inst == BpmCommon.Yes;
                                try
                                {
                                    abusBpmRequest.icdoBpmRequest.check_for_existing_inst = BpmCommon.No;
                                    InitiateNew(abusBpmRequest, lbusBpmProcessEventXr);
                                }
                                finally
                                {
                                    if (lblnResetFlag)
                                        abusBpmRequest.icdoBpmRequest.check_for_existing_inst = BpmCommon.Yes;
                                }

                                break;
                            case BpmResumeAction.InitiateOrResume:
                                // Initiate new case instance
                                InitiateNew(abusBpmRequest, lbusBpmProcessEventXr);
                                break;
                            case BpmResumeAction.ResumeOrException:
                                //Kick off the document exception flow
                                InitializeDocumentExceptionProcess(abusBpmRequest);
                                break;
                        }
                    }
                }
                PostProcessingDocumentRequest(abusBpmRequest);
            }
            else
            {
                //Kick off the document exception flow
                //Document not recognized by the application.
                InitializeDocumentExceptionProcess(abusBpmRequest);
            }
        }
        protected override void HandleDocumentRequest(busBpmRequest abusBpmRequest)
        {
            try
            {
                if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 &&
                            abusBpmRequest.icdoBpmRequest.reason_value == "UNDL")
                {
                    //Initiate the undelivered document map instance for person/organization
                    InitializeUndeliveredDocumentProcessForPersonOrOrganization(abusBpmRequest);
                    RequestProcessed(abusBpmRequest);
                }
                else
                {
                    HandleDocumentRequestFW(abusBpmRequest);
                }
            }
            finally
            {
               if(utlPassInfo.iobjPassInfo.idictParams.ContainsKey("RunningInstanceExists"))
                {
                    //AfterResumeActivityInstance
                    utlPassInfo.iobjPassInfo.idictParams.Remove("RunningInstanceExists");
                }
            }
        }

        protected override void AfterResumeActivityInstance(busBpmRequest abusBpmRequest, busBpmProcessEventXr abusBpmProcessEventXr, busBpmActivityInstance abusBpmActivityInstance)
        {
            if(abusBpmRequest.icdoBpmRequest.source_value == BpmRequestSource.ScanAndIndex)
            {
                utlPassInfo.iobjPassInfo.idictParams["RunningInstanceExists"] = true ;
                SetChecklistInstanceReceivedDate(abusBpmRequest, abusBpmProcessEventXr, abusBpmActivityInstance);
            }
            base.AfterResumeActivityInstance(abusBpmRequest, abusBpmProcessEventXr, abusBpmActivityInstance);
        }

        public void SetChecklistInstanceReceivedDate(busBpmRequest abusBpmRequest, busBpmProcessEventXr abusBpmProcessEventXr, busBpmActivityInstance abusBpmActivityInstance)
        {
            abusBpmActivityInstance.LoadBpmActivityInstanceChecklist();
            Collection<busBpmActivityInstanceChecklist> lcolAssociatedChecklistItems = abusBpmActivityInstance.iclbBpmActivityInstanceChecklist.Where(checklistInstance => checklistInstance.ibusBpmEvent.icdoBpmEvent.bpm_event_id == abusBpmProcessEventXr.icdoBpmProcessEventXr.event_id).ToCollection();
            foreach (busBpmActivityInstanceChecklist lobjChecklistInstance in lcolAssociatedChecklistItems)
            {
                if (DateTime.MinValue.Equals(lobjChecklistInstance.icdoBpmActivityInstanceChecklist.received_date))
                {
                    lobjChecklistInstance.icdoBpmActivityInstanceChecklist.received_date = abusBpmRequest.icdoBpmRequest.created_date;
                    lobjChecklistInstance.icdoBpmActivityInstanceChecklist.Update();
                }
            }
        }

        public override bool CanInitiateCaseInstance(busBpmRequest abusBpmRequest, busBpmProcessEventXr abusBpmProcessEventXr)
        {
            bool lblnCanInitiateCaseInstance = true;
            if (abusBpmRequest.icdoBpmRequest.source_value == BpmRequestSource.ScanAndIndex && utlPassInfo.iobjPassInfo.idictParams.ContainsKey("RunningInstanceExists"))
            {
                lblnCanInitiateCaseInstance = false;
            }
            if (abusBpmRequest.icdoBpmRequest.source_value == BpmRequestSource.ScanAndIndex && abusBpmRequest.icdoBpmRequest.doc_type == "1038")
            {
                abusBpmRequest.icdoBpmRequest.check_for_existing_inst = busConstant.Flag_No;
                lblnCanInitiateCaseInstance = true;
            }
            //PIR 25562	BPM workflows not creating for sfn 10766 Notice of Change when there is a recent one in system - even though one instance is exists initiate new one 
            if (abusBpmRequest.icdoBpmRequest.source_value == BpmRequestSource.ScanAndIndex &&
                abusBpmRequest.icdoBpmRequest.doc_type == "10766" && abusBpmProcessEventXr.ibusBpmProcess.icdoBpmProcess.name == "Process Person Demographic")
            {
                abusBpmRequest.icdoBpmRequest.check_for_existing_inst = busConstant.Flag_No;
                lblnCanInitiateCaseInstance = true;
            }
            if (lblnCanInitiateCaseInstance)
            {
                lblnCanInitiateCaseInstance = base.CanInitiateCaseInstance(abusBpmRequest, abusBpmProcessEventXr);
            }
            return lblnCanInitiateCaseInstance;
        }

        private void HandleInprogressInstances(busBpmRequest abusBpmRequest, busBpmProcessEventXr abusBpmProcessEventXr)
        {
            //check if any in progress instances available if so attach document.
            //entSolBpmProcessInstance.CheckInProgressInstanceForDocumentRequestByRequestIdAndPersonId
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            lcolParameters.Add(DBFunction.GetDBParameter("@REQUEST_ID", "int", abusBpmRequest.icdoBpmRequest.request_id, utlPassInfo.iobjPassInfo.iconFramework));
            string lstrQuery = null;
            if (abusBpmRequest.icdoBpmRequest.person_id > 0)
            {
                lstrQuery = "entSolBpmProcessInstance.CheckInProgressInstanceForDocumentRequestByRequestIdAndPersonId";
                lcolParameters.Add(DBFunction.GetDBParameter("@PERSON_ID", "int", abusBpmRequest.icdoBpmRequest.person_id, utlPassInfo.iobjPassInfo.iconFramework));
            }
            else
            {
                lstrQuery = "entSolBpmProcessInstance.CheckInProgressInstanceForDocumentRequestByRequestIdAndOrgId";
                lcolParameters.Add(DBFunction.GetDBParameter("@ORG_ID", "int", abusBpmRequest.icdoBpmRequest.org_id, utlPassInfo.iobjPassInfo.iconFramework));
            }
            lcolParameters.Add(DBFunction.GetDBParameter("@ACTIVITYID", "int", abusBpmProcessEventXr.icdoBpmProcessEventXr.activity_id, utlPassInfo.iobjPassInfo.iconFramework));
            lcolParameters.Add(DBFunction.GetDBParameter("@PROCESSID", "int", abusBpmProcessEventXr.icdoBpmProcessEventXr.process_id, utlPassInfo.iobjPassInfo.iconFramework));
            lcolParameters.Add(DBFunction.GetDBParameter("@EVENTID", "int", abusBpmProcessEventXr.icdoBpmProcessEventXr.event_id, utlPassInfo.iobjPassInfo.iconFramework));
            DataTable ldtResult = DBFunction.DBSelect(lstrQuery, lcolParameters, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
            if (ldtResult?.Rows?.Count > 0)
            {
                utlPassInfo.iobjPassInfo.idictParams["RunningInstanceExists"] = true;
                busBpmCaseInstance lbusBpmCaseInstance = null;
                busBpmProcessInstance lbusBpmProcessInstance = null;
                int lintProcessInstanceId = 0;
                foreach (DataRow ldtRow in ldtResult.Rows)
                {
                    lintProcessInstanceId = Convert.ToInt32(ldtRow["PROCESS_INSTANCE_ID"]);
                    lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
                    lbusBpmCaseInstance.LoadModelWithProcessInst(Convert.ToInt32(ldtRow["PROCESS_INSTANCE_ID"]));
                    lbusBpmProcessInstance = lbusBpmCaseInstance.iclbBpmProcessInstance.Where(pi => pi.icdoBpmProcessInstance.process_instance_id == lintProcessInstanceId).FirstOrDefault();
                    AttachDocumentToProcessInstance(abusBpmRequest, lbusBpmProcessInstance, abusBpmProcessEventXr);
                    busBpmRequestAction lbusBpmRequestAction = new busBpmRequestAction() { icdoBpmRequestAction = new doBpmRequestAction() };
                    abusBpmRequest.icdoBpmRequest.case_instance_id = lbusBpmCaseInstance.icdoBpmCaseInstance.case_instance_id;
                    abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcessInstance.icdoBpmProcessInstance.process_id;
                    lbusBpmRequestAction.NewRequestAction(lbusBpmCaseInstance, abusBpmRequest, abusBpmProcessEventXr); // PIR 25206 - This will tell us to which running instance the coming-in document attached to
                    lintProcessInstanceId = 0;
                    lbusBpmProcessInstance = null;
                    lbusBpmCaseInstance = null;
                }
            }
        }

        protected override void RequestProcessed(busBpmRequest abusBpmRequest)
        {
            if (abusBpmRequest.icdoBpmRequest.status_value != BpmRequestStatus.Restricted)
                base.RequestProcessed(abusBpmRequest);
        }
        protected override void PostProcessingDocumentRequest(busBpmRequest abusBpmRequest)
        {
            busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();

            lbusNeobaseBpmRequest.IncomingDocumentReceived(abusBpmRequest);
        }
        protected override void AttachDocumentToProcessInstance(busBpmRequest abusBpmRequest, busBpmProcessInstance abusBpmProcessInstance, busBpmProcessEventXr abusBpmProcessEventXr)
        {
            if (abusBpmRequest.icdoBpmRequest.source_value == BpmRequestSource.ScanAndIndex)
            {
                //// commented check to attach document everytime its scanned to any in progress instance.
                //DataTable ldtbBpmProcessInstanceAttachment = busBase.Select<doBpmPrcsInstAttachments>(new string[3] { enmBpmProcessInstanceAttachments.doc_type.ToString(), enmBpmProcessInstanceAttachments.ecm_guid.ToString(), enmBpmProcessInstanceAttachments.bpm_process_instance_id.ToString() },
                //  new object[3] { abusBpmRequest.icdoBpmRequest.doc_type, abusBpmRequest.icdoBpmRequest.ecm_guid, abusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id }, null, null);

                //if (ldtbBpmProcessInstanceAttachment.Rows.Count == 0)
                //{
                    busBpmProcessInstanceAttachments lbusBpmProcessInstanceAttachment = new busBpmProcessInstanceAttachments() { icdoBpmProcessInstanceAttachments = new doBpmPrcsInstAttachments() };
                    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.bpm_process_instance_id = abusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id;
                    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.doc_type = abusBpmRequest.icdoBpmRequest.doc_type;
                    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.ecm_guid = abusBpmRequest.icdoBpmRequest.ecm_guid;
                    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.doc_class = abusBpmRequest.icdoBpmRequest.doc_class;
                   // lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.additional_info = "";
                    RequestAdditionalInfo obj = HelperFunction.JsonDeserialize(abusBpmRequest.icdoBpmRequest.additional_info, typeof(RequestAdditionalInfo)) as RequestAdditionalInfo;
                    if (obj != null)
                    {
                        ProcessInstanceAttachmentsAdditionalInfo addInfo = new ProcessInstanceAttachmentsAdditionalInfo();
                        cdoCodeValue lcdoImageCategoryCodeValue = busGlobalFunctions.GetCodeValueByDescription(
                        busConstant.ImageDoc_Category_Code_ID, obj.IMAGE_DOC_CATEGORY);
                        addInfo.IMAGE_DOC_CATEGORY_ID = busConstant.ImageDoc_Category_Code_ID;
                        addInfo.IMAGE_DOC_CATEGORY_VALUE= lcdoImageCategoryCodeValue.code_value;

                        cdoCodeValue lcdoFileNetDocTypeCodeValue = busGlobalFunctions.GetCodeValueByDescription(
                            busConstant.FileNet_Document_Type_Code_ID, obj.FILENET_DOCUMENT_TYPE);
                        addInfo.FILENET_DOCUMENT_TYPE_ID= busConstant.FileNet_Document_Type_Code_ID;
                        addInfo.FILENET_DOCUMENT_TYPE_VALUE = lcdoFileNetDocTypeCodeValue.code_value;
                        lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.additional_info = HelperFunction.JsonSeserialize(addInfo);
                    }
                    lbusBpmProcessInstanceAttachment.icdoBpmProcessInstanceAttachments.Insert();
                //}

                abusBpmProcessInstance.LoadBpmActivityInstances();

                if (abusBpmProcessInstance.iclbBpmActivityInstance?.Count() > 0)
                {
                    foreach (busBpmActivityInstance lbusBpmActivityInstance in abusBpmProcessInstance.iclbBpmActivityInstance)
                    {
                        if (lbusBpmActivityInstance.ibusBpmActivity != null && lbusBpmActivityInstance.ibusBpmActivity is busBpmUserTask)
                        {
                            if (lbusBpmActivityInstance.icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.InProcess
                                || lbusBpmActivityInstance.icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.Resumed
                                || lbusBpmActivityInstance.icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.Suspended)
                                lbusBpmActivityInstance.LoadBpmActivityInstanceChecklist();
                            lbusBpmActivityInstance.UpdateBPMChecklist();
                            SetChecklistInstanceReceivedDate(abusBpmRequest, abusBpmProcessEventXr, lbusBpmActivityInstance);
                        }
                    }
                }
                PostAttachDocument(abusBpmRequest, abusBpmProcessInstance, abusBpmProcessEventXr);
            }
        }
        const string QUeryToCheckIfAllDocumentsReceived = "select count(1) from SGW_BPM_ACTY_INST_CHECKLIST AIC WITH(NOLOCK) INNER JOIN SGW_BPM_ACTIVITY_CHECKLIST AC WITH(NOLOCK) ON AIC.ACTIVITY_CHECKLIST_ID = AC.ACTIVITY_CHECKLIST_ID WHERE ACTIVITY_INSTANCE_ID=@ACTIVITY_INSTANCE_ID AND (COMPLETED_IND IS NULL OR COMPLETED_IND = 'N') AND AC.BPM_EVENT_ID <> @EVENT_ID";
        protected override bool CheckIfAllDocumentsReceived(busBpmActivityInstance abusBpmActivityInstance, busBpmProcessEventXr abusBpmProcessEventXr)
        {
            if (abusBpmProcessEventXr != null)
            {
                Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                lcolParameters.Add(DBFunction.GetDBParameter("@ACTIVITY_INSTANCE_ID", "int32", abusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id, utlPassInfo.iobjPassInfo.iconFramework));
                lcolParameters.Add(DBFunction.GetDBParameter("@EVENT_ID", "int32", abusBpmProcessEventXr.icdoBpmProcessEventXr.event_id, utlPassInfo.iobjPassInfo.iconFramework));
                int lintCount = DBFunction.DBGetInt(QUeryToCheckIfAllDocumentsReceived, lcolParameters, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                return lintCount == 0;
            }
            return true;
        }
        /// <summary>
        /// Start the document exception workflow
        /// </summary>
        /// <param name="abusBpmRequest"></param>
        private void InitializeUndeliveredDocumentProcessForPersonOrOrganization(busBpmRequest abusBpmRequest)
        {
            //Need to call this method here to tell communication module about receipt of
            //turn around or undelivered document
            busNeobaseBpmRequest lbusNeobaseBpmRequest = ClassMapper.GetObject<busNeobaseBpmRequest>();
            lbusNeobaseBpmRequest.IncomingDocumentReceived(abusBpmRequest);

            if (abusBpmRequest.icdoBpmRequest.person_id > 0)
            {
                busBpmProcess lbusBpmProcess = busNeobaseBpmActivityInstance.GetBpmProcess("sbpPersonAddressUndeliveredProcess ", "Person Address Undelivered Process", utlPassInfo.iobjPassInfo);

                if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
                {
                    abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

                    //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
                    lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
                    abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);

                    busBpmCaseInstance lbusBpmCaseInstance = InitiateCaseInstance(abusBpmRequest);

                    if (lbusBpmCaseInstance != null)
                    {
                        AttachDocumentToProcessInstance(abusBpmRequest, lbusBpmCaseInstance.iclbBpmProcessInstance[0], null);
                    }
                }
            }
            else if (abusBpmRequest.icdoBpmRequest.org_id > 0)
            {
                busBpmProcess lbusBpmProcess = busNeobaseBpmActivityInstance.GetBpmProcess("sbpOrganizationAddressUndeliveredProcess", "Organization Address Undelivered Process", utlPassInfo.iobjPassInfo);

                if (lbusBpmProcess.icdoBpmProcess.process_id > 0)
                {
                    abusBpmRequest.icdoBpmRequest.process_id = lbusBpmProcess.icdoBpmProcess.process_id;

                    //Create the new request parameter for the TrackingId since map needs value of tracking id from request table.
                    busBpmRequestParameter lbusBpmRequestParameter = new busBpmRequestParameter() { icdoBpmRequestParameter = new doBpmRequestParameter() };
                    lbusBpmRequestParameter.icdoBpmRequestParameter.request_id = abusBpmRequest.icdoBpmRequest.request_id;
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_name = "TrackingId";
                    lbusBpmRequestParameter.icdoBpmRequestParameter.parameter_value = abusBpmRequest.icdoBpmRequest.tracking_id.ToString();
                    lbusBpmRequestParameter.icdoBpmRequestParameter.Insert();
                    abusBpmRequest.iclbBpmRequestParameter.Add(lbusBpmRequestParameter);
                    busBpmCaseInstance lbusBpmCaseInstance = InitiateCaseInstance(abusBpmRequest);
                    if (lbusBpmCaseInstance != null)
                    {
                        AttachDocumentToProcessInstance(abusBpmRequest, lbusBpmCaseInstance.iclbBpmProcessInstance[0], null);
                    }
                }
            }

        }

        [TimerActionMethod("ProcessUploadedDocsToFileNetWSS")]
        public void ProcessUploadedDocsToFileNetWSS()
        {
            InitializePassInfo();
            try
            {
                /*1. The supporting documents uploaded from MSS are converted to
				tif format by Neevia here*/
                ConvertUploadedDocToTif();

                /*2. The converted supporting documents are uploaded to 
				file net here*/
                MoveImagedDocumentsToFileNet();

                /*3. The supporting documents are moved from uploaded 
				to purged folder*/
                PurgeBenAppImagedFiles();
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }
            //Starting back the timer
            //this.processUploadedDocsToFileNetWSS.Start();
        }

        private void ConvertUploadedDocToTif()
        {
            //Uploaded Documents conversion to TIFF
            DataTable ldtbList = busNeoSpinBase.Select<cdoDocUpload>(new string[2] { enmDocUpload.doc_status_value.ToString(), enmDocUpload.converted_to_image_flag.ToString() },
                                        new object[2] { busConstant.CorrespondenceStatus_Ready_For_Imaging, busConstant.Flag_No }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(istrDocUploadedPath))
                    istrDocUploadedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsUpld");
                if (string.IsNullOrEmpty(istrDocImagedPath))
                    istrDocImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsImge");
                foreach (DataRow ldtrDocUpload in ldtbList.Rows)
                {
                    busDocUpload lbusDocUpload = new busDocUpload { icdoDocUpload = new cdoDocUpload() };
                    lbusDocUpload.icdoDocUpload.LoadData(ldtrDocUpload);
                    string[] lstrFiles = Directory.GetFiles(istrDocUploadedPath, lbusDocUpload.istrUploadedFileName + ".*");
                    if (lstrFiles.Length > 0)
                    {
                        string lstrImgdUploadedFileName = istrDocImagedPath + lbusDocUpload.istrUploadedTiffFileName;

                        Neevia.docConverter dcUpload = new Neevia.docConverter();
                        dcUpload.convertFile(lstrFiles[0], lstrImgdUploadedFileName, 12000);

                        bool lblnUpldConvertedToTIFF = false;
                        int lintIterations = 0;
                        while (!lblnUpldConvertedToTIFF && lintIterations <= 5)
                        {
                            if (File.Exists(lstrImgdUploadedFileName))
                            {
                                FileInfo info = new FileInfo(lstrImgdUploadedFileName);
                                if (info.Length > 0)
                                    lblnUpldConvertedToTIFF = true;
                            }
                            lintIterations++;
                        }
                        //Commented when services deployed as windows services. Doesn't work.
                        //iobjCorBuilder.m_corr.OpenDoc(lstrFiles[0]);
                        //iobjCorBuilder.m_corr.PrintDoc(istrImagingPrinterName, lstrImgdUploadedFileName);
                        //bool lblnConvertedToTIFF = false;
                        //while (!lblnConvertedToTIFF)
                        //{
                        //    System.Threading.Thread.Sleep(10000);
                        //    if (File.Exists(lstrImgdUploadedFileName))
                        //    {
                        //        FileInfo info = new FileInfo(lstrImgdUploadedFileName);
                        //        if (info.Length > 0)
                        //            lblnConvertedToTIFF = true;
                        //    }
                        //}
                        //iobjCorBuilder.m_corr.CloseActiveDoc();
                        if (lblnUpldConvertedToTIFF)
                        {
                            lbusDocUpload.icdoDocUpload.converted_to_image_flag = "Y";
                            lbusDocUpload.icdoDocUpload.Update();
                        }
                    }
                }
            }
        }
        private void MoveImagedDocumentsToFileNet()
        {
            if (String.IsNullOrEmpty(istrDocUploadedPath))
            {
                istrDocUploadedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsUpld");
            }
            if (String.IsNullOrEmpty(istrDocImagedPath))
            {
                istrDocImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsImge");
            }
            DataTable ldtbImagedDocs = busNeoSpinBase.Select<cdoDocUpload>(new string[2] { enmDocUpload.doc_status_value.ToString(), enmDocUpload.converted_to_image_flag.ToString() },
                                 new object[2] { busConstant.CorrespondenceStatus_Ready_For_Imaging, busConstant.Flag_Yes }, null, null);
            foreach (DataRow ldtrImagedDoc in ldtbImagedDocs.Rows)
            {
                busDocUpload lbusDocUpload = new busDocUpload { icdoDocUpload = new cdoDocUpload() };
                lbusDocUpload.icdoDocUpload.LoadData(ldtrImagedDoc);
                string lstrImagedFileName = istrDocImagedPath + lbusDocUpload.istrUploadedTiffFileName;
                bool lblnSuccess = false;
                utlPassInfo.iobjPassInfo.istrUserID = lbusDocUpload.icdoDocUpload.created_by;
                try
                {
                    ArrayList larrErrorList = new ArrayList();
                    string lstrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lbusDocUpload.icdoDocUpload.org_id);
                    larrErrorList = busFileNetHelper.UploadFileNetDocumentWSS(lstrImagedFileName, lbusDocUpload.icdoDocUpload.person_id, lstrOrgCode);
                    if (larrErrorList.Count > 0)
                    {
                        throw new Exception(((utlError)larrErrorList[0]).istrErrorMessage);
                    }
                    busBpmEvent lbusDocument = new busBpmEvent();
                    if (lbusDocument.FindByPrimaryKey(lbusDocUpload.icdoDocUpload.document_id))
                    {
                        larrErrorList = busFileNetHelper.ProcessFileNetDocumentWSS(lstrImagedFileName, lbusDocument.icdoBpmEvent.screen_id);
                        if (larrErrorList.Count > 0)
                        {
                            if (((utlError)larrErrorList[0]).istrErrorMessage.Contains("A uniqueness requirement has been violated."))
                                larrErrorList.Clear();
                            else
                                throw new Exception(((utlError)larrErrorList[0]).istrErrorMessage);
                        }
                        if (larrErrorList.Count == 0)
                            lblnSuccess = true;
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
                finally
                {
                    if (lblnSuccess)
                    {
                        //utlPassInfo.iobjPassInfo.istrUserID = NEOFLOW_SERVICE;
                        lbusDocUpload.icdoDocUpload.doc_status_value = busConstant.CorrespondenceStatus_ImagedNOTPurged;
                        lbusDocUpload.icdoDocUpload.Update();
                        InitiateWorkFlowForDoc(lbusDocUpload);
                    }
                }
            }
        }

        private void PurgeBenAppImagedFiles()
        {
            if (String.IsNullOrEmpty(istrDocUploadedPath))
            {
                istrDocUploadedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsUpld");
            }
            if (string.IsNullOrEmpty(istrPurgedPath))
            {
                istrDocPurgedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("DocsPurg");
            }

            DataTable ldtbToPurgeFiles = busNeoSpinBase.Select<cdoDocUpload>(new string[2] { enmDocUpload.doc_status_value.ToString(),
                enmDocUpload.converted_to_image_flag.ToString() }, new object[2] { busConstant.CorrespondenceStatus_ImagedNOTPurged, busConstant.Flag_Yes }, null, null);
            foreach (DataRow ldtrtoPurge in ldtbToPurgeFiles.Rows)
            {
                busDocUpload lbusDocUpload = new busDocUpload { icdoDocUpload = new cdoDocUpload() };
                lbusDocUpload.icdoDocUpload.LoadData(ldtrtoPurge);
                try
                {
                    string[] lstrUploadedFileNames = Directory.GetFiles(istrDocUploadedPath, lbusDocUpload.istrUploadedFileName + ".*");
                    if (lstrUploadedFileNames.Length > 0)
                    {
                        string lstrSourcePath = lstrUploadedFileNames[0];
                        string lstrFileName = !string.IsNullOrEmpty(lstrUploadedFileNames[0]) ? lstrUploadedFileNames[0].Substring(lstrUploadedFileNames[0].LastIndexOf("\\") + 1) : string.Empty;
                        if (!string.IsNullOrEmpty(lstrFileName))
                        {
                            string lstrDestinationPath = istrDocPurgedPath + lstrFileName;
                            if (File.Exists(lstrSourcePath))
                            {
                                File.Move(lstrSourcePath, lstrDestinationPath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
                //tlPassInfo.iobjPassInfo.istrUserID = NEOFLOW_SERVICE;
                lbusDocUpload.icdoDocUpload.doc_status_value = busConstant.CorrespondenceStatus_Imaged;
                lbusDocUpload.icdoDocUpload.imaged_date = DateTime.Now;
                lbusDocUpload.icdoDocUpload.Update();
            }
        }

        [TimerActionMethod("NeoSpinServicePollingTick")]
        public void NeoSpinServicePollingTick()
        {
            InitializePassInfo();
            //Task Begins
            try
            {
                //Converting the Document to TIFF Conversion
                ConvertGeneratedDocToTiff();
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }
            InitializePassInfo();
            try
            {
                MoveImagedCorrespondenceToFileNet();
                //Delete correspondence from Published correspondence folder if message is cleared
                DeletePublishedCorrespondenceAfterMessageCleared();

                //Purged Imaged correspondence file
                PurgeImagedFiles();
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }

        }

        // Move the Generated (But Imaged) Document to Purged Folder
        private void PurgeImagedFiles()
        {
            //Generated Path
            if (String.IsNullOrEmpty(istrGeneratedPath))
            {
                istrGeneratedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
            }
            //Purged Path
            if (string.IsNullOrEmpty(istrPurgedPath))
            {
                istrPurgedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrPurg");
            }

            DataTable ldtbList = busNeoSpinBase.Select("cdoCorTracking.LoadFilesToBePurged", new object[0] { });
            foreach (DataRow ldtrResult in ldtbList.Rows)
            {
                busCorTracking ibusCorTracking = new busCorTracking();
                ibusCorTracking.icdoCorTracking = new cdoCorTracking();
                cdoCorTracking icdoCorTracking = ibusCorTracking.icdoCorTracking;
                icdoCorTracking.LoadData(ldtrResult);
                ibusCorTracking.ibusCorTemplates = new busCorTemplates();
                busCorTemplates ibusCorTemplates = ibusCorTracking.ibusCorTemplates;
                ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
                ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);
                try
                {
                    // Moved this step as a separate step and update with new status as even if the file move fails due to 
                    // some error system will not try to push the imaged file to filenet.
                    string lstrSourcePath = istrGeneratedPath + ibusCorTracking.istrWordFileName;
                    string lstrDestinationPath = istrPurgedPath + ibusCorTracking.istrWordFileName;
                    if (!ibusCorTracking.FileExists(lstrSourcePath))
                    {
                        lstrSourcePath = istrGeneratedPath + ibusCorTracking.istrWordFileNameDoc;
                        lstrDestinationPath = istrPurgedPath + ibusCorTracking.istrWordFileNameDoc;
                    }
                    if (File.Exists(lstrSourcePath))
                    {
                        File.Move(lstrSourcePath, lstrDestinationPath);
                    }
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
                // The file status will not be updated till the file exists and moved to purged folder.
                //utlPassInfo.iobjPassInfo.istrUserID = NEOFLOW_SERVICE;
                icdoCorTracking.cor_status_value = busConstant.CorrespondenceStatus_Imaged;
                icdoCorTracking.imaged_date = DateTime.Now;
                icdoCorTracking.Update();
            }
        }

        private void DeletePublishedCorrespondenceAfterMessageCleared()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoWssMessageDetail.LoadClearedMessageWithCorrespondenceLink", new object[0] { });
            foreach (DataRow dr in ldtbList.Rows)
            {
                busWssMessageDetail lbusMessageDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lbusMessageDetail.icdoWssMessageDetail.LoadData(dr);
                if (File.Exists(lbusMessageDetail.icdoWssMessageDetail.correspondence_link))
                {
                    File.Delete(lbusMessageDetail.icdoWssMessageDetail.correspondence_link);
                    lbusMessageDetail.icdoWssMessageDetail.correspondence_link = null;
                    lbusMessageDetail.icdoWssMessageDetail.Update();
                }
                lbusMessageDetail = null;
            }
        }

        private void ConvertGeneratedDocToTiff()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoCorTracking.LoadCorrForTiffConversion", new object[0] { });
            //Generated Path
            if (String.IsNullOrEmpty(istrGeneratedPath))
            {
                istrGeneratedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
            }
            //Imaged Path
            if (String.IsNullOrEmpty(istrImagedPath))
            {
                istrImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrImag");
            }
            //imaging printer name
            if (String.IsNullOrEmpty(istrImagingPrinterName))
            {
                istrImagingPrinterName = utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(44, "TIFF");
            }
            foreach (DataRow ldtrResult in ldtbList.Rows)
            {
                busCorTracking ibusCorTracking = new busCorTracking();

                ibusCorTracking.icdoCorTracking = new cdoCorTracking();
                cdoCorTracking icdoCorTracking = ibusCorTracking.icdoCorTracking;
                icdoCorTracking.LoadData(ldtrResult);
                ibusCorTracking.ibusCorTemplates = new busCorTemplates();
                busCorTemplates ibusCorTemplates = ibusCorTracking.ibusCorTemplates;
                ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
                ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);
                string lstrFileName = istrGeneratedPath + ibusCorTracking.istrWordFileName;
                if (!ibusCorTracking.FileExists(lstrFileName))
                {
                    lstrFileName = istrGeneratedPath + ibusCorTracking.istrWordFileNameDoc;
                }

                string lstrImagedFileName = istrImagedPath + ibusCorTracking.istrTifFileName;

                //Neevia doc convertor tool to convert to tiff as previous method of conversion is not supported when servcies are running as windows services.
                Neevia.docConverter dc = new Neevia.docConverter();
                dc.convertFile(lstrFileName, istrImagedPath, 12000);

                //Converting into TIFF Format
                //Open the doc and printer to the directory 
                //Commented when services deployed as windows services. Doesn't work.
                //iobjCorBuilder.m_corr.OpenDocReadonly(lstrFileName);
                //iobjCorBuilder.m_corr.PrintDoc(istrImagingPrinterName, istrImagedPath + ibusCorTracking.istrTifFileName);
                //iobjCorBuilder.m_corr.CloseActiveDoc();

                //Idle for Few Seconds In order to Convert to TIFF..                
                //Wait Untill TIFF Image Gets Converted
                bool lblnConvertedToTIFF = false;
                int lintCount = 0;
                while (!lblnConvertedToTIFF)
                {
                    System.Threading.Thread.Sleep(5000);
                    if (File.Exists(istrImagedPath + ibusCorTracking.istrTifFileName))
                    {
                        FileInfo info = new FileInfo(istrImagedPath + ibusCorTracking.istrTifFileName);
                        if (info.Length > 0)
                            lblnConvertedToTIFF = true;
                        else
                            lintCount++;
                    }
                    else
                        lintCount++;
                    if (lintCount == 3)
                        break;
                }
                //Update the Flag
                if (lblnConvertedToTIFF)
                {
                    icdoCorTracking.converted_to_image_flag = "Y";
                    icdoCorTracking.Update();
                }
            }
        }
        private void LogExecutionFlow(string astrMessage)
        {
            if (NeoSpin.Common.ApplicationSettings.Instance.IsLoggingEnabled)
            {
                Sagitec.Common.utlThreadSafeFileLogger.LoggerInstance.WriteToLog(astrMessage);
            }
        }
        private void MoveImagedCorrespondenceToFileNet()
        {

            LogExecutionFlow("Entering MoveImagedCorrespondenceToFileNet.");
            //Generated Path
            if (String.IsNullOrEmpty(istrGeneratedPath))
            {
                istrGeneratedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
            }
            //Imaged Path
            if (String.IsNullOrEmpty(istrImagedPath))
            {
                istrImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrImag");
            }
            //Purged Path
            if (string.IsNullOrEmpty(istrPurgedPath))
            {
                istrPurgedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrPurg");
            }

            DataTable ldtbList = busNeoSpinBase.Select("cdoCorTracking.LoadCorrForFileNetTransfer", new object[0] { });
            foreach (DataRow ldtrResult in ldtbList.Rows)
            {
                busCorTracking ibusCorTracking = new busCorTracking();
                ibusCorTracking.icdoCorTracking = new cdoCorTracking();
                cdoCorTracking icdoCorTracking = ibusCorTracking.icdoCorTracking;
                icdoCorTracking.LoadData(ldtrResult);
                ibusCorTracking.ibusCorTemplates = new busCorTemplates();
                busCorTemplates ibusCorTemplates = ibusCorTracking.ibusCorTemplates;
                ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
                ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);
                string lstrFileName = istrGeneratedPath + ibusCorTracking.istrWordFileName;
                if (!ibusCorTracking.FileExists(lstrFileName))
                {
                    lstrFileName = istrGeneratedPath + ibusCorTracking.istrWordFileNameDoc;
                }
                string lstrImagedFileName = istrImagedPath + ibusCorTracking.istrTifFileName;
                bool lblnSuccess = false;
                //Assign the Cor Tracking Created By User as Current User  
                utlPassInfo.iobjPassInfo.istrUserID = icdoCorTracking.created_by;
                try
                {
                    ArrayList larrErrorList = new ArrayList();
                    string lstrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(icdoCorTracking.org_id);
                    larrErrorList = busFileNetHelper.UploadFileNetDocument(lstrImagedFileName, icdoCorTracking.person_id, lstrOrgCode);
                    if (larrErrorList.Count > 0)
                    {
                        throw new Exception(((utlError)larrErrorList[0]).istrErrorMessage);
                    }

                    larrErrorList = busFileNetHelper.ProcessFileNetDocument(lstrImagedFileName);
                    if (larrErrorList.Count > 0)
                    {
                        if (((utlError)larrErrorList[0]).istrErrorMessage.Contains("A uniqueness requirement has been violated."))
                            larrErrorList.Clear();
                        else
                            throw new Exception(((utlError)larrErrorList[0]).istrErrorMessage);
                    }
                    if (larrErrorList.Count == 0)
                        lblnSuccess = true;
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                }
                finally
                {
                    // If successfully pushed to filenet once, update the status so that the system will not try to take that record again.
                    if (lblnSuccess)
                    {
                        // utlPassInfo.iobjPassInfo.istrUserID = NEOFLOW_SERVICE;
                        icdoCorTracking.cor_status_value = busConstant.CorrespondenceStatus_ImagedNOTPurged;
                        icdoCorTracking.Update();
                    }
                }
            }
            LogExecutionFlow("Exiting MoveImagedCorrespondenceToFileNet.");
        }

        private void InitiateWorkFlowForDoc(busDocUpload lbusDocUpload)
        {
            doBpmRequest lcdoBpmRequest = new doBpmRequest();
            lcdoBpmRequest.person_id = lbusDocUpload.icdoDocUpload.person_id;
            busBpmEvent lbusDocument = new busBpmEvent();
            lbusDocument.FindByPrimaryKey(lbusDocUpload.icdoDocUpload.document_id);
            lcdoBpmRequest.doc_type = lbusDocument.icdoBpmEvent.doc_type;
            lcdoBpmRequest.doc_class = busConstant.ImageDocCategoryMember;
            lcdoBpmRequest.doc_type =
            ((lcdoBpmRequest.doc_type == "54398") || (lcdoBpmRequest.doc_type == "54399")) ?
            busConstant.FileNetDocumentTypeForm : busConstant.FileNetDocumentTypeCorrespondence;
            lcdoBpmRequest.status_value = BpmRequestStatus.NotProcessed;
            lcdoBpmRequest.source_value = BpmRequestSource.ScanAndIndex;
            lcdoBpmRequest.Insert();
        }

        private void InitializePassInfo()
        {

            utlPassInfo.iobjPassInfo = BPMDBHelper.CreatePassInfo(istrUserId);
            utlPassInfo.iobjPassInfo.idictParams.Add(utlConstants.istrRequestIPAddress, utlFunctions.GetSystemIPAddress());

        }
        private void FreePassInfo(utlPassInfo aobjPassInfo)
        {
            BPMDBHelper.FreePassInfo(aobjPassInfo);

        }   
        
        [TimerActionMethod("ProcessNeospinBatch")]
        public void ProcessNeospinBatch()
        {
            //We are stopping the Timer here because if the task execution time takes more than interval time
            //this.processNeospinBatchTimer.Stop();
            InitializePassInfo();

            utlPassInfo.iobjPassInfoProcessLog = utlPassInfo.iobjPassInfo;

            //Task Begins
            try
            {
                //intialize step in batch to be called.
                //need to be called only once, when service is started
                if (!iblnInitializeBatch)
                {
                    //Get the Step No for Initialize Step
                    cdoBatchSchedule lcdoBatchSchedule = new cdoBatchSchedule();
                    lcdoBatchSchedule.SelectRow(new object[1] { 1 });

                    if (lcdoBatchSchedule.batch_schedule_id > 0)
                    {
                        RunBatch(lcdoBatchSchedule.step_no, lcdoBatchSchedule.step_no);
                        iblnInitializeBatch = true;
                    }
                }
                //loading datatable from db cache which contains list of batch steps to be executed
                DataTable ldtBatch = utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeValues(5011);
                DataView ldvBatch = ldtBatch.DefaultView;
                ldvBatch.Sort = "data3";
                ldtBatch = new DataTable();
                ldtBatch = ldvBatch.ToTable();

                foreach (DataRow dr in ldtBatch.Rows)
                {
                    if (dr["data1"] != DBNull.Value)
                    {
                        RunBatch(Convert.ToInt32(dr["data1"]), Convert.ToInt32(dr["data1"]));
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }
            //Starting back the timer
            //this.processNeospinBatchTimer.Start();
        }

        private void RunBatch(int aintStartStep, int aintLastStep)
        {
            frmNeoSpinBatch lobjNeoSpinBatch;
            lobjNeoSpinBatch = new frmNeoSpinBatch(aintStartStep, aintLastStep, false, true);
            lobjNeoSpinBatch.SetStepIndexFromService();
            lobjNeoSpinBatch.iblnFromNeoFlowService = true;
            lobjNeoSpinBatch.RunBatch();
        }

        [TimerActionMethod("CheckRequests")]
        public void CheckRequests()
        {
            try
            {
                utlPassInfo.iobjPassInfo = BPMDBHelper.CreatePassInfo(istrUserId);
                BPMDBHelper.BeginTransaction(utlPassInfo.iobjPassInfo);

                DBFunction.DBNonQuery("entWorkflowRequest.UpdateFailedRequests", new object[] {},
                    utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                BPMDBHelper.Commit(utlPassInfo.iobjPassInfo);
            }
            catch (Exception ex)
            {
                BPMDBHelper.RollbackTransaction(utlPassInfo.iobjPassInfo);
                ExceptionManager.Publish(new Exception("Error occured while resetting status of unprocessed document requests.", ex));
            }
        }

        [TimerActionMethodAttribute("ProcessNeospinBatchWSS")]
        public void ProcessNeospinBatchWSS()
        {
            //We are stopping the Timer here because if the task execution time takes more than interval time
            //this.processNeospinBatchTimerWSS.Stop();
            InitializePassInfo();

            utlPassInfo.iobjPassInfoProcessLog = utlPassInfo.iobjPassInfo;

            //Task Begins
            try
            {
                RunBatch(busConstant.EmailNotificationStepNumber, busConstant.EmailNotificationStepNumber);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
            }
            finally
            {
                FreePassInfo(utlPassInfo.iobjPassInfo);
            }
            //Starting back the timer
            //this.processNeospinBatchTimerWSS.Start();
        }

        //private void InitializeWorkflow(busWorkflowRequest abusWorkflowRequest)
        //{
        //    ArrayList larrList = new ArrayList();
        //    string lstrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_UnProcessed;
        //    bool lblnInitiateGenericWorkflow = false;

        //    //If the Trigger initialize by Scanning & Indexing
        //    if (!String.IsNullOrEmpty(abusWorkflowRequest.icdoWorkflowRequest.document_code))
        //    {
        //        /*****************************************************************************
        //         * 1)Load the list of Processes waiting for this document. (Required = Yes , Approved = NO in checklist)
        //         * 2)If no document waits for this document, get all the "Always Initiate" Processes for this document and Initiate the Workflow. 
        //         *      2.1) if no always initiated process for this document, then ignore the request.
        //         * 3)Loop through all the instances waiting for this document, and update the received date in checklist table for those instances.
        //         *      3.1) if any process is in suspended mode, resume it (based on the resume option condition)
        //         * **************************************************************************************/
        //        if (abusWorkflowRequest.ibusDocument == null)
        //            abusWorkflowRequest.LoadDocument();
        //        if (abusWorkflowRequest.ibusDocument.icdoDocument.document_id > 0)
        //        {
        //            if (abusWorkflowRequest.ibusDocument.icdoDocument.ignore_process_flag == busConstant.Flag_Yes)
        //            {
        //                lstrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Ignored;
        //                //DO NOT GENERATE GENERIC WORKFLOW.
        //            }
        //            else
        //            {
        //                Collection<busActivityInstance> lclbActivityInstance = LoadRunningProcessesWaitingforDocument(abusWorkflowRequest);
        //                //If Running Instances waiting for this document, Attach it
        //                bool lblnRunningInstanceExists = false;
        //                if (lclbActivityInstance.Count > 0)
        //                {
        //                    foreach (busActivityInstance lobjActivityInstance in lclbActivityInstance)
        //                    {
        //                        //PROD PIR 5005 : Do the Resume Process (Attach to old) only if the instance is in first activity.
        //                        //This apprach is specific to NDPERS only. Not a Standard Workflow Appraoch.
        //                        if (lobjActivityInstance.ibusActivity == null)
        //                            lobjActivityInstance.LoadActivity();
        //                        if (lobjActivityInstance.ibusActivity.icdoActivity.iblnFirstActivityFlag)
        //                        {
        //                            if (lobjActivityInstance.icdoActivityInstance.status_value == busConstant.ActivityStatusSuspended)
        //                            {
        //                                //Resume only if the Resume Action is Any Document / Empty, else Check All other Documents Received
        //                                if ((lobjActivityInstance.icdoActivityInstance.resume_action_value == busConstant.ResumeActionAnyDocument) ||
        //                                    (String.IsNullOrEmpty(lobjActivityInstance.icdoActivityInstance.resume_action_value)) ||
        //                                    (IsAllDocumentsReceived(lobjActivityInstance.icdoActivityInstance.process_instance_id, abusWorkflowRequest.ibusDocument.icdoDocument.document_id))
        //                                   )
        //                                {
        //                                    //Assign the Suspended User ID to Resumed User Id   
        //                                    utlPassInfo.iobjPassInfo.istrUserID = lobjActivityInstance.icdoActivityInstance.checked_out_user;

        //                                    larrList = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lobjActivityInstance, utlPassInfo.iobjPassInfo);

        //                                    //If there is any unexpected errors like Engine Not Running, It will remain with unprocessed status
        //                                    if ((larrList.Count > 0) && (larrList[0] is utlError)) return;
        //                                }
        //                            }

        //                            lblnRunningInstanceExists = true;
        //                            //Update the Received Date in Process Instance CheckList
        //                            UpdateProcessInstanceCheckList(lobjActivityInstance.icdoActivityInstance.process_instance_id, abusWorkflowRequest);

        //                            //Insert the Image Data
        //                            InsertWFImageData(abusWorkflowRequest, lobjActivityInstance.icdoActivityInstance.process_instance_id);
        //                        }
        //                    }
        //                }
        //                if (lblnRunningInstanceExists)
        //                {
        //                    //Reassign the User ID 
        //                    utlPassInfo.iobjPassInfo.istrUserID = NEOFLOW_SERVICE;
        //                    lstrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Processed;
        //                }
        //                else
        //                {
        //                    //If No Running Instances waiting for this document, 
        //                    //Load All the "Always Initiate" Process for this document and Initiate the Workflow
        //                    Collection<busDocumentProcessCrossref> lclbDocumentProcessCrossref =
        //                        LoadAllInitiateActionProcessesForDocument(abusWorkflowRequest.ibusDocument.icdoDocument.document_id);
        //                    if (lclbDocumentProcessCrossref.Count > 0)
        //                    {
        //                        bool lblnProcessedEntryFound = false;
        //                        foreach (busDocumentProcessCrossref lobjDocumentProcessCrossref in lclbDocumentProcessCrossref)
        //                        {
        //                            larrList = CreateWorkflow(abusWorkflowRequest,
        //                                                      abusWorkflowRequest.ibusDocument.icdoDocument.document_id,
        //                                                      lobjDocumentProcessCrossref.icdoDocumentProcessCrossref.process_id,
        //                                                      ref lstrWorkflowProcessStatus, ref lblnInitiateGenericWorkflow);

        //                            if (lstrWorkflowProcessStatus == busConstant.WorkflowProcessStatus_Processed)
        //                                lblnProcessedEntryFound = true;

        //                            //If there is any unexpected errors like Engine Not Running, It will remain with unprocessed status
        //                            if ((larrList.Count > 0) && (larrList[0] is utlError)) return;

        //                        }
        //                        lstrWorkflowProcessStatus = lblnProcessedEntryFound ? busConstant.WorkflowProcessStatus_Processed : busConstant.WorkflowProcessStatus_Ignored;
        //                    }
        //                    else
        //                    {
        //                        lstrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Ignored;
        //                        lblnInitiateGenericWorkflow = true;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //PROD FIX : when there is no document associated, simply ignore the request.
        //            lstrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Ignored;
        //            lblnInitiateGenericWorkflow = true;
        //        }
        //    }
        //    else if (abusWorkflowRequest.icdoWorkflowRequest.process_id != 0)
        //    {
        //        /****************************************************************************************************
        //         * Initialize Workflow by Process (Online Initialization)
        //         * *************************************************************************************************/
        //        larrList = CreateWorkflow(abusWorkflowRequest, 0, abusWorkflowRequest.icdoWorkflowRequest.process_id, ref lstrWorkflowProcessStatus, ref lblnInitiateGenericWorkflow);
        //        if ((larrList.Count > 0) && (larrList[0] is utlError)) return;
        //    }

        //    if (lblnInitiateGenericWorkflow)
        //    {
        //        /****************************************************************************************************
        //        * Initialize Generic Workflow When the request goes to ignored in certain cases
        //        * *************************************************************************************************/
        //        larrList = CreateWorkflow(abusWorkflowRequest, abusWorkflowRequest.ibusDocument.icdoDocument.document_id,
        //            busConstant.Map_Resolve_Incoming_Mail, ref lstrWorkflowProcessStatus, ref lblnInitiateGenericWorkflow);
        //        if ((larrList.Count > 0) && (larrList[0] is utlError)) return;
        //    }

        //    //Update the Process Status
        //    abusWorkflowRequest.icdoWorkflowRequest.status_value = lstrWorkflowProcessStatus;
        //    abusWorkflowRequest.icdoWorkflowRequest.Update();
        //}

        //private Collection<busNeobaseBpmActivityInstance> LoadRunningProcessesWaitingforDocument(busWorkflowRequest abusWorkflowRequest)
        //{
        //    Collection<busNeobaseBpmActivityInstance> lclbActivityInstance = new Collection<busNeobaseBpmActivityInstance>();
        //    if (abusWorkflowRequest.icdoWorkflowRequest.person_id != 0)
        //    {
        //        lclbActivityInstance =
        //            busGlobalFunctions.LoadRunningWorkflowByDocumentAndPerson(
        //                abusWorkflowRequest.icdoWorkflowRequest.person_id, abusWorkflowRequest.icdoWorkflowRequest.document_code);
        //    }
        //    else if (!String.IsNullOrEmpty(abusWorkflowRequest.icdoWorkflowRequest.org_code))
        //    {
        //        lclbActivityInstance =
        //            busGlobalFunctions.LoadRunningWorkflowByDocumentAndOrgCode(
        //                abusWorkflowRequest.icdoWorkflowRequest.org_code, abusWorkflowRequest.icdoWorkflowRequest.document_code);
        //    }

        //    return lclbActivityInstance;

        //}
        //private bool IsAllDocumentsReceived(int aintWorkflowInstanceId, int aintDocumentID)
        //{
        //    DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoProcessInstanceChecklist.LoadAllApprovedRequiredDocumentsExceptGivenDocument",
        //      new object[2] { aintWorkflowInstanceId, aintDocumentID });

        //    if (ldtpActivityInstance.Rows.Count > 0)
        //        return false;

        //    return true;
        //}

        //public static Collection<busNeobaseBpmActivityInstance> LoadRunningWorkflowByDocumentAndPerson(int aintPersonID, string astrDocumentCode)
        //{
        //    Collection<busNeobaseBpmActivityInstance> lclbActivityInstance = new Collection<busNeobaseBpmActivityInstance>();
        //    DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndPerson",//need to convert query
        //        new object[2] { aintPersonID, astrDocumentCode });
        //    busBase lobjBase = new busBase();
        //    lclbActivityInstance = lobjBase.GetCollection<busNeobaseBpmActivityInstance>(ldtpActivityInstance, "icdoBpmActivityInstance");
        //    return lclbActivityInstance;
        //}
        //public static Collection<busNeobaseBpmActivityInstance> LoadRunningWorkflowByDocumentAndOrgCode(string astrOrgCode, string astrDocumentCode)
        //{
        //    Collection<busNeobaseBpmActivityInstance> lclbActivityInstance = new Collection<busNeobaseBpmActivityInstance>();
        //    DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndOrgCode",
        //        new object[2] { astrOrgCode, astrDocumentCode });
        //    busBase lobjBase = new busBase();
        //    lclbActivityInstance = lobjBase.GetCollection<busNeobaseBpmActivityInstance>(ldtpActivityInstance, "icdoBpmActivityInstance");
        //    return lclbActivityInstance;
        //}

        //private void UpdateProcessInstanceCheckList(int aintProcessInstanceID, busWorkflowRequest abusWorkflowRequest)
        //{
        //    busProcessInstanceChecklist lobjProcessInstanceChecklist = new busProcessInstanceChecklist();
        //    if (lobjProcessInstanceChecklist.FindProcessInstanceChecklistByInstanceAndDocument(aintProcessInstanceID, abusWorkflowRequest.ibusDocument.icdoDocument.document_id))
        //    {
        //        lobjProcessInstanceChecklist.icdoProcessInstanceChecklist.received_date = abusWorkflowRequest.icdoWorkflowRequest.initiated_date;
        //        lobjProcessInstanceChecklist.icdoProcessInstanceChecklist.Update();
        //    }
        //}

        //private Collection<busDocumentProcessCrossref> LoadAllInitiateActionProcessesForDocument(int aintDocumentID)
        //{
        //    //Loading the Always Initiate Process 
        //    Collection<busDocumentProcessCrossref> lclbDocumentProcessCrossref = new Collection<busDocumentProcessCrossref>();
        //    DataTable ldtpDocumentProcessCrossref =
        //        busNeoSpinBase.Select("cdoDocumentProcessCrossref.LoadAlwaysInitiateProcessesForDocument",
        //                              new object[1] { aintDocumentID });

        //    if (ldtpDocumentProcessCrossref.Rows.Count == 0)
        //    {
        //        //Load Resume or Initiate Process for the Document
        //        ldtpDocumentProcessCrossref =
        //            busNeoSpinBase.Select("cdoDocumentProcessCrossref.LoadResumeOrInitiateProcessesForDocument",
        //                                  new object[1] { aintDocumentID });
        //    }

        //    busBase lobjBase = new busBase();
        //    lclbDocumentProcessCrossref = lobjBase.GetCollection<busDocumentProcessCrossref>(ldtpDocumentProcessCrossref, "icdoDocumentProcessCrossref");
        //    return lclbDocumentProcessCrossref;
        //}

        //private ArrayList CreateWorkflow(busWorkflowRequest abusWorkflowRequest, int aintDocumentID, int aintProcessID, ref string astrWorkflowProcessStatus, ref bool ablnInitiateGenericWorkflow)
        //{
        //    ArrayList larrList = new ArrayList();

        //    busProcess lbusProcess = new busProcess();
        //    lbusProcess.FindProcess(aintProcessID);

        //    //If the Process is Inactive, We should not Initiate the Workflow
        //    //For Scanning & Indexing Related workflow, we need to put this check here only.
        //    if (lbusProcess.icdoProcess.status_value == "INAT")
        //    {
        //        astrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Ignored;
        //        ablnInitiateGenericWorkflow = true;
        //        return larrList;
        //    }

        //    //UCS - 081 : Execute Death Match Inbound file
        //    //If an active workflow is present for a person, workflow should not be intiated and need to update the filenet entry with Ignored status
        //    if (aintProcessID == busConstant.Map_Process_Death_Match)
        //    {
        //        busActivityInstance lobjActivityInstance = new busActivityInstance();
        //        DataTable ldtActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadAllInstancesByProcessAndReference",
        //                                             new object[2] { abusWorkflowRequest.icdoWorkflowRequest.process_id, abusWorkflowRequest.icdoWorkflowRequest.person_id });
        //        if (ldtActivityInstance.Rows.Count > 0)
        //        {
        //            astrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Ignored;
        //            return larrList;
        //        }
        //    }

        //    WorkflowResult lobjWorkflowResult = null;

        //    try
        //    {
        //        if (abusWorkflowRequest.ibusOrganization == null)
        //            abusWorkflowRequest.LoadOrganization();
        //        if (workflowServiceURL.IsNullOrEmpty())
        //            workflowServiceURL = NeoSpin.Common.ApplicationSettings.Instance.NEOFLOW_SERVICE_TIER_URL;
        //        IWorkflowEngine engine = (IWorkflowEngine)Activator.GetObject(typeof(IWorkflowEngine), workflowServiceURL);
        //        lobjWorkflowResult = engine.Run(abusWorkflowRequest.icdoWorkflowRequest.workflow_request_id,
        //                                        aintProcessID,
        //                                        abusWorkflowRequest.icdoWorkflowRequest.person_id,
        //                                        abusWorkflowRequest.ibusOrganization.icdoOrganization.org_id,
        //                                        abusWorkflowRequest.icdoWorkflowRequest.reference_id,
        //                                        abusWorkflowRequest.icdoWorkflowRequest.created_by,
        //                                        abusWorkflowRequest.icdoWorkflowRequest.contact_ticket_id);

        //        if ((lobjWorkflowResult.InstanceId != null) && ((lobjWorkflowResult.Status != WorkflowStatus.Aborted) ||
        //                                          (lobjWorkflowResult.Status != WorkflowStatus.Terminated)))
        //        {
        //            /*****************************************************************************************
        //           * Inserting Image Data into WF_IMAGE_DATA Table (If Available)                 
        //           ****************************************************************************************/
        //            InsertWFImageData(abusWorkflowRequest, lobjWorkflowResult.process_instance_id);

        //            /*****************************************************************************************
        //             * Get the Checklist Document from SGT_DOCUMENT_PROCESS_CROSSREF table
        //             * and put it into SGT_PROCESS_INSTANCE_CHECKLIST table
        //             ****************************************************************************************/
        //            MoveCheckListDocumentsToProcessInstance(abusWorkflowRequest, lobjWorkflowResult, aintProcessID, aintDocumentID);

        //            astrWorkflowProcessStatus = busConstant.WorkflowProcessStatus_Processed;
        //        }
        //        else
        //        {
        //            //Return with Error, so that it remain with Unprocessed Status
        //            string lstrErrorMessage = "An error occured in creating workflow for the system request id: " +
        //                abusWorkflowRequest.icdoWorkflowRequest.workflow_request_id + " Workflow Status : " + lobjWorkflowResult.Status.ToString();
        //            ExceptionManager.Publish(new Exception(lstrErrorMessage));

        //            utlError lutlError = new utlError();
        //            lutlError.istrErrorMessage = lstrErrorMessage;
        //            larrList.Add(lutlError);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Return with Error, so that it remain with Unprocessed Status
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = ex.Message;
        //        larrList.Add(lutlError);

        //        ExceptionManager.Publish(new Exception("An error occured in creating workflow for the system request id: " +
        //            abusWorkflowRequest.icdoWorkflowRequest.workflow_request_id + " Error Message : " + ex));
        //    }

        //    return larrList;
        //}

        //private void InsertWFImageData(busWorkflowRequest abusWorkflowRequest, int aintProcessInstanceID)
        //{
        //    //This code is commented because Document Type could be null for ORG Related Documents
        //    //&&(!String.IsNullOrEmpty(acdoWfImageDataFromFilenet.filenet_document_type))
        //    if (
        //        (!String.IsNullOrEmpty(abusWorkflowRequest.icdoWorkflowRequest.image_doc_category))
        //       )
        //    {
        //        cdoProcessInstanceImageData lcdoProcessInstanceImageData = new cdoProcessInstanceImageData();
        //        cdoCodeValue lcdoImageCategoryCodeValue = busGlobalFunctions.GetCodeValueByDescription(
        //            busConstant.ImageDoc_Category_Code_ID, abusWorkflowRequest.icdoWorkflowRequest.image_doc_category);
        //        lcdoProcessInstanceImageData.image_doc_category_value = lcdoImageCategoryCodeValue.code_value;
        //        cdoCodeValue lcdoFileNetDocTypeCodeValue = busGlobalFunctions.GetCodeValueByDescription(
        //            busConstant.FileNet_Document_Type_Code_ID, abusWorkflowRequest.icdoWorkflowRequest.filenet_document_type);
        //        lcdoProcessInstanceImageData.filenet_document_type_value = lcdoFileNetDocTypeCodeValue.code_value;
        //        lcdoProcessInstanceImageData.document_code = abusWorkflowRequest.icdoWorkflowRequest.document_code;
        //        lcdoProcessInstanceImageData.initiated_date = abusWorkflowRequest.icdoWorkflowRequest.initiated_date;
        //        lcdoProcessInstanceImageData.process_instance_id = aintProcessInstanceID;
        //        lcdoProcessInstanceImageData.Insert();
        //    }
        //}

        //private void MoveCheckListDocumentsToProcessInstance(busWorkflowRequest abusWorkflowRequest,
        //                                                         WorkflowResult aobjWorkflowResult,
        //                                                         int aintProcessID, int aintDocumentID)
        //{
        //    Collection<busDocumentProcessCrossref> lclbDocumentProcessCrossref = new Collection<busDocumentProcessCrossref>();

        //    //If the Process initiated by Document, load all the process document ref by Document ID
        //    DataTable ldtpDocumentProcessCrossref = busNeoSpinBase.Select("cdoDocumentProcessCrossref.LoadAllDocumentProcessRefByProcess",
        //        new object[1] { aintProcessID });

        //    if (ldtpDocumentProcessCrossref != null && ldtpDocumentProcessCrossref.Rows.Count > 0)
        //    {
        //        busBase lobjBase = new busBase();
        //        lclbDocumentProcessCrossref =
        //            lobjBase.GetCollection<busDocumentProcessCrossref>(ldtpDocumentProcessCrossref, "icdoDocumentProcessCrossref");
        //        foreach (busDocumentProcessCrossref lobjDocumentProcessCrossref in lclbDocumentProcessCrossref)
        //        {
        //            cdoProcessInstanceChecklist lcdoCdoProcessInstanceChecklist = new cdoProcessInstanceChecklist();
        //            lcdoCdoProcessInstanceChecklist.process_instance_id = aobjWorkflowResult.process_instance_id;
        //            lcdoCdoProcessInstanceChecklist.document_id = lobjDocumentProcessCrossref.icdoDocumentProcessCrossref.document_id;
        //            lcdoCdoProcessInstanceChecklist.required_flag = busConstant.Flag_Yes;
        //            lcdoCdoProcessInstanceChecklist.approved_flag = busConstant.Flag_No;
        //            //Assign the Initiated Date for the Current Document
        //            if (aintDocumentID == lobjDocumentProcessCrossref.icdoDocumentProcessCrossref.document_id)
        //                lcdoCdoProcessInstanceChecklist.received_date = abusWorkflowRequest.icdoWorkflowRequest.initiated_date;
        //            lcdoCdoProcessInstanceChecklist.Insert();
        //        }
        //    }
        //}
        //public void LoadUnprocessedWorkflowInstance()
        //{
        //    DataTable ldtbWfUnprocessedInstance = busBase.Select<cdoWorkflowRequest>(new string[1] { "status_value" },
        //          new object[1] { busConstant.WorkflowProcessStatus_UnProcessed }, null, null);

        //    //Loading the Collection            
        //    busBase lbusBase = new busBase();
        //    iclbWfUnprocessedInstance = lbusBase.GetCollection<busWorkflowRequest>(ldtbWfUnprocessedInstance, "icdoWorkflowRequest");
        //}
        protected override void SendTraceInstActDtlToQueue(utlBpmTraceInstActnDtl lobjBpmTraceInstActnDtl)
        {
            if (utlBpmLogInfo.Instance != null
            && utlBpmLogInfo.Instance.iblnBpmTracing
            && utlThreadStatic.Instance.iobjBpmTraceInstActn != null)
            {
                if (lobjBpmTraceInstActnDtl != null)
                {
                    lobjBpmTraceInstActnDtl.end_time = utlPassInfo.iobjPassInfo.ApplicationDateTime;
                    MessageQueueHelper.SendToQueue(lobjBpmTraceInstActnDtl);
                }
            }
        }
        [TimerActionMethod("UpdateActivityInstanceElegibleUsers")]
        public virtual void UpdateActivityInstanceElegibleUsers()
        {
            UpdateActivityInstanceUser();
        }
        public void UpdateActivityInstanceUser()
        {
            DateTime ldtStartDatetime = DateTime.Now;
            bool lblnCheckForUnprocessedInstances = true;
            while (lblnCheckForUnprocessedInstances)
            {
                string lstrExceptionMessage = string.Empty;
                try
                {
                    utlPassInfo.iobjPassInfo = BPMDBHelper.CreatePassInfo(BPMService._istrUserId);
                    //Get all UnAssigned Items and send to BPM to have a Role Check again, and then update the a activity instance user
                    DataTable ldtbActivityInstance = busBase.Select("entBpmActivityInstance.GetUnAssignedItemsForRoleCheck", new object[1] { ldtStartDatetime });
                    lblnCheckForUnprocessedInstances = ldtbActivityInstance?.Rows?.Count == 5;
                    if (ldtbActivityInstance?.Rows?.Count > 0)
                    {
                        foreach (DataRow ldtrRow in ldtbActivityInstance.Rows)
                        {
                            busBpmActivityInstance lbusBpmActivityInstance = new busBpmActivityInstance();
                            try
                            {
                                busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
                                if (ldtrRow["ACTIVITY_INSTANCE_ID"].IsNotNull() && ((int)ldtrRow["ACTIVITY_INSTANCE_ID"]) > 0)
                                {
                                    lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(((int)ldtrRow["ACTIVITY_INSTANCE_ID"]));
                                    lbusBpmActivityInstance.UpdateEligibleUsers();
                                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
                                    lcolParameters.Add(DBFunction.GetDBParameter("@ACTIVITY_INSTANCE_ID", "int", lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id, utlPassInfo.iobjPassInfo.iconFramework));
                                    DBFunction.DBNonQuery("UPDATE SGW_BPM_ACTIVITY_INSTANCE SET MODIFIED_DATE=getdate() WHERE ACTIVITY_INSTANCE_ID = @ACTIVITY_INSTANCE_ID", lcolParameters, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ldtrRow["ACTIVITY_INSTANCE_ID"].IsNotNull())
                                {
                                    lstrExceptionMessage = "Activity Instance Id = " + Convert.ToString(ldtrRow["ACTIVITY_INSTANCE_ID"]) + " : ";
                                    ExceptionManager.Publish(new Exception(lstrExceptionMessage + ex.Message + " " + ex.StackTrace));
                                }
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    lstrExceptionMessage = "Outside the item loop :";
                    ExceptionManager.Publish(new Exception(lstrExceptionMessage + ex.Message + " " + ex.StackTrace));
                }
                finally
                {
                    BPMDBHelper.FreePassInfo(utlPassInfo.iobjPassInfo);
                }
            }
        }
        #region Resume Stuck Instances Processing 
        public bool iblnServiceStopped { get; set; }
        public int iintQueueItemReprocessingInterval { get; private set; }
        public int iintActivityInstanceReprocessingInterval { get; private set; }

        public override void Stop()
        {
            this.iblnServiceStopped = true;
            base.Stop();
        }
        [TimerActionMethod("ResumeStuckInstances")]
        public void ResumeStuckInstances()
        {
            try
            {
                bool lblnCanProcess = true;
                utlPassInfo.iobjPassInfo = BPMDBHelper.CreatePassInfo(this.istrUserId);
                while (!this.iblnServiceStopped && lblnCanProcess)
                {
                    DataTable ldtStuckInstances = busBase.Select("entBpmQueue.GetStuckInstancesforAutoResume", new object[] { });
                    if (!ldtStuckInstances.IsNullOrEmpty())
                    {
                        foreach (DataRow row in ldtStuckInstances.Rows)
                        {
                            this.CheckAndAutoResumeStuckInstance(row);
                        }
                    }
                    else
                    {
                        lblnCanProcess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
            }
            finally
            {
                BPMDBHelper.FreePassInfo(utlPassInfo.iobjPassInfo);
            }
        }

        private void CheckAndAutoResumeStuckInstance(DataRow row)
        {
            string lstQueueItemStatus = row.CheckAndGetValue<string>("STATUS_VALUE");
            switch (lstQueueItemStatus)
            {
                case BpmQueueStatus.Picked:
                    DateTime dtQueueItemModifiedDate = row.CheckAndGetValue<DateTime>("MODIFIED_DATE");
                    // Reprocess the queque item after 10 mins
                    if (dtQueueItemModifiedDate.AddMinutes(this.iintQueueItemReprocessingInterval) < DateTime.Now)
                    {
                        this.ProcessStukInstance(row, this.iintActivityInstanceReprocessingInterval);
                    }
                    break;
                case BpmQueueStatus.Failed:
                    this.ProcessStukInstance(row, aintActivityInstanceProcessingInterval: 0);
                    break;
            }

        }

        private static ReadOnlyCollection<string> ExcludeActivityTypesForResume = new List<string>()
                                                                                    {
                                                                                    BpmActivityTypes.ServiceTask,
                                                                                    BpmActivityTypes.BusinessRuleTask
                                                                                    }.AsReadOnly();


        private void ProcessStukInstance(DataRow row, int aintActivityInstanceProcessingInterval)
        {
            try
            {
                BPMDBHelper.BeginTransaction(utlPassInfo.iobjPassInfo);

                string lstrActivityInstanceStatus = row.CheckAndGetValue<string>("ACTIVITY_INSTANCE_STATUS");
                int lintActivityInstanceID = row.CheckAndGetValue<int>("ACTIVITY_INSTANCE_ID");
                int lintACTV_INST_ID_TO_EXEC = row.CheckAndGetValue<int>("ACTV_INST_ID_TO_EXEC");
                string lstrQUEUE_REQUEST_TYPE_VALUE = row.CheckAndGetValue<string>("QUEUE_REQUEST_TYPE_VALUE");
                if (lstrQUEUE_REQUEST_TYPE_VALUE == BpmQueueRequestType.ResumeTaskRequest)
                {
                    DataTable ldtActivityInstance = busBase.Select("entBpmQueue.GetActivityInstanceToResumeStuckInstance", new object[1] { lintACTV_INST_ID_TO_EXEC });
                    if (ldtActivityInstance.Rows.Count == 1)
                    {
                        lintActivityInstanceID = lintACTV_INST_ID_TO_EXEC;
                        lstrActivityInstanceStatus = ldtActivityInstance.Rows[0].CheckAndGetValue<string>("STATUS_VALUE");
                        row["ACTIVITY_INSTANCE_MODIFIED_DATE"] = ldtActivityInstance.Rows[0]["MODIFIED_DATE"];
                        row["ACTIVITY_TYPE_VALUE"] = ldtActivityInstance.Rows[0]["ACTIVITY_TYPE_VALUE"];
                        row["INITIATOR_INSTANCE_ID"] = ldtActivityInstance.Rows[0]["INITIATOR_INSTANCE_ID"];
                        row["ACTIVITY_INSTANCE_STATUS"] = ldtActivityInstance.Rows[0]["STATUS_VALUE"];
                        row["CASE_INSTANCE_ID"] = ldtActivityInstance.Rows[0]["CASE_INSTANCE_ID"];
                        if (row.Table.Columns.Contains("REQUEST_ID"))
                        {
                            row["CASE_INSTANCE_ID"] = ldtActivityInstance.Rows[0]["CASE_INSTANCE_ID"];
                        }
                    }
                }

                if (lintActivityInstanceID == 0) // Activity Instance is not created
                {
                    this.ResetQueueItemStatus(row);
                }
                else
                {
                    DateTime dtActInstModifiedDate = row.CheckAndGetValue<DateTime>("ACTIVITY_INSTANCE_MODIFIED_DATE");
                    // Reprocess the queque item with some delay
                    if (dtActInstModifiedDate.AddMinutes(aintActivityInstanceProcessingInterval) < DateTime.Now)
                    {
                        switch (lstrActivityInstanceStatus)
                        {
                            case BpmActivityInstanceStatus.NotStarted: //UNPC
                                this.ResetQueueItemStatus(row);
                                break;
                            case BpmActivityInstanceStatus.InProcess:

                                string lstrActivityType = row.CheckAndGetValue<string>("ACTIVITY_TYPE_VALUE");
                                if (!ExcludeActivityTypesForResume.Contains(lstrActivityType))
                                {
                                    this.ResetQueueItemStatus(row);
                                }
                                else
                                {
                                    //Manual Resume Flow
                                }
                                break;
                            case BpmActivityInstanceStatus.Processed:
                            case BpmActivityInstanceStatus.Approved:
                            case BpmActivityInstanceStatus.Rejected:
                                this.InitiateOutgoingActivities(lintActivityInstanceID);
                                this.DeleteProcessedQueueItem(row);
                                break;
                        }
                    }
                }

                //Updatting BPM Request Status if the instance is stuck @ start event
                this.CheckAndUpdateBPMRequestStatus(row);

                BPMDBHelper.Commit(utlPassInfo.iobjPassInfo);

            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                BPMDBHelper.RollbackTransaction(utlPassInfo.iobjPassInfo);
            }

        }

        private void DeleteProcessedQueueItem(DataRow row)
        {
            doBpmQueue ldoBpmQueue = new doBpmQueue();
            ldoBpmQueue.LoadData(row);
            ldoBpmQueue.Delete();
        }

        private void CheckAndUpdateBPMRequestStatus(DataRow row)
        {
            string lstrActivityTypeValue = row.CheckAndGetValue<string>("ACTIVITY_TYPE_VALUE");
            string lstrInitiatorInstanceID = row.CheckAndGetValue<string>("INITIATOR_INSTANCE_ID");

            if (lstrActivityTypeValue == BpmActivityTypes.StartEvent && string.IsNullOrWhiteSpace(lstrInitiatorInstanceID))
            {
                int lintBPMRequestID = row.CheckAndGetValue<int>("REQUEST_ID");
                if (lintBPMRequestID != 0)
                {
                    busBpmRequest lbusBpmRequest = new busBpmRequest();
                    if (lbusBpmRequest.FindByPrimaryKey(lintBPMRequestID))
                    {
                        if (lbusBpmRequest.icdoBpmRequest.status_value != BpmRequestStatus.Processed)
                        {
                            lbusBpmRequest.icdoBpmRequest.status_value = BpmRequestStatus.Processed;
                            lbusBpmRequest.icdoBpmRequest.case_instance_id = row.CheckAndGetValue<int>("CASE_INSTANCE_ID");
                            lbusBpmRequest.icdoBpmRequest.Update();

                            //if (lobjbusBpmCaseInstance != null && busBpmRequestAction.iblnRequestAction)
                            //{
                            //    busBpmRequestAction lobjbusBpmRequestAction = new busBpmRequestAction();
                            //    lobjbusBpmRequestAction.NewRequestAction(lobjbusBpmCaseInstance, abusBpmRequest);
                            //}

                        }
                    }
                }
            }
        }
        private void InitiateOutgoingActivities(int lintActivityInstanceID)
        {
            busBpmCaseInstance lobjStuckCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
            busBpmActivityInstance lobjStuckActivityInstance = lobjStuckCaseInstance.LoadWithActivityInst(lintActivityInstanceID);
            if (lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.activity_type_value == BpmActivityTypes.EndEvent)
            {
                //if the activity is part of sub process or call activity, then complete sub-process or call activity and initiate next activity in main process
                if (!DBNull.Value.Equals(lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.parent_activity_id) && lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.parent_activity_id != 0)
                {
                    busBpmActivity lbusParentBpmActivity = lobjStuckActivityInstance.ibusBpmActivity.ibusBpmProcess.iclbBpmActivity.
                        Where(itm => itm.icdoBpmActivity.activity_id ==
                        lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.parent_activity_id).FirstOrDefault();
                    busBpmActivityInstance lbusParentActivityInstance =
                        lobjStuckActivityInstance.ibusBpmProcessInstance.iclbBpmActivityInstance.Where(activityInstance =>
                        activityInstance.icdoBpmActivityInstance.activity_id ==
                        lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.parent_activity_id &&
                        activityInstance.icdoBpmActivityInstance.status_value != BpmActivityInstanceStatus.Completed).FirstOrDefault();
                    if (null == lbusParentActivityInstance)
                    {
                        lbusParentActivityInstance = lobjStuckActivityInstance.ibusBpmProcessInstance.iclbBpmActivityInstance.
                            Where(activityInstance => activityInstance.icdoBpmActivityInstance.activity_id ==
                            lobjStuckActivityInstance.ibusBpmActivity.icdoBpmActivity.parent_activity_id).FirstOrDefault();
                    }
                    if (lbusParentBpmActivity is busBpmCallingProcess)
                    {
                        ((busBpmCallingProcess)lbusParentBpmActivity).SetOutParametersFormCalledProcess(lbusParentActivityInstance,
                            lobjStuckActivityInstance);
                    }
                    if (null != lbusParentActivityInstance)
                    {

                        lbusParentActivityInstance.ibusBpmProcessInstance.CompleteAndInitiate(lbusParentActivityInstance, null,
                            aenmActivityInitiateType: enmActivityInitiateType.InQueue);
                    }
                }
                else
                {
                    lobjStuckActivityInstance.ibusBpmProcessInstance.CompleteProcessInstance();
                }

            }
            else
            {
                lobjStuckActivityInstance.ibusBpmProcessInstance.InitiateOutGoingActivities(lobjStuckActivityInstance,
                    aenmActivityInitiateType: enmActivityInitiateType.InQueue);
            }
        }
        private void ResetQueueItemStatus(DataRow row)
        {
            doBpmQueue ldoBpmQueue = new doBpmQueue();
            ldoBpmQueue.LoadData(row);
            ldoBpmQueue.status_value = BpmQueueStatus.Unprocessed;
            ldoBpmQueue.Update();
        }
        #endregion
        protected override int ProcessRequests(Collection<busBpmRequest> lclbUnprocessedRequests, int lintSubTransactionId)
        {
            utlBpmTraceInstActnDtl lobjBpmTraceInstActnDtl = new utlBpmTraceInstActnDtl();
            try
            {
                //if (lclbUnprocessedRequests?.Count > 0)
                //{
                //    lclbUnprocessedRequests.ForEach(item => ProcessDetailsRecord(BpmTimerActionMethods.RequestHandler, item, ++lintSubTransactionId));
                //}
                if (lclbUnprocessedRequests?.Count > 0)
                {
                    Dictionary<string, List<busBpmRequest>> ldictRequests = new Dictionary<string, List<busBpmRequest>>();
                    foreach (busBpmRequest lbusBpmRequest in lclbUnprocessedRequests)
                    {
                        string key;
                        if (lbusBpmRequest.icdoBpmRequest.person_id != 0)
                        {
                            key = string.Format("p_{0}", lbusBpmRequest.icdoBpmRequest.person_id);
                        }
                        else if (lbusBpmRequest.icdoBpmRequest.org_id != 0)
                        {
                            key = string.Format("o_{0}", lbusBpmRequest.icdoBpmRequest.org_id);
                        }
                        else if (lbusBpmRequest.icdoBpmRequest.reference_id != 0)
                        {
                            key = string.Format("r_{0}", lbusBpmRequest.icdoBpmRequest.reference_id);
                        }
                        else
                        {
                            //to handle requests doesn't have person id , org id, reference id
                            key = "u_0";
                        }
                        if (!ldictRequests.ContainsKey(key))
                        {
                            ldictRequests.Add(key, new List<busBpmRequest>() { });
                        }

                        ldictRequests[key].Add(lbusBpmRequest);
                    }
                    //processing requests in groups 
                    Parallel.ForEach(ldictRequests, (keyValuePair) =>
                    {
                        foreach (busBpmRequest lbusBpmRequest in keyValuePair.Value)
                        {
                            ProcessDetailsRecord(BpmTimerActionMethods.RequestHandler, lbusBpmRequest, System.Threading.Interlocked.Increment(ref lintSubTransactionId));
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(new Exception("Error occured while processing Requests.", ex));
                busProcessBpmLog.ProcessException(BpmTimerActionMethods.RequestHandler, ex, "ProcessRequests", lintSubTransactionId);

            }
            return lintSubTransactionId;
        }
    }
}
