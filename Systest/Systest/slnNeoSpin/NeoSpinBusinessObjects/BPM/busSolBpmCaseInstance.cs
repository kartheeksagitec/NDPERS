using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Data;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmCaseInstance:
    /// Inherited from busBpmCaseInstanceGen, the class is used to customize the business object busBpmCaseInstanceGen.
    /// </summary>
    [Serializable]
    public class busSolBpmCaseInstance : busNeobaseBpmCaseInstance //busBpmCaseInstance
    {

        public int contact_ticket_id { get; set; }
        public string istrOrgCode { get; set; }

        public void FindProcessInstanceByContactTicket(int aintContactTicketId)
        {
            string lstrQuery = "SELECT CI.* FROM SGW_BPM_CASE_INSTANCE CI WITH(NOLOCK) " +
                "INNER JOIN SGW_BPM_CASE_INST_PARAMETER CPI WITH(NOLOCK) " +
                "ON CI.CASE_INSTANCE_ID=CPI.CASE_INSTANCE_ID WHERE CPI.PARAMETER_NAME='contact_ticket_id' and CPI.PARAMETER_VALUE='" + aintContactTicketId + "'";
            DataTable ldtCaseInstance = DBFunction.DBSelect(lstrQuery, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if(ldtCaseInstance.Rows.Count > 0)
            {
                icdoBpmCaseInstance.LoadData(ldtCaseInstance.Rows[0]);
                LoadBpmProcessInstances();
            }
        }

        public override void AfterInitiateCaseInstance()
        {
            base.AfterInitiateCaseInstance();
            busBpmCaseInstanceParameter lobjcaseInstaceParamtercontactTicketId = iclbBpmCaseInstanceParameter.Where(ip => ip.icdoBpmCaseInstanceParameter.parameter_name.ToLower() == "contact_ticket_id").FirstOrDefault();
            bool lblncreatenewContactTicket = false;
            if (lobjcaseInstaceParamtercontactTicketId==null)
            {
                lblncreatenewContactTicket = true;
                lobjcaseInstaceParamtercontactTicketId = new busBpmCaseInstanceParameter();
                lobjcaseInstaceParamtercontactTicketId.icdoBpmCaseInstanceParameter.parameter_name = "contact_ticket_id";
                lobjcaseInstaceParamtercontactTicketId.icdoBpmCaseInstanceParameter.case_instance_id = this.icdoBpmCaseInstance.case_instance_id;
                iclbBpmCaseInstanceParameter.Add(lobjcaseInstaceParamtercontactTicketId);
            }
            else if(lobjcaseInstaceParamtercontactTicketId.icdoBpmCaseInstanceParameter.parameter_value==null || lobjcaseInstaceParamtercontactTicketId.icdoBpmCaseInstanceParameter.parameter_value == "0")
            {
                lblncreatenewContactTicket = true;
            }
            
            if(lblncreatenewContactTicket)
            {
                busContactTicket lobjbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lobjbusContactTicket.icdoContactTicket.contact_type_value = "WKRF";
                lobjbusContactTicket.icdoContactTicket.person_id = icdoBpmCaseInstance.person_id;
                lobjbusContactTicket.icdoContactTicket.org_id = icdoBpmCaseInstance.org_id;
                lobjbusContactTicket.icdoContactTicket.status_value = "OPEN";
                lobjbusContactTicket.icdoContactTicket.Insert();

                lobjcaseInstaceParamtercontactTicketId.icdoBpmCaseInstanceParameter.parameter_value = lobjbusContactTicket.icdoContactTicket.contact_ticket_id.ToString();          
            }
        }

        public override void AfterCompleteCaseInstance()
        {
            base.AfterCompleteCaseInstance();

            if (icdoBpmCaseInstance.case_instance_id > 0)
            {
                if (icdoBpmCaseInstance.status_value == busConstant.ProcessInstanceStatusProcessed ||
                    icdoBpmCaseInstance.status_value == busConstant.ProcessInstanceStatusTerminated ||
                    icdoBpmCaseInstance.status_value == busConstant.ProcessInstanceStatusAborted)
                {
                    busBpmCaseInstanceParameter lobjBpmCaseInstanceParameter = iclbBpmCaseInstanceParameter.Where(ip => ip.icdoBpmCaseInstanceParameter.parameter_name.ToLower() == "contact_ticket_id").FirstOrDefault();

                    if (lobjBpmCaseInstanceParameter.IsNotNull() && lobjBpmCaseInstanceParameter.icdoBpmCaseInstanceParameter.parameter_value.IsNotNullOrEmpty())
                    {
                        busContactTicket lobjContactTicket = new busContactTicket();
                        if (lobjContactTicket.FindContactTicket(Convert.ToInt32(lobjBpmCaseInstanceParameter.icdoBpmCaseInstanceParameter.parameter_value)))
                        {
                            if (lobjContactTicket.icdoContactTicket.status_value != busConstant.ContactTicketStatusClosed)
                            {
                                lobjContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusClosed;
                                lobjContactTicket.icdoContactTicket.NeedHistory = true;
                                lobjContactTicket.icdoContactTicket.Update();
                            }
                        }
                    }
                }
            }
        }
    }
}

