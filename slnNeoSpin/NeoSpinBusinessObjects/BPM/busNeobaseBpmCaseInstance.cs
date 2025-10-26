using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using System;

namespace NeoBase.BPM
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmCaseInstance:
    /// Inherited from busBpmCaseInstanceGen, the class is used to customize the business object busBpmCaseInstanceGen.
    /// </summary>
    [Serializable]
    public class busNeobaseBpmCaseInstance : busBpmCaseInstance
    {
        public busNeobaseBpmCaseInstance() : base()
        {
            if (iobjPassInfo != null)
            {
                ibusPerson = busBase.CreateNewObject(BpmClientBusinessObjects.istrPerson);
                ibusOrganization = busBase.CreateNewObject(BpmClientBusinessObjects.istrOrganization);
            }//ibusPerson = new busPerson() { icdoPerson = new doPerson() };
            //ibusOrganization = new busOrganization() { icdoOrganization = new doOrganization() };
        }

        /// <summary>
        /// Load Method
        /// </summary>
        /// <param name="aintCaseInstanceId"></param>
        public void LoadBpmCaseInstance(int aintCaseInstanceId)
        {
            ibusBpmCase = busBpmCase.GetBpmCase(icdoBpmCaseInstance.case_id);
            if (ibusBpmCase != null)
            {
                foreach (busBpmProcess lbusBpmProcess in ibusBpmCase.iclbBpmProcess)
                {
                    lbusBpmProcess.iclbBpmActivity.ForEach(bpmActivity => bpmActivity.LoadRoles());
                }
            }

        }


        /// <summary>
        /// Loads additional activity instances
        /// </summary>
        /// <param name="abusBpmActivityInstance"></param>
        public override void LoadAdditionalDetailsWithActivityInstance(busBpmActivityInstance abusBpmActivityInstance)
        {
            if (abusBpmActivityInstance is busNeobaseBpmActivityInstance)
            {
                ((busNeobaseBpmActivityInstance)abusBpmActivityInstance).LoadProcessInstanceNotes();
            }
        }

        //internal busSolBpmActivityInstance LoadWithSolActivityInst(int v)
        //{
        //    //string lstrQueryRef = "entBpmActivityInstance.GetUserAssignedActivitiesByUserId";
        //    DataTable ldtbactivityinstance = busBase.Select<doBpmActivityInstance>(new string[1] { "activity_instance_id"},
        //                                                  new object[1] {v}, null, null);
        //    busSolBpmActivityInstance lbusSolBpmActivityInstance = new busSolBpmActivityInstance();
        //    lbusSolBpmActivityInstance.icdoBpmActivityInstance = new doBpmActivityInstance();
        //    if (ldtbactivityinstance.IsNotNullOrEmpty())
        //    {
        //        lbusSolBpmActivityInstance.icdoBpmActivityInstance.LoadData(ldtbactivityinstance.Rows[0]);
        //    }
        //    return lbusSolBpmActivityInstance;
        //    //throw new NotImplementedException();
        //}
    }
}
