#region [Using directives]
using NeoBase.BPM;
using NeoBase.Common;
using NeoBase.Common.DataObjects;
using NeoSpin.BusinessObjects;
//using NeoBase.Security.DataObjects;
using NeoSpin.Common;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
#endregion [Using directives]

namespace NeoBase.BPM
{
    [Serializable]
    public class busBpmSupervisor : busBpmSupervisorBAM
    {
        public Collection<busSolBpmActivityInstance> iclbOpenBPMs { get; set; }
        public Collection<busSolBpmActivityInstance> iclbCompletedBPMs { get; set; }
        public void LoadBpmSupervisorDashboardData()
        {
            LoadDashboardAndCompletedBPMsData();
            LoadOpenBPMs();
        }
        public void LoadDashboardAndCompletedBPMsData()
        {
            LoadDashboardData();
            LoadCompletedBPMs();
        }
        public void LoadOpenBPMs()
        {
            iclbOpenBPMs = new Collection<busSolBpmActivityInstance>();
            object[] larrParameters = new object[] { utlPassInfo.iobjPassInfo.iintUserSerialID, utlPassInfo.iobjPassInfo.istrUserID };
            DataTable ldtblist = DBFunction.DBSelect("entBpmActivityInstance.LoadOpenBPMs", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            foreach (DataRow ldrDataRow in ldtblist.Rows)
            {
                busSolBpmActivityInstance lbusSolBpmActivityInstance = new busSolBpmActivityInstance()
                {
                    icdoBpmActivityInstance = new doBpmActivityInstance(),
                    ibusBpmProcessInstance = new busBpmProcessInstance() { icdoBpmProcessInstance = new doBpmProcessInstance(),
                    ibusBpmProcess = new busBpmProcess() { icdoBpmProcess = new doBpmProcess()}, ibusBpmCaseInstance = new busBpmCaseInstance()
                    {
                        icdoBpmCaseInstance = new doBpmCaseInstance() }
                    },
                };
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["PROCESS_INSTANCE_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["PROCESS_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.LoadData(ldrDataRow);
                lbusSolBpmActivityInstance.icdoBpmActivityInstance.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["ACTIVITY_INSTANCE_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["ACTIVITY_ID"].ToString()));

                lbusSolBpmActivityInstance.istrOutstandingDays = ldrDataRow["istrOutstandingDays"].ToString();
                lbusSolBpmActivityInstance.istrRoleDesc = ldrDataRow["ROLE_DESCRIPTION"].ToString();
                lbusSolBpmActivityInstance.istrOrgCode = ldrDataRow["ORG_CODE"].ToString();
                iclbOpenBPMs.Add(lbusSolBpmActivityInstance);
            }
        }
        public void LoadCompletedBPMs()
        {
            iclbCompletedBPMs = new Collection<busSolBpmActivityInstance>();
            object[] larrParameters = new object[] { utlPassInfo.iobjPassInfo.iintUserSerialID, utlPassInfo.iobjPassInfo.istrUserID, idtDateFrom.Date, idtDateTo.Date };//
            DataTable ldtblist = DBFunction.DBSelect("entBpmActivityInstance.LoadCompletedBPMs", larrParameters, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            foreach (DataRow ldrDataRow in ldtblist.Rows)
            {
                busSolBpmActivityInstance lbusSolBpmActivityInstance = new busSolBpmActivityInstance()
                {
                    icdoBpmActivityInstance = new doBpmActivityInstance(),
                    ibusBpmProcessInstance = new busBpmProcessInstance()
                    {
                        icdoBpmProcessInstance = new doBpmProcessInstance(),
                        ibusBpmProcess = new busBpmProcess() { icdoBpmProcess = new doBpmProcess() },
                        ibusBpmCaseInstance = new busBpmCaseInstance()
                        {
                            icdoBpmCaseInstance = new doBpmCaseInstance()
                        }
                    },
                };
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["PROCESS_INSTANCE_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["PROCESS_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.icdoBpmCaseInstance.LoadData(ldrDataRow);
                lbusSolBpmActivityInstance.icdoBpmActivityInstance.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["ACTIVITY_INSTANCE_ID"].ToString()));
                lbusSolBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.FindByPrimaryKey(Convert.ToInt32(ldrDataRow["ACTIVITY_ID"].ToString()));

                lbusSolBpmActivityInstance.istrRoleDesc = ldrDataRow["ROLE_DESCRIPTION"].ToString();
                lbusSolBpmActivityInstance.istrOrgCode = ldrDataRow["ORG_CODE"].ToString();
                
                iclbCompletedBPMs.Add(lbusSolBpmActivityInstance);
            }
        }
    }
}
