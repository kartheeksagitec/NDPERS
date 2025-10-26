#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Linq;
#endregion

namespace NeoSpinBatch
{
    public class busPostRetirementAccountOwnerDeathBatch : busPostRetirementDeathBatch
    {
        public void PostRetirementAccountOwnerDeathBatch()
        {
            istrStepName = "Post-Retirement Account Owner Death Batch";
            // Get all Records where the Death notification is created for the Member on the Batch run date.
            /***** Post-Retirement Application will be created for deceased Member's Beneficiary whose 
                              Payee Account status other than Receiving, Approved, Review and Suspended. ******/
            DataTable ldtbResults = busBase.Select("cdoBenefitApplication.PostRetirementAccountOwnerDeathBatch",
                                                    new object[1] { DateTime.Now });
            if (ldtbResults.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Processing Post-Retirement Account Owner Death  Batch", "INFO", istrStepName);
                foreach (DataRow ldtr in ldtbResults.Rows)
                {
                    iintActiveSpousePERSLinkID = 0;

                    // Initialize new objects every time.
                    InitializeObjects();

                    ibusMemberPayeeAccount.icdoPayeeAccount.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusPlan.icdoPlan.LoadData(ldtr);
                    ibusMemberPayeeAccount.ibusMember.icdoPerson.LoadData(ldtr);
                    ibusDeathNotification.icdoDeathNotification.LoadData(ldtr);
                    //UAT PIR:2080
                    bool iblnIsTermCertainDatePastDate = false;

                    if (ibusMemberPayeeAccount.icdoPayeeAccount.IsTermCertainBenefitOption())
                    {
                        //UAT PIR:2080                      
                        if (ibusDeathNotification.icdoDeathNotification.date_of_death >= ibusMemberPayeeAccount.icdoPayeeAccount.term_certain_end_date)                            
                        {
                            if(IsMemberAccountBalancelessthanZero(ibusMemberPayeeAccount,ibusDeathNotification.icdoDeathNotification.date_of_death))
                            {
                            /***** No Application will be created if Term certain Benefit option is chooses and the payment is done.******/
                            UpdateFlag(busConstant.PostRetirementAccountOwnerDeath);
                            idlgUpdateProcessLog("No Application Created For Member ID:" +
                                                Convert.ToString(ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id) +
                                                ", since the Member deceased after Payment Term certain end date.", "INFO", istrStepName);
                            continue;
                            }
                            else
                            {
                                iblnIsTermCertainDatePastDate =true;
                            }
                        }
                    }
                    if (IsMemberEligibleForPostRetirementDeathApplication(iblnIsTermCertainDatePastDate))
                    {
                        busPersonAccount lobjRTWPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        if (icdoPostRetirementDeathBenfitOptionRef.is_monthly_benefit_flag == busConstant.Flag_Yes)
                        {
                            /***** Specific RTW Member Case.******/
                            if (ibusMemberPayeeAccount.ibusMember.IsRTWMember(ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id,
                                busConstant.PayeeStatusForRTW.IgnoreStatus, ref iintRTWPayeeAccountID))
                            {
                                if (ibusMemberPayeeAccount.ibusMember.iclbRetirementAccount.IsNull())
                                    ibusMemberPayeeAccount.ibusMember.LoadRetirementAccount();

                                //Since we are allowing any status for an RTW Member we are checking whether the person account other than withdrawn is > 1.
                                var iclbfilteredPlan = ibusMemberPayeeAccount.ibusMember.iclbRetirementAccount.Where(
                                    o => o.icdoPersonAccount.plan_id == ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id &&
                                    o.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn);

                                if (iclbfilteredPlan.Count() > 1)
                                {
                                    lobjRTWPersonAccount = ibusMemberPayeeAccount.ibusMember.LoadActivePersonAccountByPlan(
                                        ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id);
                                    lobjRTWPersonAccount.LoadTotalPSC();
                                    if (lobjRTWPersonAccount.icdoPersonAccount.Total_PSC >= 24)
                                        istrIsLessThan2Yrs = busConstant.Flag_Yes;
                                    else
                                        istrIsLessThan2Yrs = busConstant.Flag_No;
                                }
                            }
                        }
                        if (ibusMemberPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan.IsNull())
                            ibusMemberPayeeAccount.ibusMember.LoadActiveBeneForGivenPlan(
                                                ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id,
                                                ibusDeathNotification.icdoDeathNotification.date_of_death);
                        if (icdoPostRetirementDeathBenfitOptionRef.account_relation_value == busConstant.AccountRelationshipJointAnnuitant)
                        {
                            if (iintActiveSpousePERSLinkID > 0)
                            {

                                var lvarFiltered = ibusMemberPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan.Where(
                                    o => o.icdoPersonBeneficiary.beneficiary_person_id == iintActiveSpousePERSLinkID);

                                if (lvarFiltered.Count() > 0)
                                {
                                    /***** Create Post-Retirement Application.******/
                                    int lintBenefitApplicationID = CreatePSTDApplication(
                                                        iintActiveSpousePERSLinkID, 0, busConstant.FamilyRelationshipSpouse,
                                                        ibusPersonAccountRetirement.icdoPersonAccount.person_account_id,
                                                        ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                        busConstant.PostRetirementAccountOwnerDeath,
                                                        busConstant.AccountRelationshipJointAnnuitant, 0);
                                    if (lintBenefitApplicationID != 0)
                                    {
                                        /***** Insert Application Person Account for RTW Person Account.******/
                                        if (lobjRTWPersonAccount.icdoPersonAccount.person_account_id != 0)
                                        {
                                            cdoBenefitApplicationPersonAccount lcdoBenAppPersonAccount = new cdoBenefitApplicationPersonAccount
                                            {
                                                benefit_application_id = lintBenefitApplicationID,
                                                person_account_id = lobjRTWPersonAccount.icdoPersonAccount.person_account_id,
                                                is_person_account_selected_flag = busConstant.Flag_Yes,
                                                is_application_person_account_flag = busConstant.Flag_No
                                            };
                                            lcdoBenAppPersonAccount.Insert();
                                        }

                                        /***** Initialize Post-Retirement Death Account Owner Death Workflow.******/
                                        InitializeWorkFlow(busConstant.Map_PSTD_Account_Owner_Application, ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id,
                                                lintBenefitApplicationID, busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                                    }
                                }
                                else
                                {
                                    idlgUpdateProcessLog("No Application Created for PERSLinkID : " + Convert.ToString(iintActiveSpousePERSLinkID) +
                                                                ", Since he/she is not a Active Spouse", "INFO", istrStepName);
                                }
                            }
                        }
                        else
                        {
                            foreach (busPersonBeneficiary lobjBeneficiary in ibusMemberPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan)
                            {
                                if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                                {
                                    /***** Create Post-Retirement Application.******/
                                    int lintApplicationID = CreatePSTDApplication(0,
                                                    lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id,
                                                    lobjBeneficiary.icdoPersonBeneficiary.relationship_value,
                                                    ibusPersonAccountRetirement.icdoPersonAccount.person_account_id,
                                                    ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                    busConstant.PostRetirementAccountOwnerDeath,
                                                    busConstant.AccountRelationshipBeneficiary, 0);

                                    /***** Initialize Post-Retirement Death Account Owner Death Workflow.******/
                                    if (lintApplicationID != 0)
                                        InitializeWorkFlow(busConstant.Map_PSTD_Account_Owner_Application, ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id,
                                                        lintApplicationID, busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                                }
                                else if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                                {
                                    lobjBeneficiary.ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
                                    lobjBeneficiary.ibusBeneficiaryPerson.FindPerson(lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);

                                    if (lobjBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_death == DateTime.MinValue)
                                    {
                                        /***** Create Post-Retirement Application.******/
                                        int lintApplicationID = CreatePSTDApplication(
                                                    lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, 0,
                                                    lobjBeneficiary.icdoPersonBeneficiary.relationship_value,
                                                    ibusPersonAccountRetirement.icdoPersonAccount.person_account_id,
                                                    ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                    busConstant.PostRetirementAccountOwnerDeath,
                                                    busConstant.AccountRelationshipBeneficiary, lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id); //UAT PIR: 1244

                                        /***** Initialize Post-Retirement Death Account Owner Death Workflow.******/
                                        if (lintApplicationID != 0)
                                            InitializeWorkFlow(busConstant.Map_PSTD_Account_Owner_Application,
                                                        ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id, lintApplicationID,
                                                        busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                                    }
                                    else
                                    {
                                        idlgUpdateProcessLog("No Application Created for PERSLinkID : " +
                                                    Convert.ToString(lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id) + ", Since he was deceased.",
                                                    "INFO", istrStepName);
                                    }
                                }
                            }
                        }
                    }
                    idlgUpdateProcessLog("Checking whether the deceased and the alternate payee have not received any payments: " , "INFO", istrStepName);
                    InitializeWorkflow(ibusDeathNotification.icdoDeathNotification.person_id);
                    idlgUpdateProcessLog("Checking whether the deceased and the alternate payee have not received any payments:Completed ", "INFO", istrStepName);
                    UpdateFlag(busConstant.PostRetirementAccountOwnerDeath);
                }
            }
            else
                idlgUpdateProcessLog("No Records Found", "INFO", istrStepName);
        }

        /// Returns true for all Members, except if the Member Account Balance is Zero.
        /// Loads the Post-Retirement Death Benefit Option Reference.
        private bool IsMemberEligibleForPostRetirementDeathApplication(bool iblnIsTermCertainDatePastDeathDate)
        {
            /// Loads the Post-Retirement Death Benefit Option Reference.
            LoadPSTDBenefitOptionRef(busConstant.PostRetirementAccountOwnerDeath,iblnIsTermCertainDatePastDeathDate);

            if (icdoPostRetirementDeathBenfitOptionRef.post_retirement_death_benefit_option_ref_id != 0)
            {
                if (ibusMemberPayeeAccount.ibusApplication.ibusPersonAccount.IsNull())
                    ibusMemberPayeeAccount.ibusApplication.LoadPersonAccountByApplication();
                ibusPersonAccountRetirement.icdoPersonAccount = ibusMemberPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount;
                ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccountRetirement.icdoPersonAccount.person_account_id);

                /***** 2. For Refund, if the Member account Balance is equals zero no need to Create any applications. ******/
                bool lblnMinimumGuaranteeAmtValid = true;
                if (icdoPostRetirementDeathBenfitOptionRef.is_monthly_benefit_flag == busConstant.Flag_No)
                {
                    decimal ldecMemberAccountBalance = 0M;
                    ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ibusDeathNotification.icdoDeathNotification.date_of_death, 
                                                                        ibusMemberPayeeAccount.icdoPayeeAccount.benefit_account_type_value);
                    ldecMemberAccountBalance = ibusPersonAccountRetirement.Member_Account_Balance_ltd;
                    ibusMemberPayeeAccount.LoadPaymentDetails();
                    //UAT PIR: 1224
                    //For Disability since the Minimum Guarantee is not stored, it is calculated as on date of death so that the interest amount is added.
                    //Do not reduce the Already paid amount to the account owner.
                    if (ibusMemberPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        if ((ldecMemberAccountBalance) <= 0M)
                            lblnMinimumGuaranteeAmtValid = false;
                    }
                    else
                    {
                        if ((ibusMemberPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ibusMemberPayeeAccount.idecpaidgrossamount) <= 0M)
                            lblnMinimumGuaranteeAmtValid = false;
                    }
                }

                /***** 3. Load the corresponding Reference Values from DB Cache. ******/
                if (lblnMinimumGuaranteeAmtValid)
                {
                    if ((icdoPostRetirementDeathBenfitOptionRef.account_relation_value == "JANT") &&
                        (icdoPostRetirementDeathBenfitOptionRef.is_spouse_at_death == busConstant.Flag_No))
                    {
                        // Joint annuitant at death has to be provided the benefits
                        if (ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id > 0)
                        {
                            if (ibusMemberPayeeAccount.ibusApplication.ibusJointAnniutantPerson.IsNull())
                                ibusMemberPayeeAccount.ibusApplication.LoadJointAnniutantPerson();
                            if (ibusMemberPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.date_of_death == DateTime.MinValue)
                            {
                                iintActiveSpousePERSLinkID = ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id;
                                return true;
                            }
                            else
                            {
                                idlgUpdateProcessLog("No Application Created for PERSLinkID : " +
                                                        Convert.ToString(ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id)
                                                        + ", Since he was deceased.", "INFO", istrStepName);
                                return false;
                            }
                        }
                    }
                    else if ((icdoPostRetirementDeathBenfitOptionRef.account_relation_value == "JANT") &&
                        (icdoPostRetirementDeathBenfitOptionRef.is_spouse_at_death == busConstant.Flag_Yes))
                    {
                        // Joint Annuitant at death has to be provided the benefits                        
                        LoadJointAnnuitant();
                        if (ibusJointAnnuitant.icdoPerson.date_of_death == DateTime.MinValue)
                        {
                            iintActiveSpousePERSLinkID = ibusJointAnnuitant.icdoPerson.person_id;
                            return true;
                        }
                        else
                        {
                            idlgUpdateProcessLog("No Application Created for PERSLinkID : " +
                                        Convert.ToString(ibusJointAnnuitant.icdoPerson.person_id) + ", Since he was deceased.", "INFO", istrStepName);
                            return false;
                        }
                    }
                    else
                        return true;
                }
            }
            return false;
        }

        //UCS - 054
        // BR- 54-48
        //initiate the Alternate Payee’s benefit payments 
        //if neither Member nor Alternate Payee have been issued their first payment
        //Active Job Service DRO Model 
        private void InitializeWorkflow(int aintPersonId)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintPersonId))
            {
                if (lobjPerson.iclbDROApplication.IsNull())
                    lobjPerson.LoadDROApplications();

                var lenumQualifiedDROApplications = lobjPerson.iclbDROApplication
                                                                              .Where(lobjDROApp => lobjDROApp.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified
                                                                                    && lobjDROApp.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel);

                foreach (busBenefitDroApplication lobjDROApplication in lenumQualifiedDROApplications)
                {
                    if (lobjDROApplication.iclbPayeeAccount.IsNull())
                        lobjDROApplication.LoadPayeeAccount();

                    var lintCountOfPayeeAccount = lobjDROApplication.iclbPayeeAccount.Count;

                    if (lintCountOfPayeeAccount == 0)
                    {
                        if (lobjDROApplication.ibusAlternatePayee.IsNull())
                            lobjDROApplication.LoadAlternatePayee();

                        if (lobjDROApplication.ibusAlternatePayee.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            idlgUpdateProcessLog("Initialize Workflow for PERSLinkID : " +
                                        Convert.ToString(aintPersonId) + ", Since he was deceased.", "INFO", istrStepName);

                            InitializeWorkFlow(busConstant.Map_Process_QDRO_Calculation,
                                aintPersonId, lobjDROApplication.icdoBenefitDroApplication.dro_application_id,
                                busConstant.WorkflowProcessStatus_UnProcessed, busConstant.WorkflowProcessSource_Batch);
                            break;
                        }
                    }
                }
            }
        }
    }
}
