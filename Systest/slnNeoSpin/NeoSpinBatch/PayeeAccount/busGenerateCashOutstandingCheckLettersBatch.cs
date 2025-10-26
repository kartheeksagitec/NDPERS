#region Using directives
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace NeoSpinBatch
{
    public class busGenerateCashOutstandingCheckLettersBatch : busNeoSpinBatch
    {
        //property to load the data which are in outstanding status
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDitribution { get; set; }
        public void GenerateCashOutstandingCheckLetters()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog("Generate Cash Outstanding Check Letters Batch Started", "INFO", istrProcessName);
            busBase lobjBase = new busBase();

            //  load the data which are in outstanding status
            DataTable ldtbOutStandingChecks = busBase.Select("cdoPaymentHistoryDistribution.LoadAllOutstatingChecks", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            iclbPaymentHistoryDitribution = new Collection<busPaymentHistoryDistribution>();
            foreach (DataRow ldtrCheck in ldtbOutStandingChecks.Rows)
            {
                busPaymentHistoryDistribution lbusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(ldtrCheck);
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrCheck);
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.LoadData(ldtrCheck);
                lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPaymentSchedule.icdoPaymentSchedule.LoadData(ldtrCheck);
                lbusPaymentHistoryDistribution.ibusRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                lbusPaymentHistoryDistribution.ibusRemittance.icdoRemittance.LoadData(ldtrCheck);

                if (lbusPaymentHistoryDistribution.ibusRemittance.icdoRemittance.remittance_history_header_id > 0)
                {
                    lbusPaymentHistoryDistribution.istrPremiumRefundCheck = busConstant.Flag_Yes;
                }
                int lintNofDays = busGlobalFunctions.DateDiffInDays(lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date,
                                                                                        iobjSystemManagement.icdoSystemManagement.batch_date);
                //PIR 17644 
                if (lintNofDays >= 60 && lintNofDays < 120 && lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.person_id > 0 &&
                   (lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                    lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
                {
                    if (lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund)
                    {
                        lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value = busConstant.Refund;
                    }
                    else
                    {
                        lbusPaymentHistoryDistribution.ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                    }
                }
                //if the check is in outstanding status for more than 180 days generate Cancell payment histroy work flow 
                //if (lintNofDays >= 180)
                //{
                //    if (lbusPaymentHistoryDistribution.ibusOrganization == null)
                //        lbusPaymentHistoryDistribution.LoadOrganization();
                    
                //    InitializeWorkFlow(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.person_id, lbusPaymentHistoryDistribution.ibusOrganization.icdoOrganization.org_code,
                //    lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.Map_Process_Cancel_Payment_History);
                //}
                //if the check is in outstanding status for more than 120 days , Generate ‘Uncashed Benefit Checks’ workflow 
                if (lintNofDays >= 120)
                {
                    if (lbusPaymentHistoryDistribution.ibusOrganization == null)
                        lbusPaymentHistoryDistribution.LoadOrganization();
                    InitializeWorkFlow(lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.person_id, lbusPaymentHistoryDistribution.ibusOrganization.icdoOrganization.org_code,
                      lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_history_header_id, busConstant.Map_Process_Uncashed_Benefit_Checks);
                }//if the check is in outstanding status for more than 90 days ,Generate ‘2nd Notice’ letter requesting members cash any Refund\Roll-over check
                else if (lintNofDays >= 90)
                { //generate 2nd notice letter
                    lbusPaymentHistoryDistribution.istr2ndCashOutstandingNotice = busConstant.Flag_Yes;
                    if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.person_id > 0)
                    {
                        CreateCorrespondence(lbusPaymentHistoryDistribution, "PAY-4021");
                    }
                    else
                    {
                        CreateCorrespondence(lbusPaymentHistoryDistribution, "PAY-4022");
                    }
                }//if the check is in outstanding status for more than 60 days ,Generate ‘1st Notice’ letter requesting members cash any Refund\Roll-over checks if the difference between batch run date 
                else if (lintNofDays >= 60)
                {   //generate 1st notice letter
                    if (lbusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.person_id > 0)
                    {
                        CreateCorrespondence(lbusPaymentHistoryDistribution, "PAY-4021");
                    }
                    else
                    {
                        CreateCorrespondence(lbusPaymentHistoryDistribution, "PAY-4022");
                    }
                }
            }
            idlgUpdateProcessLog("Generate Cash Outstanding Check Letters Batch Ended", "INFO", istrProcessName);
        }
        //Create Correspondence
        private void CreateCorrespondence(busPaymentHistoryDistribution aobjPaymentHistoryDistribution, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPaymentHistoryDistribution);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence(astrCorName, aobjPaymentHistoryDistribution, lhstDummyTable);
        }
        //Initiate Worflow
        private void InitializeWorkFlow(int aintPersonID,string astrOrgCode, int aintPaymentHistoryHeaderID, int aintTypeID)
        {
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            int lintOrgId = 0;
            if(!string.IsNullOrEmpty(astrOrgCode))
                lintOrgId = busGlobalFunctions.GetOrgIdFromOrgCode(astrOrgCode);
            busWorkflowHelper.InitiateBpmRequest(aintTypeID, aintPersonID, lintOrgId, aintPaymentHistoryHeaderID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch);
        }
    }
}