#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    public class busPostRetirementAlternatePayeeDeathBatch : busPostRetirementDeathBatch
    {
        public void ProcesPostRetirementAlternatePayeeDeath()
        {
            istrStepName = "Post-Retirement Alternate Payee Death Batch";
            // Get all Records where the Death notification is created for the Member on the Batch run date.
            DataTable ldtbResults = busBase.Select("cdoBenefitApplication.PostRetirementAlternatePayeeDeathBatch",
                                                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            if (ldtbResults.Rows.Count > 0)
            {
                foreach (DataRow ldtr in ldtbResults.Rows)
                {
                    // Initialize new objects everytime.
                    InitializeObjects();

                    ibusMemberPayeeAccount.icdoPayeeAccount.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.LoadData(ldtr);                       
                    ibusMemberPayeeAccount.ibusPlan.icdoPlan.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusMember.icdoPerson.LoadData(ldtr);
                    ibusDeathNotification.icdoDeathNotification.LoadData(ldtr);
                    ibusFirstBeneficiary.icdoPersonBeneficiary.LoadData(ldtr);

                    //UAT PIR:2080
                    bool iblnIsTermCertainDatePastDate = false;

                    // Payee Account status other than Receiving, Approved, Review and Suspended.
                    if (ibusMemberPayeeAccount.icdoPayeeAccount.IsTermCertainBenefitOption())
                    {
						//UAT PIR:2080 Fix.
                        if (ibusDeathNotification.icdoDeathNotification.date_of_death >= ibusMemberPayeeAccount.icdoPayeeAccount.term_certain_end_date)                            
                        {
                            if (IsMemberAccountBalancelessthanZero(ibusMemberPayeeAccount, ibusDeathNotification.icdoDeathNotification.date_of_death))
                            {
                                UpdateFlag(busConstant.PostRetirementAlternatePayeeDeath);
                                idlgUpdateProcessLog("No Application Created For Member ID:" +
                                                    Convert.ToString(ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id) +
                                                    ", since the Member deceased after Payment Term certain end date.", "INFO", istrStepName);
                                continue;
                            }
                            else
                            {
                                iblnIsTermCertainDatePastDate = true;
                            }
                        }
                    }
                    /// Loads the Post-Retirement Death Benefit Option Reference.
                    LoadPSTDBenefitOptionRef(busConstant.PostRetirementAlternatePayeeDeath,iblnIsTermCertainDatePastDate);

                    if (icdoPostRetirementDeathBenfitOptionRef.post_retirement_death_benefit_option_ref_id != 0)
                    {
                        //if (ibusMemberPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan.IsNull())

                        //    ibusMemberPayeeAccount.LoadDROApplication();
    
                        //    ibusMemberPayeeAccount.ibusMember.LoadActiveBeneForGivenPlan(
                        //                        ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id,
                        //                        ibusDeathNotification.icdoDeathNotification.date_of_death);

                        //foreach (busPersonBeneficiary lobjBeneficiary in ibusMemberPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan)
                        //{


                        if (ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                            {
                                /***** Create Post-Retirement Application.******/
                                int lintApplicationID = CreatePSTDApplication(0,
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id,
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.relationship_value,
                                                ibusMemberPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.person_account_id,
                                                ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.PostRetirementAlternatePayeeDeath,
                                                busConstant.AccountRelationshipBeneficiary, ibusMemberPayeeAccount.icdoPayeeAccount.payee_perslink_id);

                                /***** Initialize Post-Retirement Death Alternate Payee Death Workflow.******/
                                if (lintApplicationID != 0)
                                    InitializeWorkFlow(busConstant.Map_PSTD_Alternate_Payee_Application, ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id,
                                                        lintApplicationID, busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                            }
                        else if (ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                            {
                                ibusFirstBeneficiary.ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
                                ibusFirstBeneficiary.ibusBeneficiaryPerson.FindPerson(ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);

                                if (ibusFirstBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_death == DateTime.MinValue)
                                {
                                    /***** Create Post-Retirement Application.******/
                                    int lintApplicationID = CreatePSTDApplication(
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, 0,
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.relationship_value,
                                                ibusMemberPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.person_account_id,
                                                ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id, busConstant.PostRetirementAlternatePayeeDeath,
                                                busConstant.AccountRelationshipBeneficiary, ibusMemberPayeeAccount.icdoPayeeAccount.payee_perslink_id);

                                    /***** Initialize Post-Retirement Alternate Payee Death Workflow.******/
                                    if (lintApplicationID != 0)
                                        InitializeWorkFlow(busConstant.Map_PSTD_Alternate_Payee_Application,
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, lintApplicationID,
                                                busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                                }
                                else
                                {
                                    idlgUpdateProcessLog("No Application Created for PERSLinkID : " +
                                                Convert.ToString(ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_person_id) + ", Since he was deceased.",
                                                "INFO", istrStepName);
                                }
                            }
                        //}
                    }
                    
                    UpdateFlag(busConstant.PostRetirementAlternatePayeeDeath);
                }
            }
            else
                idlgUpdateProcessLog("No Records Found", "INFO", istrStepName);
        }
    }
}
