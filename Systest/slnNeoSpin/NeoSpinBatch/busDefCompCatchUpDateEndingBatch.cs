#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
#endregion

namespace NeoSpinBatch
{
    class busDefCompCatchUpDateEndingBatch : busNeoSpinBatch
    { 

        public void ProcessReport(DateTime ldtBatchDate)
        {    
            istrProcessName = "Deferred Comp 3 Yr Catch-Up Date Ending Report";
            idlgUpdateProcessLog("Generating Report for Deferred Comp 3 Yr Catch-Up Date Ending", "INFO", istrProcessName);

            DataTable ldtbDefComp3yrCatchupDetails = DBFunction.DBSelect("cdoPersonAccountDeferredComp.rpt3YrCatchUpEnding",
                new object[1] { ldtBatchDate}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            ldtbDefComp3yrCatchupDetails.TableName = busConstant.ReportTableName;
            if (ldtbDefComp3yrCatchupDetails.Rows.Count > 0)
            {
                CreateReport("rptDeferred3YrCatchUpEnding.rpt", ldtbDefComp3yrCatchupDetails);
                idlgUpdateProcessLog("Report created successfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
        }
    }
}
