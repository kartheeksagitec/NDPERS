#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWorkflowRequest:
    /// Inherited from busWorkflowRequestGen, the class is used to customize the business object busWorkflowRequestGen.
    /// </summary>
    [Serializable]
    public class busWorkflowRequest : busWorkflowRequestGen
    {
        public busOrganization ibusOrganization { get; set; }

        public void LoadOrganization()
        {
            if (ibusOrganization == null)
            {
                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            }
            if (icdoWorkflowRequest.org_code.IsNotNullOrEmpty())
                ibusOrganization.FindOrganizationByOrgCode(icdoWorkflowRequest.org_code);
        }

        public busContactTicket ibusContactTicket { get; set; }

        public void LoadContactTicket()
        {
            if (ibusContactTicket == null)
            {
                ibusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            }
            if (icdoWorkflowRequest.contact_ticket_id > 0)
                ibusContactTicket.FindContactTicket(icdoWorkflowRequest.contact_ticket_id);
        }

        public busDocument ibusDocument { get; set; }

        public void LoadDocument()
        {
            if (ibusDocument == null)
                ibusDocument = new busDocument();

            ibusDocument.FindDocumentByDocumentCode(icdoWorkflowRequest.document_code);
        }
    }
}
