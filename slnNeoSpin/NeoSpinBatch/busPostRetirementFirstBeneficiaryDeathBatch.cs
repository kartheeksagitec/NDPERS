#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpinBatch
{
    public class busPostRetirementFirstBeneficiaryDeathBatch : busPostRetirementDeathBatch
    {
        public void PostRetirementFirstBeneficiaryDeathBatch()
        {
            istrStepName = "Post-Retirement First Beneficiary Death Batch";
            // Get all Records where the Death notification is created for the Member on the Batch run date.
            DataTable ldtbResults = busBase.Select("cdoBenefitApplication.PostRetirementFirstBeneficiaryDeathBatch",
                                                    new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            if (ldtbResults.Rows.Count > 0)
            {
                foreach (DataRow ldtr in ldtbResults.Rows)
                {
                    iintActiveSpousePERSLinkID = 0;

                    InitializeObjects();

                    ibusFirstBeneficiary.icdoPersonBeneficiary.LoadData(ldtr);
                    ibusMemberPayeeAccount.icdoPayeeAccount.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusPlan.icdoPlan.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusMember.icdoPerson.LoadData(ldtr);
                    ibusDeathNotification.icdoDeathNotification.LoadData(ldtr);

                    //This one is a Temporary fix since the load method is loading the account owner. To Revisit again.
                    ibusDeathNotification.FindDeathNotification(ibusDeathNotification.icdoDeathNotification.death_notification_id);  
                    // Payee Account status other than Receiving, Approved, Review and Suspended.
                    if (ibusMemberPayeeAccount.icdoPayeeAccount.IsTermCertainBenefitOption())
                    {
						//UAT PIR:2080 Fix.
                        if ((ibusDeathNotification.icdoDeathNotification.date_of_death >= ibusMemberPayeeAccount.icdoPayeeAccount.term_certain_end_date)
                            && IsMemberAccountBalancelessthanZero(ibusMemberPayeeAccount,ibusDeathNotification.icdoDeathNotification.date_of_death))
                        {
                            UpdateFlag(busConstant.PostRetirementFirstBeneficiaryDeath);
                            idlgUpdateProcessLog("No Application Created For Member ID:" + 
                                                Convert.ToString(ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id) + 
                                                ", since the Paid gross amount exceeds Member Account balance", "INFO", istrStepName);
                            continue;
                        }
                    }
                    if (IsMemberEligibleForPostRetirementDeathApplication())
                    {
                        if (ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                        {
                            /***** Create Post-Retirement Application.******/
                            int lintApplicationID = CreatePSTDApplication(0, ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id,
                                            ibusFirstBeneficiary.icdoPersonBeneficiary.relationship_value,
                                            ibusPersonAccountRetirement.icdoPersonAccount.person_account_id,
                                            ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                            busConstant.PostRetirementFirstBeneficiaryDeath,
                                            busConstant.AccountRelationshipBeneficiary,ibusMemberPayeeAccount.icdoPayeeAccount.payee_perslink_id);

                            /***** Initialize Post-Retirement First Benficiary Death Workflow.******/
                            if (lintApplicationID != 0)
                                InitializeWorkFlow(busConstant.Map_PSTD_First_Beneficiary_Application, 0,
                                                lintApplicationID, busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch, 
                                                ibusFirstBeneficiary.icdoPersonBeneficiary.benificiary_org_id);
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
                                            ibusPersonAccountRetirement.icdoPersonAccount.person_account_id,
                                            ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                            busConstant.PostRetirementFirstBeneficiaryDeath,
                                            busConstant.AccountRelationshipBeneficiary, ibusMemberPayeeAccount.icdoPayeeAccount.payee_perslink_id);

                                /***** Initialize Post-Retirement First Benficiary Death Workflow.******/
                                if (lintApplicationID != 0)
                                    InitializeWorkFlow(busConstant.Map_PSTD_First_Beneficiary_Application,
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
                    }
                    else //Else case when member is not eligle
                    {
                        if (!string.IsNullOrEmpty(ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_name))
                        {
                            idlgUpdateProcessLog("No Application Created For Applicant :" +
                                                    ibusFirstBeneficiary.icdoPersonBeneficiary.beneficiary_name + 
                                                    ", since the Applicant is not eligible to receive First Beneficiary Death benefits.", "INFO", istrStepName);
                        }
                    }
                    UpdateFlag(busConstant.PostRetirementFirstBeneficiaryDeath);
                }
            }
            else
                idlgUpdateProcessLog("No Records Found", "INFO", istrStepName);
        }

        /// Returns true for all Members, except if the Member Account Balance is Zero.
        /// Loads the Post-Retirement Death Benefit Option Reference.
        private bool IsMemberEligibleForPostRetirementDeathApplication()
        {
            /// Loads the Post-Retirement Death Benefit Option Reference.
            LoadPSTDBenefitOptionRef(busConstant.PostRetirementFirstBeneficiaryDeath,false);

            if (icdoPostRetirementDeathBenfitOptionRef.post_retirement_death_benefit_option_ref_id != 0)
            {
                if (ibusMemberPayeeAccount.ibusApplication.ibusPersonAccount.IsNull())
                    ibusMemberPayeeAccount.ibusApplication.LoadPersonAccountByApplication();
                ibusPersonAccountRetirement.icdoPersonAccount = ibusMemberPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount;
                ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccountRetirement.icdoPersonAccount.person_account_id);

                /***** For Refund, if the Member account Balance is equals zero no need to Create any applications. ******/
                if (icdoPostRetirementDeathBenfitOptionRef.is_monthly_benefit_flag == busConstant.Flag_No)
                {
                    //decimal ldecMemberAccountBalance = 0M;
                    //ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ibusDeathNotification.icdoDeathNotification.date_of_death);
                    //ldecMemberAccountBalance = ibusPersonAccountRetirement.Total_Account_Balance_ltd;
                    ibusMemberPayeeAccount.LoadPaymentDetails();
                    if (ibusMemberPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ibusMemberPayeeAccount.idecpaidgrossamount <= 0M)
                    {
                        idlgUpdateProcessLog("No Application Created for PERSLinkID : " +
                                    Convert.ToString(ibusPersonAccountRetirement.icdoPersonAccount.person_id)
                                    + ", Since Member Account Balance is 0", "INFO", istrStepName);
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }
    }
}
