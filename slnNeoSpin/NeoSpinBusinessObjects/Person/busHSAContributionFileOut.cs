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
    public class busHSAContributionFileOut : busFileBaseOut
    {

        public DateTime idtTodaysDate { get; set; }
        public string istrCoCode { get; set; }
        public string istrGroupName { get; set; }
        

        public Collection<busProviderReportDataInsurance> iclbProviderReportDataInsurance { get; set; }
        
        public override void InitializeFile()
        {
            // PIR 9170
            //TEST_Today’sDate_CoCode_GroupName_Contribution

            istrCoCode = busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo);//File name for new HSA provider
            //istrCoCode = "700075";
            istrGroupName = "NDPERS";
            istrFileName = istrGroupName + "_" + idtTodaysDate.ToString(busConstant.DateFormat) + "_" + istrCoCode + ".csv";
        }

       
        public void LoadHSAContributionData(DataTable adtbHSAEnrollment)
        {
            iclbProviderReportDataInsurance = (Collection<busProviderReportDataInsurance>)iarrParameters[0];
            idtTodaysDate = DateTime.Now;
        }
    }
}
