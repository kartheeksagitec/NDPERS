using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using Sagitec.Bpm;

namespace NeoSpinBatch
{
    class busACHPullAutomation : busNeoSpinBatch
    {
        public void CreateACHPull(string astrDepositTapeBenType = "", bool ablnIsIBSPull = false)
        {
            string astrBenefitTypeDescription = GetBenTypeDescription(astrDepositTapeBenType);
            istrProcessName = $"ACH Pull for {astrBenefitTypeDescription}";
            DataTable ldtbPendingACHRequests = new DataTable();
            if (!ablnIsIBSPull)
            {
                idlgUpdateProcessLog($"Loading All Pending Debit ACH Requests for {astrBenefitTypeDescription} Headers.", "INFO", istrProcessName);
                ldtbPendingACHRequests = busNeoSpinBase.Select("entEmployerPayrollHeader.LoadPendingDebitACHRequest", new object[1] { astrDepositTapeBenType });
            }
            else
                idlgUpdateProcessLog($"Starting ACH Pull for {astrBenefitTypeDescription}.", "INFO", istrProcessName);
            if ((!ablnIsIBSPull && ldtbPendingACHRequests.Rows.Count > 0) || ablnIsIBSPull)
            {
                busDepositTape lobjDepositTape = new busDepositTape { icdoDepositTape = new cdoDepositTape() };
                if (ablnIsIBSPull)
                {
                    astrDepositTapeBenType = busConstant.PayrollHeaderBenefitTypeInsr;
                }
                else
                    lobjDepositTape.istrHeaderType = astrDepositTapeBenType;
                try
                {
                    if (!utlPassInfo.iobjPassInfo.iblnInTransaction)
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                    lobjDepositTape.CreateDepositTape(astrDepositTapeBenType);
                    if (!ablnIsIBSPull)
                    {
                        switch (astrDepositTapeBenType)
                        {
                            case busConstant.PayrollHeaderBenefitTypeInsr:
                                lobjDepositTape.btnACHPullInsurance_Click();
                                break;
                            case busConstant.PayrollHeaderBenefitTypeRtmt:
                                lobjDepositTape.btnACHPullRetirement_Click();
                                break;
                            case busConstant.PayrollHeaderBenefitTypeDefComp:
                                lobjDepositTape.btnACHPullDefComp_Click();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        lobjDepositTape.btnACHPull_Click();
                    }
                    ArrayList larrError = lobjDepositTape.btnApply_Click();
                    if (larrError.Count == 0)
                    {
                        if (!ablnIsIBSPull)
                        {
                            int aintProcessId = LoadProcessIdToBeInitiated(astrDepositTapeBenType);
                            if (aintProcessId > 0)
                            {
                                busBpmRequest lbusBpmRequest = lobjDepositTape.InitiateWorkflowForDebitACHRequest(aintProcessId);
                                if (lbusBpmRequest.IsNull())
                                {
                                    idlgUpdateProcessLog("BPM Request Cannot Be Raised as there is already a running instance for the same process and organization.", "ERR", istrProcessName);
                                }
                                else
                                    idlgUpdateProcessLog($"BPM Request Successfully Raised For {astrBenefitTypeDescription} ACH Pull.", "INFO", istrProcessName);
                            }
                        }
                    }
                    else
                        idlgUpdateProcessLog("The status cannot be changed to Applied as the 'Total Deposit Amount' does not match with 'Total Remittance Amount.'", "INFO", istrProcessName);
                    if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                        utlPassInfo.iobjPassInfo.Commit();
                    idlgUpdateProcessLog($"ACH File Successfully Generated for {astrBenefitTypeDescription}.", "INFO", istrProcessName);
                }
                catch (Exception e)
                {
                    if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                        utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog($"Error occurred While Creating Outbound File for ACH Pull for {astrBenefitTypeDescription}: " + e.Message, "ERR", istrProcessName);
                }
                if (!ablnIsIBSPull)
                {
                    try
                    {
                        idlgUpdateProcessLog($"Remittance Allocation Started For {astrBenefitTypeDescription}.", "INFO", istrProcessName);
                        if (!utlPassInfo.iobjPassInfo.iblnInTransaction)
                            utlPassInfo.iobjPassInfo.BeginTransaction();
                        lobjDepositTape.AllocateRemittanceToEmployerHeader(iobjBatchSchedule.batch_schedule_id);
                        if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                            utlPassInfo.iobjPassInfo.Commit();
                        idlgUpdateProcessLog($"Remittance Allocation Ended For {astrBenefitTypeDescription}.", "INFO", istrProcessName);
                    }
                    catch (Exception ex)
                    {
                        if (utlPassInfo.iobjPassInfo.iblnInTransaction)
                            utlPassInfo.iobjPassInfo.Rollback();
                        idlgUpdateProcessLog("Error occurred While allocating remittances For {astrBenefitTypeDescription} headers post {astrBenefitTypeDescription} ACH Pull." + ex.Message, "ERR", istrProcessName);
                    }
                }
                idlgUpdateProcessLog($"ACH pull process Successfully Completed for {astrBenefitTypeDescription}.", "INFO", istrProcessName);
            }
            else if (!ablnIsIBSPull)
            {
                idlgUpdateProcessLog($"No Pending Debit Requests exist for {astrBenefitTypeDescription} Headers.", "INFO", istrProcessName);
            }
        }

        private int LoadProcessIdToBeInitiated(string astrDepositTapeBenType)
        {
            int lintProcessId = 0;
            switch (astrDepositTapeBenType)
            {
                case busConstant.PayrollHeaderBenefitTypeInsr:
                    lintProcessId = busConstant.Map_ACH_Pull_For_Insurance;
                    break;
                case busConstant.PayrollHeaderBenefitTypeRtmt:
                    lintProcessId = busConstant.Map_ACH_Pull_For_Retirement;
                    break;
                case busConstant.PayrollHeaderBenefitTypeDefComp:
                    lintProcessId = busConstant.Map_ACH_Pull_For_DeferredCompensation;
                    break;
                default:
                    break;
            }
            return lintProcessId;
        }

        private string GetBenTypeDescription(string astrDepositTapeBenType)
        {
            string lstrBenTypeDescription = string.Empty;
            switch (astrDepositTapeBenType)
            {
                case busConstant.PayrollHeaderBenefitTypeInsr:
                    lstrBenTypeDescription = "Insurance";
                    break;
                case busConstant.PayrollHeaderBenefitTypeRtmt:
                    lstrBenTypeDescription = "Retirement";
                    break;
                case busConstant.PayrollHeaderBenefitTypeDefComp:
                    lstrBenTypeDescription = "Deferred Compensation";
                    break;
                default:
                    lstrBenTypeDescription = "IBS Insurance";
                    break;
            }
            return lstrBenTypeDescription;
        }
    }
}
