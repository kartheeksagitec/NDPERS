#region Using directives

using System;
using NeoSpin.CustomDataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public partial class busWFMimicIndexing : busExtendBase
    {
        public busWFMimicIndexing()
        {

        }

        public doBpmRequest icdoWorkflowRequest { get; set; }

        public bool FindWFMimicIndexing(int aintWorkflowRequestID)
        {
            bool lblnResult = false;
            if (icdoWorkflowRequest == null)
            {
                icdoWorkflowRequest = new doBpmRequest();
            }
            if (icdoWorkflowRequest.SelectRow(new object[1] { aintWorkflowRequestID }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
