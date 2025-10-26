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

#endregion

namespace NeoSpinBatch
{
    class busMonthlyEmployerStatementBatch : busNeoSpinBatch
    {
        public busMonthlyEmployerStatementBatch()
        {

        }

        public void GenerateMonthlyEmployerStatment()
        {
            istrProcessName = "Generate Monthly Employer Statement For Active Employer";

            idlgUpdateProcessLog("Generate Monthly Employer Statement For Active Employers Batch Started", "INFO", istrProcessName);

            busEmployerPayrollMonthlyStatement lobjMonthlyEmployerPayrollStatement = new busEmployerPayrollMonthlyStatement();
            lobjMonthlyEmployerPayrollStatement.icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
            lobjMonthlyEmployerPayrollStatement.iblnIsFrombatch = true;

            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollMonthlyStatement.GetActiveOrgPlan", new object[] { });

            lobjMonthlyEmployerPayrollStatement.LoadOrgPlan(ldtbList);
            //prod pir 6488 : need to include service purchase posted header only once
            lobjMonthlyEmployerPayrollStatement.iblnServicePurchaseIncluded = false;
            foreach (busOrgPlan lobjOrgPlan in lobjMonthlyEmployerPayrollStatement.iclbActiveOrgPlan)
            {
                idlgUpdateProcessLog(
                    "Processing Org Code for " + lobjOrgPlan.ibusOrganization.icdoOrganization.org_code +
                    " and Plan for " + lobjOrgPlan.ibusPlan.icdoPlan.plan_name, "INFO", istrProcessName);
                lobjMonthlyEmployerPayrollStatement.GenerateMonthlyEmployerStatment(iobjSystemManagement.icdoSystemManagement.batch_date, lobjOrgPlan);
            }

            idlgUpdateProcessLog("Generate Monthly Employer Statement For Active Employers Batch Ended", "INFO", istrProcessName);
        }
    }
}
