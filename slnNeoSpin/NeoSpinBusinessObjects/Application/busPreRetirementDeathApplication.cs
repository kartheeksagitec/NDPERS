#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Linq;

#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPreRetirementDeathApplication : busBenefitApplication
    {
        private Collection<busBenefitApplication> _iclbOtherApplications;
        public Collection<busBenefitApplication> iclbOtherApplications
        {
            get { return _iclbOtherApplications; }
            set { _iclbOtherApplications = value; }
        }

		//PIR 14391
        public busBenefitApplication ibusBenefitApplication { get; set; }
		
		//PIR 14391
        public void LoadBenefitApplication()
        {
            if (ibusBenefitApplication.IsNull())
                ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            ibusBenefitApplication.icdoBenefitApplication = icdoBenefitApplication;
            ibusBenefitApplication.ibusPersonAccount = ibusPersonAccount;
        }

        # region Load Methods

        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans()
        {
            return LoadBenefitOptionsBasedOnPlans(icdoBenefitApplication.plan_id, icdoBenefitApplication.benefit_account_type_value);
        }

        //Load Account Relationship based on the Plan
        public Collection<cdoCodeValue> LoadAccountRelationshipByPlan()
        {
            Collection<cdoCodeValue> lclcAccountRelationshipResult = new Collection<cdoCodeValue>();

            DataTable ldtbAccountRelationship = iobjPassInfo.isrvDBCache.GetCodeValues(2225);
            if (ldtbAccountRelationship.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbAccountRelationship.Rows)
                {
                    cdoCodeValue lobjcodevalue = new cdoCodeValue();
                    if (icdoBenefitApplication.plan_id != busConstant.PlanIdJobService)
                    {
                        if (dr["code_value"].ToString() == busConstant.AccountRelationshipBeneficiary)
                        {
                            lobjcodevalue.LoadData(dr);
                            lclcAccountRelationshipResult.Add(lobjcodevalue);
                        }
                    }
                    else
                    {
                        if ((dr["code_value"].ToString() != busConstant.AccountRelationshipMember)
                            && (dr["code_value"].ToString() != busConstant.AccountRelationshipJointAnnuitant)
                            && (dr["code_value"].ToString() != busConstant.AccountRelationshipAlternatePayee))
                        {
                            lobjcodevalue.LoadData(dr);
                            lclcAccountRelationshipResult.Add(lobjcodevalue);
                        }
                    }
                }
            }
            lclcAccountRelationshipResult = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order", lclcAccountRelationshipResult);
            return lclcAccountRelationshipResult;
        }
        public Collection<busBenefitApplication> LoadOtherApplicationsForJobService()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadActiveContacts();

            int lintCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o => o.icdoPersonContact.contact_person_id != icdoBenefitApplication.recipient_person_id
                && o.icdoPersonContact.contact_person_id > 0).Count();

            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();

            Collection<busBenefitApplication> iclbJobServiceApplications = new Collection<busBenefitApplication>();

            if (lintCount > 0)
            {
                foreach (busPersonContact lobjTemppersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
                {
                    if ((lobjTemppersonContact.icdoPersonContact.contact_person_id != icdoBenefitApplication.recipient_person_id)
                        && (lobjTemppersonContact.icdoPersonContact.contact_person_id >0))
                    {
                        busBenefitApplication lobjPreRetirementDeathApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                         lobjPreRetirementDeathApplication= ibusPersonAccount.ibusPerson.iclbBenefitApplication.Where(o =>
                            (o.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id)
                            && (o.icdoBenefitApplication.recipient_person_id==lobjTemppersonContact.icdoPersonContact.contact_person_id)
                            && (o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                            && ((o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                            && (o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied))).FirstOrDefault();
                        if ((lobjPreRetirementDeathApplication.IsNotNull())
                            && (lobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_application_id >0))
                        {
                            lobjPreRetirementDeathApplication.LoadRecipient();
                            lobjPreRetirementDeathApplication.icdoBenefitApplication.istrPersonName = lobjPreRetirementDeathApplication.ibusRecipient.icdoPerson.FullName;
                            iclbJobServiceApplications.Add(lobjPreRetirementDeathApplication);
                        }
                    }
                }
            }

            return iclbJobServiceApplications;
        }

        public void LoadOtherApplications()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                ibusPersonAccount.ibusPerson.LoadBeneficiary();

            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();

            _iclbOtherApplications = new Collection<busBenefitApplication>();

            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            {
                _iclbOtherApplications = LoadOtherApplicationsForJobService();
            }

            foreach (busPersonBeneficiary lobjBeneficiary in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjBeneficiary.ibusPersonAccountBeneficiary == null)
                    lobjBeneficiary.LoadPersonAccountBeneficiary();

                if (lobjBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount == null)
                    lobjBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();

                bool lblnRecordExists = false;
                if (lobjBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id && 
                    lobjBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) // PROD PIR ID 7076
                {
                    busBenefitApplication lobjOthrApplication = new busBenefitApplication();
                    lobjOthrApplication.icdoBenefitApplication = new cdoBenefitApplication();


                    if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                    {
                        if ((lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id))
                        {
                            lblnRecordExists = true;
                        }
                        else
                        {
                            foreach (busBenefitApplication lobjApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
                            {
                                if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == lobjApplication.icdoBenefitApplication.recipient_person_id)
                                {

                                    if (lobjApplication.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id)
                                    {
                                        if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                                        && (lobjApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id))
                                        {
                                            lobjApplication.LoadRecipient();
                                            lobjOthrApplication = lobjApplication;
                                            lobjOthrApplication.icdoBenefitApplication.istrPersonName = lobjApplication.ibusRecipient.icdoPerson.FullName;
                                            lobjOthrApplication.icdoBenefitApplication.received_date = lobjApplication.icdoBenefitApplication.received_date;

                                            //PIR - 1459
                                            // check if the Person already added to collection for the same plan
                                            var lintCount = IsApplicantAlreadyInOtherApplicationCollection(lobjApplication);
                                            if (lintCount == 0)
                                            {
                                                iclbOtherApplications.Add(lobjOthrApplication);
                                                lblnRecordExists = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                    {
                        foreach (busBenefitApplication lobjApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
                        {
                            if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)
                            {
                                lblnRecordExists = true;
                            }
                            else
                            {
                                if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id == lobjApplication.icdoBenefitApplication.payee_org_id)
                                {
                                    if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                                    && (lobjApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id))
                                    {
                                        lobjApplication.LoadApplicantOrganization();
                                        lobjOthrApplication = lobjApplication;

                                        //PIR - 1459
                                        // check if the Person already added to collection for the same plan
                                        var lintCount = IsApplicantAlreadyInOtherApplicationCollection(lobjApplication);
                                        if (lintCount == 0)
                                        {
                                            iclbOtherApplications.Add(lobjOthrApplication);
                                            lblnRecordExists = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!lblnRecordExists)
                    {
                        if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                        {
                            lobjOthrApplication.icdoBenefitApplication.recipient_person_id = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id;

                            lobjOthrApplication.LoadRecipient();
                            lobjOthrApplication.icdoBenefitApplication.istrPersonName = lobjOthrApplication.ibusRecipient.icdoPerson.FullName;
                        }
                        else if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                        {
                            lobjOthrApplication.icdoBenefitApplication.payee_org_id = lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
                            lobjOthrApplication.LoadApplicantOrganization();
                        }
                        else
                        {
                            lobjOthrApplication.icdoBenefitApplication.istrPersonName = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_name;
                        }
                        if (IsApplicantAlreadyInOtherApplicationCollection(lobjOthrApplication) == 0)
                        {
                            iclbOtherApplications.Add(lobjOthrApplication);
                        }
                    }
                }
            }
            
        }

        private int IsApplicantAlreadyInOtherApplicationCollection(busBenefitApplication lobjApplication)
        {
            var lintCount = 0;
            if (lobjApplication.icdoBenefitApplication.recipient_person_id != 0)
            {
                lintCount = (from n in iclbOtherApplications
                             where n.icdoBenefitApplication.recipient_person_id == lobjApplication.icdoBenefitApplication.recipient_person_id
                             && n.icdoBenefitApplication.plan_id == lobjApplication.icdoBenefitApplication.plan_id
                             select n).Count<busBenefitApplication>();
            }
            if (lobjApplication.icdoBenefitApplication.payee_org_id != 0)
            {
                lintCount = (from n in iclbOtherApplications
                             where (n.icdoBenefitApplication.payee_org_id == lobjApplication.icdoBenefitApplication.payee_org_id)
                             && n.icdoBenefitApplication.plan_id == lobjApplication.icdoBenefitApplication.plan_id
                             select n).Count<busBenefitApplication>();
            }
            return lintCount;
        }

        public void SetAccountRelationshipforJobService()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadContacts();
            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if ((lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)
                    && (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse))
                {
                    icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipSpouse;
                    break;
                }
                else if ((lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)
                    && (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild))
                {
                    icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipChild;
                }
            }
        }

        //Load Family relationsship based on the relationship of applicant based on the person with applicant
        public void SetFamilyRelationship()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                ibusPersonAccount.ibusPerson.LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
                {
                    icdoBenefitApplication.family_relationship_value = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value;
                    break;
                }
            }
			//PIR:4777 The Relationship should be set only when it is not set From Contacts
			//to avoid resetting the relationship. JobService can be omitted since the relation ship can be also set from Contact and
			//Contact relationship takes the precedence
            if ((icdoBenefitApplication.plan_id == busConstant.PlanIdJobService) || (icdoBenefitApplication.family_relationship_value.IsNullOrEmpty()))
            {
                if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                    ibusPersonAccount.ibusPerson.LoadContacts();

                foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
                {
                    if (lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)
                    {
                        string astrRelationshipdesc = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(icdoBenefitApplication.family_relationship_id, lobjPersonContact.icdoPersonContact.relationship_value);
                        if (!(astrRelationshipdesc.IsNullOrEmpty()))
                        {
                            icdoBenefitApplication.family_relationship_value = lobjPersonContact.icdoPersonContact.relationship_value;
                        }
                        else
                        {
                            icdoBenefitApplication.family_relationship_value = busConstant.FamilyRelationshipOthers;
                        }
                        break;
                    }
                }
            }
            if (String.IsNullOrEmpty(icdoBenefitApplication.family_relationship_value))
                icdoBenefitApplication.family_relationship_value = busConstant.FamilyRelationshipOthers;
        }

        #endregion

        # region Rules
        //BR- 53- 18
        //this rule checks if any beneficiary death notification exits 
        // with status as In Progress or Non responsive
        public bool IsAllBeneficiariesAlive()
        {
            if (ibusPersonAccount == null) LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null) ibusPersonAccount.LoadPerson();
            return ibusPersonAccount.ibusPerson.IsAnyBeneficiaryDeathInProgressorNonResponsive();
        }

        //BR- 53- 19
        //this rule checks if any beneficiary death notification exits 
        // with status as In Progress or Non responsive
        public bool AreAllDependentsAlive()
        {
            if (ibusPersonAccount == null) LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null) ibusPersonAccount.LoadPerson();
            return ibusPersonAccount.ibusPerson.IsAnyDependentDeathInProgressorNonResponsive();
        }

        //BR-053-28
        // cehck if any Payee account exists for this person
        public bool IsPayeeAccountExistForPerson()
        {
            if (iclbPayeeAccountByPersonID == null)
                LoadPayeeAccountForPerson();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccountByPersonID)
            {
                lobjPayeeAccount.LoadActivePayeeStatus();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus != null)
                {
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirmentRecieving)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void LoadPayeeAccountForPerson()
        {
            iclbPayeeAccountByPersonID = new Collection<busPayeeAccount>();
            DataTable ldtbList = Select<cdoPayeeAccount>(
               new string[1] { "payee_perslink_id" },
               new object[1] { icdoBenefitApplication.member_person_id }, null, null);
            iclbPayeeAccountByPersonID = GetCollection<busPayeeAccount>(ldtbList, "icdoPayeeAccount");
        }

        //BR- 29
        public bool IsApplicationAlreadyExistsForPersonAndMember()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            foreach (busBenefitApplication lobjApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                if (lobjApplication.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id)
                {
                    // UAT PIR ID 1132
                    if (ibusPersonAccount.IsNull()) LoadPersonAccountByApplication();
                    if (lobjApplication.ibusPersonAccount.IsNull()) lobjApplication.LoadPersonAccountByApplication();
                    //UAT PIR: 1527 Recipient Person ID in the check when checking Pre Retirement Death Applications.
                    if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                        || (lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
                    {
                        if ((lobjApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id) && // UAT PIR ID 1500
                            (lobjApplication.icdoBenefitApplication.status_value != busConstant.ApplicationStatusProcessed) &&
                            (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied) &&
                            (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled) &&
                            (lobjApplication.ibusPersonAccount.icdoPersonAccount.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id) &&
                            (lobjApplication.icdoBenefitApplication.recipient_person_id == icdoBenefitApplication.recipient_person_id)
                            )
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((lobjApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id) && // UAT PIR ID 1500
                            (lobjApplication.icdoBenefitApplication.status_value != busConstant.ApplicationStatusProcessed) &&
                            (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied) &&
                            (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled) &&
                            (lobjApplication.ibusPersonAccount.icdoPersonAccount.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id)
                            )
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //BR-30
        public bool IsPreRetDethApplnAlreadyExistsForPersonAndMember()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
                ibusPersonAccount.ibusPerson.LoadBenefitApplication();
            foreach (busBenefitApplication lobjApplication in ibusPersonAccount.ibusPerson.iclbBenefitApplication)
            {
                if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    && (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied)
                    && (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                    && (lobjApplication.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id)
                    && (lobjApplication.icdoBenefitApplication.recipient_person_id == icdoBenefitApplication.recipient_person_id)
                    && (lobjApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id))
                {
                    return true;
                }
            }
            return false;
        }

        //BR- 31
        private bool IsDeathNotificationExistForMember()
        {
            Collection<busDeathNotification> iclbDeathNotification = new Collection<busDeathNotification>();

            DataTable ldtbDeathNotification = Select<cdoDeathNotification>(new string[1] { "person_id" }, new object[1] { icdoBenefitApplication.member_person_id }, null, null);
            iclbDeathNotification = GetCollection<busDeathNotification>(ldtbDeathNotification, "icdoDeathNotification");

            if (iclbDeathNotification.Where(i => i.icdoDeathNotification.action_status_value != busConstant.DeathNotificationActionStatusCancelled
                   && i.icdoDeathNotification.action_status_value != busConstant.DeathNotificationActionStatusErroneous
                   && i.icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes).Count() > 0)
            {

                return true;
            }          
            return false;
        }

        public bool IsFamilyRelationshipIsSpouseForJS()
        {
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                    return true;
            return false;
        }

        //UAT PIR - 1781
        public bool IsFamilyRelationshipIsChildForJS()
        {
            bool lblnResult = true;
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            {
                //PIR - 1370 uat - irrespective of relationship this warning needs to display if applicable
                //if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipChild)
                {
                    lblnResult = IsInEligibleChildrenExists();
                }
            }
            return lblnResult;
        }

        //BR-053-33
        public bool IsPersonEligibleForSelectedOption()
        {
            if (icdoBenefitApplication.istrIsPersonVested == null)
                CheckIsPersonVested();

            //get count of beneficiries
            int lintBeneficiaryCount = GetBeneficiaryCountBasedOnPlan();

            if (!String.IsNullOrEmpty(icdoBenefitApplication.benefit_option_value))
            {
                if (icdoBenefitApplication.istrIsPersonVested != busConstant.Flag_Yes)
                {
                    if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                        return false;
                }
                else
                {

                    if (ibusPersonAccount.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    {
                        if ((icdoBenefitApplication.plan_id == busConstant.PlanIdMain)
                             || (icdoBenefitApplication.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                                || (icdoBenefitApplication.plan_id == busConstant.PlanIdNG)
                                || (icdoBenefitApplication.plan_id == busConstant.PlanIdLE)
                                || (icdoBenefitApplication.plan_id == busConstant.PlanIdLEWithoutPS)
                                || (icdoBenefitApplication.plan_id == busConstant.PlanIdBCILawEnf) // pir 7943
                                || (icdoBenefitApplication.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                        {
                            if (lintBeneficiaryCount > 1)
                            {
                                if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                    return false;
                            }
                            else if (IsPrimaryBeneficiary())
                            {
                                //check if the applicant is spouse to member
                                if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                                {
                                    if (icdoBenefitApplication.benefit_sub_type_value != busConstant.ApplicationBenefitSubTypeNormal)
                                    {
                                        if ((icdoBenefitApplication.benefit_option_value != busConstant.BenefitOption50Percent)
                                            && (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund))
                                            return false;
                                    }
                                    else
                                    {
                                        if ((icdoBenefitApplication.benefit_option_value != busConstant.BenefitOption100Percent)
                                            && ((icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)))
                                            return false;
                                    }
                                }
                                else
                                {
									//PIR:4777  
									//BR-053-33 As per this rule the 100%JS and 50%Life Time is applicable only for Primary Beneficiary who is a Spouse
                                    if(icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (icdoBenefitApplication.plan_id == busConstant.PlanIdJudges)
                        {
                            if (IsPrimaryBeneficiary())
                            {
                                if (lintBeneficiaryCount > 1)
                                {
                                    if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                        return false;
                                }
                                //check if the applicant is spouse to member
                                else if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                                {
                                    if ((icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                       && (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionMonthlyLifeTimeBenefit))
                                        return false;
                                }
                            }
                        }
                        else if (icdoBenefitApplication.plan_id == busConstant.PlanIdHP)
                        {
                            if (IsPrimaryBeneficiary())
                            {
                                if (lintBeneficiaryCount > 1)
                                {
                                    if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                        return false;
                                }
                                else if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                                {
                                    if (icdoBenefitApplication.benefit_sub_type_value != busConstant.ApplicationBenefitSubTypeNormal)
                                    {
                                        if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOption50Percent)
                                            return false;
                                    }
                                    else if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC ||
                                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                        {
                            //UAT PIR - 1149
                            //if the primary and only bene is Spouse
                            if (IsPrimaryBeneficiary())
                            {
                                if ((lintBeneficiaryCount == 1) && (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse))
                                {
                                    if ((icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionPeriodicPayment)
                                                && (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                                        return false;
                                }
                            }
                        }
                    }
                    else if ((!IsActiveSpouseExists()) && ((icdoBenefitApplication.plan_id != busConstant.PlanIdJobService)))
                    {
                        //if single or no spouse is valid beneficiary
                        if (icdoBenefitApplication.benefit_option_value != busConstant.BenefitOptionRefund)
                            return false;
                    }
                }

                //need not to check if the person is vested for Job service
                if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                {
                    if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionSpouseBenefit)
                    {
                        if (!IsActiveSpouseExists())
                        {
                            return false;
                        }
                    }

                    if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionChildTimeBenefit)
                    {
                        if (icdoBenefitApplication.account_relationship_value == busConstant.AccountRelationshipSpouse)
                        {
                            return false;
                        }

                        if (!IsChildExists())
                        {
                            return false;
                        }                        
                    }

                    //UAT PIR ID 1370
                    //else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionChildTimeBenefit)
                    //{
                    //    if (IsInEligibleChildrenExists())
                    //    {
                    //        return false;
                    //    }
                    //}
                    else if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption10YearCertain)
                    {
                        if (!IsPersonNotHavingSpouseOrChildren())
                        {
                            return false;
                        }
                    }
                    else if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionBeneficiaryBenefit) ||
                             (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRefund))
                    {
                        if (IsPersonNotHavingSpouseOrChildren())
                        {
                            return false;
                        }

                        if (!IsSpouseDeceased() && !IsChildPayeeAccountInCompletedStatus())   //if spouse is deceased and no eligible children exists
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private int GetBeneficiaryCountBasedOnPlan()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                ibusPersonAccount.ibusPerson.LoadBeneficiary();

            int lintCount = 0;
            foreach (busPersonBeneficiary lobjBen in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjBen.ibusPersonAccountBeneficiary == null)
                    lobjBen.LoadPersonAccountBeneficiary();
                if (lobjBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary != null)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(ibusPersonAccount.ibusPerson.icdoPerson.date_of_death,
                                                    lobjBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                    lobjBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date)
                            && lobjBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary)// PIR 7076
                    {

                        if (lobjBen.ibusPersonAccountBeneficiary.ibusPersonAccount == null)
                            lobjBen.ibusPersonAccountBeneficiary.LoadPersonAccount();

                        if (lobjBen.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id)
                            lintCount++;
                    }
                }
            }
            return lintCount;
        }

        private bool IsSpouseDeceased()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadActiveContacts();

            //int lintSpouseCount=ibusPersonAccount.ibusPerson.icolPersonContact

            int lintSpouseCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o =>
            (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse) && (o.icdoPersonContact.contact_person_id > 0)).Count();

            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)                    
                {
                    if (lobjPersonContact.ibusContactPerson == null)
                        lobjPersonContact.LoadContactPerson();

                    if (lobjPersonContact.ibusContactPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        return true;
                }                
            }
            return false;
        }

        private bool IsChildPayeeAccountInCompletedStatus()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadActiveContacts();            

            int lintChildCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o =>
            (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild) && (o.icdoPersonContact.contact_person_id > 0)).Count();

            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild) && (lobjPersonContact.icdoPersonContact.contact_person_id >0))
                {
                    lobjPersonContact.LoadContactPerson();

                    lobjPersonContact.ibusContactPerson.LoadBenefitApplication();

                    if (lobjPersonContact.ibusContactPerson.iclbBeneficiaryApplication.IsNotNull())
                    {
                        var lobjTempApplication = lobjPersonContact.ibusContactPerson.iclbBeneficiaryApplication.Where(o => (o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                            && (o.icdoBenefitApplication.member_person_id == icdoBenefitApplication.member_person_id)
                            && (o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                            && (o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied)).FirstOrDefault();

                        if (lobjTempApplication.IsNotNull())
                        {
                            lobjTempApplication.LoadPayeeAccount();

                            if (lobjTempApplication.iclbPayeeAccount.Count == 0)
                            {
                                return false;
                            }
                            else
                            {
                                foreach (busPayeeAccount lobjTemppayeeAccount in lobjTempApplication.iclbPayeeAccount)
                                {
                                    lobjTemppayeeAccount.LoadActivePayeeStatus();
                                    if (!(lobjTemppayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    else
                        return false;
                }
            }
            return true;
        }



        private bool IsInEligibleChildrenExists()
        {
            //Eligibility of In Eligible Child is 1) Child who is married 2) Child whose age is greater than 18.

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadActiveContacts();

            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                {
                    if ((lobjPersonContact.icdoPersonContact.contact_person_id > 0)
                        && (lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id))
                    {
                        lobjPersonContact.LoadContactPerson();

                        decimal idecChildAgeasonDOD = busGlobalFunctions.CalulateAge(lobjPersonContact.ibusContactPerson.icdoPerson.date_of_birth, ibusPersonAccount.ibusPerson.icdoPerson.date_of_death);
                        if ((lobjPersonContact.ibusContactPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                            || (idecChildAgeasonDOD > 18.0M))
                        {
                            return false;
                        }
                    }                                        
                }
            }
            return true;
        }

        private bool IsPersonNotHavingSpouseOrChildren()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadActiveContacts();

            if (ibusPersonAccount.ibusPerson.icolPersonContact.IsNotNull())
            {
                int lintCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                    || (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)).Count();

                if (lintCount > 0) 
                    return false;
            }            
            return true;
        }

        private bool IsChildExists()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadContacts();
            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)
                {
                    // UAT PIR ID 1367 - Only one possible spouse exists for a member. Hence no need to check for status.
                    if (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                    {
                        return true;                    
                    }
                }
            }
            return false;
        }

        private bool IsActiveSpouseExists()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                ibusPersonAccount.ibusPerson.LoadContacts();
            foreach (busPersonContact lobjPersonContact in ibusPersonAccount.ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)
                {
                    // UAT PIR ID 1367 - Only one possible spouse exists for a member. Hence no need to check for status.
                    if (lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                    {
                    return true;
                    }
                }
            }
            return false;
        }

        private bool IsPrimaryBeneficiary()
        {
            if (icdoBenefitApplication.recipient_person_id != 0)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson == null)
                    ibusPersonAccount.LoadPerson();
                if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                    ibusPersonAccount.ibusPerson.LoadBeneficiary();

                foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
                {
                    lobjPersonBeneficiary.LoadPersonAccountBeneficiaryData();
                    foreach (busPersonAccountBeneficiary lobjPersonAccountBen in lobjPersonBeneficiary.iclbPersonAccountBeneficiary)
                    {
                        if ((icdoBenefitApplication.recipient_person_id == lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id)
                            && (lobjPersonAccountBen.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //check Benefit option is auto refund
        public bool IsBenOptAutoRefund()
        {
            if (icdoBenefitApplication.ienuObjectState == ObjectState.Insert)
            {
                if (!string.IsNullOrEmpty(icdoBenefitApplication.benefit_option_value))
                {
                    string lstrData3 = icdoBenefitApplication.benefit_option_value;
                    if (lstrData3 == busConstant.BenefitOptionAutoRefundData3)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsBenOptRefund()
        {
            if (!string.IsNullOrEmpty(icdoBenefitApplication.benefit_option_value))
            {
                string lstrData3 = icdoBenefitApplication.benefit_option_value;
                if (lstrData3 == busConstant.BenefitOptionRefund)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidPreRetirementDeathBenefitAppExistsForContact(int aintPersonId)
        {
            busPerson ibusperson = new busPerson {icdoPerson= new cdoPerson()};
            ibusperson.FindPerson(aintPersonId);

            if(ibusperson.IsNotNull())
            {
                ibusperson.LoadPayeeAccount(false);

                int lintPreRetirementDeathCount = ibusperson.iclbPayeeAccount.Where(o=>(o.icdoPayeeAccount.benefit_account_type_value==busConstant.ApplicationBenefitTypePreRetirementDeath)).Count();

                if(lintPreRetirementDeathCount >0)
                {
                    foreach(busPayeeAccount lobjPayeeAccount in ibusperson.iclbPayeeAccount)
                    {
                        //CHCEK If the Pre Retirement Death Payee account for the current application Owner or not.
                        lobjPayeeAccount.LoadApplication();
                        lobjPayeeAccount.LoadActivePayeeStatus();                            

                        if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value==busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                            (lobjPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id==icdoBenefitApplication.plan_id) &&
                            (lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id == icdoBenefitApplication.member_person_id)
                            && (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()))
                        {                          
                            return true;
                        }
                    }
                }
            }
            return false;
        }    
    


        //check if the applicant is beneficiary to Account owner
        public bool IsApplicantIsBeneficiaryToAccoutnOwner()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                ibusPersonAccount.ibusPerson.LoadBeneficiary();
            bool lblnRecordExists = false;
            int lintActiveContactsCount = 0;
            if (ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
            {
                //If JobService then they can be also in Contact set as Spouse or Child.
                if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                    ibusPersonAccount.ibusPerson.LoadActiveContacts();

                lintActiveContactsCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o =>
                     (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                && (o.icdoPersonContact.contact_person_id > 0)).Count();

                int lintCount = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o => (o.icdoPersonContact.contact_person_id == icdoBenefitApplication.recipient_person_id)).Count();

                if (lintCount > 0)
                {
                    return true;
                }
            }


            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
            {
                if (icdoBenefitApplication.recipient_person_id > 0)
                {
                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitApplication.recipient_person_id)
                    {
                        lblnRecordExists = true;
                    }
                }
                if (icdoBenefitApplication.payee_org_id > 0)
                {
                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id == icdoBenefitApplication.payee_org_id)
                    {
                        lblnRecordExists = true;
                    }
                }
                if (lblnRecordExists)
                {
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary == null)
                        lobjPersonBeneficiary.LoadPersonAccountBeneficiary();
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                    {
                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount == null)
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();

                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == icdoBenefitApplication.plan_id)
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(ibusPersonAccount.ibusPerson.icdoPerson.date_of_death, lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date))
                            {
                                if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                                {
                                    //A person is eligible to enter applications..if no active spouse or Child exists
                                    //or Spouse and child have already received their benefits.

                                    if (lintActiveContactsCount == 0)
                                    {
                                        return true;
                                    }
                                    else if (lintActiveContactsCount > 0)
                                    {
                                        //Check if both Spouse and Child of the person have already received their Pre Retirement Benefits and they are in Payment complete status
                                        //If JobService then they can be also in Contact set as Spouse or Child.
                                        int lintAppcount = 0;

                                        if (ibusPersonAccount.ibusPerson.icolPersonContact == null)
                                            ibusPersonAccount.ibusPerson.LoadActiveContacts();

                                        var icolTempPersonContact = ibusPersonAccount.ibusPerson.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                                                                    || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                                                                    && (o.icdoPersonContact.contact_person_id > 0));

                                        foreach (busPersonContact lobjTempPersonContact in icolTempPersonContact)
                                        {
                                            if (IsValidPreRetirementDeathBenefitAppExistsForContact(lobjTempPersonContact.icdoPersonContact.contact_person_id))
                                            {
                                                lintAppcount = lintAppcount + 1;
                                            }
                                            else if (lobjTempPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                                            {
                                                lobjTempPersonContact.LoadContactPerson();
                                                if (lobjTempPersonContact.ibusContactPerson.icdoPerson.date_of_death != DateTime.MinValue)
                                                {
                                                    lintAppcount = lintAppcount + 1;
                                                }
                                            }

                                        }

                                        if (lintActiveContactsCount == lintAppcount)
                                            return true;
                                    }
                                    else
                                        return true;
                                }
                                else
                                    return true;
                            }
                        }
                    }
                }                
            }
            return false;
        }

        #endregion

        # region Button Logic

        public override ArrayList btnVerfiyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            //set employment end date
            SetOrgIdAsLatestEmploymentOrgId();
            this.ValidateHardErrors(utlPageMode.All);
            if (idtTerminationDate != DateTime.MinValue)
                icdoBenefitApplication.termination_date = idtTerminationDate;

            if (idtTerminationDate == DateTime.MinValue)
            {
                lobjError = AddError(1946, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (!IsDeathNotificationExistForMember())
            {
                lobjError = AddError(1992, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            alReturn = base.btnVerfiyClicked();
            return alReturn;
        }

        public override ArrayList btnPendingVerificationClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            alReturn = base.btnPendingVerificationClicked();
            return alReturn;
        }
        public override ArrayList btnDenyClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            IsDeniedButtonClicked = true;
            alReturn = base.btnDenyClicked();
            return alReturn;
        }
        #endregion

        # region Override methods
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            SetOrgIdAsLatestEmploymentOrgId();
            SetBenefitSubType();
            base.BeforeValidate(aenmPageMode);
        }
        # endregion


        // This method will load the corresponding Calculation ID for this application.
        // Calculation button visibility depends on this Calculation ID.
        // If Calculation ID is 0 Calculation Maintenance New mode else Update mode.
        public void LoadBenefitCalculationID()
        {
            // PROD PIR ID 5729 -- If calculation is "canceled" do not link new application to canceled calculation.
            DataTable ldtbResult = SelectWithOperator<cdoBenefitCalculation>(
                                        new string[5] { "person_id", "plan_id", "benefit_account_type_value", "Calculation_Type_Value", "action_status_value" },
                                        new string[5] { "=", "=", "=", "=", "<>" },
                                        new object[5] { icdoBenefitApplication.member_person_id, 
                                                        icdoBenefitApplication.plan_id,
                                                        icdoBenefitApplication.benefit_account_type_value,
                                                        busConstant.CalculationTypeFinal,
                                                        busConstant.ApplicationActionStatusCancelled}, "benefit_calculation_id desc");
            if (ldtbResult.Rows.Count > 0)
            {
                icdoBenefitApplication.benefit_calculation_id = Convert.ToInt32(ldtbResult.Rows[0]["benefit_calculation_id"]);
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            /*****************************************************************************************************************************************
            * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
            * Code Starts here
            ******************************************************************************************************************************************/
            if (icdoBenefitApplication.benefit_calculation_id == 0)
                LoadBenefitCalculationID();            

            busPreRetirementDeathBenefitCalculation ibusPreRetirementDeathBenefitCalculation = new busPreRetirementDeathBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };

            if (icdoBenefitApplication.benefit_calculation_id > 0)
            {
                ibusPreRetirementDeathBenefitCalculation.FindBenefitCalculation(icdoBenefitApplication.benefit_calculation_id);

                if (ibusPreRetirementDeathBenefitCalculation.IsNotNull())
                {
                    if (ibusPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value != busConstant.CalculationStatusReview)
                    {
                        ibusPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.CalculationStatusReview;
                        ibusPreRetirementDeathBenefitCalculation.icdoBenefitCalculation.Update();

                        ibusPreRetirementDeathBenefitCalculation.iblnIsApplicationModified = true;
                        ibusPreRetirementDeathBenefitCalculation.ValidateSoftErrors();
                    }
                }
            }
            /************************************************************************************************************************************************
             * Change from Raj on the Calculation being set to Review when the Calculation is not in Review Status and Action status is not approved.
             * Code Ends here
            *************************************************************************************************************************************************/
        }
        # region Correspondence

        private bool IsMemberSpouse
        {
            get
            {
                if (icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                {
                    return true;
                }
                return false;
            }
        }

        public string istrAppDateinLongFormat
        {
            get
            {
                return icdoBenefitApplication.received_date == DateTime.MinValue ? string.Empty : icdoBenefitApplication.received_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        # endregion
    }
}
