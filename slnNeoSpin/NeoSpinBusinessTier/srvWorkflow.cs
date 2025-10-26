//#region Using directives

//using System;
//using System.Data;
//using System.Collections;
//using NeoSpin.BusinessObjects;
//using NeoSpin.CustomDataObjects;
//using Sagitec.Common;


//#endregion

//namespace NeoSpin.BusinessTier
//{
//    public class srvWorkflow : srvNeoSpin
//    {
//        public srvWorkflow()
//        {
//            //
//            // TODO: Add constructor logic here
//            //
//        }

//        public busDocument FindDocument(int Aintdocumentid)
//        {
//            busDocument lobjDocument = new busDocument();
//            if (lobjDocument.FindDocument(Aintdocumentid))
//            {
//            }
//            return lobjDocument;
//        }

//        public busDocumentLookup LoadDocuments(DataTable adtbSearchResult)
//        {
//            busDocumentLookup lobjDocumentLookup = new busDocumentLookup();
//            lobjDocumentLookup.LoadDocuments(adtbSearchResult);
//            return lobjDocumentLookup;
//        }

//        public busDocument NewDocument()
//        {
//            busDocument lobjDocument = new busDocument();
//            lobjDocument.icdoDocument = new cdoDocument();
//            return lobjDocument;
//        }
//        public busDocumentProcessCrossref FindDocumentProcessCrossref(int Aintdocumentprocesscrossrefid)
//        {
//            busDocumentProcessCrossref lobjDocumentProcessCrossref = new busDocumentProcessCrossref();
//            if (lobjDocumentProcessCrossref.FindDocumentProcessCrossref(Aintdocumentprocesscrossrefid))
//            {
//                lobjDocumentProcessCrossref.LoadDocument();
//                lobjDocumentProcessCrossref.istrDocumentCode = lobjDocumentProcessCrossref.ibusDocument.icdoDocument.document_code;
//            }
//            return lobjDocumentProcessCrossref;
//        }

//        public busDocumentProcessCrossRefLookup LoadDocumentProcessCrossRefs(DataTable adtbSearchResult)
//        {
//            busDocumentProcessCrossRefLookup lobjDocumentProcessCrossRefLookup = new busDocumentProcessCrossRefLookup();
//            lobjDocumentProcessCrossRefLookup.LoadDocumentProcessCrossrefs(adtbSearchResult);
//            return lobjDocumentProcessCrossRefLookup;
//        }

//        public busDocumentProcessCrossref NewDocumentProcessCrossref(int aintProcessID)
//        {
//            busDocumentProcessCrossref lobjDocumentProcessCrossref = new busDocumentProcessCrossref();
//            lobjDocumentProcessCrossref.icdoDocumentProcessCrossref = new cdoDocumentProcessCrossref();
//            if (aintProcessID != 0)
//            {
//                lobjDocumentProcessCrossref.icdoDocumentProcessCrossref.process_id = aintProcessID;
//            }
//            return lobjDocumentProcessCrossref;
//        }

//        public busWFMimicIndexing FindWFMimicIndexing(int aintWFImageDataFilenetID)
//        {
//            busWFMimicIndexing lobjWFMimicIndexing = new busWFMimicIndexing();
//            if (lobjWFMimicIndexing.FindWFMimicIndexing(aintWFImageDataFilenetID))
//            {

//            }
//            return lobjWFMimicIndexing;
//        }

//        public busProcess FindProcess(int aintprocessid)
//        {
//            busProcess lobjProcess = new busProcess();
//            if (lobjProcess.FindProcess(aintprocessid))
//            {
//                lobjProcess.LoadActivityList();
//                lobjProcess.LoadDocumentProcessCrossrefs();
//            }

//            return lobjProcess;
//        }

//        public busProcessLookup LoadProcess(DataTable adtbSearchResult)
//        {
//            busProcessLookup lobjProcessLookup = new busProcessLookup();
//            lobjProcessLookup.LoadProcess(adtbSearchResult);
//            return lobjProcessLookup;
//        }

//        public busActivity FindActivity(int aintactivityid)
//        {
//            busActivity lobjActivity = new busActivity();
//            if (lobjActivity.FindActivity(aintactivityid))
//            {
//                lobjActivity.LoadProcess();
//                lobjActivity.LoadRoles();
//            }

//            return lobjActivity;
//        }

//        public busProcessInstance FindProcessInstance(int aintprocessinstanceid)
//        {
//            busProcessInstance lobjProcessInstance = new busProcessInstance();
//            if (lobjProcessInstance.FindProcessInstance(aintprocessinstanceid))
//            {
//                lobjProcessInstance.LoadContactTicket();
//                lobjProcessInstance.LoadOrganization();
//                lobjProcessInstance.LoadPerson();
//                lobjProcessInstance.LoadProcess();
//                lobjProcessInstance.LoadWorkflowRequest();
//            }

//            return lobjProcessInstance;
//        }

//        public busActivityInstance FindActivityInstance(int aintactivityinstanceid)
//        {
//            busActivityInstance lobjActivityInstance = new busActivityInstance();
//            if (lobjActivityInstance.FindActivityInstance(aintactivityinstanceid))
//            {
//                lobjActivityInstance.LoadActivity();
//                lobjActivityInstance.ibusActivity.LoadRoles();
//                lobjActivityInstance.LoadProcessInstance();
//                lobjActivityInstance.ibusProcessInstance.ibusProcess = lobjActivityInstance.ibusActivity.ibusProcess;
//                lobjActivityInstance.ibusProcessInstance.LoadOrganization();
//                lobjActivityInstance.ibusProcessInstance.LoadPerson();
//                lobjActivityInstance.ibusProcessInstance.LoadContactTicket();
//                lobjActivityInstance.LoadProcessInstanceImageData();
//                lobjActivityInstance.LoadProcessInstanceHistory();
//                lobjActivityInstance.LoadProcessInstanceChecklist();
//                lobjActivityInstance.LoadProcessInstanceNotes();
//                //prod pir 4118
//                //default suspension date to today's date + 30 days
//                if (lobjActivityInstance.icdoActivityInstance.suspension_end_date == DateTime.MinValue)
//                    lobjActivityInstance.icdoActivityInstance.suspension_end_date = DateTime.Now.AddDays(30);
//                //to set istrProceName and istrActivityName
//                if (lobjActivityInstance.ibusActivity != null && lobjActivityInstance.ibusActivity.icdoActivity != null)
//                    lobjActivityInstance.icdoActivityInstance.istrActivityName = lobjActivityInstance.ibusActivity.icdoActivity.name;
//                if (lobjActivityInstance.ibusProcessInstance != null && lobjActivityInstance.ibusProcessInstance.ibusProcess != null &&
//                    lobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess != null)
//                {
//                    lobjActivityInstance.icdoActivityInstance.istrProcessName = lobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name;
//                }
//                lobjActivityInstance.EvaluateInitialLoadRules();
//            }

//            return lobjActivityInstance;
//        }

//        public busWorkflowRequest FindWorkflowRequest(int aintworkflowrequestid)
//        {
//            busWorkflowRequest lobjWorkflowRequest = new busWorkflowRequest();
//            if (lobjWorkflowRequest.FindWorkflowRequest(aintworkflowrequestid))
//            {
//            }

//            return lobjWorkflowRequest;
//        }

//        public busProcessInstanceImageData FindProcessInstanceImageData(int aintprocessinstanceimagedataid)
//        {
//            busProcessInstanceImageData lobjProcessInstanceImageData = new busProcessInstanceImageData();
//            if (lobjProcessInstanceImageData.FindProcessInstanceImageData(aintprocessinstanceimagedataid))
//            {
//            }

//            return lobjProcessInstanceImageData;
//        }

//        public busReassignWork SearchAndLoadReassignmentBasket()
//        {
//            busReassignWork lobjReassignWork = new busReassignWork();
//            lobjReassignWork.SearchAndLoadReassignmentBasket();
//            return lobjReassignWork;
//        }

//        public busReassignWorkDetail LoadReassignmentWorkDetail(int aintActivityInstanceID)
//        {
//            busReassignWorkDetail lobjReassignWorkDetail = new busReassignWorkDetail();
//            lobjReassignWorkDetail.LoadReassigmentDetail(aintActivityInstanceID);
//            return lobjReassignWorkDetail;
//        }

//        public busProcessInitiation InitiateOnlineProcessInitiation()
//        {
//            busProcessInitiation lobjProcessInitiation = new busProcessInitiation();
//            return lobjProcessInitiation;
//        }

//        public busMyBasket SearchAndLoadMyBasket()
//        {
//            busMyBasket lobjMyBasket = null;
//            try
//            {
//                lobjMyBasket = iobjPassInfo.iutlObjectStore.GetObjectFromDB("wfmMyBasketMaintenance", 0) as busMyBasket; //Maintain my basket page state per session
//            }
//            catch { }
//            if (lobjMyBasket.IsNull())
//                lobjMyBasket = new busMyBasket();
//            //Set the Default Filter As Work Pool
//            if (string.IsNullOrEmpty(lobjMyBasket.istrMyBasketFilter)) //PROD PIR 22963 - My Basket Filter Should Be Retained
//                lobjMyBasket.istrMyBasketFilter = busConstant.MyBasketFilter_WorkPool;
//            lobjMyBasket.SearchAndLoadMyBasket();
//            return lobjMyBasket;
//        }

//        public busActivityInstanceHistory FindActivityInstanceHistory(int aintactivityinstancehistoryid)
//        {
//            busActivityInstanceHistory lobjActivityInstanceHistory = new busActivityInstanceHistory();
//            if (lobjActivityInstanceHistory.FindActivityInstanceHistory(aintactivityinstancehistoryid))
//            {
//                lobjActivityInstanceHistory.LoadActivityInstance();
//            }

//            return lobjActivityInstanceHistory;
//        }


//        //        public override void ModifyActivityRedirectInformation(utlWorkflowActivityInfo aobjWorkflowActivityInfo, utlProcessMaintainance.utlActivity aobjActivity, string astrButtonID, ArrayList aarrResult)
//        protected override void ModifyActivityRedirectInformation(utlWorkflowActivityInfo aobjWorkflowActivityInfo, utlProcessMaintainance.utlActivity aobjActivity, string astrButtonID, object aobjBase)
//        {
//            busActivityInstance lobAct = (busActivityInstance)aobjWorkflowActivityInfo.ibusBaseActivityInstance;
//            if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit && lobAct.icdoActivityInstance.activity_id == 171)
//            {
//                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
//                if (lobjPayeeAccount.FindPayeeAccount(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmPreRetirementDeathFinalCalculationMaintenance";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmPostRetirementDeathFinalCalculationMaintenance";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("reference_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Initialize_Process_Death_Notification_Workflow
//                       && lobAct.icdoActivityInstance.activity_id == 57 && lobAct.icdoActivityInstance.reference_id == 0)
//            {
//                if (lobAct.ibusProcessInstance.ibusWorkflowRequest == null)
//                    lobAct.ibusProcessInstance.LoadWorkflowRequest();

//                if (lobAct.ibusProcessInstance.ibusContactTicket == null)
//                    lobAct.ibusProcessInstance.LoadContactTicket();

//                if (lobAct.ibusProcessInstance.ibusContactTicket.icdoContactTicket.contact_ticket_id > 0)
//                {
//                    if (lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice == null)
//                    {
//                        lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice = new busDeathNotice();
//                    }
//                    lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice.FindDeathNoticeByContactTicket(lobAct.ibusProcessInstance.icdoProcessInstance.contact_ticket_id);
//                    aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("person_id", lobAct.ibusProcessInstance.icdoProcessInstance.person_id);
//                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("dateof_death", lobAct.ibusProcessInstance.ibusContactTicket.ibusDeathNotice.icdoDeathNotice.death_date);
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Update_Dues_Rate_Table
//                        && (lobAct.icdoActivityInstance.activity_id == 189 || lobAct.icdoActivityInstance.activity_id == 190) && lobAct.icdoActivityInstance.reference_id == 0)
//            {
//                if (lobAct.ibusProcessInstance.ibusOrganization == null)
//                    lobAct.ibusProcessInstance.LoadOrganization();

//                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("org_code", lobAct.ibusProcessInstance.ibusOrganization.icdoOrganization.org_code);
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans ||
//                lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Enroll_Retiree_Insurance_Plans ||
//                lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Enroll_COBRA_Insurance_Plans) //PIR 18493
//            {
//                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
//                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanRetirementEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestDBElectedOfficialMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRquestPensionPlanDCRetirementEnrollmentMaintenance";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestGHDVEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestLifeEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                    else if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_IBS_Insurance ||
//                lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_Insurance ||
//                lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_Retirement ||
//                lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_ACH_Pull_For_DeferredCompensation)
//            {
//                if (aobjWorkflowActivityInfo.istrURL == "wfmDepositTapeMaintenance")
//                {
//                    aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_activity_instance_id", lobAct.icdoActivityInstance.activity_instance_id);
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow)
//            {
//                //process instance is not loaded thats y loaded here again. for PIR - 1724
//                lobAct.LoadProcessInstance();
//                lobAct.ibusProcessInstance.LoadWorkflowRequest();

//                int lintBeneficiaryPersonId = 0;
//                if (!String.IsNullOrEmpty(lobAct.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.additional_parameter1))
//                    lintBeneficiaryPersonId = Convert.ToInt32(lobAct.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.additional_parameter1);

//                aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("member_person_id", lobAct.ibusProcessInstance.icdoProcessInstance.person_id);
//                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("recipient_person_id", lintBeneficiaryPersonId);
//                aobjWorkflowActivityInfo.ihstLaunchParameters.Add("benefit_application_id", lobAct.icdoActivityInstance.reference_id);
//            }  //PIR-10697 Start Sets the parameter required for the Find method in the Launching screen
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.MapMSSHDVAnnualEnrollment)
//            {
//                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
//                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeGHDV)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestGHDVEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.MSSLifeInsuranceAnnualEnrollment)
//            {
//                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
//                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeLife)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestLifeEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.MSSFlexcompAnnualEnrollment)
//            {
//                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
//                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }//PIR-10697 End  
//            else if (lobAct.ibusActivity.icdoActivity.process_id == busConstant.WSSProcessUpdateFlexCompPlan)//PIR-10820 Start
//            {
//                busWssPersonAccountEnrollmentRequest lobjEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
//                if (lobjEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lobAct.icdoActivityInstance.reference_id))
//                {
//                    if (lobjEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeFlexComp)
//                    {
//                        aobjWorkflowActivityInfo.ihstLaunchParameters = new Hashtable();
//                        aobjWorkflowActivityInfo.istrURL = "wfmViewRequestFlexCompEnrollmentMaintenanceLOB";
//                        aobjWorkflowActivityInfo.ihstLaunchParameters.Add("aint_request_id", lobAct.icdoActivityInstance.reference_id);
//                    }
//                }
//            }
//            //F/W Upgrade launch parameters were getting passed as null when the user does not do what he is expected to do as part of the activity.
//            utlProcessMaintainance.utlForm lobjForm = null;
//            foreach (var item in aobjActivity.iarrItems)
//            {
//                lobjForm = item as utlProcessMaintainance.utlForm;
//                if (lobjForm.IsNotNull() && lobjForm.ienmPageMode == aobjWorkflowActivityInfo.ienmPageMode)
//                    break;
//                else
//                    lobjForm = null;
//            }
//            if (lobjForm.IsNotNull())
//            {
//                string lstrParameterValue = string.Empty;
//                foreach (utlProcessMaintainance.utlParameter lutlParameter in lobjForm.icolParameters)
//                {
//                    if (lutlParameter.istrParameterValueSource == "Parameter")
//                    {
//                        lstrParameterValue = Convert.ToString(aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrParameterName]);
//                        if (string.IsNullOrEmpty(lstrParameterValue))
//                            lstrParameterValue = Convert.ToString(aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrFieldName]);
//                        if (string.IsNullOrEmpty(lstrParameterValue))
//                        {
//                            switch (lutlParameter.istrDataType)
//                            {
//                                case "int":
//                                    lstrParameterValue = "0";
//                                    break;
//                                case "string":
//                                    lstrParameterValue = string.Empty;
//                                    break;
//                            }
//                            aobjWorkflowActivityInfo.ihstLaunchParameters[lutlParameter.istrParameterName] = lstrParameterValue;
//                        }
//                    }
//                }
//            }
//            //PIR-10820 End
//        }

//        public busProcessInstanceImageData OpenFileNetImage(string astrObjectStore,
//                                                              string astrVersionSeriesId,
//                                                              string astrDocumentId,
//                                                              string astrDocumentTitle)
//        {
//            busProcessInstanceImageData lobjbusWfImageData = new busProcessInstanceImageData();
//            lobjbusWfImageData.icdoProcessInstanceImageData = new cdoProcessInstanceImageData();
//            lobjbusWfImageData.object_store = astrObjectStore;
//            lobjbusWfImageData.version_series_id = astrVersionSeriesId;
//            lobjbusWfImageData.document_id = astrDocumentId;
//            lobjbusWfImageData.document_title = astrDocumentTitle;
//            return lobjbusWfImageData;
//        }
//        //19011
//        public ArrayList GenerateCSVofBackLogSummary()
//        {
//            busActivityInstance ibusActivityInstance = new busActivityInstance();
//            ArrayList larlstResult = new ArrayList();
//            larlstResult.Add("BacklogSummaryReport" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_tt") + ".csv");
//            larlstResult.Add(ibusActivityInstance.GenerateCSVofBackLogSummary());
//            larlstResult.Add(System.Net.Mime.MediaTypeNames.Application.Octet);
//            return larlstResult;
//        }
//    }
//}
