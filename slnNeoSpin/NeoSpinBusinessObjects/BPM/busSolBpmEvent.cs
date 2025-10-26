using NeoBase.BPM;
using NeoBase.Common;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busSolBpmEvent: busNeobaseBpmEvent //busBpmEvent
    {
        public override void LoadBpmProcessEventXrs()
        {
            DataTable adtbList = Select("entSolution.LoadSolBpmProcessEventXrs", new object[1] { icdoBpmEvent.bpm_event_id });
            iclbBpmProcessEventXr = GetCollection<busBpmProcessEventXr>(adtbList, "icdoBpmProcessEventXr");
        }
    }
}
