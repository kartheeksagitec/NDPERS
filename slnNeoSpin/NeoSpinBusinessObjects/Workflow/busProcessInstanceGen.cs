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
    /// Class NeoSpin.BusinessObjects.busProcessInstanceGen:
    /// Inherited from busBase, used to create new business object for main table cdoProcessInstance and its children table. 
    /// </summary>
	[Serializable]
	public class busProcessInstanceGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busProcessInstanceGen
        /// </summary>
		public busProcessInstanceGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busProcessInstanceGen.
        /// </summary>
		public cdoProcessInstance icdoProcessInstance { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busContactTicket.
        /// </summary>
		public busContactTicket ibusContactTicket { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busOrganization.
        /// </summary>
		public busOrganization ibusOrganization { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busPerson.
        /// </summary>
		public busPerson ibusPerson { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busProcess.
        /// </summary>
		public busProcess ibusProcess { get; set; }

        /// <summary>
        /// Gets or sets the non-collection object of type busWfImageDataFromFilenet.
        /// </summary>
		public busWorkflowRequest ibusWorkflowRequest { get; set; }

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.FindProcessInstance():
        /// Finds a particular record from cdoProcessInstance with its primary key. 
        /// </summary>
        /// <param name="aintprocessinstanceid">A primary key value of type int of cdoProcessInstance on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindProcessInstance(int aintprocessinstanceid)
		{
			bool lblnResult = false;
			if (icdoProcessInstance == null)
			{
				icdoProcessInstance = new cdoProcessInstance();
			}
			if (icdoProcessInstance.SelectRow(new object[1] { aintprocessinstanceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.LoadContactTicket():
        /// Loads non-collection object ibusContactTicket of type busContactTicket.
        /// </summary>
		public virtual void LoadContactTicket()
		{
			if (ibusContactTicket == null)
			{
				ibusContactTicket = new busContactTicket();
			}
			ibusContactTicket.FindContactTicket(icdoProcessInstance.contact_ticket_id);
		}

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.LoadOrganization():
        /// Loads non-collection object ibusOrganization of type busOrganization.
        /// </summary>
		public virtual void LoadOrganization()
		{
			if (ibusOrganization == null)
			{
				ibusOrganization = new busOrganization();
			}
			ibusOrganization.FindOrganization(icdoProcessInstance.org_id);
		}

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.LoadPerson():
        /// Loads non-collection object ibusPerson of type busPerson.
        /// </summary>
		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoProcessInstance.person_id);
		}

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.LoadProcess():
        /// Loads non-collection object ibusProcess of type busProcess.
        /// </summary>
		public virtual void LoadProcess()
		{
			if (ibusProcess == null)
			{
				ibusProcess = new busProcess();
			}
			ibusProcess.FindProcess(icdoProcessInstance.process_id);
		}

        /// <summary>
        /// NeoSpin.busProcessInstanceGen.LoadWorkflowRequest():
        /// Loads non-collection object ibusWorkflowRequest of type busWorkflowRequest.
        /// </summary>
		public virtual void LoadWorkflowRequest()
		{
			if (ibusWorkflowRequest == null)
			{
				ibusWorkflowRequest = new busWorkflowRequest();
			}
			ibusWorkflowRequest.FindWorkflowRequest(icdoProcessInstance.request_id);
		}

	}
}
