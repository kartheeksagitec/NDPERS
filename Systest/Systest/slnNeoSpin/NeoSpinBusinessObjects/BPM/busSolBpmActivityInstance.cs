#region [Using Directives]
using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.CustomDataObjects;
//using NeoBase.Security;
//using NeoBase.Security.DataObjects;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
#endregion [Using Directives]

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmActivityInstance:
    /// Inherited from busBpmActivityInstanceGen, the class is used to customize the business object busBpmActivityInstanceGen.
    /// </summary>
    [Serializable]
    public class busSolBpmActivityInstance : busNeobaseBpmActivityInstance //busBpmActivityInstance
    {
        /// <summary>
        /// Property to get the Checkout details.
        /// </summary>
        public string istrViewActivity { get { return "View"; } }
        public bool iblnIsApprovalActivity { get; set; }
        public string istrNewModeScreen { get; set; }
        public string istrUpdateModeScreen { get; set; }
        public string istrRoleDescription { get; set; }
        public bool iblnNextActivityInitiation { get; set; }
        public Collection<busBpmActivityInstanceHistory> iclbBpmActivityInstHistory { get; set; } 

        #region [Properties] Commented Define in busNeobaseBpmActivityInstance

        // /// <summary>
        // /// Property to get the Person details.
        // /// </summary>
        // public int iintPerson_Id { get; set; }

        // /// <summary>
        // /// Property to get the Organization details.
        // /// </summary>
        // public int iintOrg_Id { get; set; }

        // /// <summary>
        // /// Property to get the Source.
        // /// </summary>
        // public string istrSource { get; set; }

        // /// <summary>
        // /// Property to get the Organization details.
        // /// </summary>
        // public DateTime idtCheckedOutDate { get; set; }

        // /// <summary>
        // /// Property to get the PRIORITY details.
        // /// </summary>
        // public string istrPriority { get; set; }

        // /// <summary>
        // /// Property to get the Process Instance Created Date.
        // /// </summary>
        // public DateTime idtInitiatedDate { get; set; }

        // /// <summary>
        // /// Property to get the Activity Instance Start Date details.
        // /// </summary>
        // public DateTime idtStartDate { get; set; }

        // /// <summary>
        // /// Property to get the Activity Instance END Date details.
        // /// </summary>
        // public DateTime idtEndDate { get; set; }

        // /// <summary>
        // /// Property to get the Person Name.
        // /// </summary>
        // public string istrPersonName { get; set; }

        // /// <summary>
        // /// Property to get the Org Name.
        // /// </summary>
        // public string istrOrgName { get; set; }

        // /// <summary>
        // /// Property to get the Launch details.
        // /// </summary>
        // public string istrLaunch { get { return "Launch"; } }

        // /// <summary>
        // /// Property to get the Checkout details.
        // /// </summary>
        // public string istrCheckout { get { return "Checkout"; } }

        // /// <summary>
        // /// Property to get the Resume details.
        // /// </summary>
        // public string istrResume { get { return "Resume"; } }

        // /// <summary>
        // /// Property to get the Unassign details.
        // /// </summary>
        // public string istrUnassign { get { return "Unassign"; } }

        // public string istrIsTerminated { get; set; }

        // /// <summary>
        // /// Property used to set grid record color based on error.
        // /// 
        // /// </summary>
        // public string istrReassignErrorCSS { get; set; }

        // /// <summary>
        // /// Property to hold all the Escalation messages collection.
        // /// </summary>
        // public Collection<busSolBpmUsersEscalationMessage> iclbEscalationMessages { get; set; }

        // /// <summary>
        // /// Property to hold the holiday List.
        // /// </summary>
        // public Collection<DateTime> iclbHolidayList { get; set; }

        // /// <summary>
        // /// Property to hold the Document List.
        // /// </summary>
        // public Collection<busBpmProcessInstanceAttachments> iclbDocuments { get; set; }

        // /// <summary>
        // /// Collection for process instance notes.
        // /// </summary>
        // public Collection<busNotes> iclbProcessInstanceNotes { get; set; }

        // /// <summary>
        // /// Collection of activities assigned to the user.
        // /// </summary>
        // //public Collection<busSolBpmActivityInstance> iclbUserAssignedActivities { get; set; }

        // /// <summary>
        // /// Collection of BPM Escalation.
        // /// </summary>
        // public Collection<busBpmEscalation> iclbBpmEscalation { get; set; }

        // /// <summary>
        // /// Property to hold the Document Upload
        // /// </summary>
        //// public busBpmDocumentUpload ibusDocumentUpload { get; set; }

        // ///// <summary>
        // ///// property to hold the Process Instance 
        // ///// </summary>
        // public busBpmProcessInstance ibusProcessInstance { get; set; }

        // /// <summary>
        // /// Collection for Notification Messages.
        // /// </summary>
        // public Collection<busBpmProcessInstance> iclbBpmProcessInstance { get; set; }

        // ///// <summary>
        // ///// Collection for uploaded Document
        // ///// </summary>
        // public Collection<busBpmDocumentUpload> iclbBpmDocumentUplaod { get; set; }

        // ///// <summary>
        // ///// Collection to upload file 
        // ///// </summary>
        // public Dictionary<string, Collection<utlPostedFile>> idictHttpPostedFiles { get; set; }

        // /// <summary>
        // /// Used to get value whether 
        // /// </summary>
        // public bool iblnHasAssignedActivities
        // {
        //     get
        //     {
        //         //return true;
        //         if (iclbUserAssignedActivities.IsNotNull() && iclbUserAssignedActivities.Count > 0)
        //             return true;
        //         else
        //             return false;
        //     }
        // }

        /// <summary>
        /// Collection for Restrict Notification Message.
        /// </summary>
        public Collection<busSolBpmProcessInstanceRestrictNotifyXr> iclbSolBpmProcessInstanceRestrictNotifyXr { get; set; }


        public Collection<busSolBpmProcessInstanceAttachments> iclbProcessInstanceImageData { get; set; }


        #endregion

        #region [Properties]

        ///// <summary>
        ///// Property to hold the Activity Instance of Person.
        ///// </summary>
        public busPerson ibusSolActivityInstancePerson { get; set; }
        ///// <summary>
        ///// Property to hold the Activity Instance of Organization.
        ///// </summary>
        public busOrganization ibusSolActivityInstanceOrganization { get; set; }


        // /// <summary>
        // /// Property to hold the assigned user tasks escalation messages collection.
        // /// </summary>
        //public Collection<busSolBpmUsersEscalationMessage> iclbAssignedActivitiesEscalationMessages { get; set; }

        // /// <summary>
        // /// Property to hold the collection for user tasks escalation messages assigned to others.
        // /// </summary>
        //public Collection<busSolBpmUsersEscalationMessage> iclbActivitiesEscalationMessagesAssignedToOthers { get; set; }

        // /// <summary>
        // /// Property to hold the process escalation messages collection.
        // /// </summary>
        // public Collection<busSolBpmUsersEscalationMessage> iclbProcessEscalationMessages { get; set; }

        #endregion [Properties]

        #region [Constructor]

        public busSolBpmActivityInstance()
            : base()
        {
            iclbBpmActivityInstanceChecklist = new Collection<busBpmActivityInstanceChecklist>();
            idictHttpPostedFiles = new Dictionary<string, Collection<utlPostedFile>>();
        }

        #endregion [Constructor]

        #region [Overridden Methods]

        public override void AfterAssignActivityInstance()
        {
            if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpProcessAutoRefund"))
            {
                if (ibusBpmActivity.icdoBpmActivity.name.Equals("Enter and Calculate Auto Refund"))
                {
                    if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
                    {
                        //CheckoutActivity(busConstant.PERSLinkBatchUser);
                        //suspend for 30 days
                        //any document
                        icdoBpmActivityInstance.resume_action_value = BpmCommon.ResumeActionAnyDocument;
                        icdoBpmActivityInstance.suspension_end_date = DateTime.Now.AddDays(30).Date;
                        icdoBpmActivityInstance.suspension_reason_value = "OTHR";
                        //iobjPassInfo.istrUserID = busConstant.PERSLinkBatchUser;
                        UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Suspended);
                        //iobjPassInfo.istrUserID = "BPM Service";
                    }
                }
            }
            else if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpACHPullForIBSInsurance"))
            {
                if (ibusBpmActivity.icdoBpmActivity.name.Equals("Run ACH Pull Automation – IBS batch"))
                {
                    if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
                    {
                        //CheckoutActivity(busConstant.PERSLinkBatchUser);
                        //suspend for 30 days
                        //any document
                        icdoBpmActivityInstance.resume_action_value = BpmCommon.ResumeActionAnyDocument;
                        icdoBpmActivityInstance.suspension_end_date = DateTime.Now.AddDays(busGlobalFunctions.DateDiffInDays(DateTime.Today, DateTime.Today.GetLastDayofMonth().AddDays(4))).Date;
                        icdoBpmActivityInstance.suspension_reason_value = "OTHR";
                        //iobjPassInfo.istrUserID = busConstant.PERSLinkBatchUser;
                        UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Suspended);
                        //iobjPassInfo.istrUserID = "BPM Service";
                    }
                }
            }
            else if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpProcessDeathNotification"))
            {
                if (ibusBpmActivity.icdoBpmActivity.name.Equals("Enter Insurance Check Data"))
                {
                    if (ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_value.Equals(BpmRequestSource.Batch))
                    {
                        //CheckoutActivity(busConstant.PERSLinkBatchUser);
                        //suspend for 30 days
                        //any document
                        if (icdoBpmActivityInstance.status_value != BpmActivityInstanceStatus.Suspended)
                        {
                            icdoBpmActivityInstance.resume_action_value = BpmCommon.ResumeActionAnyDocument;
                            icdoBpmActivityInstance.suspension_end_date = DateTime.Now.AddDays(30).Date;
                            icdoBpmActivityInstance.suspension_reason_value = "OTHR";
                            //iobjPassInfo.istrUserID = busConstant.PERSLinkBatchUser;
                            UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Suspended);
                            //iobjPassInfo.istrUserID = "BPM Service";
                        }
                    }
                }

                if (ibusBpmActivity.icdoBpmActivity.name.Equals("Process Claim Received"))
                {
                    //CheckoutActivity(busConstant.PERSLinkBatchUser);
                    //suspend for 30 days
                    //any document
                    if (icdoBpmActivityInstance.status_value != BpmActivityInstanceStatus.Suspended)
                    {
                        icdoBpmActivityInstance.resume_action_value = BpmCommon.ResumeActionAnyDocument;
                        icdoBpmActivityInstance.suspension_end_date = DateTime.Now.AddDays(30).Date;
                        icdoBpmActivityInstance.suspension_reason_value = "OTHR";
                        //iobjPassInfo.istrUserID = busConstant.PERSLinkBatchUser;
                        UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Suspended);
                        //iobjPassInfo.istrUserID = "BPM Service";
                    }
                }
            }
            base.AfterAssignActivityInstance();
        }

        /// <summary>
        /// This function is used to Load Other Objects of the current Activity Instance.
        /// </summary>
        /// <param name="adtrRow"></param>
        /// <param name="abusBusBase"></param>

        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBusBase)
        {
            if (abusBusBase is busNeobaseBpmActivityInstance)
            {
                //busSolBpmActivityInstance
                busNeobaseBpmActivityInstance lbusActivityInstance = (busNeobaseBpmActivityInstance)abusBusBase;
                lbusActivityInstance.ibusBpmActivity = busBpmActivity.GetBpmActivityByActivityType(adtrRow[enmBpmActivity.activity_type_value.ToString()].ToString());
                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.name.ToString()]))//enmBpmActivity.name
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow[enmBpmActivity.name.ToString()].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.process_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.process_id = Convert.ToInt32(adtrRow[enmBpmActivity.process_id.ToString()]);
                }

                lbusActivityInstance.ibusBpmProcessInstance = new busSolBpmProcessInstance();
                lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = lbusActivityInstance.icdoBpmActivityInstance.process_instance_id;
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = new busBpmProcess();
                //busSolBpmCaseInstance
                busNeobaseBpmCaseInstance lbusSolBpmCaseInstance = new busNeobaseBpmCaseInstance();
                lbusSolBpmCaseInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusSolBpmCaseInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = lbusSolBpmCaseInstance;


                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest = new busBpmRequest();
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.LoadData(adtrRow);

                if (!Convert.IsDBNull(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                }

                if (!Convert.IsDBNull(adtrRow[enmPerson.person_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                    if (lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance is busSolBpmCaseInstance)
                    {
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.person_id = Convert.ToInt32(adtrRow[enmPerson.person_id.ToString()]);
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.first_name = adtrRow[enmPerson.first_name.ToString()].ToString();
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.last_name = adtrRow[enmPerson.last_name.ToString()].ToString();
                        ((busPerson)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusPerson).icdoPerson.middle_name = adtrRow[enmPerson.middle_name.ToString()].ToString();

                    }
                }

                if (!Convert.IsDBNull(adtrRow[enmOrganization.org_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                    if (lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance is busSolBpmCaseInstance)
                    {
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();

                    }
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessName]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[BpmCommon.ProcessName].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[BpmCommon.ProcessName].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[BpmCommon.ProcessDescription].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[BpmCommon.ProcessDescription].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.SourceDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow[BpmCommon.SourceDescription].ToString();
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow[BpmCommon.SourceDescription].ToString();
                }
            }
            else if (abusBusBase is busNeobaseBpmUsersEscalationMessage)
            {
                ((busNeobaseBpmUsersEscalationMessage)abusBusBase).CallLoadOtherObjects(adtrRow, abusBusBase);
            }
            else if (abusBusBase is busBpmProcessInstanceAttachments)
            {
                busBpmProcessInstanceAttachments lbusBpmProcessInstanceAttachments = abusBusBase as busBpmProcessInstanceAttachments;
                if (lbusBpmProcessInstanceAttachments.ibusBpmEvent == null)
                {
                    lbusBpmProcessInstanceAttachments.ibusBpmEvent = new busBpmEvent { icdoBpmEvent = new doBpmEvent() };
                }
                if (adtrRow.Table.Columns.Contains("EVENT_DESC"))
                {
                    lbusBpmProcessInstanceAttachments.ibusBpmEvent.icdoBpmEvent.event_desc = adtrRow["EVENT_DESC"].ToString();
                }
            }
            else if (abusBusBase is busSolBpmProcessInstanceRestrictNotifyXr)
            {
                if (!Convert.IsDBNull(adtrRow["NOTIFICATION_MESSAGE"]))
                {
                    ((busSolBpmProcessInstanceRestrictNotifyXr)abusBusBase).istrNotificationMessages = adtrRow["NOTIFICATION_MESSAGE"].ToString();
                }
            }
            else if(abusBusBase is busBpmActivityInstanceHistory)
            {
                busBpmActivityInstanceHistory lbusBpmActivityInstanceHistory = (busBpmActivityInstanceHistory)abusBusBase;
				
                if (lbusBpmActivityInstanceHistory.ibusBpmActivityInstance == null)
                {
                    lbusBpmActivityInstanceHistory.LoadBpmActivityInstance();
                }
                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusBpmActivityInstanceHistory.icdoBpmActivityInstanceHistory.status_description  = adtrRow["STATUS_DESCRIPTION"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["ACTIVITY_NAME"]))
                {
                    lbusBpmActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name= adtrRow["ACTIVITY_NAME"].ToString();
                }
            }
        }

        /// <summary>
        /// This function is used to Load the related objects of the Process Instance.
        /// </summary>
        public override void LoadRelatedObjects()
        {
            this.LoadProcessInstanceNotes();
        }

        public ArrayList SolUnassignActivityInstance()
        {
            ArrayList larrResult = new ArrayList();
            if (this.ibusBpmActivity is busBpmUserTask)
            {
                try
                {
                    if (this.ibusBpmProcessInstance.iclbBpmActivityInstance.Where(ai => ai.icdoBpmActivityInstance.activity_id == this.ibusBpmActivity.icdoBpmActivity.activity_id).Count() > 1)
                        iobjPassInfo.idictParams["UnassignActivityInstance"] = true;
                    larrResult.AddRange(((busBpmUserTask)this.ibusBpmActivity).UncheckUserTaskActivity(this));
                }
                finally
                {
                    if (iobjPassInfo.idictParams.ContainsKey("UnassignActivityInstance"))
                    {
                        iobjPassInfo.idictParams.Remove("UnassignActivityInstance");
                    }
                }
            }
            return larrResult;
        }

        /// <summary>
        /// Check user is available for the curent date or not.. 
        /// </summary>
        /// <param name="alngUserSerialId">User Serial Id</param>
        /// <param name="astrUserId">User Id</param>
        /// <returns></returns>
        public override bool IsUserAvailable(long alngUserSerialId, string astrUserId)
        {
            if (iobjPassInfo.idictParams.ContainsKey("UnassignActivityInstance"))
            {
                iobjPassInfo.idictParams.Remove("UnassignActivityInstance");
                return false;
            }
               

            int lintCount = 0;
            if (alngUserSerialId > 0)
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entUserTimeoff.GetUserAvailableCountByUserSerialId",
                new object[1] { Convert.ToInt32(alngUserSerialId) }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            else if (!string.IsNullOrEmpty(astrUserId))
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entUserTimeoff.GetUserAvailableCountByUserId",
                new object[1] { astrUserId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            return !(lintCount > 0);
        }

        //public override ArrayList UpdateBpmActivityInstanceByStatus(string astrStatus, bool ablnThrowException = true)
        //{
        //    if (astrStatus == BpmActivityInstanceStatus.Resumed && icdoBpmActivityInstance.due_date == DateTime.MinValue)
        //        icdoBpmActivityInstance.due_date = busNeoBaseGlobalFunctions.GetSystemDate();
        //    return base.UpdateBpmActivityInstanceByStatus(astrStatus, ablnThrowException);
        //}

        public override ArrayList UpdateBpmActivityInstanceByStatus(string astrStatus, bool ablnThrowException = false)
        {
            ArrayList larrResult = new ArrayList();
            utlError lobjError = null;

            if (icdoBpmActivityInstance.activity_instance_id > 0)
            {
                try
                {
                    if (astrStatus == BpmActivityInstanceStatus.Returned || astrStatus == BpmActivityInstanceStatus.ReturnedToAudit)
                    {
                        astrStatus = BpmActivityInstanceStatus.Rejected;
                    }
                    if (astrStatus == BpmActivityInstanceStatus.Suspended)
                    {
                        icdoBpmActivityInstance.suspension_start_date = DateTimeExtensions.ApplicationDateTime;
                        //icdoBpmActivityInstance.checked_out_user = iobjPassInfo.istrUserID;
                        DisableEscalationInstances();
                        if (this.ibusBpmActivity is busBpmUserTask)
                        {
                            InitiateSuspendedActivityEscalations();
                        }
                        //Update the Status, UserId of Activity Instance                    
                        icdoBpmActivityInstance.status_value = astrStatus;
                    }
                    else if (astrStatus == BpmActivityInstanceStatus.Resumed)
                    {
                        TimeSpan ltsSuspendedDuration = TimeSpan.Zero;
                        if (ibusBpmActivity is busBpmUserTask && icdoBpmActivityInstance.suspension_start_date != DateTime.MinValue
                            && icdoBpmActivityInstance.due_date != DateTime.MinValue)
                        {
                            //Clearing out the values for last suspension details as we are resuming activity instance.
                            ltsSuspendedDuration = DateTimeExtensions.ApplicationDateTime.Subtract(icdoBpmActivityInstance.suspension_start_date);
                            icdoBpmActivityInstance.due_date = icdoBpmActivityInstance.due_date.Add(ltsSuspendedDuration);
                        }
                        DisableSuspendedActivityEscalationInstances();
                        ResumeEscalationInstances(ltsSuspendedDuration);
                        icdoBpmActivityInstance.suspension_reason_value = null;
                        icdoBpmActivityInstance.resume_action_value = null;
                        istrResumeActionValue = null;
                        //icdoBpmActivityInstance.comments = null;
                        if (astrStatus == BpmActivityInstanceStatus.Resumed && icdoBpmActivityInstance.due_date == DateTime.MinValue)
                            icdoBpmActivityInstance.due_date = busNeoBaseGlobalFunctions.GetSystemDate();
                        icdoBpmActivityInstance.suspension_start_date = DateTime.MinValue;
                        icdoBpmActivityInstance.suspension_end_date = DateTime.MinValue;

                        if (icdoBpmActivityInstance.status_value == BpmActivityInstanceStatus.Restricted)
                        {
                            ibusBpmProcessInstance.ibusBpmCaseInstance.ContinueRestrictedCaseInstance();
                        }
                        else
                        {
                            icdoBpmActivityInstance.status_value = astrStatus;
                        }
                    }
                    else if (astrStatus == BpmActivityInstanceStatus.InProcess)
                    {
                        if (ibusBpmActivity is busBpmUserTask)
                        {
                            icdoBpmActivityInstance.checked_out_date = DateTimeExtensions.ApplicationDateTime;
                            icdoBpmActivityInstance.checked_out_user = iobjPassInfo.istrUserID;
                            this.LoadBpmActivityInstanceEligibleUsers();
                            foreach (busBpmActivityInstanceUsers lbusBpmActivicityInstanceUsers in iclbBpmActivityInstanceUsers)
                            {
                                lbusBpmActivicityInstanceUsers.Delete();
                            }
                        }
                        //Update the Status, UserId of Activity Instance                    
                        icdoBpmActivityInstance.status_value = astrStatus;
                    }

                    icdoBpmActivityInstance.Update();

                    if (astrStatus == BpmActivityInstanceStatus.Resumed)
                    {
                        if (icdoBpmActivityInstance.initiator_instance_id != 0)
                            ibpmInitiator = this.ibusBpmProcessInstance.ibusBpmCaseInstance.GetActivityInstanceById(icdoBpmActivityInstance.initiator_instance_id);
                        if (ibusBpmActivity.IsBPMIntermediateCatchEvent)
                        {
                            ibusBpmProcessInstance.CompleteAndInitiate(this);
                        }
                        else if (!(ibusBpmActivity is busBpmUserTask))
                        {
                            ibusBpmProcessInstance.ProcessActivityInstance(this, ibusBpmActivity, ibpmInitiator, aenmActivityInitiateType: enmActivityInitiateType.InQueue);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    if (ablnThrowException)
                        throw;
                    else
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Could not checkout the activity.: " + GetExceptionMessage(Ex);

                        utlPassInfo.iobjPassInfo.idictParams[BpmColumnNames.ACTIVITY_TYPE_DESCRIPTION] = this.ibusBpmActivity.icdoBpmActivity.activity_type_description;
                        utlPassInfo.iobjPassInfo.idictParams[BpmColumnNames.ACTIVITY_INSTANCE_ID] = this.icdoBpmActivityInstance.activity_instance_id;

                        larrResult.Add(lobjError);
                    }
                }
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorMessage = "Workflow instance not found.";

                larrResult.Add(lobjError);
            }

            return larrResult;
        }

        public void LoadImageDataByProcessInstance()
        {
            iclbProcessInstanceImageData = new Collection<busSolBpmProcessInstanceAttachments>();

            ibusBpmProcessInstance.LoadBpmCaseInstance();
            Collection<busSolBpmProcessInstanceAttachments> lclbWfImageData = new Collection<busSolBpmProcessInstanceAttachments>();

            DataTable ldtbWfImageData = busBase.Select<doBpmPrcsInstAttachments>(new string[1] { "BPM_PROCESS_INSTANCE_ID" },
                  new object[1] { icdoBpmActivityInstance.process_instance_id }, null, null);

            lclbWfImageData = new busBase().GetCollection<busSolBpmProcessInstanceAttachments>(ldtbWfImageData, "icdoBpmProcessInstanceAttachments");

            //Select the Distinct Document Code To Avoid Duplicate showing on Grid                
            var lenuDistinctList = lclbWfImageData
                                        .GroupBy(i => i.icdoBpmProcessInstanceAttachments.doc_type)
                                        .Select(i => i.OrderBy(image=>image.icdoBpmProcessInstanceAttachments.created_date).First()); 
                                        //PIR 25183 - Added OrderBy - All versions of the document should get loaded from filenet
            //Loading the System Indexing Data for each Document in order to view the images
            foreach (busSolBpmProcessInstanceAttachments lobjWfImageData in lenuDistinctList)
            {
                Collection<busSolBpmProcessInstanceAttachments> lclbTempWfImageData =
                    busFileNetHelper.LoadFileNetImages(
                        lobjWfImageData.icdoBpmProcessInstanceAttachments.additional_info,
                        lobjWfImageData.icdoBpmProcessInstanceAttachments.doc_type,
                        lobjWfImageData.icdoBpmProcessInstanceAttachments.created_date,
                        ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id,
                        busGlobalFunctions.GetOrgCodeFromOrgId(ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id));
                foreach (busSolBpmProcessInstanceAttachments lobjTempWfImageData in lclbTempWfImageData)
                {
                    lobjTempWfImageData.icdoBpmProcessInstanceAttachments = lobjWfImageData.icdoBpmProcessInstanceAttachments;
                    iclbProcessInstanceImageData.Add(lobjTempWfImageData);
                }
            }

        }

        #endregion [Overridden Methods]


        #region [Public Methods]

        /// <summary>
        /// Upload Document
        /// </summary>
        /// <param name="ablnFromValidation"></param>
        //public ArrayList UploadBPMDocument(bool ablnFromValidation = false)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    if (idictHttpPostedFiles.Count > 0)
        //    {
        //        foreach (KeyValuePair<string, Collection<utlPostedFile>> dicItem in idictHttpPostedFiles)
        //        {
        //            foreach (utlPostedFile entry in dicItem.Value)
        //            {
        //                // Specify the path to save the uploaded file to.

        //                string lstrSavePath = iobjPassInfo.isrvDBCache.GetPathInfo(busNeoBaseConstants.BPM.BPM_DOCUMENT_UPLOAD);
        //                if (lstrSavePath.IsNotNullOrEmpty())
        //                {
        //                    DirectoryInfo ldrFinalPath = new DirectoryInfo(Path.Combine(lstrSavePath, Convert.ToString(ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id)));
        //                    string[] Files = Directory.GetFiles(lstrSavePath);

        //                    if (!ldrFinalPath.Exists)
        //                    {
        //                        ldrFinalPath.Create();
        //                        ldrFinalPath.Refresh();
        //                    }

        //                    string lstrTimeStamp = busNeoBaseGlobalFunctions.GetSystemDate().ToString("yyyyMMdd_HH_mm_ss"); // "" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond;

        //                    entry.istrFileName = $"{Path.GetFileNameWithoutExtension(entry.istrFileName)}_{lstrTimeStamp}{Path.GetExtension(entry.istrFileName)}";

        //                    File.WriteAllBytes(Path.Combine(ldrFinalPath.FullName, entry.istrFileName), entry.iarrFileBytes);

        //                    if (ibusDocumentUpload.IsNull())
        //                    {
        //                        ibusDocumentUpload = new NeoBase.BPMObjects.busBpmDocumentUpload();
        //                        ibusDocumentUpload.icdoBpmDocumentUpload = new NeoBase.BPMDataObjects.doBpmDocumentUpload();
        //                    }

        //                    ibusDocumentUpload.icdoBpmDocumentUpload.bpm_process_instance_id = ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id;
        //                    ibusDocumentUpload.icdoBpmDocumentUpload.bpm_document_name = entry.istrFileName;
        //                    ibusDocumentUpload.icdoBpmDocumentUpload.Insert();

        //                    LoadDocumentUpload();
        //                }
        //                else
        //                {

        //                    utlError lutlError = AddError(4037, "Could not find the path to upload documents to in System Paths for BPM_UPLD");
        //                    larrResult.Add(lutlError);
        //                    return larrResult;
        //                }
        //            }
        //        }
        //        idictHttpPostedFiles.Clear();
        //    }
        //    else
        //    {
        //        utlError lutlError = AddError(4034, "No file chosen to upload. Please choose one before proceeding.");
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    larrResult.Add(this);
        //    return larrResult;
        //}
        //public void LoadDocumentUpload()
        //{
        //    iclbBpmDocumentUplaod = GetCollection<NeoBase.BPMObjects.busBpmDocumentUpload>(Select<doBpmDocumentUpload>(new string[1] { Convert.ToString(enmBpmDocumentUpload.bpm_process_instance_id) },
        //        new object[1] { ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id }, null, null), "icdoBpmDocumentUpload");
        //}

        //public void deleteBPMDocument(ArrayList aarrSelectedObjects)
        //{
        //    if (aarrSelectedObjects.Count > 0)
        //    {
        //        foreach (busBpmDocumentUpload item in aarrSelectedObjects)
        //        {
        //            foreach (busBpmDocumentUpload lBpmDocumentUpload in iclbBpmDocumentUplaod)
        //            {
        //                string lstrSavePath = iobjPassInfo.isrvDBCache.GetPathInfo(busNeoBaseConstants.BPM.BPM_DOCUMENT_UPLOAD);
        //                string istrFileName = lBpmDocumentUpload.icdoBpmDocumentUpload.bpm_document_name;
        //                if (lBpmDocumentUpload.icdoBpmDocumentUpload.bpm_document_upload_id == item.icdoBpmDocumentUpload.bpm_document_upload_id)
        //                {
        //                    DirectoryInfo ldrFinalPath = new DirectoryInfo(Path.Combine(lstrSavePath, Convert.ToString(ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id), istrFileName));
        //                    File.Delete(Convert.ToString(ldrFinalPath));
        //                    lBpmDocumentUpload.icdoBpmDocumentUpload.Delete();
        //                }
        //            }
        //        }
        //    }
        //    LoadDocumentUpload();
        //}


        /// <summary>
        /// Load Method
        /// Dont move this to new project beacuse partial class(busPerson,busOrganization)
        /// </summary>
        public void LoadBpmProcessInstanceDetails()
        {
            // Developer : Rahul Mane
            // Date : 07-30-2021
            // PIR - 3688 - System does not load My Tasks panel and shows an error
            // Iteration - Main-Iteration10
            if (ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id > 0)
            {
                ibusBpmProcessInstance.LoadBpmProcess();
                ibusBpmProcessInstance.LoadBpmCaseInstance();
                ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                ibusBpmProcessInstance.LoadPerson();
                if (ibusBpmProcessInstance.ibusBpmCaseInstance != null)
                {
                    if (ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id > 0)
                    {
                        ibusSolActivityInstancePerson = new busPerson { icdoPerson = new cdoPerson() };
                        ibusSolActivityInstancePerson.FindByPrimaryKey(ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.person_id);
                    }
                    else
                    {
                        ibusSolActivityInstanceOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                        ibusSolActivityInstanceOrganization.FindByPrimaryKey(ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.org_id);
                    }
                }
            }
        }

        /// <summary>
        ///Method to Iterate Escalation Instance Collection and Load BPM Escalation records into Collection.
        /// </summary>
        //public void LoadActivityEscalation()
        //{
        //    if (!icolBpmEscalationInstances.IsNullOrEmpty())
        //    {
        //        foreach (busBpmEscalationInstance lbusBpmEscalationInstance in icolBpmEscalationInstances)
        //        {
        //            lbusBpmEscalationInstance.ibusBpmEscalation = new busBpmEscalation() { icdoBpmEscalation = new doBpmEscalation() };
        //            lbusBpmEscalationInstance.ibusBpmEscalation.FindByPrimaryKey(lbusBpmEscalationInstance.icdoBpmEscalationInstance.escalation_id);
        //        }
        //    }
        //}

        /// <summary>
        /// Method to check wheter the entered completed date is furture.
        /// </summary>
        /// <returns>True if date is future else false.</returns>
        //public bool IsCompletedDateFutureDate()
        //{
        //    bool lblnIsFuture = false;
        //    if (iclbBpmActivityInstanceChecklist != null)
        //    {
        //        foreach (busBpmActivityInstanceChecklist lbusBpmActivityInstanceChecklist in iclbBpmActivityInstanceChecklist)
        //        {
        //            if (lbusBpmActivityInstanceChecklist.icdoBpmActivityInstanceChecklist.completed_date != DateTime.MinValue
        //                && lbusBpmActivityInstanceChecklist.icdoBpmActivityInstanceChecklist.completed_date > busNeoBaseGlobalFunctions.GetSystemDate())
        //            {
        //                lblnIsFuture = true;
        //                return lblnIsFuture;
        //            }
        //        }
        //    }
        //    return lblnIsFuture;
        //}

        /// <summary>
        /// Add Notes for the current activity.
        /// </summary>
        /// <returns>Current object if note is added succefully, error object o.w.</returns>
        //public ArrayList AddNotes()
        //{
        //    ArrayList larrResult = new ArrayList();

        //    if (iblnNoteModeUpdate)
        //    {
        //        istrNewNotes = string.Empty;
        //        EvaluateInitialLoadRules();
        //        larrResult.Add(this);
        //        iblnNoteModeUpdate = false;
        //        return larrResult;
        //    }

        //    if (istrNewNotes.IsNullOrEmpty())
        //    {
        //        utlError lutlError = AddError(BpmMessages.Message_Id_1533, string.Empty);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }


        //    doNotes lcdoNotes = new doNotes();
        //    lcdoNotes.table_name = BpmCommon.BpmProcessInstanceTable;
        //    lcdoNotes.primary_key_id = icdoBpmActivityInstance.process_instance_id;
        //    lcdoNotes.notes_text = istrNewNotes;
        //    lcdoNotes.category_value = NeoConstant.BPM.NotesCategory.BPMProcessNotes;
        //    lcdoNotes.Insert();
        //    LoadProcessInstanceNotes();
        //    istrNewNotes = String.Empty;
        //    EvaluateInitialLoadRules();
        //    iintMessageID = BpmMessages.Message_Id_1532;
        //    larrResult.Add(this);
        //    return larrResult;
        //}

        /// <summary>
        /// This function is used for loading the notes text into the notes text box.
        /// </summary>
        /// <param name="aintNoteID">Note Id.</param>
        /// <returns>Current Object.</returns>
        //public ArrayList LoadNotes(int aintNoteID)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    busNotes lbusNotes = new busNotes() { icdoNotes = new doNotes() };

        //    lbusNotes.FindByPrimaryKey(aintNoteID);

        //    if (lbusNotes.IsNotNull())
        //    {
        //        istrNewNotes = lbusNotes.icdoNotes.notes_text;
        //    }

        //    EvaluateInitialLoadRules();
        //    iblnNoteModeUpdate = true;
        //    larrResult.Add(this);
        //    return larrResult;
        //}
        /// <summary>
        /// This function is used to reload the center left navigation details.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance id</param>
        /// <returns></returns>
        public ArrayList ReloadCenterleft(int aintActivityInstanceID)
        {
            ArrayList larrResult = new ArrayList();
            LoadCenterleftObjects(aintActivityInstanceID);
            LoadProcessInstanceHistory();
            EvaluateInitialLoadRules();
            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// This function is used to load the process instance notes.
        /// </summary>
        //public void LoadProcessInstanceNotes()
        //{
        //    if (ibusBpmProcessInstance.IsNull())
        //    {
        //        LoadBpmProcessInstance();
        //    }

        //    DataTable ldtbNotes = Select("entBpmActivityInstance.GetNotesForProcessInstanceIdAndRequestId", new object[2] { icdoBpmActivityInstance.process_instance_id, 0 });
        //    iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotes, "icdoNotes");
        //}

        /// <summary>
        /// This function is used to load the details of the current activity instance into the center left navigation panel.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance id.</param>
        public void LoadCenterleftObjects(int aintActivityInstanceID)
        {
            LoadAndCheckoutActivity(); //base

            if ((aintActivityInstanceID == 0 && iclbUserAssignedActivities.IsNotNull() && iclbUserAssignedActivities.Count > 0) ||
                 //(!(FindByPrimaryKey(aintActivityInstanceID)) && (iclbUserAssignedActivities.Count > 0)) ||
                 ((iclbUserAssignedActivities.Count > 0) && (!IsActivityInstanceAssignable())))
            {
                aintActivityInstanceID = iclbUserAssignedActivities[0].icdoBpmActivityInstance.activity_instance_id;
            }

            if (aintActivityInstanceID > 0)
            {
                FindByPrimaryKey(aintActivityInstanceID);
                /*busSolBpmActivityInstance*/
                busNeobaseBpmActivityInstance lbusSolBpmActivityInstance = iclbUserAssignedActivities.Where(objActivityInstance => objActivityInstance.icdoBpmActivityInstance.activity_instance_id == aintActivityInstanceID).FirstOrDefault();
                if (lbusSolBpmActivityInstance == null)
                {
                    if (this.FindByPrimaryKey(aintActivityInstanceID))
                    {
                        this.LoadBpmActivity();
                        this.LoadBpmProcessInstance();
                        this.ibusBpmProcessInstance.LoadBpmCaseInstance();
                        this.ibusBpmProcessInstance.LoadPerson();
                        this.ibusSolActivityInstancePerson = (busPerson)this.ibusBpmProcessInstance.ibusPerson;
                        this.LoadProcessInstanceNotes();
                        if (this.icdoBpmActivityInstance.initiator_instance_id > 0)
                        {
                            this.ibpmInitiator = new busBpmActivityInstance();
                            this.ibpmInitiator.FindByPrimaryKey(this.icdoBpmActivityInstance.initiator_instance_id);
                        }
                    }
                }
                else
                {
                    LoadProcessInstanceNotes();
                    this.icdoBpmActivityInstance = lbusSolBpmActivityInstance.icdoBpmActivityInstance;
                    this.ibpmInitiator = lbusSolBpmActivityInstance.ibpmInitiator;
                    this.ibusBpmActivity = lbusSolBpmActivityInstance.ibusBpmActivity;
                    this.ibusBpmProcessInstance = lbusSolBpmActivityInstance.ibusBpmProcessInstance;
                    this.iclbProcessInstanceNotes = lbusSolBpmActivityInstance.iclbProcessInstanceNotes;
                }
                //LoadImageDataByProcessInstance();

                if (iclbUserAssignedActivities != null && iclbUserAssignedActivities.Count > 0)
                {
                    foreach (busNeobaseBpmActivityInstance lbusTemp in iclbUserAssignedActivities)
                    {
                        if (lbusTemp.icdoBpmActivityInstance.activity_instance_id == aintActivityInstanceID)
                        {
                            lbusTemp.istrIsActivitySelected = "Y";
                        }
                        else
                        {
                            lbusTemp.istrIsActivitySelected = "N";
                        }
                    }
                }
            }
            LoadProcessInstanseRestrictNotifyXR();
            LoadEscalationMessages();
            //   LoadDocuments();

        }

        /// <summary>
        /// Load Collection For Restrict Notification 
        /// </summary>
        public void LoadProcessInstanseRestrictNotifyXR()
        {
            object[] larrParameters = new object[1] { iobjPassInfo.istrUserID };
            DataTable ldtSolBpmProcessInstanceRestrictNotifyXr = DBFunction.DBSelect("entBpmProcessInstanceRestrictNotifyXr.GetAllBpmProcessInstanceRestrictNotifyMessagesForUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbSolBpmProcessInstanceRestrictNotifyXr = GetCollection<busSolBpmProcessInstanceRestrictNotifyXr>(ldtSolBpmProcessInstanceRestrictNotifyXr, "icdoBpmProcessInstanceRestrictNotifyXr");
        }

        /// <summary>
        /// This method is used to load all the Escalation Messages.
        /// </summary>
        public void LoadEscalationMessages()
        {//busSolBpmUsersEscalationMessage
            object[] larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbAssignedActivitiesEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbAssignedActivitiesEscalationMessages = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbAssignedActivitiesEscalationMessages);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbEscalationMessagesAssignedToOtherUser = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToOtherUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbActivitiesEscalationMessagesAssignedToOthers = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbEscalationMessagesAssignedToOtherUser);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID };
            DataTable ldtbProcessEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.ProcessEscalationMessages", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbProcessEscalationMessages = GetCollection<busNeobaseBpmUsersEscalationMessage>(ldtbProcessEscalationMessages);
        }

        /// <summary>
        /// This method is used to update ecsalation messages as read
        /// </summary>
        /// <param name="aarrSelectedObjects">Selected Objects</param>
        /// <returns></returns>
        //public ArrayList UpdateUserEscalationMessage(ArrayList aarrSelectedObjects)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    string lstrEscalationMessageID = string.Empty;
        //    foreach (object larrObject in aarrSelectedObjects)
        //    {
        //        busBpmUsersEscalationMessage lbusBpmUsersEscalationMessage = larrObject as busBpmUsersEscalationMessage;
        //        lstrEscalationMessageID += string.Format("{0},", lbusBpmUsersEscalationMessage.icdoBpmUsersEscalationMessage.users_esc_message_id);
        //    }
        //    DBFunction.DBNonQuery("entBpmUsersEscalationMessage.UpdateMessageAsRead", new object[1] { lstrEscalationMessageID.TrimEnd(',') }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        //    larrResult.Add(this);
        //    return larrResult;
        //}

        /// <summary>
        /// This function is used to update the Escalation Messages of the current Activity Instance.
        /// </summary>
        /// <param name="aintActivityInstanceID">Activity instance ID</param>
        /// <returns></returns>
        public ArrayList UpdateEscalationMessages(int aintActivityInstanceID)
        {
            ArrayList larrResult = new ArrayList();
            this.PersistChanges();
            this.LoadEscalationMessages();
            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// Returns true if the user marks himself as unavailable, false o.w.
        /// </summary>
        //public bool IsUserUnavailable()
        //{
        //    busUser lbusUser = new busUser() { icdoUser = new doUser() };
        //    return true;
        //}

        //public void LoadDocuments()
        //{
        //    iclbDocuments = new Collection<busBpmProcessInstanceAttachments>();
        //    DataTable ldtbDocuments = busBase.Select("entEcmdocument.LoadDocumentsForLeftPanel", new object[1] { icdoBpmActivityInstance.process_instance_id });
        //    iclbDocuments = GetCollection<busBpmProcessInstanceAttachments>(ldtbDocuments, "icdoBpmProcessInstanceAttachments");
        //}

        /// <summary>
        /// This function is used to load and checkout the assigned activities of the logged in user. 
        /// </summary>
        public void LoadAndCheckoutActivity()
        {
            LoadAssignedActivities();

            //Check if the logged in user is on vacation then don't assign any workitem to him.
            busUser lbusUser = new busUser();
            lbusUser.FindByPrimaryKey(utlPassInfo.iobjPassInfo.iintUserSerialID);

            //If the user marks himself as unavailable then don't assign any activity for that user.
            if (IsUserAvailable(utlPassInfo.iobjPassInfo.iintUserSerialID, utlPassInfo.iobjPassInfo.istrUserID))
                return;

            //Check if the logged in user is an external user if so, don't checkout the activity.
            if (iblnIsExternalSearch)
            {
                return;
            }
        }

        /// <summary>
        /// This function is used to get the assigned activities of the logged in user.
        /// </summary>
        public void LoadAssignedActivities()
        {
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause = null;
            utlMethodInfo lutlMethodInfo;

            //Initialize the Collection to Avoid Null Exception
            iclbUserAssignedActivities = new Collection<busNeobaseBpmActivityInstance>();

            //Assign the Query Name By the Selected Filter
            lstrQuery = "MyBasketBaseQuery";

            //Load the only activities which are assigned to the current user
            lcolWhereClause = BuildWhereClause(lstrQuery, BpmCommon.INPC_Or_RESU);


            lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + lstrQuery);
            lstrQuery = lutlMethodInfo.istrCommand;
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            string lstrFinalQuery = lstrQuery + " order by activity_instance_id desc ";
            lstrFinalQuery = sqlFunction.AppendWhereClause(lstrFinalQuery, lcolWhereClause, lcolParams, iobjPassInfo.iconFramework);

            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolParams,
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbUserAssignedActivities = GetCollection<busNeobaseBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");

        }

        /// <summary>
        /// This method is used to invoke the Work flow action of the current Activity Instance.
        /// </summary>
        /// <returns></returns>
        public new ArrayList InvokeWorkflowAction()
        {
            ArrayList larrResult;
            utlError lutlError = new utlError();
            busBpmCaseInstance lbusBpmCaseInstance = ClassMapper.GetObject<busBpmCaseInstance>();
            busBpmActivityInstance lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(icdoBpmActivityInstance.activity_instance_id);
            istrResumeActionValue = icdoBpmActivityInstance.resume_action_value;
            icdoBpmActivityInstance.PopulateDescriptions();
            if(utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnCompleteActivity")
            {
                lbusBpmActivityInstance.UpdateParameter("NextStep", "Next");
            }
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnReturnActivity")
            {
                lbusBpmActivityInstance.UpdateParameter("NextStep", "Return");
            }
            //FM 6.0.0.35 change
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == busNeoBaseConstants.BPM.BTN_TERMINATE_ACTIVITY)
            {
                lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.termination_reason = this.istrTerminationReason;
            }
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnCompleteActivity")
            {
                if (!HasAllRequiredChecklistsCompleted())
                {
                    larrResult = new ArrayList();
                    lutlError = AddError(1565, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                if (IsCompletedDateFutureDate())
                {
                    larrResult = new ArrayList();
                    lutlError = AddError(1568, String.Empty);
                    larrResult.Add(lutlError);
                    return larrResult;
                }
                this.BeforeValidate(utlPageMode.All);
            }
            if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnSuspendActivity")
            {
                lbusBpmActivityInstance.icdoBpmActivityInstance.suspension_reason_value = icdoBpmActivityInstance.suspension_reason_value;
                lbusBpmActivityInstance.icdoBpmActivityInstance.comments = icdoBpmActivityInstance.comments;
                lbusBpmActivityInstance.istrResumeActionValue = icdoBpmActivityInstance.resume_action_description;
                lbusBpmActivityInstance.icdoBpmActivityInstance.resume_action_value = icdoBpmActivityInstance.resume_action_value;
                lbusBpmActivityInstance.icdoBpmActivityInstance.suspension_end_date = icdoBpmActivityInstance.suspension_end_date;
            }
            larrResult = lbusBpmActivityInstance.InvokeWorkflowAction();
            bool lblnHasErrors = false;
            foreach (object lobjResult in larrResult)
            {
                if (lobjResult is utlError || lobjResult is utlErrorList)
                {
                    lblnHasErrors = true;
                }
            }
            if (!lblnHasErrors)
            {
                larrResult.Clear();
                if (!String.IsNullOrEmpty(icdoBpmActivityInstance.checked_out_user))
                    NotifyUser.RefreshLeftPanel(icdoBpmActivityInstance.checked_out_user);
                if (iobjPassInfo.istrFormName == "wfmBPMWorkflowCenterLeftMaintenance")
                {
                    LoadCenterleftObjects(icdoBpmActivityInstance.activity_instance_id);
                    if (icdoBpmActivityInstance != null && icdoBpmActivityInstance.activity_instance_id > 0)
                    {
                        LoadBpmActivity();
                        LoadBpmProcessInstance();
                        ibusBpmProcessInstance.LoadBpmProcess();
                        ibusBpmProcessInstance.LoadBpmCaseInstance();
                        ibusBpmProcessInstance.ibusBpmCaseInstance.LoadBpmCase();
                        ibusBpmProcessInstance.LoadPerson();
                    }
                }
                else
                {
                    icdoBpmActivityInstance.Select();
                }

                if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnCompleteActivity" && 
                    ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpCreateandMaintainOrganizationInformation"))
                {
                    if (ibusBpmActivity.icdoBpmActivity.name.Equals("Enter Organization Information"))
                    {
                        if(iblnNextActivityInitiation)
                            lbusBpmActivityInstance.UpdateParameter("HasNoticeOfAppoinmentDoc", true.ToString());                        
                    }
                }
				//PIR 25668 update modified by due to avoid last step approve payee step, create and approve payee will not be same person
                if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnCompleteActivity" &&
                    ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmCase.icdoBpmCase.name.Equals("sbpProcessAutoRefund"))
                {
                    if (ibusBpmActivity.icdoBpmActivity.name.Equals("Enter Payee Account Information"))
                    {
                        busPayeeAccountStatus lbusPayeeAccountStatus = new busPayeeAccountStatus()
                        {
                            icdoPayeeAccountStatus = new cdoPayeeAccountStatus(),
                            ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() }
                        };
                        int lintPayeeAccountID = 0;
                        if (lbusBpmActivityInstance.GetBpmParameterValue("payee_account_id").IsNotNull())
                        {
                            Int32.TryParse(Convert.ToString(lbusBpmActivityInstance.GetBpmParameterValue("payee_account_id")), out lintPayeeAccountID);
                        }
                        if (lbusPayeeAccountStatus.ibusPayeeAccount.FindPayeeAccount(lintPayeeAccountID))
                        {
                            lbusPayeeAccountStatus.ibusPayeeAccount.LoadPayeeAccountStatus();
                            if (lbusPayeeAccountStatus.ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
                            {
                                lbusPayeeAccountStatus.icdoPayeeAccountStatus = lbusPayeeAccountStatus.ibusPayeeAccount.iclbPayeeAccountStatus[0].icdoPayeeAccountStatus;
                                if (lbusPayeeAccountStatus.icdoPayeeAccountStatus.modified_by != iobjPassInfo.istrUserID)
                                {
                                    lbusPayeeAccountStatus.icdoPayeeAccountStatus.modified_by = iobjPassInfo.istrUserID;
                                    lbusPayeeAccountStatus.icdoPayeeAccountStatus.Update();
                                }
                            }
                        }
                    }
                }
                EvaluateInitialLoadRules();
                larrResult.Add(this);
            }
            return larrResult;
        }

        public void LoadProcessInstanceHistory()
        {
            DataTable ldtpProcessInstanceHistory = busNeoSpinBase.Select("entBpmActivityInstance.LoadProcessInstanceHistory", new object[1] { icdoBpmActivityInstance.process_instance_id });
            iclbBpmActivityInstHistory = GetCollection<busBpmActivityInstanceHistory>(ldtpProcessInstanceHistory, "icdoBpmActivityInstanceHistory");
        }
		  
        /// <summary>
        /// This function is used to Get the activity Instance by Person Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByPersonId(string astrCaseName, string astrProcessName, int aintPersonId, utlPassInfo aobjPassInfo, string astrActivityName = null, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    //case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);
        //                    //person id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@PERSON_ID";
        //                    lobjParameter.Value = aintPersonId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByPersonId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
        //                        busBpmActivityInstance lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        //case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);
        //        //person id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@PERSON_ID";
        //        lobjParameter.Value = aintPersonId;
        //        lcolParameters.Add(lobjParameter);

        //        //activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByPersonId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busBpmCaseInstance lbusBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusBpmCaseInstance = new busBpmCaseInstance();
        //                busBpmActivityInstance lbusSolBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusSolBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// This function is used to Get the Activity Instance by Person Id & Reference Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintReferenceId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByPersonIdAndReferenceId(string astrCaseName, string astrProcessName, int aintPersonId, int aintReferenceId, utlPassInfo aobjPassInfo, string astrActivityName = null, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    //case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);
        //                    //person id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@PERSON_ID";
        //                    lobjParameter.Value = aintPersonId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    //reference id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@REFERENCE_ID";
        //                    lobjParameter.Value = aintReferenceId;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByPersonIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busBpmCaseInstance lbusBpmCaseInstance = new busBpmCaseInstance();
        //                        busBpmActivityInstance lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        //case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);
        //        //person id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@PERSON_ID";
        //        lobjParameter.Value = aintPersonId;
        //        lcolParameters.Add(lobjParameter);

        //        //activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        //reference id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@REFERENCE_ID";
        //        lobjParameter.Value = aintReferenceId;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByPersonIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busBpmCaseInstance lbusBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusBpmCaseInstance = new busBpmCaseInstance();
        //                busBpmActivityInstance lbusBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// This function is used to Get the Activity Instance by Organization Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="aintOrganizationId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByOrganizationId(string astrCaseName, string astrProcessName, int aintOrganizationId, utlPassInfo aobjPassInfo, string astrActivityName = null, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    //case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    //org id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ORG_ID";
        //                    lobjParameter.Value = aintOrganizationId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByOrganizationId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busBpmCaseInstance lbusSolBpmCaseInstance = new busBpmCaseInstance();
        //                        busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusSolBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusSolBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        //case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);

        //        //org id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ORG_ID";
        //        lobjParameter.Value = aintOrganizationId;
        //        lcolParameters.Add(lobjParameter);

        //        //activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByOrganizationId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busSolBpmCaseInstance lbusSolBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusSolBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// This function is used to Get the Activity Instance by Organization Id & Reference Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="aintOrganizationId"></param>
        /// <param name="aintReferenceId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByOrganizationIdAndReferenceId(string astrCaseName, string astrProcessName, int aintOrganizationId, int aintReferenceId, utlPassInfo aobjPassInfo, string astrActivityName = null, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    //case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    //org id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ORG_ID";
        //                    lobjParameter.Value = aintOrganizationId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    //reference id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@REFERENCE_ID";
        //                    lobjParameter.Value = aintReferenceId;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByOrganizationIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busSolBpmCaseInstance lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                        busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusSolBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusSolBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        //case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);

        //        //org id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ORG_ID";
        //        lobjParameter.Value = aintOrganizationId;
        //        lcolParameters.Add(lobjParameter);

        //        //activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        //reference id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@REFERENCE_ID";
        //        lobjParameter.Value = aintReferenceId;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByOrganizationIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busSolBpmCaseInstance lbusSolBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusSolBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// This function is used to get the Activity Instance by Person Id & Organization Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrganizationId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByPersonIdAndOrganizationId(string astrCaseName, string astrProcessName, string astrActivityName, int aintPersonId, int aintOrganizationId, utlPassInfo aobjPassInfo, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    //case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    //person id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@PERSON_ID";
        //                    lobjParameter.Value = aintPersonId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //org id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ORG_ID";
        //                    lobjParameter.Value = aintOrganizationId;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    //activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByPersonIdAndOrganizationId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busSolBpmCaseInstance lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                        busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusSolBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusSolBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        //case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);

        //        //person id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@PERSON_ID";
        //        lobjParameter.Value = aintPersonId;
        //        lcolParameters.Add(lobjParameter);

        //        //org id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ORG_ID";
        //        lobjParameter.Value = aintOrganizationId;
        //        lcolParameters.Add(lobjParameter);

        //        //activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByPersonIdAndOrganizationId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busBpmCaseInstance lbusBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusBpmCaseInstance = new busBpmCaseInstance();
        //                busBpmActivityInstance lbusSolBpmActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusSolBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        ///// <summary>
        /// This function is used to get the Activity Instance by Person Id, Organization Id & Reference Id.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="aintOrganizationId"></param>
        /// <param name="aintReferenceId"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="astrActivityInstanceStatus"></param>
        /// <returns></returns>
        //public static ArrayList GetBpmActivityInstanceByPersonIdAndOrganizationIdAndReferenceId(string astrCaseName, string astrProcessName, string astrActivityName, int aintPersonId, int aintOrganizationId, int aintReferenceId, utlPassInfo aobjPassInfo, string astrActivityInstanceStatus = BpmActivityInstanceStatus.InProcess)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    ArrayList larrActivities = new ArrayList();
        //    larrActivities = GetActivity(astrCaseName, astrProcessName, astrActivityName, aobjPassInfo);

        //    if (larrActivities.Count > 0)
        //    {
        //        if (!(larrActivities[0] is utlError || larrActivities[0] is utlErrorList))
        //        {
        //            foreach (object lobjResult in larrResult)
        //            {
        //                busBpmActivity lbusBpmActivity = lobjResult as busBpmActivity;
        //                if (lbusBpmActivity != null)
        //                {
        //                    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //                    case id
        //                    IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@CASE_ID";
        //                    lobjParameter.Value = lbusBpmActivity.ibusBpmProcess.icdoBpmProcess.case_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    person id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@PERSON_ID";
        //                    lobjParameter.Value = aintPersonId;
        //                    lcolParameters.Add(lobjParameter);

        //                    org id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ORG_ID";
        //                    lobjParameter.Value = aintOrganizationId;
        //                    lcolParameters.Add(lobjParameter);

        //                    activity instance status
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //                    lobjParameter.Value = astrActivityInstanceStatus;
        //                    lcolParameters.Add(lobjParameter);

        //                    activity id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@ACTIVITY_ID";
        //                    lobjParameter.Value = lbusBpmActivity.icdoBpmActivity.activity_id;
        //                    lcolParameters.Add(lobjParameter);

        //                    reference id
        //                    lobjParameter = DBFunction.GetDBParameter();
        //                    lobjParameter.DbType = DbType.Int32;
        //                    lobjParameter.ParameterName = "@REFERENCE_ID";
        //                    lobjParameter.Value = aintReferenceId;
        //                    lcolParameters.Add(lobjParameter);

        //                    object lobjQueryResult = DBFunction.DBExecuteScalar("entBpmActivityInstance.GetBpmActivityInstanceIdByPersonIdAndOrganizationIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //                    if (lobjQueryResult == null || (int)lobjQueryResult == 0)
        //                    {
        //                        utlError lutlError = new utlError();
        //                        lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                        larrResult.Add(lutlError);
        //                    }
        //                    else
        //                    {
        //                        busSolBpmCaseInstance lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                        busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)lobjQueryResult);
        //                        if (lbusSolBpmActivityInstance == null)
        //                        {
        //                            utlError lutlError = new utlError();
        //                            lutlError.istrErrorMessage = string.Format("Unable to find activity instance for activity named - {0}.", astrActivityName);
        //                            larrResult.Add(lutlError);
        //                        }
        //                        else
        //                        {
        //                            larrResult.Add(lbusSolBpmActivityInstance);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else //get all activities as per status
        //    {
        //        larrActivities = null; //GetCaseId(astrCaseName, aobjPassInfo);
        //        if (larrActivities[0] is utlError || larrResult[0] is utlErrorList)
        //        {
        //            return larrActivities;
        //        }

        //        Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //        case id
        //        IDbDataParameter lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@CASE_ID";
        //        lobjParameter.Value = (int)larrActivities[0];
        //        lcolParameters.Add(lobjParameter);

        //        person id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@PERSON_ID";
        //        lobjParameter.Value = aintPersonId;
        //        lcolParameters.Add(lobjParameter);

        //        org id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ORG_ID";
        //        lobjParameter.Value = aintOrganizationId;
        //        lcolParameters.Add(lobjParameter);

        //        activity instance status
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@ACTIVITY_INSTANCE_STATUS";
        //        lobjParameter.Value = astrActivityInstanceStatus;
        //        lcolParameters.Add(lobjParameter);

        //        reference id
        //        lobjParameter = DBFunction.GetDBParameter();
        //        lobjParameter.DbType = DbType.Int32;
        //        lobjParameter.ParameterName = "@REFERENCE_ID";
        //        lobjParameter.Value = aintReferenceId;
        //        lcolParameters.Add(lobjParameter);

        //        DataTable ldtbQueryResult = DBFunction.DBSelect("entBpmActivityInstance.GetBpmActivityInstanceIdsByPersonIdAndOrganizationIdAndReferenceId", lcolParameters, aobjPassInfo.iconFramework, aobjPassInfo.itrnFramework);
        //        if (ldtbQueryResult.Rows.Count > 0)
        //        {
        //            busSolBpmCaseInstance lbusSolBpmCaseInstance = null;
        //            foreach (DataRow ldtrRow in ldtbQueryResult.Rows)
        //            {
        //                lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
        //                busBpmActivityInstance lbusSolBpmActivityInstance = lbusSolBpmCaseInstance.LoadWithActivityInst((int)ldtrRow[0]);
        //                larrResult.Add(lbusSolBpmActivityInstance);
        //            }
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// Returns the process object by using case name and process name (added in NeoBaseBPM)
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="aobjPassInfo"></param>
        /// <returns></returns>
        //public static busBpmProcess GetBpmProcess(string astrCaseName, string astrProcessName, utlPassInfo aobjPassInfo)
        //{
        //    busBpmProcess lbusBpmProcess = new busBpmProcess() { icdoBpmProcess = new doBpmProcess() };
        //    ArrayList larrResult = GetCaseId(astrCaseName, aobjPassInfo);
        //    if (larrResult[0] is int)
        //    {
        //        busBpmCase lbusBpmCase = busBpmCase.GetBpmCase((int)larrResult[0]);
        //        if (lbusBpmCase != null)
        //        {
        //            lbusBpmProcess = lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.name == astrProcessName).FirstOrDefault();
        //        }
        //    }
        //    return lbusBpmProcess;
        //}
        List<String> ilistActionButtons = new List<string>() { "btnSuspendActivity", "btnResumeActivity", "btnCancelActivity", "btnCompleteActivity", "btnCheckoutActivity" };
        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            if(iobjPassInfo.istrFormName == "wfmBpmActivityInstanceMaintenance" && ilistActionButtons.Contains(iobjPassInfo.istrPostBackControlID))
            {
                aobjResponseData.ConcurrentOtherData["RefreshCenterLeft"] = true;
                Hashtable lhstCenterLeftLaunchParameters = new Hashtable();
                lhstCenterLeftLaunchParameters["aintActivityInstanceID"] = icdoBpmActivityInstance.activity_instance_id;
                aobjResponseData.ConcurrentOtherData["NewCenterLeftParams"] = HelperFunction.EncryptNavigationParams(lhstCenterLeftLaunchParameters, utlConstants.istrBPMWorkflowCenterLeftMaintenance, iobjPassInfo.istrSessionID);
            }

        }
        #endregion [Public Methods]
         public Collection<busBpmActivityInstanceChecklist> iclbProcessChecklist { get; set; }

        public void LoadProcessCheckList()
        {
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("PROCESS_INSTANCE_ID", this.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id);
            LoadCollection("iclbProcessChecklist", "entSolBpmProcessInstance.ProcessInstanceChecklist", lhstParams);
            if(iclbProcessChecklist != null)
                foreach(busBpmActivityInstanceChecklist lobjChecklistInstance in iclbProcessChecklist)
                {
                    lobjChecklistInstance.LoadBpmActivityChecklist();
                }
        }
        #region [Private Methods]

        /// <summary>
        /// This method is used to get the Case Id.(added in NeoBaseBPM)
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="aobjPassInfo"></param>
        /// <returns></returns>
        //private static ArrayList GetCaseId(string astrCaseName, utlPassInfo aobjPassInfo)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
        //    lcolParameters.Add(DBFunction.GetDBParameter("@NAME", "string", astrCaseName, utlPassInfo.iobjPassInfo.iconFramework));
        //    lcolParameters.Add(DBFunction.GetDBParameter("@Today", "datetime", DateTimeExtensions.ApplicationDateTime.Date, utlPassInfo.iobjPassInfo.iconFramework));
        //    object lobjEffectiveCaseId = DBFunction.DBExecuteScalar("entBpmCase.GetLatestEffectiveCaseDetailsByCaseName", lcolParameters, utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
        //    if (lobjEffectiveCaseId == null)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = string.Format("Unable to find case with name {0}.", astrCaseName);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    else if ((int)lobjEffectiveCaseId == 0)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = string.Format("Unable to find case with name {0}.", astrCaseName);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    larrResult.Add((int)lobjEffectiveCaseId);
        //    return larrResult;
        //}

        /// <summary>
        /// This method is used to get the Activity details.
        /// </summary>
        /// <param name="astrCaseName"></param>
        /// <param name="astrProcessName"></param>
        /// <param name="astrActivityName"></param>
        /// <param name="aobjPassInfo"></param>
        /// <returns></returns>
        //private static ArrayList GetActivity(string astrCaseName, string astrProcessName, string astrActivityName, utlPassInfo aobjPassInfo)
        //{
        //    ArrayList larrResult = new ArrayList();
        //    larrResult = GetCaseId(astrCaseName, aobjPassInfo);
        //    if (larrResult[0] is utlError || larrResult[0] is utlErrorList)
        //    {
        //        return larrResult;
        //    }

        //    busBpmCase lbusBpmCase = busBpmCase.GetBpmCase((int)larrResult[0]);
        //    if (lbusBpmCase == null)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = string.Format("Unable to find case with name {0}.", astrCaseName);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    if (lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.name == astrProcessName).Count() > 1)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = string.Format("More than one process with same name - {0} found.", astrProcessName);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    busBpmProcess lbusBpmProcess = lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.name == astrProcessName).FirstOrDefault();
        //    if (lbusBpmProcess == null)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = string.Format("Unable to find process with name - {0}.", astrProcessName);
        //        larrResult.Add(lutlError);
        //        return larrResult;
        //    }
        //    if (!string.IsNullOrEmpty(astrActivityName))
        //    {
        //        if (lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.name == astrActivityName).Count() > 1)
        //        {
        //            utlError lutlError = new utlError();
        //            lutlError.istrErrorMessage = string.Format("More than one activity with same name - {0} found.", astrActivityName);
        //            larrResult.Add(lutlError);
        //            return larrResult;
        //        }
        //        busBpmActivity lbusBpmActivity = lbusBpmProcess.iclbBpmActivity.Where(activity => activity.icdoBpmActivity.name == astrActivityName).FirstOrDefault();
        //        if (lbusBpmActivity == null)
        //        {
        //            utlError lutlError = new utlError();
        //            lutlError.istrErrorMessage = string.Format("Unable to find activity with name - {0}.", astrActivityName);
        //            larrResult.Add(lutlError);
        //            return larrResult;
        //        }
        //        else
        //        {
        //            larrResult.Add(lbusBpmActivity);
        //        }
        //    }

        //    return larrResult;
        //}

        /// <summary>
        /// Create the where clause that will be applied to the my basket base query depending upon the parameters selected in the 
        /// search screen.
        /// </summary>
        /// <param name="astrQueryId">Query id.</param>
        /// <param name="astrStatusValue">Status of the workitems.</param>
        /// <returns>Collection of where clause conditions.</returns>
        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            lcolWhereClause.Add(busNeoBase.GetWhereClause(astrStatusValue, "", "sai.status_value", "string", "in", " ", astrQueryId));

            //This condition is added to prevent the retrieving of activity instances whose process instances are completed bcz of any error.
            //we are excluding such activity instances bcz for such activity instances there may not be any valid information in persistence store,
            //and hence activity instance completion is not possible, it will fail always.
            lcolWhereClause.Add(busNeoBase.GetWhereClause("'INPC'", "", "spi.status_value", "string", "in", "and", astrQueryId));

            lcolWhereClause.Add(busNeoBase.GetWhereClause(utlPassInfo.iobjPassInfo.istrUserID, "", "SAI.CHECKED_OUT_USER", "string", "in", "and", astrQueryId));

            return lcolWhereClause;
        }

        public override ArrayList UpdateBpmActivityInstanceByEvent(enmBpmNextAction aenmNextAction, string astrStatus, string astrUserIdToAssignUserTask = null, enmActivityInitiateType aenmActivityInitiateType = enmActivityInitiateType.InQueue)
        {
            if (astrStatus == BpmActivityInstanceStatus.Returned || astrStatus == BpmActivityInstanceStatus.ReturnedToAudit)
            {
                astrStatus = BpmActivityInstanceStatus.Rejected;
            }
            return base.UpdateBpmActivityInstanceByEvent(aenmNextAction, astrStatus, astrUserIdToAssignUserTask, aenmActivityInitiateType);
        }
        #endregion [Private Methods]
    }
}
