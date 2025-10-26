#region Using directives
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.Common;
using NeoSpin.CustomDataObjects;
using Sagitec.Bpm;
using System;

#endregion
namespace NeoSpinBatch
{
    class busProcessIncompleteRefundApplicationBatch : busNeoSpinBatch
    {
        private Collection<busBenefitRefundApplication> _iclbBenefitRefundApplication;

        public Collection<busBenefitRefundApplication> iclbBenefitRefundApplication
        {
            get { return _iclbBenefitRefundApplication; }
            set { _iclbBenefitRefundApplication = value; }
        }

        public void ProcessIncompleteRefundApplication()
        {
            busBase lobjBase = new busBase();

            DataTable ltdIncompleteApp = busNeoSpinBase.Select("cdoBenefitApplication.InCompleteApplicationBatch", new object[0] { });

            iclbBenefitRefundApplication = lobjBase.GetCollection<busBenefitRefundApplication>(ltdIncompleteApp, "icdoBenefitApplication");

            foreach (busBenefitRefundApplication lobjRefundApplication in iclbBenefitRefundApplication)
            {
                //update the status of workflow as cancelled.
                // get instance id and check if greater than 0
                // update the instance status as cancelled
                // //get activity instance id for the workflow id and ref id.
                //venkat check query
                DataTable ldtbActivityInstance = busNeoSpinBase.Select("entSolBpmActivityInstance.LoadSuspendedInstancesByProcessAndReference",
                                       new object[2] { busConstant.Map_Initialize_Process_Refund_Application_And_Calculation, lobjRefundApplication.icdoBenefitApplication.benefit_application_id });
                if (ldtbActivityInstance.Rows.Count > 0)
                {
                    busBpmActivityInstance lobjActivityInstance = busWorkflowHelper.GetActivityInstance(Convert.ToInt32(ldtbActivityInstance.Rows[0]["activity_instance_id"]));

                    string lstrUserID = iobjPassInfo.istrUserID;
                    iobjPassInfo.istrUserID = lobjActivityInstance.icdoBpmActivityInstance.checked_out_user;
                    busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed,
                                                                                   lobjActivityInstance,
                                                                                   iobjPassInfo);
                    iobjPassInfo.istrUserID = lstrUserID;

                }
            }
        }
    }
}