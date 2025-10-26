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
using System.Collections;
using Sagitec.CorBuilder;
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
using System.Linq;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
#endregion

namespace NeoSpinBatch
{
    
    public class busPayeeAccountNightlyBatch : busNeoSpinBatch
    {
        public DateTime idtNextBenefitPaymentDate { get; set; }
        public void LoadNextBenefitPaymentDate()
        {
            idtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
        }
        public void UpdatePayeeAccountPullFlag()
        {
            istrProcessName = "Payee Account Nightly Update Batch";
            idlgUpdateProcessLog("Payee Account Nightly Update Batch started", "INFO", istrProcessName);
            LoadNextBenefitPaymentDate();

            //Update Pull Check flag to 'S', given the conditions satisfied
            idlgUpdateProcessLog("Payee Accounts - Updating Pull check flag to S", "INFO", istrProcessName);
            DBFunction.DBNonQuery("entPayeeAccount.UpdatePullCheckFlagToS", new object[1] { idtNextBenefitPaymentDate }, iobjPassInfo.iconFramework,
                                                                                iobjPassInfo.itrnFramework);

            //Select Payee Accounts with Pull Check flag = 'S' update to 'N'
            idlgUpdateProcessLog("Payee Accounts - Updating Pull check flag to N", "INFO", istrProcessName);
            DBFunction.DBNonQuery("entPayeeAccount.UpdatePullCheckFlagToN", new object[1] { idtNextBenefitPaymentDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

        }
    }
}
