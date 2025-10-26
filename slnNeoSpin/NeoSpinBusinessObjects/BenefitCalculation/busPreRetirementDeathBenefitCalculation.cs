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
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPreRetirementDeathBenefitCalculation : busBenefitCalculation
    {
        public bool iblnIsNewMode = false;

        public string istrIsVestedAndMarried
        {
            get
            {
                bool lblnIsMarried = IsMemberMarried();
                bool lblnIsVested = IsMemberVested();
                if ((lblnIsVested) && (lblnIsMarried))
                    return "VEMA";
                if ((lblnIsVested) && (!lblnIsMarried))
                    return "VENM";
                if ((!lblnIsVested) && (lblnIsMarried))
                    return "NVMA";
                if ((!lblnIsVested) && (!lblnIsMarried))
                    return "NVNM";
                return string.Empty;
            }
        }

        public decimal idecTotalPercentage { get; set; }

        public bool IsTotalBenefitPercentageSum100()
        {            
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();  
			decimal ldecTotalPercentage = 0M;

            bool iblnExcludeBenefitPercentageCheck = false;

            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
            {
                if (icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimate)
                {
                    int lintcount = iclbBenefitCalculationPayee.Where(o => (o.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionChildTimeBenefit)
                        || (o.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionSpouseBenefit)).Count();

                    if (lintcount > 0)
                    {
                        iblnExcludeBenefitPercentageCheck = true;
                    }

                }
            }

            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                ldecTotalPercentage += lobjPayee.icdoBenefitCalculationPayee.benefit_percentage;
            }
            idecTotalPercentage = ldecTotalPercentage;

            if ((ldecTotalPercentage == 100M) ||(iblnExcludeBenefitPercentageCheck))
                return true;
            return false;
        }

        public bool IsBenefitOptionExists()
        {
            int lintTotalRefundSelected = 0;
            if (!string.IsNullOrEmpty(icdoBenefitCalculation.benefit_option_value))
            {
                if (iclbBenefitCalculationPayee == null)
                    LoadBenefitCalculationPayeeForNewMode();
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
                    {
                        if (iclbBenefitCalculationPayee.Count > 1)
                        {
                            //1. If Benefit Option is other than Refund then raise an error
                            if (icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionRefund)
                                return false;

                            //2. Find the sum of the Beneficiaries if not <> 100 raise an error.
                            return IsTotalBenefitPercentageSum100();
                        }
                        else
                        {
                            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund) ||
                            (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund))
                            {
                                //2. Find the sum of the Beneficiaries if not <> 100 raise an error.
                                return IsTotalBenefitPercentageSum100();
                            }
                        }
                    }
                    else
                    {
                        if (iclbBenefitCalculationPayee.Count > 1)
                        {

                            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund) ||
                                (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund))
                            {
                                lintTotalRefundSelected++;
                            }
                        }
                        else
                        {
                            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund) ||
                                (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionBeneficiaryBenefit) ||
                                (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund) ||
                                (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionBeneficiaryBenefit))
                            {
                                return IsTotalBenefitPercentageSum100();
                            }
                        }
                    }
                    Collection<cdoCodeValue> lclbBenefitOptions = new Collection<cdoCodeValue>();

                    if(icdoBenefitCalculation.benefit_account_sub_type_value.IsNullOrEmpty()) // PROD PIR 7101
                        CheckIsPersonEligibleforNormal();

                    lclbBenefitOptions = GetAvailableBenefitOptionsForBeneficiary(icdoBenefitCalculation.benefit_option_value,
                                                                            lobjPayee.icdoBenefitCalculationPayee.payee_identifier);
                    if (lclbBenefitOptions.Count == 0)
                        return false;
                    else
                    {
                        if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption100PercentJS) &&
                            (icdoBenefitCalculation.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeNormal))
                        {
                            return false;
                        }
                        else if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
                        || (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                        || (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG)
                        || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLEWithoutPS)
                        || (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf) // pir 7943
                        || (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                        {
                            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption50Percent) &&
                            (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (((iclbBenefitCalculationPayee.Count > 0) && (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)) &&
                    (iclbBenefitCalculationPayee.Count == lintTotalRefundSelected))
                {
                    return IsTotalBenefitPercentageSum100();
                }

            }
            return true;
        }

        public bool IsBenefitBeginDateValid()
        {
            if ((icdoBenefitCalculation.retirement_date != DateTime.MinValue) &&
                 (icdoBenefitCalculation.date_of_death != DateTime.MinValue))
            {
                DateTime ldtTempDate = icdoBenefitCalculation.date_of_death.AddMonths(1);
                ldtTempDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 01);
                if (icdoBenefitCalculation.retirement_date != ldtTempDate)
                    return false;
            }
            return true;
        }

        public Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans()
        {
            return LoadBenefitOptionsBasedOnPlans(icdoBenefitCalculation.plan_id, icdoBenefitCalculation.benefit_account_type_value);
        }

        public bool IsAnyApplicantsDeathNotified()
        {
            //UAT PIR: 1543.
			//UAT PIR: 1543. Warning needs to be raised when (Date of Death Exists or death has been notified) and
			//(Sum of Bene Percentage Is less than 100 or Bene Exists in the Payee Grid)  from the List of beneficiaries.

            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            ibusMember.LoadBeneficiaryForMemberByPersonAccount(ibusPersonAccount.icdoPersonAccount.person_account_id);            
            bool iblnIsTotalPercentageLessthan100=false;
            bool iblnIsAnyDeathNotified = false;

            if (!(IsTotalBenefitPercentageSum100()))
            {                
                if(idecTotalPercentage < 100.0M)
                {
                    iblnIsTotalPercentageLessthan100=true;
                }
            }         
            
            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusMember.iclbBeneficiariesByPersonAccount)
            {            
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                {
                    lobjPersonBeneficiary.LoadBeneficiaryPerson();

                    if (lobjPersonBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    {

                        DataTable ldtbResult = Select<cdoDeathNotification>(new string[1] { "PERSON_ID" },
                                                                        new object[1] { lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id }, null, "CREATED_DATE DESC");
                        busDeathNotification lobjDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
                        if (ldtbResult.Rows.Count > 0)
                        {
                            lobjDeathNotification.icdoDeathNotification.LoadData(ldtbResult.Rows[0]);
                            if ((lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusInProgress) ||
                                (lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusNonResponsive))
                            {
                                if ((iblnIsTotalPercentageLessthan100)||(IsDeathBeneficiaryExistsInPayeeGrid(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id)))
                                {
                                    iblnIsAnyDeathNotified = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if ((iblnIsTotalPercentageLessthan100)||(IsDeathBeneficiaryExistsInPayeeGrid(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id)))
                            {
                                iblnIsAnyDeathNotified = true;
                                break;
                            }
                        }
                    }
                }
            }
            return iblnIsAnyDeathNotified;
        }

        public bool IsDeathBeneficiaryExistsInPayeeGrid(int lintPersonID)
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();

            int lintCount= iclbBenefitCalculationPayee.Where(o=>o.icdoBenefitCalculationPayee.payee_person_id==lintPersonID).Count();

            if (lintCount > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsServicePurchaseAvailableInProgress()
        {
            if (iclbBenefitServicePurchaseAll == null)
                LoadBenefitServicePurchase();
            foreach (busBenefitServicePurchase lobjServicePurchase in iclbBenefitServicePurchaseAll)
            {
                if (lobjServicePurchase.ibusServicePurchaseHeader == null)
                    lobjServicePurchase.LoadServicePurchaseHeader();
                if (lobjServicePurchase.ibusServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value !=
                    busConstant.Service_Purchase_Action_Status_Paid_In_Full)
                    return true;
            }
            return false;
        }
		//Prod PIR: 4370. Extra VSC entered should be accounted inorder to find NRD Date.
		//Method to get Extra VSC entered.
        public decimal GetConsolidatedExtraServiceCredit()
        {
            decimal ldecConsolidatedExtraServiceCredit = 0.0M;
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
            {
                if (idecRemainingServiceCredit == 0.00M)
                    LoadRemainingServicePurchaseCredit();
                ldecConsolidatedExtraServiceCredit = idecRemainingServiceCredit + icdoBenefitCalculation.adjusted_tvsc;
            }
            return ldecConsolidatedExtraServiceCredit;
        }

        public void SetNormalRetirementDate(busDBCacheData abusDBCacheData = null)
        {
            if (ibusPlan == null)
                LoadPlan();
            if (ibusMember == null)
                LoadMember();
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            //Systest PIR: 1227. The Service Credit sent as the last parameter is the selected service purchase in the 
            //Estimate screen. For all others it should be zero.
            //Dont Modify the Code.
            decimal ldecConsolidatedServiceCredit = 0.0M;
            ldecConsolidatedServiceCredit = GetConsolidatedExtraServiceCredit();

            icdoBenefitCalculation.normal_retirement_date = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitCalculation.plan_id,
                                                            ibusPlan.icdoPlan.plan_code,
                                                            ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                            ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc,
                                                            icdoBenefitCalculation.is_rule_or_age_conversion, iobjPassInfo, icdoBenefitCalculation.termination_date,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate),
                                                            ldecConsolidatedServiceCredit, icdoBenefitCalculation.retirement_date, abusDBCacheData, ibusPersonAccount); //PIR 14646
        }

        public void GetCalculationByApplication(busPreRetirementDeathApplication aobjPreRetirementDeathApplication)
        {
            icdoBenefitCalculation.person_id = aobjPreRetirementDeathApplication.icdoBenefitApplication.member_person_id;
            if (ibusMember == null)
                LoadMember();
            icdoBenefitCalculation.date_of_death = ibusMember.icdoPerson.date_of_death;

            icdoBenefitCalculation.plan_id = aobjPreRetirementDeathApplication.icdoBenefitApplication.plan_id;
            if (ibusPlan == null)
                LoadPlan();
            icdoBenefitCalculation.benefit_account_type_value = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_account_type_value;
            icdoBenefitCalculation.benefit_account_type_description = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_account_type_description;

            //icdoBenefitCalculation.retirement_date = aobjPreRetirementDeathApplication.icdoBenefitApplication.retirement_date;
            //By default set the first of the month following the date of death as retirement date or benefit begin date
            DateTime ldteTempDate = icdoBenefitCalculation.date_of_death.AddMonths(1);
            DateTime ldtRetirementDate = new DateTime(ldteTempDate.Year, ldteTempDate.Month, 01);
            icdoBenefitCalculation.retirement_date = ldtRetirementDate;
            icdoBenefitCalculation.benefit_application_id = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_application_id;
            icdoBenefitCalculation.termination_date = aobjPreRetirementDeathApplication.icdoBenefitApplication.termination_date;
            icdoBenefitCalculation.plso_requested_flag = aobjPreRetirementDeathApplication.icdoBenefitApplication.plso_requested_flag;
            icdoBenefitCalculation.ssli_or_uniform_income_commencement_age = aobjPreRetirementDeathApplication.icdoBenefitApplication.ssli_age;
            icdoBenefitCalculation.uniform_income_or_ssli_flag = aobjPreRetirementDeathApplication.icdoBenefitApplication.uniform_income_flag;
            icdoBenefitCalculation.estimated_ssli_benefit_amount = aobjPreRetirementDeathApplication.icdoBenefitApplication.estimated_ssli_benefit_amount;
            icdoBenefitCalculation.reduced_benefit_flag = aobjPreRetirementDeathApplication.icdoBenefitApplication.reduced_benefit_flag;
            icdoBenefitCalculation.rhic_option_value = aobjPreRetirementDeathApplication.icdoBenefitApplication.rhic_option_value;
            icdoBenefitCalculation.rhic_option_description = aobjPreRetirementDeathApplication.icdoBenefitApplication.rhic_option_description;
            icdoBenefitCalculation.annuitant_id = aobjPreRetirementDeathApplication.icdoBenefitApplication.joint_annuitant_perslink_id;
            iintRetirementOrgId = aobjPreRetirementDeathApplication.icdoBenefitApplication.retirement_org_id;
            icdoBenefitCalculation.benefit_option_value = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_option_value;
            icdoBenefitCalculation.benefit_option_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2216, icdoBenefitCalculation.benefit_option_value);

            //PROD PIR ID 7101
            icdoBenefitCalculation.benefit_account_sub_type_value = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_sub_type_value;
            icdoBenefitCalculation.benefit_account_sub_type_description = aobjPreRetirementDeathApplication.icdoBenefitApplication.benefit_sub_type_description;
        }

        public void LoadBeneficiariesApplications()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPerson.iclbPersonBeneficiary == null)
                ibusPersonAccount.ibusPerson.LoadBeneficiary();

            iclbBeneficiariesApplications = new Collection<busBenefitApplication>();
            foreach (busPersonBeneficiary lobjBeneficiary in ibusPersonAccount.ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                {
                    busPerson lbusRecipientPerson = new busPerson();
                    lbusRecipientPerson.FindPerson(lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);
                    lbusRecipientPerson.LoadBenefitApplication();
                    foreach (busBenefitApplication lobjApplication in lbusRecipientPerson.iclbBenefitApplication)
                    {
                        if ((lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == lobjApplication.icdoBenefitApplication.recipient_person_id)
                            && (lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                        {
                            lobjApplication.LoadRecipient();
                            iclbBeneficiariesApplications.Add(lobjApplication);
                        }
                    }
                }
                if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                {
                    busOrganization lbusRecipientOrg = new busOrganization();
                    lbusRecipientOrg.FindOrganization(lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id);
                    lbusRecipientOrg.LoadBenefitApplication();
                    foreach (busBenefitApplication lobjApplication in lbusRecipientOrg.iclbBenefitApplication)
                    {
                        if ((lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id == lobjApplication.icdoBenefitApplication.payee_org_id)
                            && (lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                        {
                            lobjApplication.LoadRecipient();
                            iclbBeneficiariesApplications.Add(lobjApplication);
                        }
                    }
                }
            }
        }

        private Collection<busBenefitCalculationPayee> LoadBenefitCalculationPayeeForJobServiceMembers()
        {
            /* For JobService Member Beneficiary need not be set. The Spouse and Child Set up as Contact needs to be provided the Benefits. */

            Collection<busBenefitCalculationPayee> lclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            if (icdoBenefitCalculation.benefit_calculation_id > 0)
            {
                if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                {
                    DataTable ldtbList = Select<cdoBenefitCalculationPayee>(
                                                new string[1] { "benefit_calculation_id" },
                                                new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
                    lclbBenefitCalculationPayee = GetCollection<busBenefitCalculationPayee>(ldtbList, "icdoBenefitCalculationPayee");
                }
            }

            Collection<busBenefitCalculationPayee> iclbBenefitJobCalculationPayee = new Collection<busBenefitCalculationPayee>();

            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonContact == null)
                ibusMember.LoadActiveContacts();

            int lintCount = ibusMember.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                && (o.icdoPersonContact.contact_person_id > 0)).Count();

            if (lintCount > 0)
            {

                var icolTempPersonContact = ibusMember.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                       || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                       && (o.icdoPersonContact.contact_person_id > 0));

                iintNoofEligibleChild = 0;
                foreach (busPersonContact lobjTempPersonContact in icolTempPersonContact)
                {
                    busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee();
                    lobjPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();

                    lobjTempPersonContact.LoadContactPerson();
                    lobjPayee.ibusPayee = new busPerson();
                    lobjPayee.ibusPayee = lobjTempPersonContact.ibusContactPerson;

                    /*88888888888888888888888888888888888888888888888888888888888888888*/
                    /*1. Check whether Contact Is Eligible or not*/
                    /*88888888888888888888888888888888888888888888888888888888888888888*/                    
                    bool iblnIsTempContactEligible = false;
                    if (lobjTempPersonContact.ibusContactPerson.icdoPerson.date_of_death == DateTime.MinValue) //1. The Contact Should not be Dead
                    {
                        if (lobjTempPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                        {
                            decimal ldecChildAge = 0M;
                            CalculatePersonAge(lobjTempPersonContact.ibusContactPerson.icdoPerson.date_of_birth,
                                                                 icdoBenefitCalculation.date_of_death, ref ldecChildAge, 4);
                            //To Do: Add Case for Disabled Child also.To check with Maik.
                            //2. For Child Benefit: The Child should not be Married and should be less than Age 18.
                            //if ((ldecChildAge < 18.0M) && (lobjTempPersonContact.ibusContactPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried))
                            //{
                            iblnIsTempContactEligible = true;
                            iblnChildExists = true;
                            iintNoofEligibleChild = iintNoofEligibleChild + 1;
                            //}
                        }
                        else
                        {
                            iblnSpouseExists = true;
                            iblnIsTempContactEligible = true;
                        }
                    }
                    /*88888888888888888888888888888888888888888888888888888888888888888*/
                    if (iblnIsTempContactEligible)
                    {
                        lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                        lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = 0;
                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id = lobjTempPersonContact.icdoPersonContact.contact_person_id;
                        lobjPayee.icdoBenefitCalculationPayee.payee_org_id = lobjTempPersonContact.icdoPersonContact.contact_org_id;

                        lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth = lobjTempPersonContact.ibusContactPerson.icdoPerson.date_of_birth;
                        lobjPayee.icdoBenefitCalculationPayee.payee_first_name = lobjTempPersonContact.ibusContactPerson.icdoPerson.first_name;
                        lobjPayee.icdoBenefitCalculationPayee.payee_last_name = lobjTempPersonContact.ibusContactPerson.icdoPerson.last_name;

                        if (lobjTempPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = busConstant.PersonBeneficiaryRelationshipSpouse;
                            lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierSpouse;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipSpouse;
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = busConstant.PersonBeneficiaryRelationshipChild;
                            lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierChild;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipChild;
                        }
                        lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, lobjPayee.icdoBenefitCalculationPayee.family_relationship_value);
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value);

                        //To Set the Application and Benefit Option Details.                                            

                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                        {
                            DataTable ldtbResult = new DataTable();
                            //Since no calculation details will be available the Application details are fetched manually
                            ldtbResult = lobjPayee.LoadApplicationDetailsForRecipient(0,
                                                                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                                        icdoBenefitCalculation.person_id,
                                                                        icdoBenefitCalculation.plan_id,
                                                                        icdoBenefitCalculation.benefit_account_type_value);
                            if (ldtbResult.Rows.Count > 0)
                            {
                                lobjPayee.ibusBenefitApplication = new busBenefitApplication();
                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtbResult.Rows[0]);
                                lobjPayee.icdoBenefitCalculationPayee.benefit_application_id = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                            }
                        }
                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                        {
                            if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse)
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionSpouseBenefit;
                            }
                            else if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierChild)
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionChildTimeBenefit;
                            }
                        }                        

                        //PIR : 1639 ends
                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                        {
                            foreach (busBenefitCalculationPayee lobjBenefitPayee in lclbBenefitCalculationPayee)
                            {
                                if (((lobjPayee.icdoBenefitCalculationPayee.payee_person_id == lobjBenefitPayee.icdoBenefitCalculationPayee.payee_person_id) &&
                                    (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != 0)) ||
                                    ((lobjPayee.icdoBenefitCalculationPayee.payee_org_id == lobjBenefitPayee.icdoBenefitCalculationPayee.payee_org_id) &&
                                    (lobjPayee.icdoBenefitCalculationPayee.payee_org_id != 0)))
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjBenefitPayee.icdoBenefitCalculationPayee.payee_account_id;
                                }
                            }
                        }
                        iclbBenefitJobCalculationPayee.Add(lobjPayee);
                    }             
                }
            }
            return iclbBenefitJobCalculationPayee;
        }

        private bool IsBeneficiaryEligibleforBeneficiaryBenefit()
        {
            if (IsSpouseDeceased() && IsAllChildPayeeAccountInCompletedStatus())   //if spouse is deceased and no eligible children exists
            {
                return true;
            }
            return false;
        }

        private bool IsSpouseDeceased()
        {

            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonContact == null)
                ibusMember.LoadActiveContacts();

            int lintSpouseCount = ibusMember.icolPersonContact.Where(o =>
                        (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse) && (o.icdoPersonContact.contact_person_id > 0)).Count();

            foreach (busPersonContact lobjPersonContact in ibusMember.icolPersonContact)
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
        private bool IsAllChildPayeeAccountInCompletedStatus()
        {
            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonContact == null)
                ibusMember.LoadActiveContacts();

            int lintChildCount = ibusMember.icolPersonContact.Where(o =>
            (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild) && (o.icdoPersonContact.contact_person_id > 0)).Count();            
            foreach (busPersonContact lobjPersonContact in ibusMember.icolPersonContact)
            {
                if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild) && (lobjPersonContact.icdoPersonContact.contact_person_id > 0))
                {
                    lobjPersonContact.LoadContactPerson();

                    lobjPersonContact.ibusContactPerson.LoadBenefitApplication();

                    var lobjTempApplication = lobjPersonContact.ibusContactPerson.iclbBeneficiaryApplication.Where(o => (o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                        && (o.icdoBenefitApplication.member_person_id == icdoBenefitCalculation.person_id)
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
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void LoadBenefitCalculationPayeeForNewMode(bool ablIsFromMAS = false)
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (iclbBenefitCalculationPayee == null)
                iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();

            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
                && (!(IsBeneficiaryEligibleforBeneficiaryBenefit())))
            {
                iclbBenefitCalculationPayee = LoadBenefitCalculationPayeeForJobServiceMembers();
            }
            if(iclbBenefitCalculationPayee.Count == 0)
            {
                bool bIsspouseorChildExists = false;

                Collection<busBenefitCalculationPayee> lclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
                if (icdoBenefitCalculation.benefit_calculation_id > 0)
                {
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                    {
                        DataTable ldtbList = Select<cdoBenefitCalculationPayee>(
                                                    new string[1] { "benefit_calculation_id" },
                                                    new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
                        lclbBenefitCalculationPayee = GetCollection<busBenefitCalculationPayee>(ldtbList, "icdoBenefitCalculationPayee");
                    }
                }

                ibusMember.LoadBeneficiaryForMemberByPersonAccount(ibusPersonAccount.icdoPersonAccount.person_account_id);
                foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusMember.iclbBeneficiariesByPersonAccount)
                {
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary == null)
                        lobjPersonBeneficiary.LoadPersonAccountBeneficiary();                    

                    // SYSTEST PIR ID 1601 -- Beneficiary should be compared by start and date
                    if (busGlobalFunctions.CheckDateOverlapping(icdoBenefitCalculation.date_of_death,
                        lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                        lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date) &&
                        lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary) // PROD PIR 7076
                    {
                        busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee();
                        lobjPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                        lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                        lobjPayee.icdoBenefitCalculationPayee.benefit_percentage =
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent;
                        lobjPersonBeneficiary.LoadBeneficiaryInfo();
                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id;
                        lobjPayee.icdoBenefitCalculationPayee.payee_org_code = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_org_code;
                        lobjPayee.icdoBenefitCalculationPayee.payee_org_id = lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
                        lobjPayee.icdoBenefitCalculationPayee.payee_first_name = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_first_name;
                        lobjPayee.icdoBenefitCalculationPayee.payee_last_name = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_last_name; // UAT PIR ID 2335
                        lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_DOB;
                        lobjPayee.icdoBenefitCalculationPayee.payee_gender = lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_gender;
                        lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value;
                        lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, lobjPayee.icdoBenefitCalculationPayee.family_relationship_value);
                        if (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                        {
                            iblnSpouseExists = true;
                            if (IsJobService)
                            {
                                iblnSpouseExists = false;
                                //because for JobService the only identifier available if it comes from beneficiary is Other
                                lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierOther;
                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary; //Eventhough it is spouse...here it would be beneficiary..rare case
                            }
                            else
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierSpouse;
                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipJointAnnuitant;
                            }
                            lobjPayee.ibusPayee = new busPerson();
                            lobjPayee.ibusPayee.FindPerson(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id);

                            if (lobjPayee.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                            {
                                lobjPayee.iblnIsDeceased = true;
                                iblnSpouseExists = false;
                            }
                        }
                        else if ((lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipChild) ||
                            (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipAdoptedChild) ||
                            (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipDisabledChild) ||
                            (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipStepChild))
                        {
                            lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierOther;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierOther;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
                        }
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value);

                        // To Set the Benefit Option Name for Final

                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                        {
                            DataTable ldtbResult = new DataTable();
                            //Since no calculation details will be available the Application details are fetched manually
                            ldtbResult = lobjPayee.LoadApplicationDetailsForRecipient(lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                                        icdoBenefitCalculation.person_id,
                                                                        icdoBenefitCalculation.plan_id,
                                                                        icdoBenefitCalculation.benefit_account_type_value);
                            if (ldtbResult.Rows.Count > 0)
                            {
                                lobjPayee.ibusBenefitApplication = new busBenefitApplication();
                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtbResult.Rows[0]);
                                lobjPayee.icdoBenefitCalculationPayee.benefit_application_id = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                                if (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionAutoRefund)
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                                }
                            }

                            if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                            {
                                if (IsJobService)
                                {
                                    if (IsJobService)
                                    {
                                        //when it comes to this loop then for Jobservice, it means there are no Active Contacts. Possible Options
                                        // are 1) For Vested: Beneficiary Benefit Or 10 yr Term Certain 2) Non Vested: Refund

                                        //Step:1 Check Whether the Person Is vested or not.                                      
                                        if (IsMemberVested())
                                        {
                                            //Beneficiary benefit
                                            /*Spouse and/or Children to be set up as Contacts for automated benefit to work.
                                                a.	Beneficiaries only come into play when Spouse is deceased and any payee account linked to the children is ‘Payment Complete’.
                                                ---> Beneficiaries will get a lump sum according to the beneficiary %. The Benefit Option is “Beneficiary Benefit” 
                                             --10 Yr Term Certain
                                                --No Spouse and/or Children
                                                --a.	Beneficiaries receive 10 Year term certain Pre Retirement Death benefit. Add Benefit Option 10 Yr certain to  Jobservice and the bene’s to be paid the monthly benefit * Beneficiaries Share %*/
                                            if (IsMemberEligibleFor10YrCertain())
                                            {
                                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOption10YearCertain;
                                            }
                                            else
                                            {
                                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionBeneficiaryBenefit;
                                            }

                                        }
                                        else
                                        {
                                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                                        }
                                    }
                                    else
                                        lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                                }
                            }
                        }

                        //PIR : 1639 starts
                        //The Account RelationShip cannot be Joint Annuitant but instead it should be Beneficiary in case of 'Pre-Retirement death'
                        // and the benefit Option is Refund /Auto Refund.
                        //Case checked only for Refund. Since taking Auto Refund as Refund is done already.
                        if ((lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse) &&
                            (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund))
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value);
                        }
                        //PIR : 1639 ends
                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                        {
                            foreach (busBenefitCalculationPayee lobjBenefitPayee in lclbBenefitCalculationPayee)
                            {
                                if (((lobjPayee.icdoBenefitCalculationPayee.payee_person_id == lobjBenefitPayee.icdoBenefitCalculationPayee.payee_person_id) &&
                                    (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != 0)) ||
                                    ((lobjPayee.icdoBenefitCalculationPayee.payee_org_id == lobjBenefitPayee.icdoBenefitCalculationPayee.payee_org_id) &&
                                    (lobjPayee.icdoBenefitCalculationPayee.payee_org_id != 0)))
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjBenefitPayee.icdoBenefitCalculationPayee.payee_account_id;
                                }

                            }
                        }

                        if (ablIsFromMAS)
                        {
                            // PROD PIR ID 7329 - RHIC benefit will not calculated if more than one beneficiary exists.
                            // In MAS, we are displaying for Spouse only and hence add only the Spouse Payee.
                            if (lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                                iclbBenefitCalculationPayee.Add(lobjPayee);
                        }
                        else
                        {
                            iclbBenefitCalculationPayee.Add(lobjPayee);
                        }
                    }
                }                
            }
        }

        private bool IsMemberEligibleFor10YrCertain()
        {
            
            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonContact == null)
                ibusMember.LoadActiveContacts();

            int lintCount = ibusMember.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                && (o.icdoPersonContact.contact_person_id > 0)).Count();

            if (lintCount == 0)          
                return true;           

            return false;
        }


        public void SetPayeeIdentifier()
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if ((lobjPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipJointAnnuitant) ||
                    (lobjPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse))
                {
                    lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierSpouse;
                    iblnSpouseExists = true;
                }
                else if ((lobjPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipChild) ||
                    (lobjPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipChild))
                {
                    lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierChild;
                    iblnChildExists = true;
                }
                else
                    lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierOther;

                //If the Calculation Tuype is Final then already the benefit option is set everytime, so no need to set it again.
                if (icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeFinal)
                {
                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = icdoBenefitCalculation.benefit_option_value;
                }
            }
        }

        public void SetPayeeBenefitOption()
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            bool bIsspouseorChildExists = false;
            SetPayeeIdentifier();
            if (string.IsNullOrEmpty(icdoBenefitCalculation.benefit_option_value))
            {
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (IsJobService)
                    {
                        if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionSpouseBenefit;
                            bIsspouseorChildExists = true;
                        }
                        else if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierChild)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionChildTimeBenefit;
                            bIsspouseorChildExists = true;
                        }
                        else
                        {
                            if (bIsspouseorChildExists)
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.NotApplicable;
                            }
                            else
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                            }
                        }
                    }
                    //else
                    //    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;   
                }
            }
            else
            {
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = icdoBenefitCalculation.benefit_option_value;
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            // pir 8704
            else
            {
                SetPayeeBenefitOption();
            }
            //Reloading and Resetting the  Boolean Varaibles for Recalculating the objects.
            iblnBenefitMultiplierLoaded = false;
            iblnConsoldatedVSCLoaded = false;
            iblnConsolidatedPSCLoaded = false;

            if (icdoBenefitCalculation.annuitant_age == 0M)
                CalculateAnnuitantAge();
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            if (icdoBenefitCalculation.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoBenefitCalculation.suppress_warnings_by = iobjPassInfo.istrUserID;

            // Delete the Calculation Details if there is any change in the Application Details.
            DeleteBenefitCalculationDetails();
            DeleteBenefitCalculationPayeeDetails();

            // CALCULATE BENEFIT 
            CalculatePreRetirementDeathBenefit();

            //PIR 24243
            if (IsPersonReached401aFlag() && icdoBenefitCalculation.overridden_final_average_salary > 0 &&
               (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal))
            {
                icdoBenefitCalculation.computed_final_average_salary = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2010 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2019 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.fas_2020 = icdoBenefitCalculation.overridden_final_average_salary;
                icdoBenefitCalculation.calculation_final_average_salary = icdoBenefitCalculation.overridden_final_average_salary;
            }

            if (icdoBenefitCalculation.ienuObjectState == ObjectState.Insert)
            {
                icdoBenefitCalculation.Insert();
            }
            else //PIR 15966
            {
                icdoBenefitCalculation.recalculated_by = iobjPassInfo.istrUserID;
            }

            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            int lintResult = base.PersistChanges();
            if (iblnIsNewMode)
            {
                // Insert the Benefit Calculation Person Account only once in case of Insert.
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusBenefitCalculationPersonAccount == null)
                    ibusBenefitCalculationPersonAccount =
                        new busBenefitCalculationPersonAccount { icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount() };
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                ibusBenefitCalculationPersonAccount.icdoBenefitCalculationPersonAccount.Insert();

                // Make the Action Status initially to Pending Approval.
                icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusPending;
            }
            return lintResult;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            CreateBenefitCalculationDetails();
            CreateBenefitCalculationPayeeDetails();
            EvaluateInitialLoadRules();

            // Set Normal Retirement Date
            SetNormalRetirementDate();

            // Calculate Member Age
            CalculateMemberAge();

            // Refresh Benefit Payee
            LoadBenefitCalculationPayee();

            // PIR ID 1681
            // Refresh Benefit Options Grid From DB
            int lintCounter = 0;
            foreach (busBenefitCalculationOptions lobjBenOptions in iclbBenefitCalculationOptions)
            {
                int lintBenefitPayeeID = 0;
                lintBenefitPayeeID = iclbBenefitCalculationPayee.Where(o => o.icdoBenefitCalculationPayee.payee_sort_order == lobjBenOptions.icdoBenefitCalculationOptions.payee_sort_order).FirstOrDefault().icdoBenefitCalculationPayee.benefit_calculation_payee_id;

                if (lintBenefitPayeeID > 0)
                {
                    lobjBenOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id = lintBenefitPayeeID;
                    lobjBenOptions.icdoBenefitCalculationOptions.Update();
                }

            }
            SetPayeeIdentifier();
            LoadBenefitCalculationOptions();

            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                //if (lbusActivityInstance.ibusProcessInstance == null)
                //    lbusActivityInstance.LoadProcessInstance();
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit
                    || busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Initialize_PreRetirement_Death_Workflow)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
            }

            icdoBenefitCalculation.is_member_vested = IsMemberVested() ? busConstant.Flag_Yes : busConstant.Flag_No;
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
            {
                SetAllApplicationstoReview();
            }
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (lbusActivityInstance.ibusBpmProcessInstance == null)
                {
                    lbusActivityInstance.LoadBpmProcessInstance();
                }
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
            }
        }

        /// <summary>
        /// ************************************************************************ 
        ///                 DETERMINES THE PRE RETIREMENT DEATH BENEFIT AMOUNT 
        /// ************************************************************************
        /// </summary>
        public void CalculatePreRetirementDeathBenefit()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();

            if (!iblnConsolidatedPSCLoaded)
                CalculateConsolidatedPSC();
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                CalculateQDROAmount(true);
            else
                CalculateQDROAmount(false);

            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService) && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate))
            {
                LoadBenefitCalculationPayeeForNewMode();
            }

            iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            SetPayeeIdentifier();
            iclbBenefitCalculationOptions = CalculateAvailableDeathBenefitOptions();
            CalculateRHICOptions();

            // ********* CALCULATE MINIMUM GUARANTEE AMOUNT *********
            CalculateMinimumGuaranteedMemberAccount();

            // UCS-093 Calculate Required Minimum Distribution Amount
            if (icdoBenefitCalculation.annuitant_age == 0M)
                CalculateAnnuitantAge();
            //PIR 24911
            //if (idecMemberAgeAsonDateofDeath == 0M)
            //    CalculateMemberAge();

            decimal ldecMemberAgeAsofDateofDeath = 0;
            CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAgeAsofDateofDeath, 4);
            idecMemberAgeAsonDateofDeath = ldecMemberAgeAsofDateofDeath;

            if ((idecMemberAgeAsonDateofDeath != 0M) && (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund))
                icdoBenefitCalculation.rmd_amount = CalculateRMDAmount(idecMemberAgeAsonDateofDeath,
                                                    busConstant.ApplicationBenefitTypePreRetirementDeath, icdoBenefitCalculation.member_account_balance,icdoBenefitCalculation.retirement_date);
        }

        public void CheckIsPersonEligibleforNormal()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();
            if (ibusPersonAccount.ibusPlan == null)
                ibusPersonAccount.LoadPlan();

            if (ibusMember == null)
                LoadMember();

            DateTime ldtDateToCompare = DateTime.MinValue;
            ldtDateToCompare = icdoBenefitCalculation.retirement_date;
            decimal ldecTotalVSC = 0.0M;
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            ldecTotalVSC = icdoBenefitCalculation.credited_vsc;

            int lintTotalMonths = 0;
            decimal ldecMemberAge = 0.0M;

            lintTotalMonths = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, ldtDateToCompare, ref ldecMemberAge, 4);

            if (ibusPlan == null)
                LoadPlan();
            //this is set to null in order to check old value of benefit sub type is not retained for validation
            icdoBenefitCalculation.benefit_account_sub_type_value = null;            
            SetDualFASTerminationDate();
            decimal ldecConsolidatedExtraServiceCredit = GetConsolidatedExtraServiceCredit();

            icdoBenefitCalculation.normal_retirement_date = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitCalculation.plan_id, ibusPlan.icdoPlan.plan_code,
                                                            ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                            ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc,
                                                            icdoBenefitCalculation.is_rule_or_age_conversion, iobjPassInfo,icdoBenefitCalculation.fas_termination_date,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate),
                                                            ldecConsolidatedExtraServiceCredit, icdoBenefitCalculation.retirement_date, null, ibusPersonAccount); //PIR 14646

            if (CheckIsPersonEligibleforNormal(icdoBenefitCalculation.plan_id,
                                             ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id,
                                            icdoBenefitCalculation.benefit_account_type_value,
                                            ldecMemberAge,
                                            lintTotalMonths
                                            , ldecTotalVSC,
                                            ldtDateToCompare, ibusPersonAccount, iobjPassInfo, icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate),
                                            icdoBenefitCalculation.fas_termination_date, ldecConsolidatedExtraServiceCredit))
            {
                icdoBenefitCalculation.benefit_account_sub_type_value = busConstant.ApplicationBenefitSubTypeNormal; // PROD PIR ID 6137
            }
        }

        public Collection<busBenefitCalculationOptions> CalculateAvailableDeathBenefitOptions()
        {
            bool lblnIsAllOptionRefund = true;
            Collection<busBenefitCalculationOptions> lclbBenefitOptions = new Collection<busBenefitCalculationOptions>();

            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayeeForNewMode();

            if (string.IsNullOrEmpty(icdoBenefitCalculation.benefit_account_sub_type_value))
                CheckIsPersonEligibleforNormal();
            int lintBenefitPayeeSortOrder = 0;

            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                lintBenefitPayeeSortOrder = lintBenefitPayeeSortOrder + 1;
                //Systest PIR: 1473
                string lstrBenefitOptionValue = lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option;
                if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
                    && (string.IsNullOrEmpty(lstrBenefitOptionValue))
                    && (iclbBenefitCalculationPayee.Count > 1) && (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService))
                {
                    lstrBenefitOptionValue = busConstant.BenefitOptionRefund;
                }

                if((lstrBenefitOptionValue != busConstant.BenefitOptionRefund) && (lstrBenefitOptionValue != busConstant.BenefitOptionBeneficiaryBenefit))
                {
                    lblnIsAllOptionRefund = false;
                }
                Collection<cdoCodeValue> lclbBenefitOptionByPlan = new Collection<cdoCodeValue>();
                lclbBenefitOptionByPlan = GetAvailableBenefitOptionsForBeneficiary(lstrBenefitOptionValue,
                    lobjPayee.icdoBenefitCalculationPayee.payee_identifier);
                //lclbBenefitOptionByPlan = GetAvailableBenefitOptionsForBeneficiary(string.Empty,lobjPayee.icdoBenefitCalculationPayee.payee_identifier);
                //lclbBenefitOptionByPlan = LoadBenefitOptionsBasedOnPlans();
                foreach (cdoCodeValue lcdoBenefitOption in lclbBenefitOptionByPlan)
                {
                    string lstrTempBenefitOptionFactor = busGlobalFunctions.GetData3ByCodeValue(1903, lcdoBenefitOption.data1, iobjPassInfo);
                    if ((lstrTempBenefitOptionFactor == busConstant.BenefitOption100PercentJS) &&
                    (icdoBenefitCalculation.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeNormal))
                    {
                        continue;
                    }
                    else if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
                    || (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                    || (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG)
                    || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLEWithoutPS)
                    || (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf) // pir 7943
                    || (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                    {
                        if ((lstrTempBenefitOptionFactor == busConstant.BenefitOption50Percent) &&
                        (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal))
                        {
                            continue;
                        }
                    }
                    if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService) && (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == "CHLD")
                         && (lstrTempBenefitOptionFactor == busConstant.BenefitOptionBeneficiaryBenefit))
                    {
                        continue;
                    }

                    if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService) && (lstrTempBenefitOptionFactor == busConstant.BenefitOptionBeneficiaryBenefit))
                    {
                        if ((iblnChildExists) || (iblnSpouseExists))
                        {
                            continue;
                        }
                    }

                    if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService) && (IsMemberVested()) && (lstrTempBenefitOptionFactor == busConstant.BenefitOptionRefund))
                    {
                        continue;
                    }

                    decimal ldecpre_tax_ee_amount_ltd = 0.0M;
                    decimal ldecpost_tax_ee_amount_ltd = 0.0M;
                    decimal ldecee_er_pickup_amount_ltd = 0.0M;
                    decimal ldecinterest_amount_ltd = 0.0M;
                    decimal ldecer_vested_amount_ltd = 0.0M;
                    decimal ldecee_rhic_amount_ltd = 0.0M;
                    decimal ldecCapital_gain = 0.0M;
                    decimal ldecpost_tax_ee_ser_pur_cont_ltd = 0.0M;
                    decimal ldecee_rhic_ser_pur_cont_ltd = 0.0M;
                    decimal ldecpre_tax_ee_ser_pur_cont_ltd = 0.0M;
                    decimal ldecTotalTaxableAmount = 0.0M;
                    decimal ldecTotalNonTaxableAmount = 0.0M;

                    //Call Benefit Provision Benefit Option Function
                    busBenefitCalculationOptions lobjBenefitCalculationOptions = new busBenefitCalculationOptions();
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions();
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    decimal ldecBenefitOptionAmount = 0M; decimal ldecBenfitOptionFactor = 0M; int lintBenefitProvisionOptionId = 0;
                    if ((lobjPayee.icdoBenefitCalculationPayee.payee_identifier == "SPOU") &&
                        (lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth != DateTime.MinValue))
                    {
                        decimal ldecSpouseAge = 0.0M;
                        CalculatePersonAge(lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth, icdoBenefitCalculation.date_of_death, ref ldecSpouseAge, 4);
                        icdoBenefitCalculation.annuitant_age = ldecSpouseAge;
                    }

                    CalculateBenefitAmountForOption(lstrTempBenefitOptionFactor,
                                                        lobjPayee.icdoBenefitCalculationPayee.benefit_percentage,
                                                        lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth,
                                                        ref ldecBenefitOptionAmount, ref ldecBenfitOptionFactor, ref lintBenefitProvisionOptionId,
                                                        lobjPayee.icdoBenefitCalculationPayee.payee_identifier,
                                                        ref  ldecpre_tax_ee_amount_ltd, ref  ldecpost_tax_ee_amount_ltd, ref  ldecee_er_pickup_amount_ltd,
                                                        ref  ldecinterest_amount_ltd, ref  ldecer_vested_amount_ltd, ref  ldecee_rhic_amount_ltd,
                                                        ref  ldecCapital_gain, ref  ldecpost_tax_ee_ser_pur_cont_ltd, ref  ldecee_rhic_ser_pur_cont_ltd,
                                                        ref  ldecpre_tax_ee_ser_pur_cont_ltd, ref ldecTotalTaxableAmount, ref ldecTotalNonTaxableAmount);
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount = ldecBenefitOptionAmount;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.option_factor = ldecBenfitOptionFactor;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_provision_benefit_option_id = lintBenefitProvisionOptionId;

                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee = new busBenefitCalculationPayee();
                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id =
                        lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value =
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value;
                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_description =
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_description;
                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value =
                        lobjPayee.icdoBenefitCalculationPayee.family_relationship_value;
                    lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_description =
                        lobjPayee.icdoBenefitCalculationPayee.family_relationship_description;

                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                        lobjBenefitCalculationOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_person_id =
                            lobjPayee.icdoBenefitCalculationPayee.payee_person_id;

                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.pre_tax_ee_amount = ldecpre_tax_ee_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.post_tax_ee_amount = ldecpost_tax_ee_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_er_pickup_amount = ldecee_er_pickup_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.interest_amount = ldecinterest_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.er_vested_amount = ldecer_vested_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_rhic_amount = ldecee_rhic_amount_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.capital_gain = ldecCapital_gain;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.post_tax_ee_ser_pur_cont = ldecpost_tax_ee_ser_pur_cont_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.ee_rhic_ser_pur_cont = ldecee_rhic_ser_pur_cont_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.pre_tax_ee_ser_pur_cont = ldecpre_tax_ee_ser_pur_cont_ltd;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.taxable_amount = ldecTotalTaxableAmount;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.non_taxable_amount = ldecTotalNonTaxableAmount;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.payee_sort_order = lintBenefitPayeeSortOrder;
                    lclbBenefitOptions.Add(lobjBenefitCalculationOptions);

                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount = ldecBenefitOptionAmount;
                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_provision_benefit_option_id = lintBenefitProvisionOptionId;
                    lobjPayee.icdoBenefitCalculationPayee.payee_sort_order = lintBenefitPayeeSortOrder;
                }
            }

            int lintObjectCount = 0;
            decimal ldecSumofMemberAccount = 0;
            foreach (busBenefitCalculationOptions lobjBenefitOptions in lclbBenefitOptions)
            {
                lintObjectCount++;
                lobjBenefitOptions.LoadBenefitCalculationPayee();
                lobjBenefitOptions.LoadBenefitProvisionOption();
                ldecSumofMemberAccount = ldecSumofMemberAccount + lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount;

                if ((lblnIsAllOptionRefund) && (lintObjectCount == lclbBenefitOptions.Count))
                {
                    //Setting up the Last penny
                    lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount = lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount +
                        (icdoBenefitCalculation.member_account_balance - ldecSumofMemberAccount);
                    lobjBenefitOptions.icdoBenefitCalculationOptions.taxable_amount = lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount - lobjBenefitOptions.icdoBenefitCalculationOptions.non_taxable_amount;
                    if (IsJobService)
                    {
                        lobjBenefitOptions.icdoBenefitCalculationOptions.taxable_amount = Slice(lobjBenefitOptions.icdoBenefitCalculationOptions.taxable_amount, 2);
                    }
                    lobjBenefitOptions.icdoBenefitCalculationOptions.non_taxable_amount = lobjBenefitOptions.icdoBenefitCalculationOptions.benefit_option_amount - lobjBenefitOptions.icdoBenefitCalculationOptions.taxable_amount;
                    if (IsJobService)
                    {
                        lobjBenefitOptions.icdoBenefitCalculationOptions.non_taxable_amount = Slice(lobjBenefitOptions.icdoBenefitCalculationOptions.non_taxable_amount, 2);
                    }
                }
            }
            return lclbBenefitOptions;
        }

        public DataTable idtbBenOptionCodeValue { get; set; } //Code ID 1903
        public DataTable idtbDeathBenOptionCodeValue { get; set; } //Code ID 2406

        public Collection<cdoCodeValue> GetAvailableBenefitOptionsForBeneficiary(string astrBenefitOption, string astrPayeeIdentifier)
        {
            string astrUniqueBenefitOption = string.Empty;
            Collection<cdoCodeValue> lclbRHICOptionDetails = new Collection<cdoCodeValue>();
            DataTable ldtbResult1 = new DataTable();
            if (!(string.IsNullOrEmpty(astrPayeeIdentifier)))
            {
                if (string.IsNullOrEmpty(astrBenefitOption))
                {
                    lclbRHICOptionDetails = LoadDeathBenefitOptionsBasedOnPlans(icdoBenefitCalculation.plan_id, icdoBenefitCalculation.benefit_account_type_value,
                                            astrPayeeIdentifier, istrIsVestedAndMarried, iobjPassInfo, idtbDeathBenOptionCodeValue);
                }
                else
                {
                    if (idtbBenOptionCodeValue == null)
                        idtbBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1903);
                    DataTable ldtbResult = idtbBenOptionCodeValue.AsEnumerable().Where(i => i.Field<string>("data1") == icdoBenefitCalculation.plan_id.ToString() &&
                                                                i.Field<string>("data2") == icdoBenefitCalculation.benefit_account_type_value &&
                                                                i.Field<string>("data3") == astrBenefitOption).AsDataTable();
                    if (ldtbResult.Rows.Count > 0)
                    {
                        astrUniqueBenefitOption = Convert.ToString(ldtbResult.Rows[0]["code_value"]);
                    }


                    if (idtbDeathBenOptionCodeValue == null)
                        idtbDeathBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2406);

                    ldtbResult1 = idtbDeathBenOptionCodeValue.AsEnumerable().Where(i => i.Field<string>("data1") == astrUniqueBenefitOption &&
                                                                i.Field<string>("data2") == istrIsVestedAndMarried &&
                                                                i.Field<string>("data3") == astrPayeeIdentifier).AsDataTable();
                    foreach (DataRow dr in ldtbResult1.Rows)
                    {
                        cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                        lcdoCodeValue.LoadData(dr);
                        lclbRHICOptionDetails.Add(lcdoCodeValue);
                    }
                }
            }
            return lclbRHICOptionDetails;
        }

        public static Collection<cdoCodeValue> LoadDeathBenefitOptionsBasedOnPlans(int aintPlanId, string astrBenefitAccountType,
                                                        string astrBenefitPayeeIdentifier, string astrIsVestedAndMarried, utlPassInfo aobjPassInfo, DataTable adtbDeathBenOption = null)
        {
            Collection<cdoCodeValue> lclcBenefitOption = new Collection<cdoCodeValue>();
            if ((!string.IsNullOrEmpty(astrBenefitAccountType)) &&
                (!string.IsNullOrEmpty(astrBenefitPayeeIdentifier)) &&
                (!string.IsNullOrEmpty(astrIsVestedAndMarried)))
            {
                Collection<busCodeValue> lclbBenefitOption = busGlobalFunctions.LoadCodeValueByData1(1903, aintPlanId.ToString());
                foreach (busCodeValue lobjCodeValue in lclbBenefitOption)
                {
                    if (lobjCodeValue.icdoCodeValue.data2 == astrBenefitAccountType)
                    {
                        if (adtbDeathBenOption == null)
                            adtbDeathBenOption = aobjPassInfo.isrvDBCache.GetCodeValues(2406);

                        DataTable ldtbResult = adtbDeathBenOption.AsEnumerable().Where(i => i.Field<string>("data1") == lobjCodeValue.icdoCodeValue.code_value &&
                                                                    i.Field<string>("data2") == astrIsVestedAndMarried &&
                                                                    i.Field<string>("data3") == astrBenefitPayeeIdentifier).AsDataTable();
                        if (ldtbResult.Rows.Count > 0)
                        {
                            busCodeValue lobjCodeValueNew = new busCodeValue();
                            lobjCodeValueNew.icdoCodeValue = new cdoCodeValue();
                            lobjCodeValueNew.icdoCodeValue.LoadData(ldtbResult.Rows[0]);
                            lclcBenefitOption.Add(lobjCodeValueNew.icdoCodeValue);
                        }
                    }
                }
            }
            lclcBenefitOption = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order", lclcBenefitOption);
            return lclcBenefitOption;
        }

        public void CalculateBenefitAmountForOption(string astrBenefitOptionType, decimal adecBeneficiaryPercentage, DateTime adtePayeeDOB,
                                    ref decimal adecBeneficiaryOptionAmount, ref decimal adecBenefitOptionFactor, ref int aintBenefitProvisionOptionID,
                                    string astrPayeeIdentifier, ref  decimal ldecpre_tax_ee_amount_ltd, ref  decimal ldecpost_tax_ee_amount_ltd,
                                    ref  decimal ldecee_er_pickup_amount_ltd, ref decimal ldecinterest_amount_ltd, ref  decimal ldecer_vested_amount_ltd,
                                    ref  decimal ldecee_rhic_amount_ltd, ref decimal ldecCapital_gain, ref  decimal ldecpost_tax_ee_ser_pur_cont_ltd,
                                    ref  decimal ldecee_rhic_ser_pur_cont_ltd, ref decimal ldecpre_tax_ee_ser_pur_cont_ltd, ref decimal adecTotalTaxableAmount,
                                    ref decimal adecTotalNonTaxableAmount)
        {
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            ibusBenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption();
            ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption();
            LoadBenefitProvisionBenefitOption(icdoBenefitCalculation.benefit_account_type_value, astrBenefitOptionType,
                                                icdoBenefitCalculation.retirement_date,
                                                ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id);
            adecBenefitOptionFactor = 0M;
            adecBeneficiaryOptionAmount = 0M;

            if ((astrBenefitOptionType != "REFD") && (astrBenefitOptionType != busConstant.BenefitOptionBeneficiaryBenefit))
            {
                // ********* CALCULATE FAS *********
                if (iclbBenefitCalculationFASMonths == null)
                    iclbBenefitCalculationFASMonths = new Collection<busBenefitCalculationFasMonths>();
                CalculateFAS();

                // ********* CALCULATE BENEFIT MULTIPLIER *********
                if (!iblnBenefitMultiplierLoaded)
                    CalculateBenefitMultiplier();

                // ********* CALCULATE UNREDUCED BENEFIT AMOUNT *********
                decimal adecReducedAmount1 = 0.0M;
                decimal adecReducedAmount2 = 0.0M;
                icdoBenefitCalculation.unreduced_benefit_amount =
                                                            CalculateUnReducedMonthlyBenefitAmount(
                                                            icdoBenefitCalculation.calculation_final_average_salary,
                                                            idecFASBenefitMultiplier, icdoBenefitCalculation.plan_id,
                                                            icdoBenefitCalculation.benefit_account_type_value,
                                                            adecReducedAmount1, adecReducedAmount2,
                                                            ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_formula_value);
                icdoBenefitCalculation.reduced_monthly_after_plso_deduction = icdoBenefitCalculation.unreduced_benefit_amount - icdoBenefitCalculation.qdro_amount;
            }
            Collection<busBenefitProvisionBenefitOption> lclbProvisionBenefitOptions = new Collection<busBenefitProvisionBenefitOption>();
            lclbProvisionBenefitOptions = GetAvailableOptionsDetails(
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id,
                                                        DateTime.Today,
                                                        icdoBenefitCalculation.benefit_account_type_value,
                                                        astrBenefitOptionType,
                                                        string.Empty);
            // Load all the Benefit Provision Options
            foreach (busBenefitProvisionBenefitOption lobjProvisionOption in lclbProvisionBenefitOptions)
            {
                aintBenefitProvisionOptionID = lobjProvisionOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                if ((string.IsNullOrEmpty(lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value) ||
                    lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueFactor))
                {
                    adecBeneficiaryOptionAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction *
                                                  ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor;
                    adecBenefitOptionFactor = ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor;
                }
                if (lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueMemberAndSurvivorAge)
                {
                    decimal ldecMemberAge = 0M;
                    decimal ldecSpouseAge = 0M;
                    CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAge, 4);
                    CalculatePersonAge(adtePayeeDOB, icdoBenefitCalculation.retirement_date, ref ldecSpouseAge, 4);
                    adecBenefitOptionFactor = GetOptionFactorsForPlan(ldecMemberAge, ldecSpouseAge, icdoBenefitCalculation.benefit_account_type_value,
                                            icdoBenefitCalculation.plan_id, astrBenefitOptionType);
                    adecBeneficiaryOptionAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction * adecBenefitOptionFactor;
                }
                if ((lobjProvisionOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueOther) &&
                    (IsJobService))
                {
                    adecBenefitOptionFactor = 1;
                    if (astrPayeeIdentifier == "SPOU")
                        adecBeneficiaryOptionAmount = GetJSSpouseDeathBenefit();
                    else if ((astrPayeeIdentifier == "CHLD") && (astrBenefitOptionType != busConstant.BenefitOptionBeneficiaryBenefit))
                        adecBeneficiaryOptionAmount = GetJSChildDeathBenefit();
                    else if(astrBenefitOptionType == busConstant.BenefitOption10YearCertain)
                        adecBeneficiaryOptionAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction * adecBeneficiaryPercentage / 100;
                }
            }
            decimal idecMemberAccountBalance = 0.0M;
            if ((astrBenefitOptionType == "REFD") || (astrBenefitOptionType == busConstant.BenefitOptionBeneficiaryBenefit))
            {
                adecBeneficiaryOptionAmount = GetRefundBenefit(adecBeneficiaryPercentage, ref idecMemberAccountBalance, ref  ldecpre_tax_ee_amount_ltd,
                                                                ref  ldecpost_tax_ee_amount_ltd, ref  ldecee_er_pickup_amount_ltd,
                                                                ref  ldecinterest_amount_ltd, ref  ldecer_vested_amount_ltd, ref  ldecee_rhic_amount_ltd,
                                                                ref  ldecCapital_gain, ref  ldecpost_tax_ee_ser_pur_cont_ltd, ref  ldecee_rhic_ser_pur_cont_ltd,
                                                                ref  ldecpre_tax_ee_ser_pur_cont_ltd, astrBenefitOptionType, ref adecTotalTaxableAmount, ref adecTotalNonTaxableAmount);
                icdoBenefitCalculation.member_account_balance = idecMemberAccountBalance;
                adecBenefitOptionFactor = 1;
            }

            if (IsJobService)
            {
                adecBeneficiaryOptionAmount = adecBeneficiaryOptionAmount - icdoBenefitCalculation.paid_up_annuity_amount;
                adecBeneficiaryOptionAmount = Slice(adecBeneficiaryOptionAmount, 2);
            }
            else
            {
                adecBeneficiaryOptionAmount = Math.Round(adecBeneficiaryOptionAmount, 2);
            }

            if (astrBenefitOptionType != "REFD" && astrBenefitOptionType != busConstant.BenefitOptionBeneficiaryBenefit)
            {
                icdoBenefitCalculation.final_monthly_benefit = adecBeneficiaryOptionAmount;

                if (icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes)
                {
                    // Spouse Monthly Benefit, The Monthly Non-taxable is not calculating> should equals the "Exclusion Method" Simplified Method
                    if (ibusPersonAccount == null) LoadPersonAccount();
                    if (ibusPersonAccount.ibusPersonAccountRetirement.IsNull())  ibusPersonAccount.LoadPersonAccountRetirement();
                    decimal adecTotalPaidTaxableAmount = 0.0M; decimal adecTotalPaidNonTaxableAmount = 0.0M; decimal ldecNonTaxableMemberAccountBalance = 0M;
                    DateTime ldteTerminationDate = icdoBenefitCalculation.date_of_death.AddMonths(1).GetLastDayofMonth();
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
                    {
                        ldteTerminationDate = icdoBenefitCalculation.retirement_date;
                        if (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes)
                            ldteTerminationDate = icdoBenefitCalculation.termination_date.GetLastDayofMonth();
                    }
                    ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDate,
                                    icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes);
                    if (astrBenefitOptionType == busConstant.BenefitOptionBeneficiaryBenefit)
                    {
                        FetchAlreadyPaidAmountForSpouseandChild(ref adecTotalPaidTaxableAmount, ref adecTotalPaidNonTaxableAmount);
                        ldecNonTaxableMemberAccountBalance = (ldecNonTaxableMemberAccountBalance - adecTotalPaidNonTaxableAmount > 0) ? ldecNonTaxableMemberAccountBalance - adecTotalPaidNonTaxableAmount : 0;
                    }
                    ldecNonTaxableMemberAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;

                    decimal ldecSpouseAge = 0M; decimal ldecExclusionRatioAmount = 0M; decimal ldecMemberAge = 0M;
                    CalculatePersonAge(adtePayeeDOB, icdoBenefitCalculation.retirement_date, ref ldecSpouseAge, 4);
                    CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAge, 4);
                    string lstrExclusiontypeValue = string.Empty;
                    if (astrBenefitOptionType == busConstant.ApplicationBenefitOptionValueData3Value50JointSurvivor ||
                        astrBenefitOptionType == busConstant.ApplicationBenefitOptionValueData3Value55JointSurvivor ||
                        astrBenefitOptionType == busConstant.ApplicationBenefitOptionValueData3Value75JointSurvivor ||
                        astrBenefitOptionType == busConstant.ApplicationBenefitOptionValueData3Value100JointSurvivor)
                        lstrExclusiontypeValue = busConstant.ExclusionCalcPaymentTypeJointLife;
                    else if (astrBenefitOptionType == busConstant.BenefitOptionSingleLife)
                        lstrExclusiontypeValue = busConstant.ExclusionCalcPaymentTypeSingleLife;
                    CalculateMonthlyTaxComponents(icdoBenefitCalculation.created_date, ldecMemberAge, ldecSpouseAge, lstrExclusiontypeValue, ldecNonTaxableMemberAccountBalance,
                            adecBeneficiaryOptionAmount, 0, 0, ref adecTotalNonTaxableAmount, ref adecTotalTaxableAmount, ref ldecExclusionRatioAmount, adecBeneficiaryPercentage);
                }
            }
        }

        public decimal GetJSSpouseDeathBenefit()
        {
            //The Values 40% and 55% can  be moved to busConstants.	idecMonthlyBasedAnnuityAmount = icdoBenefitCalculation.UnreducedMonthlyBenefitAmount 
            decimal ldecFAS = 0M;
            decimal ldecMonthlyBasedAnnuityAmount = icdoBenefitCalculation.reduced_monthly_after_plso_deduction;
            ldecFAS = (icdoBenefitCalculation.computed_final_average_salary * 40) / 100;
            if (ldecMonthlyBasedAnnuityAmount > ldecFAS)
                return ((ldecMonthlyBasedAnnuityAmount * 55) / 100);
            else
                return ((ldecFAS * 55) / 100);
        }

        public decimal GetJSChildDeathBenefit()
        {
            bool lblnSpouseExists = false;
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse)
                {
                    lblnSpouseExists = true;
                    break;
                }
            }
            decimal ldecFASPercentage = 0M;
            decimal ldecDefaultChildAmount = 0M;
            decimal ldecDefaultChildNumeratorAmount = 0M;
            if (lblnSpouseExists)
            {
                ldecFASPercentage = 60.0M / 100.0M;
                ldecDefaultChildAmount = 75;
                ldecDefaultChildNumeratorAmount = 225;
            }
            else
            {
                ldecFASPercentage = 75.0M / 100.0M;
                ldecDefaultChildAmount = 90;
                ldecDefaultChildNumeratorAmount = 270;
            }
            decimal ldecFASChildAmount = icdoBenefitCalculation.computed_final_average_salary * ldecFASPercentage;
            decimal ldecCalculatedChildAmount = 0M;
            decimal lintNumberofEligibleChildren = CalculateNumberofEligibleChildForJobservice();

            if (lintNumberofEligibleChildren != 0)
                ldecCalculatedChildAmount = ldecDefaultChildNumeratorAmount / lintNumberofEligibleChildren;
            // Find the Least of the Three
            decimal ldecChildDeathBenefit = ldecFASChildAmount;
            if (ldecChildDeathBenefit > ldecCalculatedChildAmount)
                ldecChildDeathBenefit = ldecCalculatedChildAmount;
            if (ldecChildDeathBenefit > ldecDefaultChildAmount)
                ldecChildDeathBenefit = ldecDefaultChildAmount;
            return ldecChildDeathBenefit;
        }

        public int CalculateNumberofEligibleChildForJobservice()
        {
            int lintNumberofEligibleChild = 0;
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();

            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierChild)
                {
                  lintNumberofEligibleChild += 1;
                }
            }            
            return lintNumberofEligibleChild;
        }


        public void CalculateRHICOptions()
        {
            if (ibusJointAnnuitant == null)
                LoadJointAnnuitant();
            if (!iblnConsolidatedPSCLoaded)
                CalculateConsolidatedPSC();
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            LoadBenefitProvisionBenefitOption();

            //Systest PIR: 1473
            //if there are multiple Payee then the only Option possible is Refund. So no need to Calculate RHIC Amount when the Payee Count is > 1
            if (iclbBenefitCalculationPayee.Count == 1)
            {
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    //RHIC Needs to be calculated only only if it is a Monthly Benefit
                    if ((lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse)
                        && (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option != busConstant.BenefitOptionRefund))
                    {
                        icdoBenefitCalculation.standard_rhic_amount = CalculateStandardRHICAmount(
                                                        Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero),
                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor,
                                                        icdoBenefitCalculation.plan_id);
                        icdoBenefitCalculation.unreduced_rhic_amount = icdoBenefitCalculation.standard_rhic_amount;
                        icdoBenefitCalculation.rhic_factor_amount = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.rhic_service_factor;
                        string lstrBenefitOption;
                        lstrBenefitOption = lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option;

                        Collection<busBenefitProvisionBenefitOption> lclbProvisionBenefitOption =
                                GetAvailableOptionsDetails(ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id,
                                icdoBenefitCalculation.created_date, icdoBenefitCalculation.benefit_account_type_value, lstrBenefitOption, string.Empty);

                        Collection<cdoCodeValue> lclbBenefitOptionByPlan = new Collection<cdoCodeValue>();
                        lclbBenefitOptionByPlan = GetAvailableBenefitOptionsForBeneficiary(lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option,
                        lobjPayee.icdoBenefitCalculationPayee.payee_identifier);

                        iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();
                        foreach (busBenefitProvisionBenefitOption lobjBenefitProvisionBenefitOption in lclbProvisionBenefitOption)
                        {

                            //The option returned should be available in the Beneficiary's Allowed options
                            bool blnIsOptionExists = false;
                            foreach (cdoCodeValue lcdoBenefitOption in lclbBenefitOptionByPlan)
                            {
                                string lstrTempBenefitOptionFactor = busGlobalFunctions.GetData3ByCodeValue(1903, lcdoBenefitOption.data1, iobjPassInfo);
                                if (lstrTempBenefitOptionFactor == lobjBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value)
                                {
                                    blnIsOptionExists = true;
                                    break;
                                }
                            }

                            if (blnIsOptionExists == false)
                            {
                                continue;
                            }

                            if ((lobjBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption100PercentJS) &&
                                (icdoBenefitCalculation.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeNormal))
                            {
                                continue;
                            }
                            else if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
                            || (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020) //PIR 20232
                            || (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG)
                            || (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety) //PIR 25729
                            || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE) || (icdoBenefitCalculation.plan_id == busConstant.PlanIdLEWithoutPS)
                            || (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf)) // pir 7943
                            {
                                if ((lobjBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption50Percent) &&
                                (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal))
                                {
                                    continue;
                                }
                            }

                            if ((lobjBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionRefund)
                                || ((lobjBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionAutoRefund)))
                            {
                                continue;
                            }
                            busBenefitRHICOption lobjSpouseStandardRHICOption = new busBenefitRHICOption();
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption = new cdoBenefitRhicOption();
                            lobjSpouseStandardRHICOption.ibusBenefitProvisionOption = new busBenefitProvisionBenefitOption();
                            lobjSpouseStandardRHICOption.ibusBenefitProvisionOption.icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption();
                            lobjSpouseStandardRHICOption.ibusBenefitProvisionOption = lobjBenefitProvisionBenefitOption;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.benefit_provision_benefit_option_id =
                                        lobjSpouseStandardRHICOption.ibusBenefitProvisionOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.option_factor = 0;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.rhic_option_value = busConstant.RHICOptionStandard;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.rhic_option_description =
                                   iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, busConstant.RHICOptionStandard);
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.member_rhic_amount = 0;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_percentage = 1;
                            lobjSpouseStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_amount =
                                           Math.Round(icdoBenefitCalculation.standard_rhic_amount, 2, MidpointRounding.AwayFromZero);
                            iclbBenefitRHICOption.Add(lobjSpouseStandardRHICOption);
                            break;
                        }
                    }
                }
            }
        }

        public decimal GetRefundBenefit(decimal adecBeneficiaryPercentage, ref decimal adecMemberAccountBalance,
            ref decimal adecpre_tax_ee_amount_ltd, ref decimal adecpost_tax_ee_amount_ltd, ref decimal adecee_er_pickup_amount_ltd,
            ref decimal adecinterest_amount_ltd, ref decimal adecer_vested_amount_ltd, ref decimal adecee_rhic_amount_ltd,
            ref decimal adecCapital_gain, ref decimal adecpost_tax_ee_ser_pur_cont_ltd, ref decimal adecee_rhic_ser_pur_cont_ltd,
            ref decimal adecpre_tax_ee_ser_pur_cont_ltd, string astrBenefitOptionType, ref decimal adectotaltaxableAmount, ref decimal adectotalnontaxableAmount)
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            busPersonAccountRetirement lobjMemberAccountRetirement = new busPersonAccountRetirement();
            lobjMemberAccountRetirement.icdoPersonAccount = new cdoPersonAccount();
            lobjMemberAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            DateTime ldteTerminationDate = new DateTime();
            decimal adecTotalPaidTaxableAmount=0.0M;
            decimal adecTotalPaidNonTaxableAmount=0.0M;

            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
                //&& (icdoBenefitCalculation.termination_date == DateTime.MinValue)
                )
                ldteTerminationDate = icdoBenefitCalculation.date_of_death.AddMonths(1).GetLastDayofMonth();
            else
                ldteTerminationDate = icdoBenefitCalculation.termination_date.GetLastDayofMonth(); // PROD PIR ID 4851

            lobjMemberAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDate, icdoBenefitCalculation.benefit_account_type_value, false, icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund);

            adecMemberAccountBalance = lobjMemberAccountRetirement.Member_Account_Balance_ltd -
                                        (icdoBenefitCalculation.taxable_qdro_amount + icdoBenefitCalculation.non_taxable_qdro_amount);

            if (astrBenefitOptionType == busConstant.BenefitOptionRefund)
            {
                adecpre_tax_ee_amount_ltd = lobjMemberAccountRetirement.pre_tax_ee_amount_ltd - idecquadro_ee_pre_tax_amount;
                adecpost_tax_ee_amount_ltd = lobjMemberAccountRetirement.post_tax_ee_amount_ltd - idecquadro_ee_post_tax_amount;
                adecee_er_pickup_amount_ltd = lobjMemberAccountRetirement.ee_er_pickup_amount_ltd - idecquadro_ee_er_pickup_amount;
                adecinterest_amount_ltd = lobjMemberAccountRetirement.interest_amount_ltd - idecquadro_interest_amount;
                adecer_vested_amount_ltd = lobjMemberAccountRetirement.er_vested_amount_ltd - idecquadro_er_vested_amount;
                adecee_rhic_amount_ltd = lobjMemberAccountRetirement.ee_rhic_amount_ltd;
                adecCapital_gain = lobjMemberAccountRetirement.icdoPersonAccountRetirement.capital_gain - idecquadro_capital_gain;
                adecpost_tax_ee_ser_pur_cont_ltd = lobjMemberAccountRetirement.post_tax_ee_ser_pur_cont_ltd;
                adecee_rhic_ser_pur_cont_ltd = lobjMemberAccountRetirement.ee_rhic_ser_pur_cont_ltd;
                adecpre_tax_ee_ser_pur_cont_ltd = lobjMemberAccountRetirement.pre_tax_ee_ser_pur_cont_ltd;

                adecpre_tax_ee_amount_ltd = adecpre_tax_ee_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecpost_tax_ee_amount_ltd = adecpost_tax_ee_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecee_er_pickup_amount_ltd = adecee_er_pickup_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecinterest_amount_ltd = adecinterest_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecer_vested_amount_ltd = adecer_vested_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecee_rhic_amount_ltd = adecee_rhic_amount_ltd * (adecBeneficiaryPercentage / 100);
                adecCapital_gain = adecCapital_gain * (adecBeneficiaryPercentage / 100);
                adecpost_tax_ee_ser_pur_cont_ltd = adecpost_tax_ee_ser_pur_cont_ltd * (adecBeneficiaryPercentage / 100);
                adecee_rhic_ser_pur_cont_ltd = adecee_rhic_ser_pur_cont_ltd * (adecBeneficiaryPercentage / 100);
                adecpre_tax_ee_ser_pur_cont_ltd = adecpre_tax_ee_ser_pur_cont_ltd * (adecBeneficiaryPercentage / 100);
                adectotaltaxableAmount = lobjMemberAccountRetirement.Pre_Tax_Employee_Contribution_ltd * (adecBeneficiaryPercentage / 100);
                adectotalnontaxableAmount = lobjMemberAccountRetirement.Post_Tax_Total_Contribution_ltd * (adecBeneficiaryPercentage / 100);
            }
            else
            {//For beneficiary benefit
                FetchAlreadyPaidAmountForSpouseandChild(ref  adecTotalPaidTaxableAmount, ref  adecTotalPaidNonTaxableAmount);
                adectotaltaxableAmount = 0;
                adectotalnontaxableAmount = 0;

                if (adecTotalPaidTaxableAmount < lobjMemberAccountRetirement.Pre_Tax_Employee_Contribution_ltd)
                {
                    adectotaltaxableAmount = Slice((lobjMemberAccountRetirement.Pre_Tax_Employee_Contribution_ltd - adecTotalPaidTaxableAmount)                       
                        * (adecBeneficiaryPercentage / 100),2);
                }

                if (adecTotalPaidNonTaxableAmount < lobjMemberAccountRetirement.Post_Tax_Total_Contribution_ltd)
                {
                    adectotalnontaxableAmount = Slice((lobjMemberAccountRetirement.Post_Tax_Total_Contribution_ltd - adecTotalPaidNonTaxableAmount)
                        * (adecBeneficiaryPercentage / 100),2);
                }                

                if(adecMemberAccountBalance - (adecTotalPaidTaxableAmount + adecTotalPaidNonTaxableAmount) <0)
                {
                    adecMemberAccountBalance =0;
                }
                else
                {
                    adecMemberAccountBalance= Slice(adecMemberAccountBalance - (adecTotalPaidTaxableAmount + adecTotalPaidNonTaxableAmount),2);
                }
            }

            return (adecMemberAccountBalance * (adecBeneficiaryPercentage / 100));
        }

        // Final Screen Approve Button Click Event handled
        public ArrayList btnApprove_Clicked()
        {
            ArrayList larrErrors = new ArrayList();
            //PIR 15600
            if (icdoBenefitCalculation.rhic_effective_date == DateTime.MinValue
                && !(icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || //PIR 23590
                icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025)) //PIR 25920
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10279, "");
                larrErrors.Add(lobjError);
            }
            this.ValidateHardErrors(utlPageMode.All);

            bool lblnRMDAmountNotReduced = false;
            if (larrErrors.Count == 0)
            {
                if (iclbBenefitCalculationPayee != null)
                {
                    int lintBenefitAccountID = 0; int lintPayeeAccountID = 0;
                    decimal ldecPreTaxContribution = 0M;
                    decimal ldecPostTaxContribution = 0M;
                    decimal ldecPayeeAccountPreTaxContribution = 0.0M;
                    decimal ldecPayeeAccountPostTaxContribution = 0.0M;
                    decimal ldecExclusionRatioAmount = 0M;
                    decimal ldecTaxableAmount = 0M;
                    decimal ldecNonTaxableAmount = 0M;

                    ldecPayeeAccountPreTaxContribution = icdoBenefitCalculation.taxable_amount - icdoBenefitCalculation.taxable_qdro_amount;
                    ldecPreTaxContribution = icdoBenefitCalculation.taxable_amount - icdoBenefitCalculation.taxable_qdro_amount;
                    ldecPayeeAccountPostTaxContribution = icdoBenefitCalculation.non_taxable_amount - icdoBenefitCalculation.non_taxable_qdro_amount;
                    ldecPostTaxContribution = icdoBenefitCalculation.non_taxable_amount - icdoBenefitCalculation.non_taxable_qdro_amount;

                    if (iclbBenefitCalculationPayeeFromDB == null)
                    {
                        iclbBenefitCalculationPayeeFromDB = new Collection<busBenefitCalculationPayee>();
                        iclbBenefitCalculationPayeeFromDB = LoadBenefitCalculationPayeeFromDB();
                    }

                    DateTime ldteTermCertainEndDate = DateTime.MinValue;
                    if (IsTermCertainBenefitOption())
                    {
                       //Payee Account Benefit Begin date.
                        int lintNumberofYears = GetTermCertainYears(icdoBenefitCalculation.benefit_option_value);
                        ldteTermCertainEndDate = icdoBenefitCalculation.retirement_date.AddMonths(lintNumberofYears);
                    }

                    // 1. For all existing Payee
                    int lintPayeeCounter = 0;
                    foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                    {
                        if (lobjPayee.ibusBenefitApplication == null)
                            lobjPayee.LoadBenefitApplication();
                        // 2. Only if the Application is Valid/Verified.
                        if (((lobjPayee.ibusBenefitApplication.icdoBenefitApplication.status_value == busConstant.StatusValid) &&
                            (lobjPayee.ibusBenefitApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)) ||
                            (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments &&
                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.status_value == busConstant.StatusProcessed &&
                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified))
                        {
                            // Load Benefit Option Corresponding to this Payee.
                            busBenefitCalculationOptions lobjPayeeBenefitOption =
                                                    new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                            if (lintPayeeCounter < iclbBenefitCalculationOptions.Count)
                            {
                                lobjPayeeBenefitOption = iclbBenefitCalculationOptions[lintPayeeCounter];
                            }
                            else
                            {
                                continue;
                            }

                            decimal ldecpre_tax_ee_amount_ltd = 0.0M;
                            decimal ldecpost_tax_ee_amount_ltd = 0.0M;
                            decimal ldecee_er_pickup_amount_ltd = 0.0M;
                            decimal ldecinterest_amount_ltd = 0.0M;
                            decimal ldecer_vested_amount_ltd = 0.0M;
                            decimal ldecee_rhic_amount_ltd = 0.0M;
                            decimal ldecCapital_gain = 0.0M;
                            decimal ldecpost_tax_ee_ser_pur_cont_ltd = 0.0M;
                            decimal ldecee_rhic_ser_pur_cont_ltd = 0.0M;
                            decimal ldecpre_tax_ee_ser_pur_cont_ltd = 0.0M;
                            decimal ldecBeneTaxableAmount = 0.0M;
                            decimal ldecBeneNonTaxableAmount = 0.0M;

                            if (lobjPayeeBenefitOption != null)
                            {
                                ldecpre_tax_ee_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.pre_tax_ee_amount +
                                    lobjPayeeBenefitOption.icdoBenefitCalculationOptions.pre_tax_ee_ser_pur_cont;
                                ldecpost_tax_ee_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.post_tax_ee_amount +
                                                               lobjPayeeBenefitOption.icdoBenefitCalculationOptions.post_tax_ee_ser_pur_cont;
                                ldecee_er_pickup_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.ee_er_pickup_amount;
                                ldecinterest_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.interest_amount;
                                ldecer_vested_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.er_vested_amount;
                                ldecee_rhic_amount_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.ee_rhic_amount +
                                                        lobjPayeeBenefitOption.icdoBenefitCalculationOptions.ee_rhic_ser_pur_cont;
                                ldecCapital_gain = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.capital_gain;
                                ldecpost_tax_ee_ser_pur_cont_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.post_tax_ee_ser_pur_cont; ;
                                ldecee_rhic_ser_pur_cont_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.ee_rhic_ser_pur_cont;
                                ldecpre_tax_ee_ser_pur_cont_ltd = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.pre_tax_ee_ser_pur_cont;

                                ldecBeneTaxableAmount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.taxable_amount;
                                ldecBeneNonTaxableAmount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.non_taxable_amount;
                                //Prod PIR:4760 Payee benefit Amount needs to set when not set since while Approving this property also needs to be present.
								if (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount <= 0.0M)
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount;
                                }
                            }

                            // ldecQDROAmount is just a provision. Will be revisited and can be removed when not needed.
                            decimal ldecQDROAmount = 0.0M;

                            DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                            DateTime ldteBenefitBeginDate = new DateTime();
							//UAT PIR: 2076. Benefit Begin Date set as Retirement Date.
                            ldteBenefitBeginDate = icdoBenefitCalculation.retirement_date;

                            decimal ldecSpouseRHICAmount = 0M;
                            if ((lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse)
                                && ((lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option != busConstant.BenefitOptionRefund)))
                                ldecSpouseRHICAmount = icdoBenefitCalculation.unreduced_rhic_amount;

                            // 3. Creates or Updates the Benefit Account
                            lintBenefitAccountID = IsBenefitAccountExists(ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            icdoBenefitCalculation.benefit_account_type_value);
                            lintBenefitAccountID = ManageBenefitAccount(ldecPreTaxContribution, ldecPostTaxContribution,
                                                            busConstant.StatusValid, icdoBenefitCalculation.rhic_option_value, 0,
                                                            icdoBenefitCalculation.credited_psc, icdoBenefitCalculation.credited_vsc,
                                                            iintRetirementOrgId, lintBenefitAccountID, DateTime.MinValue, 0.0M, string.Empty, 0M, 0M,ldecSpouseRHICAmount);

                            string lstrBenefitAccountSubType = busConstant.BenefitAccountSubTypePreRetirementDeathBenefit;
                            decimal ldecNonTaxableBeginningBalance = (ldecPostTaxContribution < 0) ? 0M : ldecPostTaxContribution;
                            if (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund)
                            {
                                ldecNonTaxableBeginningBalance = Math.Round((ldecNonTaxableBeginningBalance *
                                                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage) / 100M, 2, MidpointRounding.AwayFromZero);
                                lstrBenefitAccountSubType = busConstant.BenefitAccountSubTypePreRetirementDeathRefund;
                            }


                            // 4. Creates or Updates the Payee Account
                            if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id == 0)
                            {
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                            lintBenefitAccountID,
                                                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                            icdoBenefitCalculation.benefit_account_type_value, true);
                            }
                            else
                            {
                                lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                            lintBenefitAccountID,
                                                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                            icdoBenefitCalculation.benefit_account_type_value, false);
                            }
                            decimal ldecPayeeMinimumGuaranteeAmount = 0.0M;
                            ldecPayeeMinimumGuaranteeAmount = icdoBenefitCalculation.minimum_guarentee_amount * ((lobjPayee.icdoBenefitCalculationPayee.benefit_percentage) / 100M);

                            //UCS - 079 : if payee account exists and calculation type is Adjustments, need to store history for minimum guarantee amount
                            //--Start--//
                            if (lintPayeeAccountID > 0 && icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                            {
                                //loading payee account bus. object
                                busPayeeAccount lobjPA = new busPayeeAccount();
                                lobjPA.FindPayeeAccount(lintPayeeAccountID);
                                //Creating minimum guarantee history
                                busPayeeAccountMinimumGuaranteeHistory lobjMinimumGuaranteeHistory = new busPayeeAccountMinimumGuaranteeHistory();
                                lobjMinimumGuaranteeHistory.CreateMinimumGuaranteeHistory(lobjPA.icdoPayeeAccount.payee_account_id,
                                                                                            lobjPA.icdoPayeeAccount.minimum_guarantee_amount,
                                                                                            DateTime.Now,
                                                                                            busConstant.Flag_No);

                            }
                            //--End--//
                            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                            {
                                lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id,
                                                                icdoBenefitCalculation.benefit_calculation_id,
                                                                lintBenefitAccountID, 0, busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                                lstrBenefitAccountSubType, busConstant.Flag_No, DateTime.MinValue, DateTime.MinValue,
                                                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                                lobjPayee.icdoBenefitCalculationPayee.family_relationship_value,
                                                                ldecPayeeMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance,
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option, ldecSpouseRHICAmount,
                                                                lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, ldteTermCertainEndDate
                                                                , string.Empty, 0,icdoBenefitCalculation.graduated_benefit_option_value);
                            }
                            else
                            {
                                lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id,
                                                                icdoBenefitCalculation.benefit_calculation_id,
                                                                lintBenefitAccountID, 0, busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                                lstrBenefitAccountSubType, busConstant.Flag_No, ldteBenefitBeginDate, DateTime.MinValue,
                                                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                                lobjPayee.icdoBenefitCalculationPayee.family_relationship_value,
                                                                ldecPayeeMinimumGuaranteeAmount, ldecNonTaxableBeginningBalance,
                                                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option, ldecSpouseRHICAmount,
                                                                lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M, ldteTermCertainEndDate
                                                                , string.Empty, 0,icdoBenefitCalculation.graduated_benefit_option_value);
                            }
                            lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lintPayeeAccountID;

                            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                            if (ibusBaseActivityInstance.IsNotNull())
                            {
                                lobjPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                                // lobjPayeeAccount.SetProcessInstanceParameters();
                                lobjPayeeAccount.SetCaseInstanceParameters();
                            }
                            if (lobjPayee.ibusBenefitProvisionBenefitOption == null)
                                lobjPayee.LoadBenefitProvisionBenefitOption();                            

                            // 4. If Benefit Option is not Refund Manage Benefit Account
                            if ((lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option != busConstant.BenefitOptionRefund)
                                && (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option !=busConstant.BenefitOptionBeneficiaryBenefit))
                            {
                                decimal ldecSpouseAge = 0.0M;
                                CalculatePersonAge(lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth,
                                                                icdoBenefitCalculation.retirement_date, ref ldecSpouseAge, 4);
                                CalculateMonthlyTaxComponents(icdoBenefitCalculation.created_date, ldecSpouseAge, 0,
                                                    lobjPayee.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value,
                                                    ldecPayeeAccountPostTaxContribution, lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount, 0,
                                                    0, ref ldecNonTaxableAmount, ref ldecTaxableAmount, ref ldecExclusionRatioAmount,
                                                    lobjPayee.icdoBenefitCalculationPayee.benefit_percentage);

                                if (ldecTaxableAmount > 0M)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount, ldecTaxableAmount, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                                //UCS - 079 : If new amount is 0.0M, need to end date existing one
                                else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount, 0, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                                if (ldecNonTaxableAmount > 0M)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, ldecNonTaxableAmount, string.Empty, 0,
                                                                            ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                                //UCS - 079 : If new amount is 0.0M, need to end date existing one
                                else if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, 0, string.Empty, 0,
                                                                            ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                            }
                            else
                            {
                                decimal ldecpretaxeeamountltd = Math.Round(ldecpre_tax_ee_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldecposttaxeeamountltd = Math.Round(ldecpost_tax_ee_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldeceeerpickupamountltd = Math.Round(ldecee_er_pickup_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldecinterestamountltd = Math.Round(ldecinterest_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldecervestedamountltd = Math.Round(ldecer_vested_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldeceerhicamountltd = Math.Round(ldecee_rhic_amount_ltd, 2, MidpointRounding.AwayFromZero);
                                decimal ldecCapitalgain = Math.Round(ldecCapital_gain, 2, MidpointRounding.AwayFromZero);

                                ldecBeneTaxableAmount = Math.Round(ldecBeneTaxableAmount, 2, MidpointRounding.AwayFromZero);
                                ldecBeneNonTaxableAmount = Math.Round(ldecBeneNonTaxableAmount, 2, MidpointRounding.AwayFromZero);

                                if (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund)
                                {
                                    // RMD Amount is reduced from the possible Taxable components and if not from Nontaxable component
                                    if ((icdoBenefitCalculation.rmd_amount > 0M) &&
                                       (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse))
                                    {
                                        if (ldecinterestamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldecinterestamountltd = ldecinterestamountltd - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldecpretaxeeamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldecpretaxeeamountltd = ldecpretaxeeamountltd = icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldeceeerpickupamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldeceeerpickupamountltd = ldeceeerpickupamountltd - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldecervestedamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldecervestedamountltd = ldecervestedamountltd - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldecCapitalgain >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldecCapitalgain = ldecCapitalgain - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldecposttaxeeamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldecposttaxeeamountltd = ldecposttaxeeamountltd - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else if (ldeceerhicamountltd >= icdoBenefitCalculation.rmd_amount)
                                        {
                                            ldeceerhicamountltd = ldeceerhicamountltd - icdoBenefitCalculation.rmd_amount;
                                        }
                                        else
                                            lblnRMDAmountNotReduced = true;

                                        if (lblnRMDAmountNotReduced)
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(4228, string.Empty);
                                            larrErrors.Add(lobjError);
                                            break;
                                        }
                                    }

                                    if (ldeceerhicamountltd > 0.0M)
                                    {
                                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemRHICEEAmount,
                                            ldeceerhicamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                    }
                                    // PROD PIR ID 5543
                                    // Do we need to skip these steps if the plan is DC, benefit option is Refund for Pre/Post retirement calculations ?
                                    // – Yes, skip for DC.  No PAPIT entries should be created unless RHIC needs to be refunded.
                                    // Confirmed mail dated Mon 2/21/2011 11:08 PM from Maik
                                    if (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                                        icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                                        icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                                    {
                                        if (ldeceeerpickupamountltd > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEERPickupAmount,
                                                ldeceeerpickupamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if (ldecinterestamountltd > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemEEInterestAmount,
                                                ldecinterestamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if (ldecposttaxeeamountltd > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPostTaxEEContributionAmount,
                                                ldecposttaxeeamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if (ldecpretaxeeamountltd > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemPreTaxEEContributionAmount,
                                                ldecpretaxeeamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if (ldecervestedamountltd > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemVestedERContributionAmount,
                                                ldecervestedamountltd, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if (ldecCapitalgain > 0.0M)
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemCapitalGain,
                                                ldecCapitalgain, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
                                        if ((icdoBenefitCalculation.rmd_amount > 0.0M) &&
                                           (lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse))
                                        {
                                            lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITRMDAmount,
                                                icdoBenefitCalculation.rmd_amount, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                        }
										//PIR 26960 Pre-Retirement Death refund RMD taxes did not create 
                                        if (lobjPayee.icdoBenefitCalculationPayee.payee_account_id > 0)
                                            CreateDefaultWithHolding(lobjPayee.icdoBenefitCalculationPayee.payee_account_id);
                                    }
                                }
                                else
                                {
                                    //RMD Amount is not Applicable for Beneficiary benefit
                                    if (ldecBeneTaxableAmount > 0.0M)
                                    {
                                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPSTDTaxableAmount,
                                            ldecBeneTaxableAmount, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                    }
                                    if (ldecBeneNonTaxableAmount > 0.0M)
                                    {
                                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPSTDNonTaxableAmount,
                                            ldecBeneNonTaxableAmount, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                    }
                                }

                            }
                            // Make the Payee Account Status to Review
                            lobjPayeeAccount.CreateReviewPayeeAccountStatus();

                            //UCS - 079 : If calculation type is Adjustments, then need to create Overpayment/Underpayment
                            //--Start of code --//
                            if (lintPayeeCounter == 0 && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal))
                            {
                                lobjPayeeAccount.CreateBenefitOverPaymentorUnderPayment(busConstant.BenRecalAdjustmentReasonRecalculation);
                            }
                            //--End of code --//
                            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption50PercentJS
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption100PercentJS
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption75Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption55Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption50Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionSpouseBenefit
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionMonthlyLifeTimeBenefit)
                            CreateRHICCombineOnCalculationApproval(lobjPayeeAccount, icdoBenefitCalculation.calculation_type_value);
                        }

                        foreach (busBenefitCalculationPayee lobjBenefitPayee in iclbBenefitCalculationPayeeFromDB)
                        {
                            if (((lobjBenefitPayee.icdoBenefitCalculationPayee.payee_person_id != 0) &&
                                (lobjBenefitPayee.icdoBenefitCalculationPayee.payee_person_id == lobjPayee.icdoBenefitCalculationPayee.payee_person_id)) ||
                                (lobjBenefitPayee.icdoBenefitCalculationPayee.payee_org_id != 0) &&
                                (lobjBenefitPayee.icdoBenefitCalculationPayee.payee_org_id == lobjPayee.icdoBenefitCalculationPayee.payee_org_id))
                            {
                                lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id =
                                    lobjBenefitPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                                lobjPayee.icdoBenefitCalculationPayee.update_seq = lobjBenefitPayee.icdoBenefitCalculationPayee.update_seq;
                                //pir 8704
                                lobjPayee.icdoBenefitCalculationPayee.account_relationship_id = lobjBenefitPayee.icdoBenefitCalculationPayee.account_relationship_id;
                                lobjPayee.icdoBenefitCalculationPayee.family_relationship_id = lobjBenefitPayee.icdoBenefitCalculationPayee.family_relationship_id;
                                lobjPayee.icdoBenefitCalculationPayee.payee_sort_order = lobjBenefitPayee.icdoBenefitCalculationPayee.payee_sort_order;
                            }
                        }

                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id == 0)
                            lobjPayee.icdoBenefitCalculationPayee.Insert();
                        else
                            lobjPayee.icdoBenefitCalculationPayee.Update();

                        lintPayeeCounter++;
                    }
                }
                if (!lblnRMDAmountNotReduced)
                {
                    icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusApproval;
                    icdoBenefitCalculation.action_status_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, icdoBenefitCalculation.action_status_value);
                    icdoBenefitCalculation.Update();
                }

                //PIR 18974
                IsBenefitOverpaymentBPMInitiate();

				//PIR 15966
                // Refresh the cdo
                icdoBenefitCalculation.Select();
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    lobjPayee.icdoBenefitCalculationPayee.Select();
                }
                larrErrors.Add(this);
                //Load initial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                SetAllApplicationstoReview();
                foreach (utlError lobjError in iarrErrors)
                {
                    larrErrors.Add(lobjError);
                }
            }
            return larrErrors;
        }

        // SYSTEST PIR ID 2128 - Change all the Applications status to Review if any error occurs in Applications.
        public void SetAllApplicationstoReview()
        {
            if (ibusMember.IsNull()) LoadMember();
            if (ibusMember.iclbBenefitApplication.IsNull()) ibusMember.LoadBenefitApplication();

            foreach (busBenefitApplication lobjApplication in ibusMember.iclbBenefitApplication)
            {
                if (lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    busPreRetirementDeathApplication lobjPreRetApplication = new busPreRetirementDeathApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPreRetApplication.icdoBenefitApplication = lobjApplication.icdoBenefitApplication;
                    lobjPreRetApplication.ValidateHardErrors(utlPageMode.Update);
                    if (lobjPreRetApplication.iarrErrors.Count > 0)
                    {
                        if ((lobjPreRetApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusValid) &&
                            (lobjPreRetApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified))
                        {
                            lobjPreRetApplication.icdoBenefitApplication.status_value = busConstant.ApplicationStatusReview;
                            lobjPreRetApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                            lobjPreRetApplication.icdoBenefitApplication.Update();
                        }
                    }
                }
            }
        }

        // SYSTEST PIR ID 2128 
        // Validating the Application's Hard errors and raise a Soft error,
        // which in turn changes the Application Status to Review in After Persists Changes
        public bool IsAnyApplicationsExistsPending()
        {
            if (ibusMember.IsNull()) LoadMember();
            if (ibusMember.iclbBenefitApplication.IsNull()) ibusMember.LoadBenefitApplication();

            foreach (busBenefitApplication lobjApplication in ibusMember.iclbBenefitApplication)
            {
				//PRod PIR:4777 While Checking for Error Exclude cancelled applications
                if ((lobjApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    && (lobjApplication.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled))
                {
                    busPreRetirementDeathApplication lobjPreRetApplication = new busPreRetirementDeathApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPreRetApplication.icdoBenefitApplication = lobjApplication.icdoBenefitApplication;
                    lobjPreRetApplication.ValidateHardErrors(utlPageMode.Update);
                    if (lobjPreRetApplication.iarrErrors.Count > 0)
                        return false;
                }
            }
            return true;
        }

        // SYS-PIR ID 1483 - Approve Button will be visible still all Payee's Payee Account created.
        public bool IsCalculationApproved()
        {
            if (iclbBenefitCalculationPayee != null)
            {
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (lobjPayee.icdoBenefitCalculationPayee.payee_account_id != 0)
                        return true;
                }
            }
            return false;
        }

        // SYS-PIR ID 1483 - Approve Button will be visible still all Payee's Payee Account created.
        public bool IsPayeeAccountCreatedForallPayees()
        {
            if (iclbBenefitCalculationPayee != null)
            {
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (lobjPayee.icdoBenefitCalculationPayee.payee_account_id == 0)
                        return false;
                }
            }
            return true;
        }

        // SYS-PIR ID 1477 - As Per Raj's Mail raise an error in Final calculation if employment is not end dated.
        public bool IsAllEmploymentTerminated()
        {
            if (ibusBenefitApplication == null)
                LoadBenefitApplication();
            return ibusBenefitApplication.IsAllEmploymentEndDated();
        }

        // SYS-PIR ID 1477 - As Per Raj's Mail raise an warning in Final calculation if there is no regular payroll line till termination date. 
        public bool IsContributedTillTerminationDate()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.iclbAllRetirementContributions == null)
                ibusMember.LoadAllRetirementContribution();

            // PIR 1477
            DateTime ldteTerminationDate = new DateTime();
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                (icdoBenefitCalculation.termination_date == DateTime.MinValue))
                ldteTerminationDate = icdoBenefitCalculation.date_of_death;
            else
                ldteTerminationDate = icdoBenefitCalculation.termination_date;

            var lvar = from obj in ibusMember.iclbAllRetirementContributions
                       where ((obj.icdoPersonAccountRetirementContribution.pay_period_month == ldteTerminationDate.Month) &&
                              (obj.icdoPersonAccountRetirementContribution.pay_period_year == ldteTerminationDate.Year) &&
                              ((obj.icdoPersonAccountRetirementContribution.subsystem_value == "PAYR") ||
                               (obj.icdoPersonAccountRetirementContribution.subsystem_value == "CONV")))
                       select obj;

            if (lvar.Count() > 0)
                return true;
            else
                return false;
        }

        //used in Employee death letter batch
        public bool iblnIsFromEmployeeDeathBatch { get; set; }

        /// <summary>
        /// method to load benefit calculation payee from MAS if no open beneficiary exists
        /// </summary>
        public void LoadMASBenefitCalculationPayeeForNewModeFromContact()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (iclbBenefitCalculationPayee == null)
                iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
                        
            if (iclbBenefitCalculationPayee.Count == 0)
            {                
                ibusMember.LoadContacts();
                busPersonContact lobjContact = ibusMember.icolPersonContact.Where(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse &&
                                                                                    o.icdoPersonContact.status_value == busConstant.PersonContactStatusActive).FirstOrDefault();
                if (lobjContact != null && lobjContact.icdoPersonContact.contact_person_id > 0)
                {
                    busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee();
                    lobjPayee.icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee();
                    lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = 100;
                    lobjContact.LoadContactPerson();
                    lobjPayee.icdoBenefitCalculationPayee.payee_person_id = lobjContact.icdoPersonContact.contact_person_id;
                    lobjPayee.icdoBenefitCalculationPayee.payee_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(lobjContact.icdoPersonContact.contact_org_id);
                    lobjPayee.icdoBenefitCalculationPayee.payee_org_id = lobjContact.icdoPersonContact.contact_org_id;
                    lobjPayee.icdoBenefitCalculationPayee.payee_first_name = lobjContact.ibusContactPerson.icdoPerson.first_name;
                    lobjPayee.icdoBenefitCalculationPayee.payee_last_name = lobjContact.ibusContactPerson.icdoPerson.last_name;
                    lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth = lobjContact.ibusContactPerson.icdoPerson.date_of_birth;
                    lobjPayee.icdoBenefitCalculationPayee.payee_gender = lobjContact.ibusContactPerson.icdoPerson.gender_value;
                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjContact.icdoPersonContact.relationship_value;
                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, lobjPayee.icdoBenefitCalculationPayee.family_relationship_value);

                    iblnSpouseExists = true;
                    if (IsJobService)
                    {
                        iblnSpouseExists = false;
                        //because for JobService the only identifier available if it comes from beneficiary is Other
                        lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierOther;
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary; //Eventhough it is spouse...here it would be beneficiary..rare case
                    }
                    else
                    {
                        lobjPayee.icdoBenefitCalculationPayee.payee_identifier = busConstant.PayeeIdentifierSpouse;
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipJointAnnuitant;
                    }
                    lobjPayee.ibusPayee = new busPerson();
                    lobjPayee.ibusPayee.FindPerson(lobjContact.icdoPersonContact.contact_person_id);

                    if (lobjPayee.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        lobjPayee.iblnIsDeceased = true;
                        iblnSpouseExists = false;
                    }


                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value);

                    // To Set the Benefit Option Name for Final

                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)//to handle when RecalculateBenefit is clicked from PA
                    {
                        DataTable ldtbResult = new DataTable();
                        //Since no calculation details will be available the Application details are fetched manually
                        ldtbResult = lobjPayee.LoadApplicationDetailsForRecipient(lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                                    lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                                    icdoBenefitCalculation.person_id,
                                                                    icdoBenefitCalculation.plan_id,
                                                                    icdoBenefitCalculation.benefit_account_type_value);
                        if (ldtbResult.Rows.Count > 0)
                        {
                            lobjPayee.ibusBenefitApplication = new busBenefitApplication();
                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtbResult.Rows[0]);
                            lobjPayee.icdoBenefitCalculationPayee.benefit_application_id = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                            if (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionAutoRefund)
                            {
                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                            }
                        }

                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                        {
                            if (IsJobService)
                            {
                                if (IsJobService)
                                {
                                    //when it comes to this loop then for Jobservice, it means there are no Active Contacts. Possible Options
                                    // are 1) For Vested: Beneficiary Benefit Or 10 yr Term Certain 2) Non Vested: Refund

                                    //Step:1 Check Whether the Person Is vested or not.                                      
                                    if (IsMemberVested())
                                    {
                                        //Beneficiary benefit
                                        /*Spouse and/or Children to be set up as Contacts for automated benefit to work.
                                            a.	Beneficiaries only come into play when Spouse is deceased and any payee account linked to the children is ‘Payment Complete’.
                                            ---> Beneficiaries will get a lump sum according to the beneficiary %. The Benefit Option is “Beneficiary Benefit” 
                                         --10 Yr Term Certain
                                            --No Spouse and/or Children
                                            --a.	Beneficiaries receive 10 Year term certain Pre Retirement Death benefit. Add Benefit Option 10 Yr certain to  Jobservice and the bene’s to be paid the monthly benefit * Beneficiaries Share %*/
                                        if (IsMemberEligibleFor10YrCertain())
                                        {
                                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOption10YearCertain;
                                        }
                                        else
                                        {
                                            lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionBeneficiaryBenefit;
                                        }

                                    }
                                    else
                                    {
                                        lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                                    }
                                }
                                else
                                    lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option = busConstant.BenefitOptionRefund;
                            }
                        }
                    }

                    //PIR : 1639 starts
                    //The Account RelationShip cannot be Joint Annuitant but instead it should be Beneficiary in case of 'Pre-Retirement death'
                    // and the benefit Option is Refund /Auto Refund.
                    //Case checked only for Refund. Since taking Auto Refund as Refund is done already.
                    if ((lobjPayee.icdoBenefitCalculationPayee.payee_identifier == busConstant.PayeeIdentifierSpouse) &&
                        (lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option == busConstant.BenefitOptionRefund))
                    {
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value);
                    }
                    
                    iclbBenefitCalculationPayee.Add(lobjPayee);
                }
            }
        }
    }
}
