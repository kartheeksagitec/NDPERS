using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFSAEligibilityOutboundFile : busFileBaseOut
    {
        public Collection<busPersonAccountFlexComp> iclbFSAEligibleMembers { get; set; }

        //PIR-19908 Need to change the provider code on FSA Eligibility File from 700091 to 700097
        public override void InitializeFile()
        {
            istrFileName = "FSA_Enrollment_700097_" + DateTime.Now.ToString(busConstant.DateFormat) + ".txt";
        }

        public void LoadFSAEligibleMembers(DataTable ldtbFSAEligibleMembers)
        {
            iclbFSAEligibleMembers = new Collection<busPersonAccountFlexComp>();
            iclbFSAEligibleMembers = (Collection<busPersonAccountFlexComp>)iarrParameters[0];
        }
    }
}
