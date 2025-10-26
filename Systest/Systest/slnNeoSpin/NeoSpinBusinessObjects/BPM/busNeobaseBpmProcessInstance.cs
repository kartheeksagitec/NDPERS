
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace NeoBase.BPM
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmProcessInstance:
    /// Inherited from busBpmProcessInstanceGen, the class is used to customize the business object busBpmProcessInstanceGen.
    /// </summary>
    [Serializable]
    public class busNeobaseBpmProcessInstance : busBpmProcessInstance
    {
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Process_Name { get; set; }
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Activity { get; set; }
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrEvent_Associated_Action { get; set; }
        /// <summary>
        /// <summary>
        /// Property to get the Process Name.
        /// </summary>
        public string istrProcess_Name { get; set; }
        /// <summary>
        /// Property to get Action Value
        /// </summary>
        public string istrAction { get; set; }
        /// <summary>
        /// Property to get Action Value
        /// </summary>
        public string istrActivity { get; set; }

        /// <summary>
        /// Gets or sets property Process Instance Status
        /// </summary>
        public string istrProcessInstanceStatus { get; set; }

        /// <summary>
        /// Collection to hold activity filtered instances
        /// </summary>
        public Collection<busBpmActivityInstance> iclbFilteredBpmActivityInstance { get; set; }


        public busNeobaseBpmProcessInstance()
        {
            if (iobjPassInfo != null)
            {
                ibusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
                ibusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);
            }
        }

        /// <summary>
        /// Load method
        /// </summary>
        /// <param name="aintProcessInstanceId"></param>
        public void FindBPMProcessInstance(int aintProcessInstanceId)
        {
            ibusBpmCaseInstance.ibusBpmCase = busBpmCase.GetBpmCase(ibusBpmCaseInstance.icdoBpmCaseInstance.case_id);
            if (ibusBpmCaseInstance.ibusBpmCase != null)
            {
                foreach (busBpmProcess lbusBpmProcess in ibusBpmCaseInstance.ibusBpmCase.iclbBpmProcess)
                {
                    lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
                }
            }
            ibusBpmCaseInstance.LoadOrganization();
            ibusBpmCaseInstance.LoadPerson();
            ibusBpmCaseInstance.LoadBpmRequest();
            ibusBpmProcess = ibusBpmCaseInstance.ibusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.process_id == icdoBpmProcessInstance.process_id).FirstOrDefault();
            ibusBpmProcess.iclbBpmActivity.ForEach(lbusBpmActivity => lbusBpmActivity.LoadRoles());
        }

        /// <summary>
        /// Filter the Completed and Initiated Status ActivityInstance Record 
        /// </summary>
        public override void LoadBpmActivityInstances()
        {
            base.LoadBpmActivityInstances();

            //DataTable adtbList = busBase.Select<doBpmActivityInstance>(new string[1]
            //{
            //    enmBpmActivityInstance.process_instance_id.ToString()
            //}, new object[1]
            //{
            //    (object) this.icdoBpmProcessInstance.process_instance_id
            //}, (object[])null, (string)null, false);

            //this.iclbBpmActivityInstance = this.GetCollection<busBpmActivityInstance, busBpmActivityInstance>((busBpmActivityInstance)null, ClassMapper.GetObject<busBpmActivityInstance>(), adtbList, "icdoBpmActivityInstance");
            //foreach (busBpmActivityInstance activityInstance in this.iclbBpmActivityInstance)
            //{
            //    if(this.ibusBpmProcess == null)
            //    {
            //        this.LoadBpmProcess();
            //    }
            //    //activityInstance.ibusBpmActivity = this.ibusBpmProcess.GetActivity(activityInstance.icdoBpmActivityInstance.activity_id);
            //    if(this.iclbBpmActivity == null)
            //    {
            //        this.LoadBpmActivitys();
            //    }
            //    activityInstance.ibusBpmActivity =  this.iclbBpmActivity.Where<busBpmActivity>((Func<busBpmActivity, bool>)(lbusBpmAct => lbusBpmAct.icdoBpmActivity.activity_id == activityInstance.icdoBpmActivityInstance.activity_id)).FirstOrDefault<busBpmActivity>();
            //    activityInstance.ibusBpmProcessInstance = this;
            //}

            iclbFilteredBpmActivityInstance = iclbBpmActivityInstance;
        }

        //public string istrPersonOrOrganizationName
        //{
        //    get
        //    {
        //        if (ibusBpmCaseInstance.icdoBpmCaseInstance.person_id > 0)
        //        {
        //            if (ibusBpmCaseInstance.ibusPerson == null)
        //            {
        //                ibusPerson = new busPerson { icdoPerson = new doPerson() };
        //                ((busPerson)ibusPerson).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.person_id);
        //            }
        //            return ((busPerson)ibusPerson).istrFullName;
        //        }
        //        else if (ibusBpmCaseInstance.icdoBpmCaseInstance.org_id > 0)
        //        {
        //            if (ibusBpmCaseInstance.ibusOrganization == null)
        //            {
        //                ibusOrganization = new busOrganization() { icdoOrganization = new doOrganization() };
        //                ((busOrganization)ibusOrganization).FindByPrimaryKey(ibusBpmCaseInstance.icdoBpmCaseInstance.org_id);
        //            }
        //            return ((busOrganization)ibusOrganization).icdoOrganization.org_name;
        //        }
        //        return "";
        //    }
        //}
    }
}
