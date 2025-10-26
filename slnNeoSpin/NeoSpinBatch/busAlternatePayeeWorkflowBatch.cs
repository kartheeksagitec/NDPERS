#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;


#endregion
namespace NeoSpinBatch
{

    class busAlternatePayeeWorkflowBatch : busNeoSpinBatch
    {
        private Collection<busBenefitDroApplication> _iclbQdroApplications;

        public Collection<busBenefitDroApplication> iclbQdroApplications
        {
            get { return _iclbQdroApplications; }
            set { _iclbQdroApplications = value; }
        }

        public void IniateWorkflowFolrAlternatePayee()
        {
            istrProcessName = "Alternate Payee Batch started";
            
            busBase lobjbase = new busBase();

            DateTime ldtNexPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(2);

            DataTable ldtbQdroApplications = busNeoSpinBase.Select("cdoBenefitDroApplication.CreatePayeeAccountBatch", new object[1] { ldtNexPaymentDate });

            iclbQdroApplications = lobjbase.GetCollection<busBenefitDroApplication>(ldtbQdroApplications, "icdoBenefitDroApplication");

            foreach (busBenefitDroApplication lobjQdro in iclbQdroApplications)
            {                
                if ((lobjQdro.icdoBenefitDroApplication.work_flow_intiated_flag == busConstant.Flag_No) || (lobjQdro.icdoBenefitDroApplication.work_flow_intiated_flag == null))
                {
                    idlgUpdateProcessLog("Initializing Workflow for Alternate Payee PERSLink ID " + lobjQdro.icdoBenefitDroApplication.alternate_payee_perslink_id.ToString() + " ", "INFO", istrProcessName);
                    lobjQdro.InitiateDROWorkflow();
                    lobjQdro.icdoBenefitDroApplication.work_flow_intiated_flag = busConstant.Flag_Yes;
                    lobjQdro.icdoBenefitDroApplication.Update();
                }
            }
            idlgUpdateProcessLog("Batch Ended", "INFO", istrProcessName);
        }
       
    }
}