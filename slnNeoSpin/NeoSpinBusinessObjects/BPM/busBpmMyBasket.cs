using NeoBase.BPM;
using NeoBase.Common;
using NeoBase.Common.DataObjects;
//using NeoBase.Security.DataObjects;
using NeoSpin.Common;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBpmMyBasket : busNeoSpinBase
    {
        #region [Properties]
        /// <summary>
        /// Used for Search Cretria.
        /// </summary>
        public string istrUserId { get; set; }
        /// <summary>
        /// Used for populating the list of accessible processes to the logged in user.
        /// </summary>
        public int iintProcessID { get; set; }

        /// <summary>
        /// Used for storing the People ID info entered in search criteria.
        /// </summary>
        public int iintPersonID { get; set; }    ///Already decleared this property in base class (busNeoSpinBase) Hence commented.  vishal kedar

        /// <summary>
        /// Used for storing the Process Priority entered in search criteria.
        /// </summary>
        public string istrProcessPriority { get; set; }

        /// <summary>
        /// Used for storing the Org ID info entered in search criteria.
        /// </summary>
        public int iintSearchOrgID { get; set; }

        /// <summary>
        /// Used for storing the reference id associated the workflow activity instance info entered in search criteria.
        /// </summary>
        public int iintReferenceID { get; set; }

        /// <summary>
        /// Used for storing the workflow activity instance created date from info entered in search criteria.
        /// </summary>
        public DateTime idtRequestDateFrom { get; set; }

        /// <summary>
        /// Used for storing the workflow activity instance created date to info entered in search criteria.
        /// </summary>
        public DateTime idtRequestDateTo { get; set; }
        /// <summary>
        /// Used for storing the workflow activity instance due date from info entered in search criteria.
        /// </summary>
        public DateTime idtDueDateFrom { get; set; }

        /// <summary>
        /// Used for storing the workflow activity instance due date to info entered in search criteria.
        /// </summary>
        public DateTime idtDueDateTo { get; set; }
        /// <summary>
        /// Used for storing the list of possible source values for request initiation.
        /// </summary>
        public string istrSource { get; set; }

        /// <summary>
        /// Used for storing the possible values for the my basket filter.
        /// </summary>
        public string istrMyBasketFilter { get; set; }

        /// <summary>
        /// Used for storing the possible values for the my basket filter.
        /// </summary>
        public string istrFilterType { get; set; }
        /// <summary>
        /// Used for storing the possible values for activity instance status.
        /// </summary>
        public string istrActivityInstanceStatus { get; set; }
        public string istrOrgCode { get; set; }
        /// <summary>
        /// Used for storing the list of assigned activity instances to the logged in user.
        /// </summary>
        public Collection<busNeobaseBpmActivityInstance> iclbUserAssignedActivities { get; set; }

        /// <summary>
        /// Used for storing the list of activity instances assignable to the roles to which logged in user belongs to - these are not assigned to the logged in user.
        /// </summary>
        public Collection<busSolBpmActivityInstance> iclbRoleAssignedActivities { get; set; }

        /// <summary>
        /// Used for storing the list of activity instances completed by the logged in user.
        /// </summary>
        public Collection<busSolBpmActivityInstance> iclbUserCompletedActivities { get; set; }

        /// <summary>
        /// Used for storing thr list of activity instances suspended by the logged in user.
        /// </summary>
        public Collection<busSolBpmActivityInstance> iclbUserSuspendedActivities { get; set; }

        /// <summary>
        /// Used for storing thr list of activity instances suspended by the logged in user.
        /// </summary>
        public Collection<busSolBpmActivityInstance> iclbFailedActivities { get; set; }

        /// <summary>
        /// Stores the MaxSearchCount value in web.config.
        /// </summary>
        public int iintMaxSearchCount { get; set; }

        /// <summary>
        /// Store the count of records fetched for each search criteria.
        /// </summary>
        public int iintActualRecordCount { get; set; }

        /// <summary>
        /// Used for storing the Org ID info for the logged in user through ESS portal.
        /// </summary>
        public int iintESSOrgId { get; set; }

        /// <summary>
        /// Used for storing the Org ID info for the logged in user through ESS portal.
        /// </summary>
        public bool iblnIsExternalSearch { get; set; }

        /// <summary>
        /// Used  for storing the Role id info entered in search criteria.
        /// </summary>
        public int iintRoleId { get; set; }

        /// <summary>
        /// Used for storing the Org Contact ID info for the logged in user through ESS portal.
        /// </summary>
        public int iintOrgContactID { get; set; }

        /// <summary>
        /// Used for storing the user id associated with the org contact logged in through ESS portal.
        /// </summary>
        public string istrOrgContactUserId { get; set; }

        /// <summary>
        /// Used for storing the value if the logged in user has workflow manager role assigned.
        /// </summary>
        private bool iblnIsLoggedInUserHasWfMgrRole { get; set; }

        /// <summary>
        /// Collection of roles of type workflow manager applicable to the logged in user.
        /// </summary>
        private Collection<doRoles> iclcWfMgrRoles { get; set; }


        /// <summary>
        /// Gets or sets the collection of Assigned user tasks escalation messages
        /// </summary>
        public Collection<busSolBpmUsersEscalationMessage> iclbAssignedActivitiesEscalationMessages { get; set; }


        /// <summary>
        /// Gets or sets the collection of User tasks escalation messages assigned to others
        /// </summary>
        public Collection<busSolBpmUsersEscalationMessage> iclbActivitiesEscalationMessagesAssignedToOthers { get; set; }


        /// <summary>
        /// Gets or sets the collection of Process escalation messages
        /// </summary>
        public Collection<busSolBpmUsersEscalationMessage> iclbProcessEscalationMessages { get; set; }

        public string istrDateTo
        {
            get
            {
                return (idtRequestDateTo == DateTime.MaxValue || idtRequestDateTo == DateTime.MinValue) ? string.Empty : idtRequestDateTo.ToShortDateString();
            }
            set
            {
                if (value.IsNotNull() && value != string.Empty)
                    idtRequestDateTo = Convert.ToDateTime(value);
            }
        }

        #endregion

        #region [Public Methods]

        /// <summary>
        /// Load Method
        /// </summary>
        /// <param name="aintMaxSearchCount"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="astrFilterType"></param>
        /// <returns></returns>
        //public void BPMLoadFailedActivityInstances(int aintMaxSearchCount, int aintPersonId, string astrFilterType = "All")
        //{
        //    iintMaxSearchCount = aintMaxSearchCount;
        //    iintESSOrgId = 0;
        //    iintPersonID = aintPersonId;
        //    iblnIsExternalSearch = false;
        //    istrFilterType = astrFilterType;
        //    LoadFailedActivityInstances();
        //}



        /// <summary>
        /// This method is used to get Failed Activity Instances in current user's Basket list.
        /// </summary>
        /// <returns>ArrayList of busBpmMyBasket</returns>
        //public ArrayList LoadFailedActivityInstances()
        //{
        //    ArrayList larrResult = new ArrayList();
        //    string lstrQuery;
        //    Collection<utlWhereClause> lcolWhereClause = null;
        //    Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
        //    utlMethodInfo lutlMethodInfo;


        //    //Initialize the Collection to Avoid Null Exception
        //    iclbFailedActivities = new Collection<busSolBpmActivityInstance>();

        //    //Assign the Query Name By the Selected Filter
        //    lstrQuery = "FailedActivityInstances";
        //    lcolWhereClause = BuildWhereClause(lstrQuery, "'FAIL'");


        //    lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + lstrQuery);
        //    lstrQuery = lutlMethodInfo.istrCommand;
        //    string lstrFinalQuery = "";
        //    lstrFinalQuery = sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, lcolParams, iobjPassInfo.iconFramework);
        //    lstrFinalQuery += " ORDER  BY activity_instance_id desc";


        //    DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

        //    foreach (DataRow ldtrRow in ldtbList.Rows)
        //    {
        //        iclbFailedActivities = GetCollection<busSolBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");
        //        foreach (busSolBpmActivityInstance lbusSolBpmActivityInstance in iclbFailedActivities)
        //        {
        //            lbusSolBpmActivityInstance.EvaluateInitialLoadRules();
        //        }
        //    }
        //    iintActualRecordCount = iclbFailedActivities.Count;

        //    this.EvaluateInitialLoadRules();
        //    larrResult.Add(this);
        //    return larrResult;
        //}

        /// <summary>
        /// Load Method
        /// </summary>
        /// <param name="aintMaxSearchCount"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="astrFilterType"></param>
        public virtual void BPMSearchAndLoadMyBasket(int aintMaxSearchCount, int aintPersonId, string astrFilterType = "All")
        {

            //Set the Default Filter As Work Pool
            istrMyBasketFilter = WorkflowConstants.MYBASKET_FILTER_WORKPOOL;
            iintMaxSearchCount = aintMaxSearchCount <= 0 ? 100 : aintMaxSearchCount;
            iintESSOrgId = 0;
            iintPersonID = aintPersonId;
            iblnIsExternalSearch = false;
            istrFilterType = astrFilterType;
            SearchAndLoadMyBasket();

        }

        /// <summary>
        /// This function is used to get and load the my basket screen with the workitems which satisfy the specified criteria.
        /// </summary>
        /// <returns>Current object.</returns>
        public virtual ArrayList SearchAndLoadMyBasket()
        {
            ArrayList larrResult = new ArrayList();
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause = null;
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            utlMethodInfo lutlMethodInfo;

            //Check if any search criteria has been entered or not.
            if ((iintProcessID == 0) && (iintPersonID == 0) && (iintSearchOrgID == 0)
                && (iintReferenceID == 0) && (idtRequestDateFrom.Equals(DateTime.MinValue)) && (idtRequestDateTo.Equals(DateTime.MinValue)) && (iintRoleId == 0)
                && (istrSource.IsNull()) && (istrMyBasketFilter.IsNull()))
            {
                iintMessageID = BpmMessages.Message_Id_5;
                larrResult.Add(this);
                return larrResult;
            }

            //Select my basket filter.
            if (istrMyBasketFilter.IsNull())
            {
                iintMessageID = BpmMessages.Message_Id_1547;
                larrResult.Add(this);
                return larrResult;
            }

            //Initialize the Collection to Avoid Null Exception
            iclbRoleAssignedActivities = new Collection<busSolBpmActivityInstance>();
            iclbUserAssignedActivities = new Collection<busNeobaseBpmActivityInstance>();
            iclbUserSuspendedActivities = new Collection<busSolBpmActivityInstance>();
            iclbUserCompletedActivities = new Collection<busSolBpmActivityInstance>();

            //Assign the Query Name By the Selected Filter
            lstrQuery = "MyBasketBaseQuery";

            //Build the Where Clause by the Selected Filter
            if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkPool)
            {
                if (!string.IsNullOrEmpty(istrActivityInstanceStatus))
                    lstrQuery = "MyBasketWorkPoolQuery";
                else
                    lstrQuery = "MyBasketWorkPoolQueryModified";
                lcolWhereClause = BuildWhereClause(lstrQuery, WorkflowConstants.ACTIVITYINSTANCE_STATUS_UNPC_OR_RELE);
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkAssigned)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, BpmCommon.INPC_Or_RESU);
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.SuspendedWork)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, BpmCommon.SUSP);
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.CompletedWork)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, BpmCommon.PROC_Or_CANC);
            }
            string lstrFinalQuery = "";

            if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkPool)
            {
                if (istrActivityInstanceStatus !="PROC")
                {
                    lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + lstrQuery);
                    lstrQuery = lutlMethodInfo.istrCommand;
                   

                    lstrFinalQuery = lstrQuery + " order by sai.CHECKED_OUT_USER desc, sai.activity_instance_id desc";
                }
                if(istrActivityInstanceStatus == "PROC")
                {
                    lutlMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("entBpmActivityInstance." + "MyBasketWorkPoolCompletedQuery");
                    lstrQuery = lutlMethodInfo.istrCommand;
                    lstrFinalQuery = lstrQuery + " order by sai.CHECKED_OUT_USER desc, sai.activity_instance_id desc";
                }
                
            }
            else
            {
                lstrFinalQuery = lstrQuery + " order by SAI.activity_instance_id desc ";

            }
            lstrFinalQuery = sqlFunction.AppendWhereClause(lstrFinalQuery, lcolWhereClause, lcolParams, iobjPassInfo.iconFramework);



            DataTable ldtbList = null;
            if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkPool)
            {
                IDbDataParameter iparameter = DBFunction.GetDBParameter();
                iparameter.ParameterName = "@USER_SERIAL_ID";
                iparameter.Value = utlPassInfo.iobjPassInfo.iintUserSerialID;
                iparameter.DbType = DbType.Int32;
                lcolParams.Add(iparameter);
               
                    IDbDataParameter iparameter2 = DBFunction.GetDBParameter();
                    iparameter2.ParameterName = "@CHECKED_OUT_USER";
                    iparameter2.Value = utlPassInfo.iobjPassInfo.istrUserID;
                    iparameter2.DbType = DbType.String;
                    lcolParams.Add(iparameter2);
                
                ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                ldtbList = DBFunction.DBSelect(lstrFinalQuery, new Collection<utlWhereClause>(),
                                                                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkPool)
            {
                iclbRoleAssignedActivities = GetCollection<busSolBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");

                if (iclbRoleAssignedActivities != null && iclbRoleAssignedActivities.Count > 0)
                {
                    foreach (busSolBpmActivityInstance lbusSolBpmActivityInstance in iclbRoleAssignedActivities)
                    {
                        lbusSolBpmActivityInstance.EvaluateInitialLoadRules();
                    }
                }
                iintActualRecordCount = iclbRoleAssignedActivities.Count;
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.WorkAssigned)
            {
                iclbUserAssignedActivities = GetCollection<busNeobaseBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");
                if (iclbUserAssignedActivities != null)
                    iclbUserAssignedActivities.ForEach(bpmActivityInstance => bpmActivityInstance.LoadRelatedObjects());
                iintActualRecordCount = iclbUserAssignedActivities.Count;
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.SuspendedWork)
            {
                iclbUserSuspendedActivities = GetCollection<busSolBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");
                if (iclbUserSuspendedActivities != null)
                    iclbUserSuspendedActivities.ForEach(bpmActivityInstance => bpmActivityInstance.LoadRelatedObjects());
                iintActualRecordCount = iclbUserSuspendedActivities.Count;
            }
            else if (istrMyBasketFilter == BpmMyBasketFilterConstants.CompletedWork)
            {
                iclbUserCompletedActivities = GetCollection<busSolBpmActivityInstance>(ldtbList, "icdoBpmActivityInstance");
                if (iclbUserCompletedActivities != null)
                    iclbUserCompletedActivities.ForEach(bpmActivityInstance => bpmActivityInstance.LoadRelatedObjects());
                iintActualRecordCount = iclbUserCompletedActivities.Count;
            }

            if (this.istrMyBasketFilter.IsNotNullOrEmpty())
            {
                if (this.iintActualRecordCount == 0)
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_2;
                    this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_2);
                }
                else if (this.iintActualRecordCount > this.iintMaxSearchCount)
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_3;
                    this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_3, new object[2] { this.iintActualRecordCount, this.iintMaxSearchCount });
                }
                else
                {
                    this.iintMessageID = WorkflowConstants.MESSAGE_ID_1;
                    this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_1, new object[1] { this.iintActualRecordCount });
                }
            }

            this.EvaluateInitialLoadRules();
            larrResult.Add(this);
            return larrResult;
        }

        /// <summary>
        /// This method is used to get Resume Activity Instances in current user's Basket list.
        /// </summary>
        /// <param name="aarrSelectedObjects">Object of Arraylist of Selected Activity Instance.</param>
        /// <returns>ArrayList of busBpmMyBasket</returns>
        public ArrayList ResumeActivityInstance(ArrayList aarrSelectedObjects)
        {
            ArrayList larrErrors = new ArrayList();
            foreach (Object lobjSelectedBpmActivityInstance in aarrSelectedObjects)
            {
                busBpmActivityInstance lbusSolBpmActivityInstance = (busBpmActivityInstance)lobjSelectedBpmActivityInstance;
                utlPassInfo.iobjPassInfo.idictParams["PostBackControl"] = "btnResumeActivity";
                if (utlPassInfo.iobjPassInfo.istrPostBackControlID == "btnResumeActivity")
                {
                    larrErrors = lbusSolBpmActivityInstance.InvokeWorkflowAction();
                }
                else
                {
                    lbusSolBpmActivityInstance = lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadWithActivityInst(lbusSolBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                    larrErrors = lbusSolBpmActivityInstance.UpdateBpmActivityInstanceByStatus(BpmActivityInstanceStatus.Resumed);
                }
                if ((larrErrors == null || larrErrors.Count == 0) || (larrErrors != null && larrErrors.Count > 0 && larrErrors[0] is busBpmActivityInstance))
                {
                    larrErrors.Clear();
                    this.SearchAndLoadMyBasket();
                    if (lbusSolBpmActivityInstance != null && lbusSolBpmActivityInstance.icdoBpmActivityInstance != null
                               && !String.IsNullOrEmpty(lbusSolBpmActivityInstance.icdoBpmActivityInstance.checked_out_user))
                        NotifyUser.RefreshLeftPanel(lbusSolBpmActivityInstance.icdoBpmActivityInstance.checked_out_user);
                    larrErrors.Add(this);
                }
            }
            return larrErrors;
        }

        /// <summary>
        /// This method is used to get Resume Failed Activity Instances in current user's Basket list.
        /// </summary>
        /// <param name="aintActivityInstanceId">Current users Activity Instance Id</param>
        /// <returns>ArrayList of busBpmMyBasket</returns>
        //public ArrayList ResumeFailedActivityInstance(int aintActivityInstanceId)
        //{
        //    ArrayList larrErrors = new ArrayList();
        //    busBpmActivityInstance lbusSolBpmActivityInstance = iclbFailedActivities.Where(activityInstance => activityInstance.icdoBpmActivityInstance.activity_instance_id == aintActivityInstanceId).FirstOrDefault();
        //    try
        //    {
        //        lbusSolBpmActivityInstance = lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadWithActivityInst(lbusSolBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
        //        lbusSolBpmActivityInstance.Resume(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        utlError lutlError = new utlError();
        //        lutlError.istrErrorMessage = "Unable to resume activity instance. Error : " + ex.Message;
        //        larrErrors.Add(larrErrors);
        //        return larrErrors;
        //    }
        //    this.LoadFailedActivityInstances();
        //    this.EvaluateInitialLoadRules();
        //    larrErrors.Add(this);
        //    return larrErrors;
        //}

        /// <summary>
        /// This function returns the collection of the processes accessible to the logged in user.
        /// </summary>
        public Collection<doBpmProcess> GetAccessibleUserProcesses()
        {
            DataTable ldtbProcessesForSelectedUser = busBase.Select("entBpmProcess.GetProcessesForOnlineInitialization",
                                                          new object[] { });

            Collection<doBpmProcess> lclcProcessesForSelectedUser = doBase.GetCollection<doBpmProcess>(ldtbProcessesForSelectedUser);
            return lclcProcessesForSelectedUser;
        }

        /// <summary>
        /// This method is used Get List Of Roles of Current user.
        /// </summary>
        /// <returns>Collection of DataObject of Roles</returns>
        public Collection<doRoles> GetListOfRoles()
        {
            DataTable ldtbRoles = busBase.Select("entBpmActivityInstance.GetListOfRoles",
                                                          new object[1] { iobjPassInfo.iintUserSerialID });

            Collection<doRoles> lclcRoles = doBase.GetCollection<doRoles>(ldtbRoles);
            return lclcRoles;
        }

        /// <summary>
        /// This method is used Load Escalation Messages of BPM of current user.
        /// </summary>
        public void LoadEscalationMessages()
        {
            object[] larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbAssignedActivitiesEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbAssignedActivitiesEscalationMessages = GetCollection<busSolBpmUsersEscalationMessage>(ldtbAssignedActivitiesEscalationMessages);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID, iobjPassInfo.istrUserID };
            DataTable ldtbEscalationMessagesAssignedToOtherUser = DBFunction.DBSelect("entBpmUsersEscalationMessage.UserTaskEscalationMessagesAssignedToOtherUser", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbActivitiesEscalationMessagesAssignedToOthers = GetCollection<busSolBpmUsersEscalationMessage>(ldtbEscalationMessagesAssignedToOtherUser);

            larrParameters = new object[] { iobjPassInfo.iintUserSerialID };
            DataTable ldtbProcessEscalationMessages = DBFunction.DBSelect("entBpmUsersEscalationMessage.ProcessEscalationMessages", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbProcessEscalationMessages = GetCollection<busSolBpmUsersEscalationMessage>(ldtbProcessEscalationMessages);
        }

        public ArrayList UnassignActivityInstances(ArrayList aarrSelectedObjects)
        {
            ArrayList larrResult = new ArrayList();
            foreach (busBpmActivityInstance lbusBpmActivityInstance in aarrSelectedObjects)
            {
                busBpmActivityInstance lbusSolBpmActivityInstance = lbusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.LoadWithActivityInst(lbusBpmActivityInstance.icdoBpmActivityInstance.activity_instance_id);
                busSolBpmActivityInstance lbusSolBPMActivityInstance = lbusSolBpmActivityInstance as busSolBpmActivityInstance;
                if (lbusSolBPMActivityInstance != null)
                {
                    larrResult.AddRange(lbusSolBPMActivityInstance.SolUnassignActivityInstance());
                }
                else
                {
                    larrResult.AddRange(lbusSolBpmActivityInstance.UnassignActivityInstance()); 
                }
                
                if (larrResult.Count > 0)
                    break;
            }
            if (larrResult.Count == 0)
            {
                this.SearchAndLoadMyBasket();
                larrResult.Add(this);
            }
            return larrResult;
        }

        #endregion

        #region [Overriden Methods]        

        /// <summary>
        /// Overriden to load required parameters associated with the activity instance.
        /// </summary>
        /// <param name="adtrRow">Data Row.</param>
        /// <param name="abusBusBase">Business object.</param>
        protected override void LoadOtherObjects(DataRow adtrRow, busBase abusBusBase)
        {
            if (abusBusBase is busSolBpmActivityInstance)
            {
                busSolBpmActivityInstance lbusActivityInstance = (busSolBpmActivityInstance)abusBusBase;
                lbusActivityInstance.ibusBpmActivity = busBpmActivity.GetBpmActivityByActivityType(adtrRow[enmBpmActivity.activity_type_value.ToString()].ToString());
                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.name.ToString()]))
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow[enmBpmActivity.name.ToString()].ToString();
                }
                if (adtrRow.Table.Columns.Contains("ROLE_DESCRIPTION") && !Convert.IsDBNull(adtrRow["ROLE_DESCRIPTION"]))
                {
                    lbusActivityInstance.istrRoleDescription = adtrRow["ROLE_DESCRIPTION"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow[enmBpmActivity.process_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.process_id = Convert.ToInt32(adtrRow[enmBpmActivity.process_id.ToString()]);
                }

                lbusActivityInstance.ibusBpmProcessInstance = new busSolBpmProcessInstance();
                lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = lbusActivityInstance.icdoBpmActivityInstance.process_instance_id;
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = new busBpmProcess();
                busSolBpmCaseInstance lbusSolBpmCaseInstance = new busSolBpmCaseInstance();
                lbusSolBpmCaseInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusSolBpmCaseInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = lbusSolBpmCaseInstance;


                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest = new busBpmRequest();
                lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.LoadData(adtrRow);

                if (!Convert.IsDBNull(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.icdoBpmProcessInstance.process_instance_id = Convert.ToInt32(adtrRow[enmBpmProcessInstance.process_instance_id.ToString()]);
                }

                if (!Convert.IsDBNull(adtrRow[enmPerson.person_id.ToString()]))
                {
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
                    if (lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance is busSolBpmCaseInstance)
                    {
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_id = Convert.ToInt32(adtrRow[enmOrganization.org_id.ToString()]);
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_name = adtrRow[enmOrganization.org_name.ToString()].ToString();
                        ((busOrganization)lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusOrganization).icdoOrganization.org_code = adtrRow[enmOrganization.org_code.ToString()].ToString();
                    }
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessName]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow[BpmCommon.ProcessName].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.ProcessDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow[BpmCommon.ProcessDescription].ToString();
                }

                if (!Convert.IsDBNull(adtrRow[BpmCommon.SourceDescription]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow[BpmCommon.SourceDescription].ToString();
                }
                if (adtrRow.Table.Columns.Contains("PRIORITY_CODE_VALUE") && !Convert.IsDBNull(adtrRow["PRIORITY_CODE_VALUE"]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.priority_code_value = adtrRow["PRIORITY_CODE_VALUE"].ToString();
                }
                if (adtrRow.Table.Columns.Contains("PRIORITY_DESCRIPTION") && !Convert.IsDBNull(adtrRow["PRIORITY_DESCRIPTION"]))
                {
                    lbusActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.priority_code_description = adtrRow["PRIORITY_DESCRIPTION"].ToString();
                }
            }
            else if (abusBusBase is busSolBpmUsersEscalationMessage)
            {
                ((busSolBpmUsersEscalationMessage)abusBusBase).CallLoadOtherObjects(adtrRow, abusBusBase);
            }
        }

        #endregion

        #region [Private Methods]

        /// <summary>
        /// Create the where clause that will be applied to the my basket base query depending upon the parameters selected in the search screen.
        /// </summary>
        /// <param name="astrQueryId">Query Id for selected filters.</param>
        /// <param name="astrStatusValue">Status of the work items.</param>
        /// <returns>Collection of where clause conditions depending upon the filters selected.</returns>
        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();
            
            if (iintProcessID > 0)
            {
                DataTable dt = busBase.Select<doBpmProcess>(new string[1] { enmBpmProcess.process_id.ToString() },
                new object[1] { iintProcessID }, null, null);

                if (null != dt && dt.Rows.Count == 1)
                {
                    lcolWhereClause.Add(GetWhereClause(dt.Rows[0]["name"], "", "sp.name", "string", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
                }
            }
            if (iintSearchOrgID > 0)
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintSearchOrgID, "", "pmi.org_id", "int", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if (iintRoleId > 0)
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintRoleId, "", "AR.ROLE_ID", "int", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if (iintESSOrgId > 0)
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintESSOrgId, "", "pmi.org_id", "int", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if (iintPersonID > 0)
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(iintPersonID, "", "pmi.person_id", "int", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if (istrActivityInstanceStatus.IsNotNull())
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrActivityInstanceStatus, "", "sai.status_value", "string", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }
            if ( istrActivityInstanceStatus.IsNotNullOrEmpty())
            {
                if (utlPassInfo.iobjPassInfo.istrUserID.IsNotNullOrEmpty() && istrActivityInstanceStatus.ToString() == "PROC")
                {                  
                    lcolWhereClause.Add(busNeoBase.GetWhereClause(utlPassInfo.iobjPassInfo.istrUserID, "", "sai.CHECKED_OUT_USER", "string", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
                }               
            }
            if (istrProcessPriority.IsNotNull())
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrProcessPriority, "", "PMI.PRIORITY_CODE_VALUE", "string", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if (istrUserId.IsNotNull())
            {
                lcolWhereClause.Add(busNeoBase.GetWhereClause(istrUserId, "", "sai.CHECKED_OUT_USER", "string", "=", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));
            }

            if ((idtRequestDateFrom != DateTime.MinValue) || (idtRequestDateTo != DateTime.MinValue))
            {
                DateTime ldtRequestDateFrom = idtRequestDateFrom == DateTime.MinValue ? (DateTime)SqlDateTime.MinValue : idtRequestDateFrom;
                DateTime ldtRequestDateTo = idtRequestDateTo == DateTime.MinValue ? DateTime.MaxValue : idtRequestDateTo;

                //if (idtRequestDateFrom == DateTime.MinValue)
                //{
                //    idtRequestDateFrom = (DateTime)SqlDateTime.MinValue;
                //}
                //if (idtRequestDateTo == DateTime.MinValue)
                //{
                //    idtRequestDateTo = DateTime.MaxValue;
                //}

                lcolWhereClause.Add(busNeoBase.GetWhereClause(ldtRequestDateFrom, ldtRequestDateTo, "cast(sai.CREATED_DATE as date)", "datetime", "between", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));

                if (idtRequestDateFrom == (DateTime)SqlDateTime.MinValue)
                {
                    idtRequestDateFrom = DateTime.MinValue;
                }
                if (idtRequestDateTo == DateTime.MaxValue)
                {
                    idtRequestDateTo = DateTime.MinValue;
                }
            }

            if ((idtDueDateFrom != DateTime.MinValue) || (idtDueDateTo != DateTime.MinValue))
            {
                if (idtDueDateFrom == DateTime.MinValue)
                {
                    idtDueDateFrom = (DateTime)SqlDateTime.MinValue;
                }
                if (idtDueDateTo == DateTime.MinValue)
                {
                    idtDueDateTo = DateTime.MaxValue;
                }
                
                lcolWhereClause.Add(busNeoBase.GetWhereClause(idtDueDateFrom, idtDueDateTo, "cast(sai.due_date as date)", "datetime", "between", lcolWhereClause.Count > 0 ? " and " : "", astrQueryId));

                if(idtDueDateFrom == (DateTime)SqlDateTime.MinValue)
                {
                    idtDueDateFrom = DateTime.MinValue;
                }
                if (idtDueDateTo == DateTime.MaxValue)
                    idtDueDateTo = DateTime.MinValue;
            }
            return lcolWhereClause;
        }

        #endregion

    }
}
