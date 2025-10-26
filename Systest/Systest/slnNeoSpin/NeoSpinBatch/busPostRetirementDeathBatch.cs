#region Using directives
using System;
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Linq;
using System.Collections;
#endregion

namespace NeoSpinBatch
{
    /// Objects of this Class are initialized everytime in the Corresponding Batches
    public class busPostRetirementDeathBatch : busNeoSpinBatch
    {
        public busPayeeAccount ibusMemberPayeeAccount { get; set; }

        public busDeathNotification ibusDeathNotification { get; set; }

        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }

        public cdoPostRetirementDeathBenefitOptionRef icdoPostRetirementDeathBenfitOptionRef { get; set; }

        public busPersonBeneficiary ibusFirstBeneficiary { get; set; }

        public busPerson ibusJointAnnuitant { get; set; }

        public int iintActiveSpousePERSLinkID { get; set; }

        public int iintRTWPayeeAccountID = 0;

        public string istrIsLessThan2Yrs = string.Empty;

        public string istrStepName { get; set; }

        /// Initializing the objects for every record in a batch
        public void InitializeObjects()
        {
            ibusMemberPayeeAccount = new busPayeeAccount
            {
                icdoPayeeAccount = new cdoPayeeAccount(),
                ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                ibusApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() },
                ibusDROApplication = new busBenefitDroApplication { icdoBenefitDroApplication = new cdoBenefitDroApplication() }
            };
            ibusDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
            icdoPostRetirementDeathBenfitOptionRef = new cdoPostRetirementDeathBenefitOptionRef();

            ibusPersonAccountRetirement = new busPersonAccountRetirement
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
            };

            ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() };

            ibusFirstBeneficiary = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
        }

        /// Loads the Active Joint Annuitant of the Member.
        public void LoadJointAnnuitant()
        {
            if (ibusJointAnnuitant.IsNull())
                ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() };

            if (ibusMemberPayeeAccount.ibusMember.icolPersonContact.IsNull())
                ibusMemberPayeeAccount.ibusMember.LoadContacts();

            busPersonContact lobjContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
            lobjContact = ibusMemberPayeeAccount.ibusMember.icolPersonContact.Where(
                                    lobj =>lobj.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse).FirstOrDefault();

            if (lobjContact.IsNotNull() && (lobjContact.icdoPersonContact.contact_person_id != 0))
            {
                ibusJointAnnuitant.FindPerson(lobjContact.icdoPersonContact.contact_person_id);
            }
        }

        /// Returns true if a Post-Retirement Death Application already exists for the Given Member, Plan , PSTD Reason
        public bool IsPSTDApplicationAlreadyExists(int aintMemberID,int aintOrgID,int aintPlanID,int aintOriginatingPayeeAccountID,string astrPSTDReason)
        {
            DataTable ldtbResult = new DataTable();
            if (aintMemberID != 0)
            {
                ldtbResult = busBase.SelectWithOperator<cdoBenefitApplication>(new string[8]{
                                            "MEMBER_PERSON_ID","PLAN_ID","BENEFIT_ACCOUNT_TYPE_VALUE","ORIGINATING_PAYEE_ACCOUNT_ID",
                                            "POST_RETIREMENT_DEATH_REASON_TYPE_VALUE","RECIPIENT_PERSON_ID","ACTION_STATUS_VALUE","ACTION_STATUS_VALUE"},
                                            new string[8] { "=", "=", "=", "=", "=", "=", "<>", "<>" },
                                            new object[8]{
                                            ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id,aintPlanID,
                                            busConstant.ApplicationBenefitTypePostRetirementDeath,
                                            aintOriginatingPayeeAccountID,astrPSTDReason,aintMemberID,
                                            busConstant.ApplicationActionStatusCancelled,
                                            busConstant.ApplicationActionStatusDenied}, null);
            }
            else
            {
                ldtbResult = busBase.SelectWithOperator<cdoBenefitApplication>(new string[8]{
                                            "MEMBER_PERSON_ID","PLAN_ID","BENEFIT_ACCOUNT_TYPE_VALUE","ORIGINATING_PAYEE_ACCOUNT_ID",
                                            "POST_RETIREMENT_DEATH_REASON_TYPE_VALUE","PAYEE_ORG_ID","ACTION_STATUS_VALUE","ACTION_STATUS_VALUE"},
                                            new string[8] { "=", "=", "=", "=", "=", "=", "<>", "<>" }, 
                                            new object[8]{
                                            ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id,aintPlanID,
                                            busConstant.ApplicationBenefitTypePostRetirementDeath,
                                            aintOriginatingPayeeAccountID,astrPSTDReason,aintOrgID,
                                            busConstant.ApplicationActionStatusCancelled,
                                            busConstant.ApplicationActionStatusDenied}, null);
            }
            if (ldtbResult.Rows.Count > 0)
                return true;                        
            return false;
        }

        /// Loads the Post-Retirement Death Benefit Option Reference.
        public void LoadPSTDBenefitOptionRef(string astrPSTDReasonValue,bool iblnIsTermCertainDatePastDeathDate)
        {
            //UAT PIR:2080 Setting the Term certainDate Past Death Date Flag.
            string lstrIsTermCertainDatePastDeathDate = "N";
            if (iblnIsTermCertainDatePastDeathDate)
            {
                lstrIsTermCertainDatePastDeathDate = "Y";
            }
            string lstrWhere = string.Empty;
            lstrWhere = "BENEFIT_PROVISION_ID = " + ibusMemberPayeeAccount.ibusPlan.icdoPlan.benefit_provision_id.ToString();
            if (astrPSTDReasonValue == busConstant.PostRetirementAlternatePayeeDeath)
                lstrWhere += " AND SOURCE_BENEFIT_ACCOUNT_TYPE_VALUE IS NULL";
            else
                lstrWhere += " AND SOURCE_BENEFIT_ACCOUNT_TYPE_VALUE = '" + ibusMemberPayeeAccount.icdoPayeeAccount.benefit_account_type_value + "'";
            lstrWhere += " AND SOURCE_BENEFIT_OPTION_VALUE = '" + ibusMemberPayeeAccount.icdoPayeeAccount.benefit_option_value + "'";
            lstrWhere += " AND IS_TERMDATE_PAST_DEATH_DATE_FLAG = '" + lstrIsTermCertainDatePastDeathDate + "'";            
            lstrWhere += " AND POST_RETIREMENT_DEATH_REASON_TYPE_VALUE='" + astrPSTDReasonValue + "'";

            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCacheData("sgt_post_retirement_death_benefit_option_ref", lstrWhere);
            if (ldtbList.Rows.Count > 0)
            {
                if (((ibusMemberPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
                    && (astrPSTDReasonValue != busConstant.PostRetirementFirstBeneficiaryDeath)) &&
                    ((ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJudges) ||
                    (ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP)))
                {
                    string lstrAccountRelationship = busConstant.AccountRelationshipBeneficiary;                    
                    if (ibusMemberPayeeAccount.ibusMember.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    {
                        ibusMemberPayeeAccount.ibusMember.LoadContacts();

                        busPersonContact lobjContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                        lobjContact = ibusMemberPayeeAccount.ibusMember.icolPersonContact.Where(
                                                lobj => lobj.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive &&
                                                lobj.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse).FirstOrDefault();

                        if (lobjContact.IsNull())
                        {
                            idlgUpdateProcessLog("Spouse has to be Active for Death batch to process the survivor benefits. Member PERSLink ID "
                                                + ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id.ToString(), "ERR", istrStepName);// PROD PIR ID 7489
                        }
                        else if (lobjContact.icdoPersonContact.contact_person_id != 0)
                        {
                            lstrAccountRelationship = busConstant.AccountRelationshipJointAnnuitant;
                            iintActiveSpousePERSLinkID = lobjContact.icdoPersonContact.contact_person_id;
                        }
                    }
                    DataRow[] ldtrRow = ldtbList.FilterTable(busConstant.DataType.String, "ACCOUNT_RELATION_VALUE", lstrAccountRelationship);
                    if (ldtrRow.Count() > 0)
                        icdoPostRetirementDeathBenfitOptionRef.LoadData(ldtrRow[0]);
                }
                else
                {
                    icdoPostRetirementDeathBenfitOptionRef.LoadData(ldtbList.Rows[0]);
                }
            }
        }

        // Creates a new Post-Retirement Death Application
        public int InsertPostRetirementBenefitApplication(int aintRecipientID, int aintPayeeOrgID, string astrFamilyRelationValue, string astrDeathReasonValue, string astrAccountRelationValue, int aintbeneficiarypersonid)
        {
            

            if (ibusMemberPayeeAccount.ibusMember.IsNull())
                ibusMemberPayeeAccount.LoadMember();

            int lintMemberPersonID = ibusMemberPayeeAccount.ibusMember.icdoPerson.person_id;

            if (astrDeathReasonValue == busConstant.PostRetirementFirstBeneficiaryDeath)
            {
                if (ibusMemberPayeeAccount.ibusApplication.IsNull())
                    ibusMemberPayeeAccount.LoadApplication();
                lintMemberPersonID = ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id;
            }

            cdoBenefitApplication lcdoBenefitApplication = new cdoBenefitApplication
            {
                member_person_id = lintMemberPersonID,
                plan_id = ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id,
                benefit_account_type_value = busConstant.ApplicationBenefitTypePostRetirementDeath,
                benefit_option_value = icdoPostRetirementDeathBenfitOptionRef.destination_benefit_option_value,
                originating_payee_account_id = ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                post_retirement_death_reason_type_value = astrDeathReasonValue,
                action_status_value = busConstant.ApplicationActionStatusPending,
                status_value = busConstant.ApplicationStatusValid,
                retirement_date = ibusDeathNotification.icdoDeathNotification.date_of_death.GetFirstDayofNextMonth(),
                termination_date = ibusDeathNotification.icdoDeathNotification.date_of_death,
                received_date = iobjSystemManagement.icdoSystemManagement.batch_date,
                rhic_option_value = ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value,
                recipient_person_id = aintRecipientID,
                benefit_sub_type_value = busConstant.BenefitAccountSubTypePostRetDeath,
                retirement_org_id = ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_org_id,
                account_relationship_value = astrAccountRelationValue,
                family_relationship_value = astrFamilyRelationValue,
                pre_rtw_payeeaccount_id = iintRTWPayeeAccountID,
                rtw_refund_election_value = (iintRTWPayeeAccountID == 0) ? null : busConstant.Flag_No_Value.ToUpper(),
                is_rtw_less_than_2years_flag = (iintRTWPayeeAccountID == 0) ? null : istrIsLessThan2Yrs,
                payee_org_id = aintPayeeOrgID,
                plso_requested_flag = null,
                uniform_income_flag = null,
                suppress_warnings_flag = null,
                sick_leave_purchase_indicated_flag = null,
                beneficiary_person_id = aintbeneficiarypersonid
            };
            
            /* PIR 14238 && PIR 13365 - Permanent fix - if joint annuitant Perslink id is not set here while creating post retirement application by batch, 
             * 6714 hard error was being thrown on verify click button on post retirement death application maintenance*/
            if (astrFamilyRelationValue == busConstant.FamilyRelationshipSpouse && astrAccountRelationValue == busConstant.AccountRelationshipJointAnnuitant)
            {
                lcdoBenefitApplication.joint_annuitant_perslink_id = aintRecipientID;                                           
            }
            lcdoBenefitApplication.Insert();
            return lcdoBenefitApplication.benefit_application_id;
        }

        /// Inserts the Post-Retirement Death Application
        /// Inserts a record in Benefit Application Person Account
        public int CreatePSTDApplication(int aintPERSLinkID, int aintOrgID, string astrFamilyRelation, int aintPersonAccountID, 
                                            int aintPayeeAccountID,string astrPSTDReason, string astrAccountRelation, int aintbeneficiarypersonid)
        {
            int lintApplicationID = 0;
            bool lblnApplicationAlreadyExists = false;
            if (aintPERSLinkID != 0)
            {
                if (IsPSTDApplicationAlreadyExists(aintPERSLinkID, 0, ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id, aintPayeeAccountID, astrPSTDReason))
                {
                    idlgUpdateProcessLog("Post-Retirement Application Already Exists For PERSLinkID : " + Convert.ToString(aintPERSLinkID), "INFO", istrStepName);
                    lblnApplicationAlreadyExists = true;
                }
                else
                {
                    idlgUpdateProcessLog("Processing Post-Retirement Application For PERSLinkID : " + Convert.ToString(aintPERSLinkID), "INFO", istrStepName);
                }
            }
            else
            {
                if (IsPSTDApplicationAlreadyExists(0, aintOrgID, ibusMemberPayeeAccount.ibusPlan.icdoPlan.plan_id, aintPayeeAccountID, astrPSTDReason))
                {
                    idlgUpdateProcessLog("Post-Retirement Application Already Exists For Org ID : " + Convert.ToString(aintOrgID), "INFO", istrStepName);
                    lblnApplicationAlreadyExists = true;
                }
                else
                {
                    idlgUpdateProcessLog("Processing Post-Retirement Application For Org ID : " + Convert.ToString(aintOrgID), "INFO", istrStepName);
                }
            }

            if (!lblnApplicationAlreadyExists)
            {
                lintApplicationID = InsertPostRetirementBenefitApplication(aintPERSLinkID, aintOrgID, astrFamilyRelation, astrPSTDReason, astrAccountRelation, aintbeneficiarypersonid);

                /***** Insert Benefit Application Person Account for Active Spouse.******/
                if (aintPersonAccountID != 0)
                {
                    cdoBenefitApplicationPersonAccount lcdoBenAppPersonAccount = new cdoBenefitApplicationPersonAccount
                    {
                        benefit_application_id = lintApplicationID,
                        person_account_id = aintPersonAccountID,
                        is_application_person_account_flag = busConstant.Flag_Yes,
                        is_person_account_selected_flag = busConstant.Flag_Yes,
                        payee_account_id = aintPayeeAccountID
                    };
                    lcdoBenAppPersonAccount.Insert();
                }
            }
            return lintApplicationID;
        }

        /// Updates the Flag for every record.
        public void UpdateFlag(string astrPSTDReasonValue)
        {
            if (ibusDeathNotification.IsNotNull())
            {
                switch (astrPSTDReasonValue)
                {
                    case busConstant.PostRetirementAccountOwnerDeath:
                        ibusMemberPayeeAccount.icdoPayeeAccount.account_owner_batch_initiated_flag = busConstant.Flag_Yes;
                        break;
                    case busConstant.PostRetirementAlternatePayeeDeath:
                        ibusMemberPayeeAccount.icdoPayeeAccount.alternate_payee_batch_initiated_flag = busConstant.Flag_Yes;
                        break;
                    case busConstant.PostRetirementFirstBeneficiaryDeath:
                        ibusMemberPayeeAccount.icdoPayeeAccount.first_beneficiary_batch_initiated_flag = busConstant.Flag_Yes;
                        break;
                    default:
                        break;
                }
                ibusMemberPayeeAccount.icdoPayeeAccount.Update();
            }
        }

		//UAT PIR:2080 Fix.
        public bool IsMemberAccountBalancelessthanZero(busPayeeAccount lobjPayeeAccount, DateTime ldtDateofDeath)
        {            
            bool lblnMinimumGuaranteeAmtlessthanZero= false;            
            decimal ldecMemberAccountBalance = 0M;

            lobjPayeeAccount.LoadPaymentDetails();
            //UAT PIR: 1224
            //For Disability since the Minimum Guarantee is not stored, it is calculated as on date of death so that the interest amount is added.
            //Do not reduce the Already paid amount to the accoutn owner.
            if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (lobjPayeeAccount.ibusApplication.ibusPersonAccount.IsNull())
                    lobjPayeeAccount.ibusApplication.LoadPersonAccountByApplication();

                busPersonAccountRetirement lobjbusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
                lobjbusPersonAccountRetirement.icdoPersonAccount = lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount;
                lobjbusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccountRetirement.icdoPersonAccount.person_account_id);

                lobjbusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldtDateofDeath, lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value);
                ldecMemberAccountBalance = lobjbusPersonAccountRetirement.Member_Account_Balance_ltd;
                if ((ldecMemberAccountBalance) <= 0M)
                    lblnMinimumGuaranteeAmtlessthanZero = true;
            }
            else
            {
                if ((lobjPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - lobjPayeeAccount.idecpaidgrossamount) <= 0M)
                    lblnMinimumGuaranteeAmtlessthanZero = true;
            }            
            return lblnMinimumGuaranteeAmtlessthanZero;
        }
    }
}
