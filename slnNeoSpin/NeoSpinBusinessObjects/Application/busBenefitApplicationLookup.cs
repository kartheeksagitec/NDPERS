#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitApplicationLookup : busBenefitApplicationLookupGen
    {
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busBenefitApplication lobjBenefitApplication = (busBenefitApplication)aobjBus;

            if (lobjBenefitApplication.icdoBenefitApplication.payee_org_id > 0)
            {
                lobjBenefitApplication.ibusApplicantOrganization = new busOrganization();
                lobjBenefitApplication.ibusApplicantOrganization.icdoOrganization = new cdoOrganization();
                lobjBenefitApplication.ibusApplicantOrganization.icdoOrganization.org_code = adtrRow["org_code"].ToString();
                lobjBenefitApplication.ibusApplicantOrganization.icdoOrganization.org_name = adtrRow["org_name"].ToString();
            }

            lobjBenefitApplication.ibusPersonAccount = new busPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.LoadData(adtrRow);

            lobjBenefitApplication.ibusPersonAccount.ibusPerson = new busPerson();
            lobjBenefitApplication.ibusPersonAccount.ibusPerson.icdoPerson = new cdoPerson();
            lobjBenefitApplication.ibusPersonAccount.ibusPerson.icdoPerson.LoadData(adtrRow);


            lobjBenefitApplication.ibusPersonAccount.ibusPlan = new busPlan();
            lobjBenefitApplication.ibusPersonAccount.ibusPlan.icdoPlan = new cdoPlan();
            lobjBenefitApplication.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(adtrRow);

            if (lobjBenefitApplication.icdoBenefitApplication.recipient_person_id > 0)
            {
                lobjBenefitApplication.ibusRecipient = new busPerson();
                lobjBenefitApplication.ibusRecipient.icdoPerson = new cdoPerson();
                lobjBenefitApplication.ibusRecipient.icdoPerson.first_name = adtrRow["RecipientFirstName"].ToString();
                lobjBenefitApplication.ibusRecipient.icdoPerson.middle_name = adtrRow["RecipientMiddleName"].ToString();
                lobjBenefitApplication.ibusRecipient.icdoPerson.last_name = adtrRow["RecipientLastName"].ToString();
            }
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            int lintPersonId = 0;
            int lintPlanId = 0;
            int lintPayeeAccountID = 0;
            int lintPersonAccountId = 0;
            int lintOrgId = 0;
            int lintRecepientPersonId = 0;
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            busPerson lobjAccountOwnnerPerson = new busPerson();
            DataTable ldtbGetCountOfPersonAccount;


            if (ahstParam["plan_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(1057, "");
                larrErrors.Add(lobjError);
            }
            if (((ahstParam["benefit_type"].ToString()) == "") || (ahstParam["benefit_type"].ToString()) == "ALL")
            {
                utlError lobjError = null;
                lobjError = AddError(1901, "");
                larrErrors.Add(lobjError);
            }

            if (ahstParam["aint_person_id"].ToString() == "")
            {
                utlError lobjError = null;
                lobjError = AddError(1902, "");
                larrErrors.Add(lobjError);
            }
            if (((ahstParam["benefit_type"].ToString()) != "")
                && (ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                if (ahstParam["payee_account_id"].ToString() == "")
                {
                    utlError lobjError = null;
                    lobjError = AddError(7720, "");
                    larrErrors.Add(lobjError);
                }
                else
                {
                    lintPersonId = Convert.ToInt32(ahstParam["aint_person_id"].ToString());
                    lintPayeeAccountID = Convert.ToInt32(ahstParam["payee_account_id"]);
                    lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                    bool lblnAllowTocheckforstatus = false;

                    //Systest PIR: 2634. For a Post Retirement Death Application, the benefit Account owner Entered should be the Member Perslink id of the Payee Account ID.
                    if ((lobjPayeeAccount.IsNotNull()  && (ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath))
                    {
                        int lintAccountOwnerPerslinkID = 0;
                        if (lobjPayeeAccount.icdoPayeeAccount.application_id > 0)
                        {
                            lobjPayeeAccount.LoadApplication();
                            lintAccountOwnerPerslinkID = lobjPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id;
                        }
                        else
                        {
                            lobjPayeeAccount.LoadDROApplication();
                            lintAccountOwnerPerslinkID = lobjPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.member_perslink_id;
                        }

                        if (lintPersonId != lintAccountOwnerPerslinkID)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(7734, "");
                            larrErrors.Add(lobjError);
                        }
                        else
                        {
                            lblnAllowTocheckforstatus = true;
                        }
                    }
                    //The check can be allowed for Post Retirement Applications since all the cases the Account Owner and the Payee perslink id will not be the same.
                    if ((lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id == lintPersonId) ||(lblnAllowTocheckforstatus))
                    {
                        lobjPayeeAccount.LoadActivePayeeStatus();
                        //Prod PIR: 4335. Error should be raised when the payee account status is cancelled or cancelled pending or cancelled disability.
                        if((lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value==busConstant.PayeeAccountStatusCancelled)
                            ||(lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value==busConstant.PayeeAccountStatusCancelPending)
                            || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilityCancelled)
                            )                        
                        {
                            utlError lobjError = null;
                            lobjError = AddError(7721, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    //PIR 23634 - Error on Application lookup if 'Post Retirement Death' application is created of benefit type 'Disibility'.
                    if (lobjPayeeAccount?.icdoPayeeAccount?.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(10426, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }
            if (larrErrors.Count == 0)
            {
                lintPersonId = Convert.ToInt32(ahstParam["aint_person_id"].ToString());
                lintPlanId = Convert.ToInt32(ahstParam["plan_id"].ToString());
                lobjAccountOwnnerPerson.FindPerson(lintPersonId);

                ldtbGetCountOfPersonAccount = busBase.Select("cdoBenefitApplication.GetPersonAccountForPersonAndPlan", new object[2] { lintPersonId, lintPlanId });
                if (ldtbGetCountOfPersonAccount.Rows.Count > 0)
                    lintPersonAccountId = Convert.ToInt32(ldtbGetCountOfPersonAccount.Rows[0]["person_account_id"]);

                if ((ahstParam["aint_person_id"].ToString() != "")
                    && (((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeRetirement)
                     || ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeDisability)))
                {
                    DataTable ldtbLoadPerson = busBase.Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { Convert.ToInt32(ahstParam["aint_person_id"].ToString()) }, null, null);
                    if (ldtbLoadPerson.Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(ldtbLoadPerson.Rows[0]["date_of_death"].ToString()))
                        {
                            utlError lobjError = null;
                            lobjError = AddError(1913, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    else
                    {
                        utlError lobjError = null;
                        lobjError = AddError(2009, "");
                        larrErrors.Add(lobjError);
                    }
                }
                else if ((ahstParam["aint_person_id"].ToString() != "")
                    && (((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    || (ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath))
                {
                    if ((ahstParam["aint_recipient_member_id"].ToString() == "")
                        && (ahstParam["aint_applicant_org_code_id"].ToString() == ""))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(2022, "");
                        larrErrors.Add(lobjError);
                    }
                    else
                    {
                        DataTable ldtbPerson = busBase.Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { Convert.ToInt32(ahstParam["aint_person_id"]) }, null, null);
                        if (ldtbPerson.Rows.Count == 0)
                        {
                            utlError lobjError = null;
                            lobjError = AddError(2009, "");
                            larrErrors.Add(lobjError);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(ldtbPerson.Rows[0]["date_of_death"].ToString()))
                            {
                                utlError lobjError = null;
                                lobjError = AddError(1978, "");
                                larrErrors.Add(lobjError);
                            }
                            else
                            {
                                if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath)
                                {
                                    //check if benefit option ucs-054 -15                                   
                                    if ((lobjPayeeAccount.icdoPayeeAccount.term_certain_end_date != DateTime.MinValue)
                                      && (lobjPayeeAccount.icdoPayeeAccount.term_certain_end_date <= Convert.ToDateTime(ldtbPerson.Rows[0]["date_of_death"]))
                                      &&
                                      (((lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner15YrTermCertainAndLifeOption)
                                      || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner20YrTermCertainAndLifeOption)
                                      || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner10YrTermCertainAndLifeOption)
                                      || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner5YrTermCertainAndLifeOption))
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfAP5yrTermcertainAndLifeOption)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeofAP10yrTermcertainAndLifeOption)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeofAP20yrTermCertainAndLifeOption)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeofAP10yrperiod)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeofAP15yrPeriod)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeofAP20yrperiod)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption15YearCertain)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain)
                                        || (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife))
                                        && (IsMemberAccountBalancelessthanZero(lobjPayeeAccount))) //UAT PIR: 2080
                                    {

                                        utlError lobjError = null;
                                        lobjError = AddError(7722, "");
                                        larrErrors.Add(lobjError);
                                    }
                                }
                            }
                        }
                        if ((ahstParam["aint_recipient_member_id"].ToString() != ""))
                        {
                            lintRecepientPersonId = Convert.ToInt32(ahstParam["aint_recipient_member_id"]);
                            DataTable ldtbRecPerson = busBase.Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { Convert.ToInt32(ahstParam["aint_recipient_member_id"]) }, null, null);
                            if (ldtbRecPerson.Rows.Count == 0)
                            {
                                utlError lobjError = null;
                                lobjError = AddError(2009, "");
                                larrErrors.Add(lobjError);
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(ldtbRecPerson.Rows[0]["date_of_death"].ToString()))
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(2040, "");
                                    larrErrors.Add(lobjError);
                                }
                                // PIR 8810 else condition added to avoid Contingent beneficiaries to apply for benefit
                                else
                                {
                                    DataTable ldtbBeneficiaries = busBase.Select("cdoPerson.LoadBeneficiaries", new object[1] { Convert.ToInt32(ahstParam["aint_person_id"].ToString()) });
                                    busPlan lobjPlan = new busPlan();

                                    lobjPlan.FindPlan(lintPlanId);
                                                                     
                                    for (int i = 0; i < ldtbBeneficiaries.Rows.Count; i++)
                                    {
                                        if ((ahstParam["aint_recipient_member_id"].ToString() == ldtbBeneficiaries.Rows[i]["benes_person_name"].ToString())
                                            && (lobjPlan.icdoPlan.plan_name == ldtbBeneficiaries.Rows[i]["ISTRPLANNAME"].ToString()))
                                        {
                                            if (ldtbBeneficiaries.Rows[i]["beneficiary_type_value"].ToString() == busConstant.BeneficiaryMemberTypeContingent)
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(10068, "");
                                                larrErrors.Add(lobjError);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (Convert.ToInt32(ahstParam["aint_recipient_member_id"]) == Convert.ToInt32(ahstParam["aint_person_id"]))
                            {
                                utlError lobjError = null;
                                lobjError = AddError(2023, "");
                                larrErrors.Add(lobjError);
                            }
                            else
                            {
                                if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePreRetirementDeath)
                                {
                                    DataTable ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "member_person_id", "recipient_person_id", "benefit_account_type_value", "plan_id" },
                                                                new object[4] { Convert.ToInt32(ahstParam["aint_person_id"].ToString()), Convert.ToInt32(ahstParam["aint_recipient_member_id"].ToString()), ahstParam["benefit_type"].ToString(), Convert.ToInt32(ahstParam["plan_id"].ToString()) }, null, null);
                                    foreach (DataRow dr in ldtbApplication.Rows)
                                    {
                                        if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                                        {
                                            if ((dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusDenied)
                                                      && (dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusCancelled))
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(1987, "");
                                                larrErrors.Add(lobjError);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath)
                                {
                                    DataTable ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "member_person_id", "recipient_person_id", "plan_id", "originating_payee_account_id" },
                                                                                                  new object[4] { Convert.ToInt32(ahstParam["aint_person_id"].ToString()), Convert.ToInt32(ahstParam["aint_recipient_member_id"].ToString()), 
                                                                                                      Convert.ToInt32(ahstParam["plan_id"].ToString()),Convert.ToInt32(ahstParam["payee_account_id"].ToString()) }, null, null);
                                    foreach (DataRow dr in ldtbApplication.Rows)
                                    {
                                        if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                                        {
                                            if ((dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusCancelled)
                                            && (dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusDenied))
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(1991, "");
                                                larrErrors.Add(lobjError);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePreRetirementDeath)
                            || (ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath)
                        {
                            if ((ahstParam["aint_applicant_org_code_id"].ToString() != ""))
                            {
                                lintOrgId = busGlobalFunctions.GetOrgIdFromOrgCode(ahstParam["aint_applicant_org_code_id"].ToString());

                                if (lintOrgId == 0)
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(2009, "");
                                    larrErrors.Add(lobjError);
                                }
                                else
                                {

                                    DataTable ldtbApplication = busBase.Select<cdoBenefitApplication>(new string[4] { "member_person_id", "payee_org_id", "benefit_account_type_value", "plan_id" },
                                                                               new object[4] { Convert.ToInt32(ahstParam["aint_person_id"].ToString()), lintOrgId, ahstParam["benefit_type"].ToString(), Convert.ToInt32(ahstParam["plan_id"].ToString()) }, null, null);

                                    foreach (DataRow dr in ldtbApplication.Rows)
                                    {
                                        if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                                        {
                                            if ((dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusDenied)
                                                   && (dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusCancelled))
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(1987, "");
                                                larrErrors.Add(lobjError);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    //RULE 54 18,19,20
                    string lstrDeathReasonValue = string.Empty, lstrBenefitOption = string.Empty, lstrAccountRelationshipValue = string.Empty;
                    DateTime ldtTerminationDate = DateTime.MinValue;
                    busPostRetirementDeathApplication lobjPostRetirementDeathApplication = new busPostRetirementDeathApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjPostRetirementDeathApplication.icdoBenefitApplication.plan_id = lintPlanId;

                    //****************** these fields are assigned here to avoid exception when this method is called from Lookup                   
                    lobjPostRetirementDeathApplication.icdoBenefitApplication.member_person_id = lintPersonId;
                    lobjPostRetirementDeathApplication.icdoBenefitApplication.recipient_person_id = lintRecepientPersonId;
                    lobjPostRetirementDeathApplication.icdoBenefitApplication.payee_org_id = lintOrgId;
                    lobjPostRetirementDeathApplication.icdoBenefitApplication.originating_payee_account_id = lintPayeeAccountID;
                    //**********************************************
                    lobjPostRetirementDeathApplication.ValidatePostRetirementDeathApplication(lintPersonId, lintRecepientPersonId, lintOrgId,
                                                                        lintPayeeAccountID, lintPlanId, lobjAccountOwnnerPerson.IsMarried,
                                                                        ref lstrDeathReasonValue, ref lstrBenefitOption, ref ldtTerminationDate, ref lstrAccountRelationshipValue);
                    if (string.IsNullOrEmpty(lstrBenefitOption)
                        && string.IsNullOrEmpty(lstrDeathReasonValue))
                    {
                        utlError lobjError = null;
                        lobjError = AddError(7724, "");
                        larrErrors.Add(lobjError);
                    }
                }


                if ((ahstParam["benefit_type"].ToString()) != busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    //check whether member is enrolled in the selected plan with status Enrolled Or Suspended
                    if ((ahstParam["aint_person_id"].ToString() != "") && (ahstParam["plan_id"].ToString() != ""))
                    {
                        //PIR - 1954
                        //for disability application
                        //surpass this validation if there exits a payee account with status completed or cancelled
                        bool lblnEarlyRetAppFoundForDis = false;
                        DataTable ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId = busBase.Select<cdoBenefitApplication>(new string[2] { "member_person_id", "plan_id" }, new object[2] { lintPersonId, lintPlanId }, null, null);
                        if (ahstParam["benefit_type"].ToString() == busConstant.ApplicationBenefitTypeDisability)
                        {
                            if (ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId.Rows.Count > 0)
                            {
                                var lenumGetEarlyRetApplication = ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId.AsEnumerable()
                                    .Where(dr => dr.Field<string>("benefit_sub_type_value") == busConstant.ApplicationBenefitSubTypeEarly
                                        && dr.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement)
                                        .OrderByDescending(dr => dr.Field<int>("BENEFIT_APPLICATION_ID")); //PIR 16039 - As per call with maik, order by field changed from termination date to application_id

                                if (lenumGetEarlyRetApplication.Count() > 0)
                                {
                                    int lintEarlyRetApplId = lenumGetEarlyRetApplication.First().Field<int>("benefit_application_id");

                                    lobjAccountOwnnerPerson.LoadPayeeAccount(true);

                                    var lintValidPayeeAccount = lobjAccountOwnnerPerson.iclbPayeeAccount
                                                                       .Where(lobjPA => lobjPA.icdoPayeeAccount.application_id == lintEarlyRetApplId
                                                                        && (!lobjPA.ibusPayeeAccountActiveStatus.IsStatusCompleted()
                                                                        && !lobjPA.ibusPayeeAccountActiveStatus.IsStatusCancelled())).Count();
                                    if (lintValidPayeeAccount > 0)
                                        lblnEarlyRetAppFoundForDis = true;
                                }
                            }
                        }
                        if ((ldtbGetCountOfPersonAccount.Rows.Count == 0)
                            && (!lblnEarlyRetAppFoundForDis))
                        {
                            //checks if the person has enrolled in the plan
                            utlError lobjError = null;
                            lobjError = AddError(1914, "");
                            larrErrors.Add(lobjError);
                        }
                        else if (((ahstParam["benefit_type"].ToString()) != busConstant.ApplicationBenefitTypeRefund)
                            && (((ahstParam["benefit_type"].ToString()) != busConstant.ApplicationBenefitTypePreRetirementDeath))
                            && ((ahstParam["benefit_type"].ToString()) != busConstant.ApplicationBenefitTypeDisability)) //UAT PIR - 1160
                        {
                            foreach (DataRow dr in ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId.Rows)
                            {
                                /*check if this person account is same as the dr person account id 
                                then only throw this error else continue
                                 check if the is person account application flag is y,
                                 take only that account as current account for this application
                                 dated - 01 oct 2009*/
                                busBenefitApplication lobjBenefitApplication = new busBenefitApplication
                                {
                                    icdoBenefitApplication = new cdoBenefitApplication(),
                                };
                                lobjBenefitApplication.icdoBenefitApplication.LoadData(dr);

                                lobjBenefitApplication.LoadPersonAccountDetailsInUpdateMode();

                                if (lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.person_account_id == lintPersonAccountId)
                                {
                                    //check if application exists for the person and plan with Deferred Or Verified Status
                                    //Commented PIR - 1446 -- BR - 51 - 17
                                    if (ahstParam["benefit_type"].ToString() == dr["benefit_account_type_value"].ToString())
                                    {
                                        if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                                        {
                                            if ((dr["action_status_value"].ToString() == busConstant.ApplicationActionStatusVerified)
                                                ||
                                                (dr["action_status_value"].ToString() == busConstant.ApplicationActionStatusDeferred))
                                            {
                                                utlError lobjError = null;
                                                lobjError = AddError(1915, "");
                                                larrErrors.Add(lobjError);
                                                break;
                                            }
                                        }
                                    }

                                    if ((!String.IsNullOrEmpty(dr["status_value"].ToString()))
                                        && (!String.IsNullOrEmpty(dr["benefit_sub_type_value"].ToString()))
                                        && (!String.IsNullOrEmpty(dr["benefit_account_type_value"].ToString())))
                                    {

                                        if ((dr["status_value"].ToString() == busConstant.ApplicationStatusProcessed)
                                            && (dr["benefit_sub_type_value"].ToString() == busConstant.ApplicationBenefitSubTypeEarly)
                                            && (dr["benefit_account_type_value"].ToString() == busConstant.ApplicationBenefitTypeRetirement))
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(1932, "");
                                            larrErrors.Add(lobjError);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeDisability) //// PIR - 1446 -- BR - 51 - 18
                        {
                            foreach (DataRow dr in ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId.Rows)
                            {
                                if ((dr["benefit_account_type_value"].ToString()) == busConstant.ApplicationBenefitTypeDisability)
                                {
                                    if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                                    {
                                        if ((dr["action_status_value"].ToString() == busConstant.ApplicationActionStatusVerified)
                                            ||
                                            (dr["action_status_value"].ToString() == busConstant.ApplicationActionStatusDeferred))
                                        {
                                            utlError lobjError = null;
                                            lobjError = AddError(1915, "");
                                            larrErrors.Add(lobjError);
                                        }
                                    }
                                }
                            }
                        }

                        else if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeRefund)
                        {
                            if(lintPersonAccountId > 0)
                            {
                                DataTable ldtbDatatable = busBase.Select("entBenefitRefundApplication.LoadDisabilityPayeeAccountsForPersonAccount",
                                            new object[1] { lintPersonAccountId });
                                if(ldtbDatatable.IsNotNull() && ldtbDatatable.Rows.Count > 0)
                                {
                                    utlError lobjError = null;
                                    lobjError = AddError(10383, "");
                                    larrErrors.Add(lobjError);
                                }
                            }
                            IsAnyApplicationExist(larrErrors, lintPersonId, lintPlanId, busConstant.ApplicationBenefitTypeRetirement, lintPersonAccountId);
                        }
                    }
                }
                if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeRetirement)
                {
                    IsApplicationExist(larrErrors, lintPersonId, lintPlanId, busConstant.ApplicationBenefitTypeRefund, lintPersonAccountId);
                }
                if ((ahstParam["benefit_type"].ToString()) == busConstant.ApplicationBenefitTypeDisability)
                {
                    busPerson lobjPerson = new busPerson();
                    if (lobjPerson.FindPerson(lintPersonId))
                    {
                        int lintPayeeAccount = 0;
                        bool lblnIsRTWMember = false;

                        lblnIsRTWMember = lobjPerson.IsRTWMember(lintPlanId, busConstant.PayeeStatusForRTW.SuspendedOnly, ref lintPayeeAccount);
                        if ((lintPayeeAccount > 0)
                            && (lblnIsRTWMember))
                        {
                            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccount);
                            if ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                                && ((lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                                || (lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)))
                            {
                                utlError lobjError = null;
                                lobjError = AddError(7696, "");
                                larrErrors.Add(lobjError);
                            }
                        }
                    }
                }
            }
            return larrErrors;
        }
		//UAT PIR:2080 Fix.
        private bool IsMemberAccountBalancelessthanZero(busPayeeAccount lobjPayeeAccount)
        {
            if (lobjPayeeAccount.IsNotNull())
            {
                if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
                {
                    lobjPayeeAccount.LoadPaymentDetails();
                    if ((lobjPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - lobjPayeeAccount.idecpaidgrossamount) < 0)
                    {
                        return true;
                    }
                }
                else
                {
                    lobjPayeeAccount.LoadApplication();
                    lobjPayeeAccount.LoadPayee();                    
                    if ((lobjPayeeAccount.ibusApplication.IsNotNull()) && (lobjPayeeAccount.ibusPayee.IsNotNull()))
                    {
                        lobjPayeeAccount.ibusApplication.LoadPersonAccountByApplication();                       
                        busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
                        lobjPARetirement.FindPersonAccountRetirement(lobjPayeeAccount.ibusApplication.ibusPersonAccount.icdoPersonAccount.person_account_id);
                        if (lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death != DateTime.MinValue)
                        {
                            lobjPARetirement.LoadLTDSummaryForCalculation(lobjPayeeAccount.ibusPayee.icdoPerson.date_of_death,
                                                                lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value);
                            if (lobjPARetirement.Member_Account_Balance_ltd < 0)
                            {
                                return true;
                            }
                        }
                    }
                }        
            }            
            return false;
        }
        
        private void IsAnyApplicationExist(ArrayList larrErrors, int lintPersonId, int lintPlanId, string p, int lintPersonAccountId)
        {
            DataTable ldtbBenPersonAccount = busBase.Select("cdoBenefitApplication.LoadBenefitApplicationPersonAccount",
                                                                                          new object[1] { lintPersonAccountId });

            foreach (DataRow ldtrBenApp in ldtbBenPersonAccount.Rows)
            {
                busBenefitApplicationPersonAccount lbusBenefitApplicationPersonAccount = new busBenefitApplicationPersonAccount
                {
                    icdoBenefitApplicationPersonAccount = new cdoBenefitApplicationPersonAccount()
                };
                lbusBenefitApplicationPersonAccount.ibusBenefitApplication = new busBenefitApplication
                {
                    icdoBenefitApplication = new cdoBenefitApplication()
                };
                lbusBenefitApplicationPersonAccount.icdoBenefitApplicationPersonAccount.LoadData(ldtrBenApp);
                lbusBenefitApplicationPersonAccount.ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtrBenApp);
                if (!lbusBenefitApplicationPersonAccount.ibusBenefitApplication.IsApplicationCancelledOrDenied() &&
                    lbusBenefitApplicationPersonAccount.ibusBenefitApplication.icdoBenefitApplication.member_person_id == lintPersonId &&
                    lbusBenefitApplicationPersonAccount.ibusBenefitApplication.icdoBenefitApplication.plan_id == lintPlanId)
                {
                    utlError lobjError = null;
                    lobjError = AddError(2545, "");
                    larrErrors.Add(lobjError);
                    return;
                }
            }
        }
        private void IsApplicationExist(ArrayList larrErrors, int lintPersonId, int lintPlanId, string astrBenefitType, int aintPersonAccountId)
        {
            //check if application exists for the person and plan with Deferred Or Verified Status
            DataTable ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId = busBase.Select<cdoBenefitApplication>(new string[3] { "member_person_id", "plan_id", "benefit_account_type_value" }, new object[3] { lintPersonId, lintPlanId, astrBenefitType }, null, null);

            foreach (DataRow dr in ldtbGetCntOfAppnStatusDefOrVerForPersonAndPlanId.Rows)
            {
                /*check if this person account is same as the dr person account id 
                               then only throw this error else continue
                                check if the is person account application flag is y,
                                take only that account as current account for this application
                                dated - 01 oct 2009*/
                busBenefitApplication lobjBenefitApplication = new busBenefitApplication
                {
                    icdoBenefitApplication = new cdoBenefitApplication(),
                };
                lobjBenefitApplication.icdoBenefitApplication.LoadData(dr);

                lobjBenefitApplication.LoadPersonAccountDetailsInUpdateMode();

                if (lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.person_account_id == aintPersonAccountId)
                {
                    if (!String.IsNullOrEmpty(dr["action_status_value"].ToString()))
                    {
                        if ((dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusDenied)
                               && (dr["action_status_value"].ToString() != busConstant.ApplicationActionStatusCancelled))
                        {
                            utlError lobjError = null;
                            if (astrBenefitType == busConstant.ApplicationBenefitTypeRetirement)
                            {
                                lobjError = AddError(2017, "");
                            }
                            else
                            {
                                lobjError = AddError(2545, "");
                            }
                            larrErrors.Add(lobjError);
                            break;
                        }
                    }
                }
            }
        }
    }
}