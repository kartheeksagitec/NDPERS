using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Collections;
using Sagitec.DBUtility;

namespace NeoSpinBatch
{
    class busUpdateRHICRateChangeBatch : busNeoSpinBatch
    {
        decimal idecCurrentBenefitFactor;
        decimal idecNewBenefitFactor;
        DateTime idtCurrentEffectiveDate;
        DateTime idtNewEffectiveDate;
        public busUpdateRHICRateChangeBatch()
        { }

        public void UpdateRHICRateChange()
        {
            istrProcessName = "Update RHIC Rate Change to Payee Account";
            idlgUpdateProcessLog("Update RHIC Rate Change Batch Started", "INFO", istrProcessName);

            //Get the Old and New RHIC Factor
            idlgUpdateProcessLog("Loading Current and New RHIC Factors", "INFO", istrProcessName);
            LoadCurrentAndNewRHICBenefitFactor();

            if (idecCurrentBenefitFactor == 0.00M)
            {
                //This check also avoids division by zero excepion.
                idlgUpdateProcessLog("There is no Factor Change!", "ERR", istrProcessName);
                return;
            }
            //Load All Active Payee Account
            idlgUpdateProcessLog("Updating All Active Payee Accounts", "INFO", istrProcessName);
            DataTable ldtbPayeeAccountList = busNeoSpinBase.Select("cdoPayeeAccount.LoadAllPayeeAccountNotInCompCancForRhicFactorChange", new object[0] { });
            foreach (DataRow ldrRow in ldtbPayeeAccountList.Rows)
            {
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.icdoPayeeAccount.LoadData(ldrRow);

                lbusPayeeAccount.ibusPayee = new busPerson { icdoPerson = new cdoPerson() };
                lbusPayeeAccount.ibusPayee.icdoPerson.LoadData(ldrRow);

                //Update the Rhic Amount with latest Factor Change                
                if (lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                {
                    lbusPayeeAccount.icdoPayeeAccount.rhic_amount = busGlobalFunctions.RoundToPenny(((lbusPayeeAccount.icdoPayeeAccount.rhic_amount * idecNewBenefitFactor) / idecCurrentBenefitFactor));
                    lbusPayeeAccount.icdoPayeeAccount.Update();
                }
            }


            //Load All Active Payee Account
            idlgUpdateProcessLog("Updating All Benefit Accounts", "INFO", istrProcessName);
            DataTable ldtbBenefitAccountList = busNeoSpinBase.Select("cdoPayeeAccount.LoadAllBenefitAccountForRhicFactorChange", new object[0] { });
            foreach (DataRow ldrRow in ldtbBenefitAccountList.Rows)
            {
                busBenefitAccount lbusBenefitAccount = new busBenefitAccount { icdoBenefitAccount = new cdoBenefitAccount() };
                lbusBenefitAccount.icdoBenefitAccount.LoadData(ldrRow);

                bool lblnUpdateBenefitAccount = false;
                if (lbusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0)
                {
                    lbusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount = busGlobalFunctions.RoundToPenny(((lbusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount * idecNewBenefitFactor) / idecCurrentBenefitFactor));
                    lblnUpdateBenefitAccount = true;
                }

                if (lbusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount > 0)
                {
                    lbusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount = busGlobalFunctions.RoundToPenny(((lbusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount * idecNewBenefitFactor) / idecCurrentBenefitFactor));
                    lblnUpdateBenefitAccount = true;
                }

                if (lblnUpdateBenefitAccount)
                    lbusBenefitAccount.icdoBenefitAccount.Update();
            }
            //PIR 14346 - Commented this block
            //bool lblnDoEnrollmentAdjustment = false;
            //DateTime ldtLastPostedIBSDate = busIBSHelper.GetLastPostedIBSBatchDate();
            //if (ldtLastPostedIBSDate >= idtNewEffectiveDate)
            //    lblnDoEnrollmentAdjustment = true;

            //idlgUpdateProcessLog("Creating New RHIC Combine for all Valid/Approved Records", "INFO", istrProcessName);
            ////Create New Rhic Combine Records for all Valid/Approved Records.
            //DataTable ldtbRhicList = busNeoSpinBase.Select<cdoBenefitRhicCombine>(
            //                  new string[2] { "status_value", "action_status_value" },
            //                  new object[2] { busConstant.RHICStatusValid, busConstant.RHICActionStatusApproved }, null, null);

            //foreach (DataRow ldrRow in ldtbRhicList.Rows)
            //{
            //    busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
            //    lbusBenefitRhicCombine.icdoBenefitRhicCombine.LoadData(ldrRow);

            //    idlgUpdateProcessLog("Processing Person ID " + lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id, "INFO", istrProcessName);
            //    busBenefitRhicCombine lbusBenefitNewRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
            //    lbusBenefitNewRhicCombine.icdoBenefitRhicCombine.start_date = idtNewEffectiveDate;
            //    lbusBenefitNewRhicCombine.icdoBenefitRhicCombine.person_id = lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
            //    lbusBenefitNewRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.rhic_factor_change;
            //    if (lbusBenefitNewRhicCombine.CreateAutomaticRHICCombine())
            //    {
            //        if (lblnDoEnrollmentAdjustment)
            //        {
            //            lbusBenefitNewRhicCombine.CreatePayrollAdjustment();
            //            //property to post the batch schedule ID into PAPIT if called from Update RHIC Rate Change batch
            //            lbusBenefitNewRhicCombine.iintBatchScheduleId = busConstant.BatchScheduleIDUpdateRHICRateChange;
            //            lbusBenefitNewRhicCombine.CreatePAPITAdjustment();
            //        }
            //    }
            //}

            idlgUpdateProcessLog("Update RHIC Rate Change Batch Ended", "INFO", istrProcessName);
        }

        private void LoadCurrentAndNewRHICBenefitFactor()
        {
            DataTable ldtbBenefitProvisionType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_benefit_provision_benefit_type", null);

            var lenumNewList = ldtbBenefitProvisionType.AsEnumerable()
                                           .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

            if (lenumNewList != null && lenumNewList.Count() > 0)
            {
                idtNewEffectiveDate = Convert.ToDateTime(lenumNewList.AsDataTable().Rows[0]["effective_date"]);
                idecNewBenefitFactor = lenumNewList.FirstOrDefault().Field<decimal>("RHIC_SERVICE_FACTOR");
            }

            if (idtNewEffectiveDate != DateTime.MinValue)
            {
                var lenumCurrentList = ldtbBenefitProvisionType.AsEnumerable()
                                                     .Where(ldr => ldr.Field<DateTime>("effective_date") <= idtNewEffectiveDate.AddMonths(-1))
                                                     .OrderByDescending(ldr => ldr.Field<DateTime>("effective_date"));

                if (lenumCurrentList != null && lenumCurrentList.Count() > 0)
                {
                    idtCurrentEffectiveDate = Convert.ToDateTime(lenumCurrentList.AsDataTable().Rows[0]["effective_date"]);
                    idecCurrentBenefitFactor = lenumCurrentList.FirstOrDefault().Field<decimal>("RHIC_SERVICE_FACTOR");
                }
            }
        }
    }
}
