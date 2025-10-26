#region Using directives
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.Bpm;
using System;
#endregion
namespace NeoSpinBatch
{
    class busExpiredDRONoticeBatch : busNeoSpinBatch
    {
        busBase lobjBase = new busBase();
        private Collection<busBenefitDroApplication> _iclbDROApplications;

        public Collection<busBenefitDroApplication> iclbDROApplications
        {
            get { return _iclbDROApplications; }
            set { _iclbDROApplications = value; }
        }
     
        public void Generate18MonthsDROExpiringExpiredLetter()
        {
            istrProcessName = "Expired DRO Notice Batch";
            idlgUpdateProcessLog("Expired DRO Notice 18 Months Batch Started ", "INFO", istrProcessName);
            
            DataTable ldtb18monthsapp = busBase.Select("cdoBenefitDroApplication.Load18MonthsApplications", new object[] { });
            iclbDROApplications=lobjBase.GetCollection<busBenefitDroApplication>(ldtb18monthsapp,"icdoBenefitDroApplication");

            foreach (busBenefitDroApplication lobjDroApplication in iclbDROApplications)
            {
                if (!lobjDroApplication.IsDROApplicationCancelledOrDeniedOrCancelledOrQualified())
                {
                    int lintMonth = busGlobalFunctions.DateDiffByMonth(lobjDroApplication.icdoBenefitDroApplication.received_date,
                                                                                    iobjSystemManagement.icdoSystemManagement.batch_date);
                    if (lintMonth > 18)
                    {
                        lobjDroApplication.Is18MonthsNotice = "1";                      
                        lobjDroApplication.icdoBenefitDroApplication.letter_sent_18_months_flag=busConstant.Flag_Yes;
                        lobjDroApplication.LoadAlternatePayee();
                        lobjDroApplication.icdoBenefitDroApplication.Update();
                        CreateCorrespondence(lobjDroApplication, "LEG-6000");
                        InitializeUpdateWorkFlow(busConstant.Map_Nullify_QDRO, lobjDroApplication.icdoBenefitDroApplication.member_perslink_id);
                    }

                }
            }
            idlgUpdateProcessLog("Expired DRO Notice 18 Months Batch Ended ", "INFO", istrProcessName);
        }

        public void Generate503DaysDROExpiringExpiredLetter()
        {
            istrProcessName = "Expired DRO Notice Batch";
            idlgUpdateProcessLog("Expired DRO Notice 503 Days Batch Started ", "INFO", istrProcessName);
            
            DataTable ldtb18monthsapp = busBase.Select("cdoBenefitDroApplication.Load503DaysApplications", new object[] { });
            iclbDROApplications=lobjBase.GetCollection<busBenefitDroApplication>(ldtb18monthsapp,"icdoBenefitDroApplication");

            foreach (busBenefitDroApplication lobjDroApplication in iclbDROApplications)
            {
                if (!lobjDroApplication.IsDROApplicationCancelledOrDeniedOrCancelledOrQualified())
                {
                    int lintDays = busGlobalFunctions.DateDiffInDays(lobjDroApplication.icdoBenefitDroApplication.received_date,
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date);
                    if (lintDays > 503)
                    {
                        lobjDroApplication.Is18MonthsNotice = "2";                        
                        lobjDroApplication.icdoBenefitDroApplication.letter_sent_503_days_flag = busConstant.Flag_Yes;
                        lobjDroApplication.LoadAlternatePayee();
                        lobjDroApplication.icdoBenefitDroApplication.Update();
                        CreateCorrespondence(lobjDroApplication, "LEG-6000");
                        InitializeUpdateWorkFlow(busConstant.Map_Nullify_QDRO, lobjDroApplication.icdoBenefitDroApplication.member_perslink_id);
                    }
                }
            }
        }
        //PIR 22861 change days logic from 488 to 464
        public void Generate488DaysDROExpiringExpiredLetter()
        {
            istrProcessName = "Expired DRO Notice Batch";
            idlgUpdateProcessLog("Expired DRO Notice 464 Days Batch Started ", "INFO", istrProcessName);

            DataTable ldtb18monthsapp = busBase.Select("cdoBenefitDroApplication.Load488DaysApplications", new object[] { });
            iclbDROApplications = lobjBase.GetCollection<busBenefitDroApplication>(ldtb18monthsapp, "icdoBenefitDroApplication");

            foreach (busBenefitDroApplication lobjDroApplication in iclbDROApplications)
            {
                if (!lobjDroApplication.IsDROApplicationCancelledOrDeniedOrCancelledOrQualified())
                {
                    int lintDays = busGlobalFunctions.DateDiffInDays(lobjDroApplication.icdoBenefitDroApplication.received_date,
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date);
                    if (lintDays > 464)
                    {
                        lobjDroApplication.Is18MonthsNotice = "2";
                        lobjDroApplication.icdoBenefitDroApplication.letter_sent_488_days_flag = busConstant.Flag_Yes;
                        lobjDroApplication.LoadAlternatePayee();
                        lobjDroApplication.icdoBenefitDroApplication.Update();                     
                        CreateCorrespondence(lobjDroApplication, "LEG-6000");
                        InitializeUpdateWorkFlow(busConstant.Map_Nullify_QDRO, lobjDroApplication.icdoBenefitDroApplication.member_perslink_id);
                    }
                }
            }
        }

        // PIR 22861 Created this Method to Initiate/resume the BPM 'Nullify QDRO' when ‘Expired DRO Notice Batch’ executes
        private void InitializeUpdateWorkFlow(int aintWorkflowProcessId, int aintPersonId)
        {
            busWorkflowHelper.InitiateBpmRequest(aintWorkflowProcessId, aintPersonId, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
            DataTable ldtRunningInstance = busWorkflowHelper.LoadRunningInstancesByPersonAndProcess(aintPersonId, aintWorkflowProcessId);
            if (ldtRunningInstance.Rows.Count > 0)
            {
                idlgUpdateProcessLog("WorkFlow Nullify QDRO Resumed ", "INFO", istrProcessName);
                busSolBpmCaseInstance lbusBpmCaseInstance = new busSolBpmCaseInstance();
                busBpmActivityInstance lbusActivityInstance = lbusBpmCaseInstance.LoadWithActivityInst(Convert.ToInt32(ldtRunningInstance.Rows[0]["activity_instance_id"]));
                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, lbusActivityInstance, iobjPassInfo);
            }
            else
            {
                idlgUpdateProcessLog("WorkFlow Nullify QDRO Initiated ", "INFO", istrProcessName);
                busWorkflowHelper.InitiateBpmRequest(aintWorkflowProcessId, aintPersonId, 0, 0, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
            }
        }
        private void CreateCorrespondence(busBenefitDroApplication aobjBenefitDroApplication, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjBenefitDroApplication);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence(astrCorName, aobjBenefitDroApplication, lhstDummyTable);
        }
    }
}