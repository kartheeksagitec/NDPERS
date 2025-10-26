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
#endregion

namespace NeoSpinBatch
{
    class EmployerReportPostingBatch : busNeoSpinBatch
    {
        public EmployerReportPostingBatch()
        {
        }
        
        public void ProcessEmployerReportPosting()
        {            
            bool lblnInTransaction = false;

            DataTable ldtbList = busNeoSpinBase.Select("entEmployerPayrollHeader.LoadValidAndRPSTEmployerPayrollHeader", new object[0] { });

            //prod pir 6586
            DataTable ldtPaymentItemType = new DataTable();
            DataTable ldtBenefitAccountInfo = new DataTable();
            if (ldtbList.Rows.Count > 0)
            {
                ldtPaymentItemType = busGlobalFunctions.LoadPaymentItemTypeCacheData(iobjPassInfo);
                //ldtBenefitAccountInfo = busNeoSpinBase.Select("cdoBenefitRefundCalculation.GetBenefitAccountInfo", new object[0] { });
            }

            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjEmpPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldrRow);

                try
                {
                    if (!lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }
                    lobjEmpPayrollHeader.idatRunDate = iobjSystemManagement.icdoSystemManagement.batch_date;

                    lobjEmpPayrollHeader.LoadEmployerPayrollDetailWithOtherObjects();
                    lobjEmpPayrollHeader.LoadAllPersonAccounts();

                    //prod pir 6586
                    //Load all the Benefit Account Details at once here.. PERFORMANCE Fix
                    //same person may have multiple adjustment record, so need to reload everytime
                    //lobjEmpPayrollHeader.idtBenefitAccountInfo = ldtBenefitAccountInfo;
                    if (lobjEmpPayrollHeader.ibusDBCacheData == null)
                        lobjEmpPayrollHeader.ibusDBCacheData = new busDBCacheData();
                    if (lobjEmpPayrollHeader.ibusDBCacheData.idtbCachedPaymentItemType == null)
                        lobjEmpPayrollHeader.ibusDBCacheData.idtbCachedPaymentItemType = ldtPaymentItemType;

                    if ((lobjEmpPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                        (lobjEmpPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp) ||
                        (lobjEmpPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases))
                    {
                        lobjEmpPayrollHeader.LoadAllPAEmpDetailWithChildData();
                        lobjEmpPayrollHeader.LoadAllPlanMemberTypeCrossRef();
                        lobjEmpPayrollHeader.LoadAllOrgPlans();
                        lobjEmpPayrollHeader.LoadAllOrgPlanMemberType();
                    }

                    //Assign the Payroll Header Object into Detail
                    foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in lobjEmpPayrollHeader.iclbEmployerPayrollDetail)
                    {
                        lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = lobjEmpPayrollHeader;
                        lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = lobjEmpPayrollHeader.icdoEmployerPayrollHeader;
                    }

                    if (lobjEmpPayrollHeader.iclbEmployerPayrollDetail.Count > 0) // PROD PIR ID 5251
                        lobjEmpPayrollHeader.PostEmployerReport(iobjBatchSchedule.batch_schedule_id);

                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnInTransaction = false;
                    }

                    idlgUpdateProcessLog(" Payroll Header ID : " + lobjEmpPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id + ". ",
                        "INFO", iobjBatchSchedule.step_name);
                }
                catch (Exception _exc)
                {
                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnInTransaction = false;
                    }
                    idlgUpdateProcessLog(" Payroll Header ID : " + lobjEmpPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id + ". " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                    Sagitec.ExceptionPub.ExceptionManager.Publish(_exc);
                }

                //Optimization - GC 
                lobjEmpPayrollHeader = null;
            }
        }
    }
}
