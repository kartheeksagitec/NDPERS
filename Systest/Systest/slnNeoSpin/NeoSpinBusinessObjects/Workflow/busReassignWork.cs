using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Data;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busReassignWork : busNeoSpinBase
    {
        //Search Criteria Properties
        public int iintProcessID { get; set; }
        public int iintActivityID { get; set; }
        public int iintPersonID { get; set; }
        public int iintOrgID { get; set; }
        public int iintReferenceID { get; set; }
        public string istrCheckedOutUser { get; set; }
        public DateTime idtCreatedDateFrom { get; set; }
        public DateTime idtCreatedDateTo { get; set; }
        public int iintSupervisorRoleID { get; set; }
        public int iintRoleID { get; set; }
        public int iintPriority { get; set; }

        //FW Upgrade - Change display message when clicked on Apply Filter button
        public string istrApplyFilterMessage { get; set; }
        public string istrApplyFilterButtonId { get; set; }

        public Collection<busActivityInstance> iclbReassignmentActivities { get; set; }

        //Load Activities reassigned to User
        public ArrayList SearchAndLoadReassignmentBasket()
        {
            ArrayList larrResult = new ArrayList();

            string lstrFinalQuery;
            string lstrQuery;
            Collection<utlWhereClause> lcolWhereClause;
            utlMethodInfo lobjMethodInfo;

            lstrQuery = "SearchAndLoadReassignWork";
            lcolWhereClause = BuildWhereClause(lstrQuery, "'INPC','RESU','SUSP'");
            lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance." + lstrQuery);
            lstrQuery = lobjMethodInfo.istrCommand;
            lstrFinalQuery = lstrQuery;// sqlFunction.AppendWhereClause(lstrQuery, lcolWhereClause, iobjPassInfo.iconFramework);
            lstrFinalQuery += " order by activity_instance_id desc";
            DataTable ldtbList = DBFunction.DBSelect(lstrFinalQuery, lcolWhereClause, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbReassignmentActivities = GetCollection<busActivityInstance>(ldtbList, "icdoActivityInstance");
            if(iclbReassignmentActivities.Count > 0)
            {
                istrApplyFilterMessage = iclbReassignmentActivities.Count + " Records met the search criteria.";
            }
            else
            {
                istrApplyFilterMessage = "No records met the search criteria.";
            }
            istrApplyFilterButtonId = iobjPassInfo.istrSenderID;
            larrResult.Add(this);
            return larrResult;
        }

        private Collection<utlWhereClause> BuildWhereClause(string astrQueryId, string astrStatusValue)
        {
            Collection<utlWhereClause> lcolWhereClause = new Collection<utlWhereClause>();

            lcolWhereClause.Add(GetWhereClause(astrStatusValue, "", "sai.status_value", "string", "in", " ", astrQueryId));

            utlWhereClause lobjWhereClause = GetWhereClause(iobjPassInfo.istrUserID, "", "user_id", "string", "exists", " and ", "UserRole");
            utlMethodInfo lobjMethodInfo = iobjPassInfo.isrvMetaDataCache.GetMethodInfo("cdoActivityInstance.UserSupervisorRole");
            lobjWhereClause.istrSubSelect = lobjMethodInfo.istrCommand;
            lcolWhereClause.Add(lobjWhereClause);

            if (iintProcessID > 0)
                lcolWhereClause.Add(GetWhereClause(iintProcessID, "", "spi.process_id", "int", "=", " and ", astrQueryId));

            if (iintActivityID > 0)
                lcolWhereClause.Add(GetWhereClause(iintActivityID, "", "sai.activity_id", "int", "=", " and ", astrQueryId));
            
            if (iintOrgID > 0)
                lcolWhereClause.Add(GetWhereClause(iintOrgID, "", "spi.org_id", "int", "=", " and ", astrQueryId));

            if (iintPersonID > 0)
                lcolWhereClause.Add(GetWhereClause(iintPersonID, "", "spi.person_id", "int", "=", " and ", astrQueryId));

            if (iintRoleID > 0)
                lcolWhereClause.Add(GetWhereClause(iintRoleID, "", "sa.role_id", "int", "=", " and ", astrQueryId));

            if (iintSupervisorRoleID > 0)
                lcolWhereClause.Add(GetWhereClause(iintSupervisorRoleID, "", "sa.supervisor_role_id", "int", "=", " and ", astrQueryId));

            if (iintPriority > 0)
                lcolWhereClause.Add(GetWhereClause(iintPriority, "", "pr.priority", "int", "=", " and ", astrQueryId));

            if (iintReferenceID > 0)
                lcolWhereClause.Add(GetWhereClause(iintReferenceID, "", "sai.reference_id", "int", "=", " and ", astrQueryId));

            if(istrCheckedOutUser.IsNotNullOrEmpty())
                lcolWhereClause.Add(GetWhereClause(istrCheckedOutUser, "", "sai.checked_out_user", "string", "=", " and ", astrQueryId));

            if ((idtCreatedDateFrom != DateTime.MinValue) || (idtCreatedDateTo != DateTime.MinValue))
            {
                if (idtCreatedDateFrom == DateTime.MinValue)
                {
                    idtCreatedDateFrom = DateTime.MinValue;
                }
                if (idtCreatedDateTo == DateTime.MinValue)
                {
                    idtCreatedDateTo = DateTime.MaxValue;
                }
                lcolWhereClause.Add(GetWhereClause(idtCreatedDateFrom, idtCreatedDateTo, "sai.created_date", "datetime", "between", " and ", astrQueryId));

            }
            return lcolWhereClause;
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

                 if (!Convert.IsDBNull(adtrRow["supervisor_role_id"]))
                {
                    lobjActivityInstance.ibusActivity.icdoActivity.supervisor_role_id = Convert.ToInt32(adtrRow["supervisor_role_id"]);
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
                }

                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lobjActivityInstance.ibusProcessInstance.ibusProcess.icdoProcess.description = adtrRow["Process_Description"].ToString();
                }                
            }        
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
