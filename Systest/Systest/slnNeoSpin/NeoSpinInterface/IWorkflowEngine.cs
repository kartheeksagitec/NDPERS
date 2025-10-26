using System;
using NeoSpin.Common;

namespace NeoSpin.Interface
{
    public interface IWorkflowEngine
    {
		//Fw upgrade issues - For workflow process
        WorkflowResult Run(int aintSystemRequestID, int aintProcessID, int aintPersonID, int aintOrgID, long aintReferenceID, string astrCreatedBy,int aintContactTicketID);       
        bool ResumeBookmark(Guid awinInstanceID, ActivityInstanceEventArgs aaieBookMarkValue);
        void Abort(Guid id, string reason);
        void Abort(Guid id);
        void Cancel(Guid id);
        void Terminate(Guid id, string reason);
    }
}
