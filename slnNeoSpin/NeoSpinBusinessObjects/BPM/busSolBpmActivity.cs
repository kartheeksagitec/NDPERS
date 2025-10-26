#region Using directives

using NeoBase.BPM;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.Common;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busSolBpmActivity : busNeobaseBpmActivity //busBpmActivity
    {
        public string istrNewModeScreen { get; set; }
        public string istrUpdateModeScreen { get; set; }
        public string istrRoleDescription { get; set; }
        public void LoadAdditionalInformation()
        {
            busBpmCase lbusBpmCase = busBpmCase.GetBpmCase(ibusBpmProcess.icdoBpmProcess.case_id);
            busBpmProcess lbusBpmProcess = lbusBpmCase.iclbBpmProcess.Where(process => process.icdoBpmProcess.process_id == ibusBpmProcess.icdoBpmProcess.process_id).FirstOrDefault();
            busBpmActivity lbusBpmActivity = lbusBpmProcess.iclbBpmActivity.Where(act => act.icdoBpmActivity.activity_id == icdoBpmActivity.activity_id).FirstOrDefault();
            utlBPMTask lutlTask = lbusBpmActivity.GetBpmActivityDetails() as utlBPMTask;
            if (lutlTask != null)
            {
                if (lutlTask.iobjPerformers != null && lutlTask.iobjPerformers.icolDefaultPreconditions != null && lutlTask.iobjPerformers.icolDefaultPreconditions.Count > 0)
                {
                    if (lutlTask.iobjPerformers.icolDefaultPreconditions[0].iblnIsDefaultCondition)
                    {
                        if (!string.IsNullOrEmpty(lutlTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria[0].istrRole))
                        {
                            lbusBpmActivity.role_id = int.Parse(lutlTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria[0].istrRole);
                            lbusBpmActivity.LoadRoles();
                            istrRoleDescription = ((busRoles)lbusBpmActivity.ibusRoles).icdoRoles.role_description;
                        }
                    }
                }
                
                foreach (utlBpmForm lobjForm in lutlTask.icolForms)
                {
                    if (lobjForm.ienmMode == utlPageMode.New)
                    {
                        istrNewModeScreen = lobjForm.istrFormName;
                    }
                    else if (lobjForm.ienmMode == utlPageMode.Update)
                    {
                        istrUpdateModeScreen = lobjForm.istrFormName;
                    }
                }


            }
        }

    }

    [Serializable]
    public class busSolBpmCase : busBpmCase
    {
        public override void PostDeploy(busBpmCase abusPreviousVersionCase)
        {
            base.CopyAdditionalSettings(abusPreviousVersionCase);
            foreach(busBpmProcess process in iclbBpmProcess)
            {
                foreach(busBpmActivity activity in process.iclbBpmActivity.Where(a => a is busBpmUserTask))
                {
                    DataTable ldtbList = Select<doBpmActivityRoleXr>(
                     new string[1] { enmBpmActivityRoleXr.activity_id.ToString() },
                        new object[1] { activity.icdoBpmActivity.activity_id }, null, null);
                    if (ldtbList.Rows.Count == 0)
                    {
                        utlBPMElement lutlBPMElement = activity.GetBpmActivityDetails();
                        if (lutlBPMElement != null)
                        {
                            utlBPMTask lutlBPMTask = lutlBPMElement as utlBPMTask;
                            if (lutlBPMTask != null && lutlBPMTask.iobjPerformers != null && lutlBPMTask.iobjPerformers.icolDefaultPreconditions != null && lutlBPMTask.iobjPerformers.icolDefaultPreconditions.Count > 0)
                            {
                                if (lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].iblnIsDefaultCondition && lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria.Count > 0)
                                {
                                    foreach (utlBpmUserSelectionCondition cond in lutlBPMTask.iobjPerformers.icolDefaultPreconditions[0].icolutlBpmUserSelectionCriteria)
                                    {
                                        busBpmActivityRoleXr activityRole = new busBpmActivityRoleXr() { icdoBpmActivityRoleXr = new doBpmActivityRoleXr() };
                                        activityRole.icdoBpmActivityRoleXr.activity_id = activity.icdoBpmActivity.activity_id;
                                        if (!string.IsNullOrEmpty(cond.istrRole))
                                        {
                                            int role_id = 0;
                                            int.TryParse(cond.istrRole, out role_id);
                                            activityRole.icdoBpmActivityRoleXr.role_id = role_id;
                                        }
                                        if (activityRole.icdoBpmActivityRoleXr.role_id > 0)
                                        {
                                            activityRole.icdoBpmActivityRoleXr.Insert();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
