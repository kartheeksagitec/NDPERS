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
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
using NeoSpin.DataObjects;

namespace NeoSpinBatch
{
    public class busAllocateRemittanceToEmployerHeader : busNeoSpinBatch
    {

        public decimal idecAllowableVariance { get; set; }
        //PIR-6501, if Any change in remittance allocation to Employer Header batch, then we need to make changes in Employer Report posting batch (14).
        //PIR # 20539 if Any changes made in remittance allocation to Employer Header batch, then need to make same changes in $\slnNeoSpin\NeoSpinBusinessObjects\Employer Report\busDepositTape.cs // AllocateRemittanceToEmployerHeader() files as well
        public void AllocateRemittanceToEmployerHeader()
        {
            try
            {
                iobjPassInfo.BeginTransaction();
                int lintCounter = 1;
                busBase lobjBase = new busBase();
                istrProcessName = "Allocate Remittance to Employer payroll header";
                //PROD PIR 6062 
                //1)need to delete invalid allocations for Retr and Insr header
                //2)NEED TO ALLOCATION FOR POSTED RETR AND INSR HEADER BASED ON AVAILABLE AMOUNT, NOT EXACT MATCH
                //--start--//
                idlgUpdateProcessLog("Deleting Invalid allocations for retirement & Insurance payroll headers started", "INFO", istrProcessName);
                //query to delete invalid remittance allocation and updates header to unbalanced
                DataTable ldtInvalidRetrAllocations = busBase.Select("cdoEmployerRemittanceAllocation.GetInvalidAllocationsForRetirement", new object[0] { });
                //delet the invalid allocation
                DeleteInvalidAllocationAndUpdateEmpHeader(ldtInvalidRetrAllocations);

                DataTable ldtInvalidInsrAllocations = busBase.Select("cdoEmployerRemittanceAllocation.GetInvalidAllocationsForInsurance", new object[0] { });
                //delet the invalid allocation
                DeleteInvalidAllocationAndUpdateEmpHeader(ldtInvalidInsrAllocations, true);
                idlgUpdateProcessLog("Deleting Invalid allocations for retirement & Insurance payroll headers completed", "INFO", istrProcessName);

                idlgUpdateProcessLog("Allocate Remittance to employer payroll header started", "INFO", istrProcessName);
                //query to take all retr and insr headers for remittance allocation
                DataTable ldtRetrInsrHeader = busBase.Select("cdoEmployerPayrollHeader.LoadEmpHeaderForARBatch", new object[0] { });
                //getting the allowable remittance variance
                idecAllowableVariance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ConstantAllowableVairance, iobjPassInfo));
                foreach (DataRow ldrHeader in ldtRetrInsrHeader.Rows)
                {
                    busEmployerPayrollHeader lobjHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjHeader.icdoEmployerPayrollHeader.LoadData(ldrHeader);
                    //load total contribution and applied amount by plan
                    lobjHeader.LoadContributionByPlan();
                    if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader.iclbRetirementContributionByPlan)
                        {
                            if ((lobjHeaderByPlan.idecTotalReportedRetrContributionWithoutADECAmtByPlan - lobjHeaderByPlan.idecTotalAppliedRetrContributionByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = busBase.Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { iobjSystemManagement.icdoSystemManagement.batch_date, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance, 
                                    (lobjHeaderByPlan.idecTotalReportedRetrContributionWithoutADECAmtByPlan - lobjHeaderByPlan.idecTotalAppliedRetrContributionByPlan));
                            }
                            if ((lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan - lobjHeaderByPlan.idecTotalAppliedRHICContributionByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = busBase.Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { iobjSystemManagement.icdoSystemManagement.batch_date, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeRHICContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance, 
                                    (lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan - lobjHeaderByPlan.idecTotalAppliedRHICContributionByPlan));
                            }
                            //PIR 25920 DC 2025 Allocate ADEC Amount
                            if ((lobjHeaderByPlan.idecTotalADECReportedByPlan - lobjHeaderByPlan.idecTotalADECAppliedByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = busBase.Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { iobjSystemManagement.icdoSystemManagement.batch_date, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeADECContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalADECReportedByPlan - lobjHeaderByPlan.idecTotalADECAppliedByPlan));
                            }
                        }
                    }
                    else if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                    {
                        foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader.iclbInsuranceContributionByPlan)
                        {
                            if ((lobjHeaderByPlan.idecTotalReportedByPlan - lobjHeaderByPlan.idecTotalAppliedPremiumByPlan) > 0)
                            {
                                string lstrRemittanceType = busEmployerReportHelper.GetToItemTypeByPlan(lobjHeaderByPlan.ibusPlan.icdoPlan.plan_code);
                                //query to load available remittance
                                DataTable ldtRemittance = busBase.Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                                                   new object[4] { iobjSystemManagement.icdoSystemManagement.batch_date, lobjHeader.icdoEmployerPayrollHeader.org_id, lstrRemittanceType,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalReportedByPlan - lobjHeaderByPlan.idecTotalAppliedPremiumByPlan));
                            }
                        }
                    }
                    if (lintCounter == 10)
                    {
                        iobjPassInfo.Commit();
                        idlgUpdateProcessLog("Processed 10 employer payroll header successfully", "INFO", istrProcessName);
                        iobjPassInfo.BeginTransaction();
                        lintCounter = 1;
                    }
                    lintCounter++;
                    lobjHeader = null;
                }
                //--end--//
                DataTable ldtEmployerReport = busBase.Select("cdoEmployerPayrollHeader.LoadEmployerPayrollHeaderForRemittanceAllocation", new object[0] { });
                DataTable ldtEmployerDetailReport = busBase.Select("cdoEmployerPayrollDetail.LoadDetailForAllocateRemittance", new object[0] { });
                foreach (DataRow dr in ldtEmployerReport.Rows)
                {
                    busEmployerPayrollHeader lobjEmployerHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjEmployerHeader.icdoEmployerPayrollHeader.LoadData(dr);
                    DataTable ldtDetail = ldtEmployerDetailReport.AsEnumerable()
                                            .Where(o => o.Field<int>("employer_payroll_header_id") == lobjEmployerHeader.icdoEmployerPayrollHeader.employer_payroll_header_id)
                                            .AsDataTable();
                    lobjEmployerHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                    lobjEmployerHeader.iclbEmployerPayrollDetail = lobjBase.GetCollection<busEmployerPayrollDetail>(ldtDetail, "icdoEmployerPayrollDetail");
                    lobjEmployerHeader.AllocateRemittanceFromBatch();
                    if (lintCounter == 10)
                    {
                        iobjPassInfo.Commit();
                        idlgUpdateProcessLog("Processed 10 employer payroll header successfully", "INFO", istrProcessName);
                        iobjPassInfo.BeginTransaction();
                        lintCounter = 1;
                    }
                    lintCounter++;
                }
                if (lintCounter > 1)
                    idlgUpdateProcessLog("Processed " + lintCounter.ToString() + " employer payroll header successfully", "INFO", istrProcessName);
                //PROD PIR 6062 
                //--start--//
                idlgUpdateProcessLog("Updating employer payroll header balancing status", "INFO", istrProcessName);
                //PIR # 20539 Same queries are being used from button clicked hence Pass user Name as paramenter.
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrEmpHeaderBalnFromARBatch", new object[1] { iobjBatchSchedule.batch_schedule_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateInsrEmpHeaderBalnFromARBatch", new object[1] { iobjBatchSchedule.batch_schedule_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //prod pir 6739 & 6807
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrEmpHeaderUnBalnFromARBatch", new object[1] { iobjBatchSchedule.batch_schedule_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateInsrEmpHeaderUnBalnFromARBatch", new object[1] { iobjBatchSchedule.batch_schedule_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //prod pir 6780
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrInsrEmpHeaderNORFromARBatch", new object[1] { iobjBatchSchedule.batch_schedule_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                //PIR 14938 - Create GL after header status goes to 'Balance'.
                foreach (DataRow ldrHeader in ldtRetrInsrHeader.Rows)
                {
                    busEmployerPayrollHeader lobjHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjHeader.icdoEmployerPayrollHeader.LoadData(ldrHeader);

                    if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id);

                        if (lobjHeader.icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusBalanced)
                        {
                            decimal ldecDifferenceAmount = 0;
                            lobjHeader.LoadContributionByPlan();

                            foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader.iclbRetirementContributionByPlan)
                            {
                                ldecDifferenceAmount = lobjHeaderByPlan.idecTotalCalculatedRetrContributionByPlan - lobjHeaderByPlan.idecTotalReportedRetrContributionByPlan;
                                if (Math.Abs(ldecDifferenceAmount) > 0)
                                {
                                    lobjHeader.CreateGLForDifferenceAmount(busConstant.ItemTypeContribution, ldecDifferenceAmount, lobjHeaderByPlan.iintPlanID);
                                }

                                decimal ldecDifferenceAmountRHIC = 0;
                                ldecDifferenceAmountRHIC = lobjHeaderByPlan.idecTotalCalculatedRHICContributionByPlan - lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan;
                                if (Math.Abs(ldecDifferenceAmountRHIC) > 0)
                                {
                                    lobjHeader.CreateGLForDifferenceAmount(busConstant.ItemTypeRHICContribution, ldecDifferenceAmountRHIC, lobjHeaderByPlan.iintPlanID);
                                }

                                // updating difference amount and difference_type_value
                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocation = new busEmployerRemittanceAllocation { icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation(), iclbEmployerRemittanceAllocation = new Collection<busEmployerRemittanceAllocation>() };
                                DataTable ldtbusEmployerRemittanceAllocation = busBase.Select("cdoEmployerRemittanceAllocation.GetCountOfRemittanceWithAlocSatus",
                                                                       new object[1] { lobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id });

                                busBase lbusBase = new busBase();
                                lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation = lbusBase.GetCollection<busEmployerRemittanceAllocation>(ldtbusEmployerRemittanceAllocation, "icdoEmployerRemittanceAllocation");

                                foreach (busEmployerRemittanceAllocation lbusEmployerRemiAllocation in lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation)
                                {
                                    lbusEmployerRemiAllocation.LoadRemittance();
                                }

                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocationContr = lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation.Where(i => i.ibusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution && i.ibusRemittance.icdoRemittance.plan_id == lobjHeaderByPlan.iintPlanID).OrderByDescending(k => k.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id).FirstOrDefault();
                                if (lbusEmployerRemittanceAllocationContr.IsNotNull())
                                {
                                    if (Math.Abs(ldecDifferenceAmount) > 0)
                                    {
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.difference_amount = Math.Abs(ldecDifferenceAmount);
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.difference_type_value = ldecDifferenceAmount > 0 ? busConstant.GLItemTypeUnderContribution : busConstant.GLItemTypeOverContribution;
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.Update();
                                    }
                                }

                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocationRHIC = lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation.Where(i => i.ibusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeRHICContribution && i.ibusRemittance.icdoRemittance.plan_id == lobjHeaderByPlan.iintPlanID).OrderByDescending(k => k.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id).FirstOrDefault();
                                if (lbusEmployerRemittanceAllocationRHIC.IsNotNull())
                                {
                                    if (Math.Abs(ldecDifferenceAmountRHIC) > 0)
                                    {
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.difference_amount = Math.Abs(ldecDifferenceAmountRHIC);
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.difference_type_value = ldecDifferenceAmountRHIC > 0 ? busConstant.GLItemTypeUnderRHIC : busConstant.GLItemTypeOverRHIC;
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.Update();
                                    }
                                }
                            }
                        }
                    }
                }

                idlgUpdateProcessLog("Updating employer payroll header balancing status successfully completed", "INFO", istrProcessName);
                //--end--//
                idlgUpdateProcessLog("Allocate Remittance to employer payroll header successfully completed", "INFO", istrProcessName);                
                iobjPassInfo.Commit();
            }
            catch (Exception e)
            {                
                idlgUpdateProcessLog("Allocate Remittance to employer payroll header failed", "INFO", istrProcessName);
                ExceptionManager.Publish(e);
                iobjPassInfo.Rollback();
                throw e;
            }
        }

        /// <summary>
        /// method to delete invalid remittance allocation and change header status to unbalanced
        /// </summary>
        /// <param name="adtInvalidRetrAllocations">allocation result set</param>
        private void DeleteInvalidAllocationAndUpdateEmpHeader(DataTable adtInvalidAllocations, bool ablnIsInsurance = false)
        {
            int lintEmployerPayrollHeaderID = 0;
            foreach (DataRow ldr in adtInvalidAllocations.Rows)
            {
                busEmployerRemittanceAllocation lobjRemittanceAllocation = new busEmployerRemittanceAllocation { icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation() };
                lobjRemittanceAllocation.ibusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };

                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.LoadData(ldr);
                lobjRemittanceAllocation.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_by = ldr["HED_CREATED_BY"] == DBNull.Value ? busConstant.PERSLinkBatchUser + ' ' +iobjBatchSchedule.batch_schedule_id : Convert.ToString(ldr["HED_CREATED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_date = ldr["HED_CREATED_DATE"] == DBNull.Value ?
                    iobjSystemManagement.icdoSystemManagement.batch_date : Convert.ToDateTime(ldr["HED_CREATED_DATE"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_by = ldr["HED_MODIFIED_BY"] == DBNull.Value ? busConstant.PERSLinkBatchUser + ' ' + iobjBatchSchedule.batch_schedule_id : Convert.ToString(ldr["HED_MODIFIED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_date = ldr["HED_MODIFIED_DATE"] == DBNull.Value ?
                    iobjSystemManagement.icdoSystemManagement.batch_date : Convert.ToDateTime(ldr["HED_MODIFIED_DATE"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.update_seq = ldr["HED_UPDATE_SEQ"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["HED_UPDATE_SEQ"]);
                lobjEmpHeader.iintPlanID = ldr["iintPlanID"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["iintPlanID"]);

                if (lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount > 0 &&
                    !string.IsNullOrEmpty(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_type_value) && ablnIsInsurance) //As discussed on call with Maik, no GL here for retirement headers
                {
                    lobjRemittanceAllocation.CreateGLForDifferenceAmount(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount, lobjEmpHeader.iintPlanID);
                }
                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.Delete();
                if (lintEmployerPayrollHeaderID != lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id)
                {
                    lobjEmpHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                    lobjEmpHeader.icdoEmployerPayrollHeader.Update();
                }
                lintEmployerPayrollHeaderID = lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                lobjEmpHeader = null;
                lobjRemittanceAllocation = null;
            }
        }

        /// <summary>
        /// method to allocated amount for payroll header
        /// </summary>
        /// <param name="aobjHeader">Payroll header object</param>
        /// <param name="aobjHeaderByPlan">payroll header by plan</param>
        /// <param name="adtRemittance">available remittance datatable</param>
        /// <param name="adecTotalDueAmount">due amount</param>
        private void AllocateAmount(busEmployerPayrollHeader aobjHeader, busEmployerPayrollHeader aobjHeaderByPlan, DataTable adtRemittance, decimal adecTotalDueAmount)
        {
            decimal ldecAvailableAmount = 0.00M, ldecDifferenceAmount = 0.00M;
            string lstrDifferenceTypeValue = string.Empty;
            foreach (DataRow ldrRem in adtRemittance.Rows)
            {
                busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                lobjRemittance.icdoRemittance.LoadData(ldrRem);

                ldecDifferenceAmount = 0.00M;
                lstrDifferenceTypeValue = string.Empty;
                //getting available amount for remittance
                ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lobjRemittance.icdoRemittance.remittance_id);
                if (ldecAvailableAmount > 0)
                {
                    decimal ldecAllocatedAmount = ldecAvailableAmount;
                    if (ldecAllocatedAmount > adecTotalDueAmount)
                        ldecAllocatedAmount = adecTotalDueAmount;
					
					//PIR 14938 - commented below condition. updated difference amount and created GL after header status goes to 'Balanced'
                    //if(aobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    //{
                    //if (Math.Abs(ldecAvailableAmount - adecTotalDueAmount) <= idecAllowableVariance)
                    //{
                    //    ldecDifferenceAmount = ldecAvailableAmount - adecTotalDueAmount;
                    //    if (ldecDifferenceAmount > 0)
                    //    {
                    //        lstrDifferenceTypeValue = lobjRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution ?
                    //            busConstant.GLItemTypeOverContribution : busConstant.GLItemTypeOverRHIC;
                    //    }
                    //    else if (ldecDifferenceAmount < 0)
                    //    {
                    //        lstrDifferenceTypeValue = lobjRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution ?
                    //            busConstant.GLItemTypeUnderContribution : busConstant.GLItemTypeUnderRHIC;
                    //    }
                    //}
                    //}
                    //inserting remittance allocation
                    CreateRemittanceAllocation(aobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id, lobjRemittance.icdoRemittance.remittance_id,
                        ldecAllocatedAmount, Math.Abs(ldecDifferenceAmount), lstrDifferenceTypeValue);
                    //if (Math.Abs(ldecDifferenceAmount) > 0)
                    //{
                    //    aobjHeader.CreateGLForDifferenceAmount(lobjRemittance.icdoRemittance.remittance_type_value, ldecDifferenceAmount, aobjHeaderByPlan.iintPlanID);
                    //}
                    adecTotalDueAmount -= (ldecAllocatedAmount + (ldecDifferenceAmount < 0 ? Math.Abs(ldecDifferenceAmount) : 0.00M));
                    if (adecTotalDueAmount == 0.00M)
                        break;
                }

                lobjRemittance = null;
            }
        }

        /// <summary>
        /// method to insert new remittance allocation
        /// </summary>
        /// <param name="aintEmployerPayrollHeaderID">employer payroll header id</param>
        /// <param name="aintRemittanceID">remittance id</param>
        /// <param name="adecAllocatedAmount">amount allocated</param>
        /// <param name="adecDifferenceAmount">difference amount</param>
        /// <param name="astrDifferenceTypeValue">difference type value</param>
        private void CreateRemittanceAllocation(int aintEmployerPayrollHeaderID, int aintRemittanceID, decimal adecAllocatedAmount, 
            decimal adecDifferenceAmount, string astrDifferenceTypeValue)
        {
            cdoEmployerRemittanceAllocation lcdoRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lcdoRemittanceAllocation.employer_payroll_header_id = aintEmployerPayrollHeaderID;
            lcdoRemittanceAllocation.remittance_id = aintRemittanceID;
            lcdoRemittanceAllocation.allocated_amount = adecAllocatedAmount;
            lcdoRemittanceAllocation.payroll_allocation_status_value = busConstant.Allocated;
            lcdoRemittanceAllocation.allocated_date = iobjSystemManagement.icdoSystemManagement.batch_date;
            lcdoRemittanceAllocation.difference_amount = adecDifferenceAmount;
            lcdoRemittanceAllocation.difference_type_value = astrDifferenceTypeValue;
            lcdoRemittanceAllocation.Insert();
        }
    }
}
