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

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busProcess:
    /// Inherited from busProcessGen, the class is used to customize the business object busProcessGen.
    /// </summary>
    [Serializable]
    public class busProcess : busProcessGen
    {
        //This Hash Table used in NeoFlowActivities
        public Hashtable ihstActivities { get; set; }
        public Collection<busActivity> iclbActivity { get; set; }

        public override bool FindProcess(int Aintprocessid)
        {
            if (base.FindProcess(Aintprocessid))
            {
                icdoProcess.istrWorkflowFullPath = NeoSpin.Common.ApplicationSettings.Instance.NeoFlowMapPath + icdoProcess.name + ".xaml";
                return true;
            }
            return false;
        }

        public void LoadActivityList()
        {
            DataTable ldtbActivity = Select("cdoActivity.LoadActiveActivityList", new object[1] { icdoProcess.process_id }); //PROD PIR 7979
            ihstActivities = new Hashtable();
            iclbActivity = new Collection<busActivity>();
            foreach (DataRow ldtrRow in ldtbActivity.Rows)
            {
                busActivity lobjActivity = new busActivity();
                lobjActivity.icdoActivity = new cdoActivity();
                lobjActivity.icdoActivity.LoadData(ldtrRow);
                lobjActivity.ibusProcess = this;
				//PIR - 2105
                lobjActivity.LoadRoles();
                //Load the screen name & fous control ID
                lobjActivity.GetDetails();
                ihstActivities.Add(lobjActivity.icdoActivity.name.Trim(), lobjActivity);
                iclbActivity.Add(lobjActivity);
            }
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busDocumentProcessCrossref)
            {
                busDocumentProcessCrossref lbusDocumentProcessCrossRef = (busDocumentProcessCrossref)aobjBus;
                lbusDocumentProcessCrossRef.LoadDocument();
            }
            base.LoadOtherObjects(adtrRow, aobjBus);
        }
    }
}
