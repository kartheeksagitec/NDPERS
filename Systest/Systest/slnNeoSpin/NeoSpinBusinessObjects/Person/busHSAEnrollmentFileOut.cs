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
    public class busHSAEnrollmentFileOut : busFileBaseOut
    {

        public DateTime idtTodaysDate { get; set; }
        public string istrCoCode { get; set; }
        public string istrGroupName { get; set; }
        
        public Collection<busPersonAccountGhdv> iclbPersonAccountGHDV { get; set; }
        
        public override void InitializeFile()
        {
            //TEST_Today’sDate_CoCode_GroupName_Enroll
            istrCoCode = "17306";
            istrGroupName = "North Dakota Public Employees Retirement System";
            istrFileName =  idtTodaysDate.ToString(busConstant.DateFormat) + "_" + istrCoCode + "_" + istrGroupName + "_" + "Enroll.txt";
        }
        public void LoadHSAEnrollmentData(DataTable adtbHSAEnrollment)
        {
            iclbPersonAccountGHDV = (Collection<busPersonAccountGhdv>)iarrParameters[0];
            idtTodaysDate = DateTime.Now;    
        }
    }
}
