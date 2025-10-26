﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Data;
using Sagitec.BusinessObjects;
using System.Reflection;
using Sagitec.DataObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMyBasket : busNeoSpinBase
    {
        public int iintProcessID { get; set; }
        public int iintPersonID { get; set; }
        public string istrOrgCode { get; set; }
        public int iintReferenceID { get; set; }
        public DateTime idtRequestDateFrom { get; set; }
        public DateTime idtRequestDateTo { get; set; }
        public int iintRoleID { get; set; }
        public string istrSource { get; set; }
        public string istrLastFourDigitsOfSSN { get; set; }
        public string istrMyBasketFilter { get; set; }

        //FW Upgrade - Change display message when clicked on Apply Filter button
        public string istrApplyFilterMessage { get; set; }
        public string istrApplyFilterButtonId { get; set; }

        public Collection<busActivityInstance> iclbUserAssignedActivities { get; set; }
        public Collection<busActivityInstance> iclbRoleAssignedActivities { get; set; }
        public Collection<busActivityInstance> iclbUserCompletedActivities { get; set; }
        public Collection<busActivityInstance> iclbUserSuspendedActivities { get; set; }

        public ArrayList SearchAndLoadMyBasket()
        {
            ArrayList larrResult = new ArrayList();
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause = null;
            utlMethodInfo lobjMethodInfo;

            //Validation to select My Basket Filter
            if (String.IsNullOrEmpty(istrMyBasketFilter))
            {
                utlError lobjError = new utlError { istrErrorID = "4200", istrErrorMessage = "Please Select a Basket Filter" };
                larrResult.Add(lobjError);
                return larrResult;
            }

            //Initialize the Collection to Avoid Null Exception
            iclbRoleAssignedActivities = new Collection<busActivityInstance>();
            iclbUserAssignedActivities = new Collection<busActivityInstance>();
            iclbUserSuspendedActivities = new Collection<busActivityInstance>();
            iclbUserCompletedActivities = new Collection<busActivityInstance>();

            //Assign the Query Name By the Selected Filter
            lstrQuery = "MyBasketBaseQuery";

            //Build the Where Clause by the Selected Filter
            if (istrMyBasketFilter == busConstant.MyBasketFilter_WorkPool)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, "'UNPC','RELE'");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_WorkAssigned)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, "'INPC','RESU'");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_SuspendedWork)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, "'SUSP'");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_CompletedWork)
            {
                lcolWhereClause = BuildWhereClause(lstrQuery, "'PROC','CANC'");
            }

            lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance." + lstrQuery);
            lstrQuery = lobjMethodInfo.istrCommand;
            string lstrFinalQuery = lstrQuery;// sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, iobjPassInfo.iconFramework);
            //prod pir 5170
            if (istrMyBasketFilter == busConstant.MyBasketFilter_SuspendedWork)
            {
                lstrFinalQuery += GetWhereClauseForSuspendedWork();
            }
            lstrFinalQuery += " order by activity_instance_id desc ";

            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolWhereClause, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (istrMyBasketFilter == busConstant.MyBasketFilter_WorkPool)
            {
                iclbRoleAssignedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_WorkAssigned)
            {
                iclbUserAssignedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_SuspendedWork)
            {
                iclbUserSuspendedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            }
            else if (istrMyBasketFilter == busConstant.MyBasketFilter_CompletedWork)
            {
                iclbUserCompletedActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            }
            if ((iclbRoleAssignedActivities != null) && (iclbRoleAssignedActivities.Count > 0))
            {
                istrApplyFilterMessage = "Records Displayed : Work Pool - " + iclbRoleAssignedActivities.Count;
            }
            else if ((iclbUserAssignedActivities != null) && (iclbUserAssignedActivities.Count > 0))
            {
                istrApplyFilterMessage = "Records Displayed : Assigned Work - " + iclbUserAssignedActivities.Count;
            }
            else if ((iclbUserSuspendedActivities != null) && (iclbUserSuspendedActivities.Count > 0))
            {
                istrApplyFilterMessage = "Records Displayed : Suspended Work - " + iclbUserSuspendedActivities.Count;
            }
            else if ((iclbUserCompletedActivities != null) && (iclbUserCompletedActivities.Count > 0))
            {
                istrApplyFilterMessage = "Records Displayed : Completed Work - " + iclbUserCompletedActivities.Count;
            }
            else
                istrApplyFilterMessage = "No records met the search criteria.";
            istrApplyFilterButtonId = iobjPassInfo.istrSenderID;
            this.EvaluateInitialLoadRules();
            larrResult.Add(this);
            return larrResult;
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busActivityInstance)
            {
                busActivityInstance lobjActivityInstance = (busActivityInstance)aobjBus;

                lobjActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                lobjActivityInstance.ibusActivity.ibusRoles = new busRoles { icdoRoles = new cdoRoles() };

                if (!Convert.IsDBNull(adtrRow["Name"]))
                {
                    lobjActivityInstance.ibusActivity.icdoActivity.name = adtrRow["Name"].ToString();
                    lobjActivityInstance.icdoActivityInstance.istrActivityName = adtrRow["Name"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["Display_Name"]))
                {
                    lobjActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["Display_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["PROCESS_ID"]))
                {
                    lobjActivityInstance.ibusActivity.icdoActivity.process_id = Convert.ToInt32(adtrRow["PROCESS_ID"]);
                }

                if (!Convert.IsDBNull(adtrRow["Role_Id"]))
                {
                    lobjActivityInstance.ibusActivity.icdoActivity.role_id = Convert.ToInt32(adtrRow["Role_Id"]);
                }

                if (!Convert.IsDBNull(adtrRow["role_description"]))
                {
                    lobjActivityInstance.ibusActivity.ibusRoles.icdoRoles.role_description = adtrRow["role_description"].ToString();
                }

                lobjActivityInstance.ibusProcessInstance = new busProcessInstance { icdoProcessInstance = new cdoProcessInstance() };
                lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = lobjActivityInstance.icdoActivityInstance.process_instance_id;
                lobjActivityInstance.ibusProcessInstance.ibusProcess = new busProcess { icdoProcess = new cdoProcess() };
                lobjActivityInstance.ibusProcessInstance.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjActivityInstance.ibusProcessInstance.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lobjActivityInstance.ibusProcessInstance.ibusWorkflowRequest = new busWorkflowRequest { icdoWorkflowRequest = new cdoWorkflowRequest() };

                if (!Convert.IsDBNull(adtrRow["process_instance_id"]))
                {
                    lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.process_instance_id = Convert.ToInt32(adtrRow["process_instance_id"]);
                }

                if (!Convert.IsDBNull(adtrRow["person_id"]))
                {
                    lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.person_id = Convert.ToInt32(adtrRow["person_id"]);
                    lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.person_id = Convert.ToInt32(adtrRow["person_id"]);
                    lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.first_name = adtrRow["first_name"].ToString();
                    lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.last_name = adtrRow["last_name"].ToString();
                    lobjActivityInstance.ibusProcessInstance.ibusPerson.icdoPerson.middle_name = adtrRow["middle_name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["org_id"]))
                {
                    lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.org_id = Convert.ToInt32(adtrRow["org_id"]);
                    lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_id = Convert.ToInt32(adtrRow["org_id"]);
                    lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_code = adtrRow["org_code"].ToString();
                    lobjActivityInstance.ibusProcessInstance.ibusOrganization.icdoOrganization.org_name = adtrRow["org_name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["contact_ticket_id"]))
                {
                    lobjActivityInstance.ibusProcessInstance.icdoProcessInstance.contact_ticket_id = Convert.ToInt32(adtrRow["contact_ticket_id"]);
                }

                if (!Convert.IsDBNull(adtrRow["Process_Name"]))
                {
                    lobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.name = adtrRow["Process_Name"].ToString();
                    lobjActivityInstance.icdoActivityInstance.istrProcessName = adtrRow["Process_Name"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow["Process_Description"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["Source_Description"]))
                {
                    lobjActivityInstance.ibusProcessInstance.ibusWorkflowRequest.icdoWorkflowRequest.source_description = adtrRow["Source_Description"].ToString();
                }
            }
        }

        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();

            lcolWhereClause.Add(GetWhereClause(astrStatusValue, "", "sai.status_value", "string", "in", " ", astrQueryId));

            if (istrMyBasketFilter == busConstant.MyBasketFilter_WorkPool)
            {
                utlWhereClause lobjWhereClause = GetWhereClause(iobjPassInfo.istrUserID, "", "user_id", "string", "exists", " and ", "UserRole");
                utlMethodInfo lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance.UserRole");
                lobjWhereClause.istrSubSelect = lobjMethodInfo.istrCommand;
                lcolWhereClause.Add(lobjWhereClause);
            }
            else
            {
                //For Batch Generated Suspended Workflow should be visible to all the users in suspended tab.
                //Mail Form David / Satya dated on 6/11/2009
                if (istrMyBasketFilter == busConstant.MyBasketFilter_SuspendedWork)
                {
                    //prod pir 5170 : not all suspended work to be shown in the grid
                    /*
                    string lstrUserList = "'" + iobjPassInfo.istrUserID + "','" + busConstant.PERSLinkBatchUser + "'";
                    lcolWhereClause.Add(GetWhereClause(lstrUserList, "", "checked_out_user", "string", "in", " and ", astrQueryId));*/
                }
                else
                {
                    lcolWhereClause.Add(GetWhereClause(iobjPassInfo.istrUserID, "", "checked_out_user", "string", "=", " and ", astrQueryId));
                }
            }

            if (iintProcessID > 0)
                lcolWhereClause.Add(GetWhereClause(iintProcessID, "", "sa.process_id", "int", "=", " and ", astrQueryId));

            if (istrSource.IsNotNullOrEmpty())
                lcolWhereClause.Add(GetWhereClause(istrSource, "", "swr.source_value", "string", "=", " and ", astrQueryId));

            if (istrOrgCode.IsNotNullOrEmpty())
            {
                int lintOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(istrOrgCode);
                lcolWhereClause.Add(GetWhereClause(lintOrgID, "", "spi.org_id", "int", "=", " and ", astrQueryId));
            }

            if (iintPersonID != 0)
                lcolWhereClause.Add(GetWhereClause(iintPersonID, "", "spi.person_id", "int", "=", " and ", astrQueryId));

            if (iintRoleID != 0)
                lcolWhereClause.Add(GetWhereClause(iintRoleID, "", "sa.role_id", "int", "=", " and ", astrQueryId));

            if (istrLastFourDigitsOfSSN.IsNotNullOrEmpty())
                lcolWhereClause.Add(GetWhereClause(istrLastFourDigitsOfSSN, "", "substring(p.ssn, 6, 4)", "string", "=", " and ", astrQueryId));

            if (iintReferenceID != 0)
                lcolWhereClause.Add(GetWhereClause(iintReferenceID, "", "sai.reference_id", "int", "=", " and ", astrQueryId));

            if ((idtRequestDateFrom != DateTime.MinValue) || (idtRequestDateTo != DateTime.MinValue))
            {
                if (idtRequestDateFrom == DateTime.MinValue)
                {
                    idtRequestDateFrom = DateTime.MinValue;
                }
                if (idtRequestDateTo == DateTime.MinValue)
                {
                    idtRequestDateTo = DateTime.MaxValue;
                }
                lcolWhereClause.Add(GetWhereClause(idtRequestDateFrom, idtRequestDateTo, "sai.created_date", "datetime", "between", " and ", astrQueryId));

            }
            return lcolWhereClause;
        }

        /// <summary>
        /// prod pir 5170 : to get where clause for suspended work
        /// </summary>
        /// <returns>where clause</returns>
        private string GetWhereClauseForSuspendedWork()
        {
            string lstrWhereClause = " and (checked_out_user = '" + iobjPassInfo.istrUserID + "' or (checked_out_user = '" + busConstant.PERSLinkBatchUser +
                                    "' and exists(";
            utlMethodInfo lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance.UserRole");
            lstrWhereClause += lobjMethodInfo.istrCommand + " and USR.user_id = '" + iobjPassInfo.istrUserID + "')))";

            return lstrWhereClause;
        }

        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);

            //FW Upgrade - Change display message when clicked on Apply Filter button
            if (!this.istrApplyFilterMessage.IsNullOrEmpty() && !this.istrApplyFilterButtonId.IsNullOrEmpty())
            {
                aobjResponseData.OtherData["istrApplyFilterMessage"] = this.istrApplyFilterMessage;
                aobjResponseData.OtherData["istrApplyFilterButtonId"] = this.istrApplyFilterButtonId;
            }
            this.istrApplyFilterMessage = "";
            this.istrApplyFilterButtonId = "";
        }

    }
}
