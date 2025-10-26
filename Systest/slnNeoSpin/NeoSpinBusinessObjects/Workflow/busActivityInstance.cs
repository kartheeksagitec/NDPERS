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
using NeoSpin.Common;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busActivityInstance:
    /// Inherited from busActivityInstanceGen, the class is used to customize the business object busActivityInstanceGen.
    /// </summary>
    [Serializable]
    public class busActivityInstance : busActivityInstanceGen
    {
        public string istrViewDetails
        {
            get
            {
                return "View Details";
            }
        }
        public string istrView
        {
            get
            {
                return "View";
            }
        }

        public bool iblnWorkflowEventActionClicked { get; set; }

        public Collection<busProcessInstanceImageData> iclbProcessInstanceImageData { get; set; }
        public Collection<busActivityInstanceHistory> iclbProcessInstanceHistory { get; set; }
        public Collection<busNotes> iclbProcessInstanceNotes { get; set; }
        public Collection<busProcessInstanceChecklist> iclbProcessInstanceChecklist { get; set; }

        public void LoadProcessInstanceImageData()
        {
            iclbProcessInstanceImageData = new Collection<busProcessInstanceImageData>();

            //PrePopulate for Testing Purposes
            //iclbProcessInstanceImageData.Add(new busProcessInstanceImageData(){document_id = "1",document_title = "test"});

            Collection<busProcessInstanceImageData> lclbPIImageData = new Collection<busProcessInstanceImageData>();
            DataTable ldtbPIImageData = Select<cdoProcessInstanceImageData>(new string[1] { "process_instance_id" },
                                    new object[1] { icdoActivityInstance.process_instance_id }, null, "PROCESS_INSTANCE_IMAGE_DATA_ID desc");
            lclbPIImageData = GetCollection<busProcessInstanceImageData>(ldtbPIImageData, "icdoProcessInstanceImageData");

            //Select the Distinct Document Code To Avoid Duplicate showing on Grid
            var lenuDistinctList = lclbPIImageData.GroupBy(i => i.icdoProcessInstanceImageData.document_code).Select(i => i.First());

            foreach (busProcessInstanceImageData lobjPIImageData in lenuDistinctList)
            {
                Collection<busProcessInstanceImageData> lclbTempPIImageData =
                        busFileNetHelper.LoadFileNetImages(
                            lobjPIImageData.icdoProcessInstanceImageData.image_doc_category_description,
                            lobjPIImageData.icdoProcessInstanceImageData.filenet_document_type_description,
                            lobjPIImageData.icdoProcessInstanceImageData.document_code,
                            lobjPIImageData.icdoProcessInstanceImageData.initiated_date,
                            ibusProcessInstance.icdoProcessInstance.person_id,
                            ibusProcessInstance.ibusOrganization.icdoOrganization.org_code);
                foreach (busProcessInstanceImageData lobjTempPIImageData in lclbTempPIImageData)
                {
                    lobjTempPIImageData.icdoProcessInstanceImageData = lobjPIImageData.icdoProcessInstanceImageData;
                    iclbProcessInstanceImageData.Add(lobjTempPIImageData);
                }
            }
        }

        public void LoadProcessInstanceHistory()
        {
            DataTable ldtpProcessInstanceHistory = busNeoSpinBase.Select("cdoActivityInstance.LoadProcessInstanceHistory", new object[1] { icdoActivityInstance.process_instance_id });
            iclbProcessInstanceHistory = GetCollection<busActivityInstanceHistory>(ldtpProcessInstanceHistory, "icdoActivityInstanceHistory");
        }

        public void LoadProcessInstanceChecklist()
        {
            DataTable ldtbChecklist = busNeoSpinBase.Select("cdoActivityInstance.LoadProcessInstanceCheckList", new object[1] { icdoActivityInstance.process_instance_id });
            iclbProcessInstanceChecklist = GetCollection<busProcessInstanceChecklist>(ldtbChecklist, "icdoProcessInstanceChecklist");
        }

        public void LoadProcessInstanceNotes()
        {
            if (ibusProcessInstance.icdoProcessInstance.person_id != 0)
            {
                DataTable ldtbNotesPerson = busNeoSpinBase.Select("cdoNotes.PersonLookup",
                               new object[3] { "WRFL", icdoActivityInstance.process_instance_id, ibusProcessInstance.icdoProcessInstance.person_id });
                iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotesPerson, "icdoNotes");
            }
            else if (ibusProcessInstance.icdoProcessInstance.org_id != 0)
            {
                DataTable ldtbNotesOrg = busNeoSpinBase.Select("cdoNotes.OrgLookup",
                                              new object[3] { "WRFL", icdoActivityInstance.process_instance_id, ibusProcessInstance.icdoProcessInstance.org_id });
                iclbProcessInstanceNotes = GetCollection<busNotes>(ldtbNotesOrg, "icdoNotes");
            }
        }

        public ArrayList InvokeWorkflowAction()
        {
            this.ibusBaseActivityInstance = this;
            ArrayList larrResult = new ArrayList();
            switch (iobjPassInfo.istrPostBackControlID)
            {
                case "btnSuspend":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusSuspended, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnResume":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusResumed, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnCancel":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
                    //This Property setting it 
                    icdoActivityInstance.status_value = busConstant.ActivityStatusCancelled;
                    iblnWorkflowEventActionClicked = true;
                    break;
                case "btnRelease":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusReleased, ibusBaseActivityInstance, iobjPassInfo);
                    break;
                case "btnComplete":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                    icdoActivityInstance.status_value = busConstant.ActivityStatusProcessed;
                    iblnWorkflowEventActionClicked = true;
                    break;
                case "btnReturntoAudit":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.ReturnBack, busConstant.ActivityStatusReturnedToAudit, iobjPassInfo);
                    icdoActivityInstance.status_value = busConstant.ActivityStatusReturnedToAudit;
                    iblnWorkflowEventActionClicked = true;
                    break;
                case "btnReturn":
                    larrResult = busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Return, busConstant.ActivityStatusReturned, iobjPassInfo);
                    icdoActivityInstance.status_value = busConstant.ActivityStatusReturned;

                    iblnWorkflowEventActionClicked = true;
                    break;
            }

            if (larrResult.Count == 0) //If No Error
            {
                ReloadObjects();
                larrResult.Add(this);
            }
            return larrResult;
        }

        private void ReloadObjects()
        {
            //Reload the Status 
            icdoActivityInstance.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(20, icdoActivityInstance.status_value);
            LoadProcessInstanceImageData();
            LoadProcessInstanceHistory();
            LoadProcessInstanceChecklist();
            LoadProcessInstanceNotes();
            EvaluateInitialLoadRules();
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busActivityInstanceHistory)
            {
                busActivityInstanceHistory lbusActivityInstanceHistory = (busActivityInstanceHistory)aobjBus;
                lbusActivityInstanceHistory.ibusSolutionActivityInstance = new busActivityInstance() { icdoActivityInstance = new cdoActivityInstance() };
                lbusActivityInstanceHistory.ibusSolutionActivityInstance.ibusActivity = new busActivity { icdoActivity = new cdoActivity() };
                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstanceHistory.icdoActivityInstanceHistory.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["ACTIVITY_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusSolutionActivityInstance.ibusActivity.icdoActivity.display_name = adtrRow["ACTIVITY_NAME"].ToString();
                }
            }

            if (aobjBus is busProcessInstanceChecklist)
            {
                busProcessInstanceChecklist lbusProcessInstanceCheckList = (busProcessInstanceChecklist)aobjBus;
                lbusProcessInstanceCheckList.ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
                lbusProcessInstanceCheckList.ibusDocument.icdoDocument.LoadData(adtrRow);
            }
        }

        public ArrayList btnApplyCheckList_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (iclbProcessInstanceChecklist != null)
            {
                foreach (busProcessInstanceChecklist lobjProcessInstance in iclbProcessInstanceChecklist)
                {
                    lobjProcessInstance.icdoProcessInstanceChecklist.Update();
                }
            }
            LoadProcessInstanceChecklist();
            larrlist.Add(this);
            return larrlist;
        }
        //19011
        public byte[] GenerateCSVofBackLogSummary()
        {
            DataTable ldtbsummary = busNeoSpinBase.Select("entActivityInstance.rptActivitiesInStatusExceeding3DaysSummary", new object[0] { });

            StringBuilder lsb = new StringBuilder();

            if (ldtbsummary.Rows.Count > 0)
            {
                for (int i = 0; i < ldtbsummary.Columns.Count; i++)
                {
                    lsb.Append(ldtbsummary.Columns[i].ColumnName);
                    lsb.Append(',');
                }
                lsb.Append("\r\n");
                for (int j = 0; j < ldtbsummary.Rows.Count; j++)
                {
                    for (int i = 0; i < ldtbsummary.Columns.Count; i++)
                    {
                        lsb.Append(ldtbsummary.Rows[j][ldtbsummary.Columns[i].ColumnName]);
                        lsb.Append(',');
                    }
                    lsb.Append("\r\n");
                }                
            }
            byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(lsb.ToString());
            return byteArray; 
        }
    }
}
