using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class utlProcessInstance
    {
        public int iintProcessInstanceID { get; set; }
		//Fw upgrade issues - For workflow process
        public long iintReferenceID { get; set; }
        public string istrCreatedBy { get; set; }
        public Dictionary<string, utlProcessMaintainance.utlActivity> idctActivities { get; set; }
        public string istrReturnFromAuditFlag { get; set; }
        public Dictionary<string, object> idctAdditionalParameters { get; set; }
        public string GetParameterValue(string astrParameterName)
        {
            busProcessInstance lobjPrecessInstance = new busProcessInstance();
            lobjPrecessInstance.icdoProcessInstance = new cdoProcessInstance();
            lobjPrecessInstance.icdoProcessInstance.process_instance_id = iintProcessInstanceID;
            return lobjPrecessInstance.GetParameterValue(astrParameterName);
        }
    }
}
