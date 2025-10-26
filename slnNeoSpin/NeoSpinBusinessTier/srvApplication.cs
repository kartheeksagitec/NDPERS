#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvApplication : srvNeoSpin
    {
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmRetirementApplicationLookup")
                {
                    busBenefitApplicationLookup lbusApplication = new busBenefitApplicationLookup();
                    larrErrors = lbusApplication.ValidateNew(ahstParam);
                }
                else if (astrFormName == "wfmDROApplicationLookup")
                {
                    busBenefitDroApplicationLookup lbusApplication = new busBenefitDroApplicationLookup();
                    larrErrors = lbusApplication.ValidateNew(ahstParam);
                }
                else if (astrFormName == "wfmDeathNotificationLookup")
                {
                    busDeathNotificationLookup lbusDeathNotificationLookup = new busDeathNotificationLookup();
                    larrErrors = lbusDeathNotificationLookup.ValidateNew(ahstParam);
                }
            }
            finally
            {
                //iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }
        public busRetirementDisabilityApplication FindRetirementDisabilityApplication(int aintReferenceID, string astrBenfitType)
        {
            busRetirementDisabilityApplication lobjBenefitApplication = new busRetirementDisabilityApplication();
            if (lobjBenefitApplication.FindBenefitApplication(aintReferenceID))
            {
                lobjBenefitApplication.LoadPerson();
                lobjBenefitApplication.LoadPersonAccount();
                lobjBenefitApplication.ibusPersonAccount.ibusPerson = lobjBenefitApplication.ibusPerson;
                lobjBenefitApplication.ibusPersonAccount.LoadPlan();

                lobjBenefitApplication.LoadErrors();
                lobjBenefitApplication.LoadOldSoftErrors();
                lobjBenefitApplication.LoadJointAnniutantPerson();

                lobjBenefitApplication.SetMemberAgeBasedOnRetirementDate();
                if (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    lobjBenefitApplication.LoadOtherDisabilityBenefits();
                }
                else
                {
                    //to avoid null error collection is intialised when benefit account type is other than Disability
                    lobjBenefitApplication.iclcBenAppOtherDisBenefit = new Collection<cdoBenAppOtherDisBenefit>();
                }
                lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();
                if (lobjBenefitApplication.icdoBenefitApplication.joint_annuitant_perslink_id != 0)
                {
                    lobjBenefitApplication.SetRelationshipOfJointAnnuitant();
                }

                //PIR - 1761
                //suppress warning logic changed as per above PIR
                //lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;

                if (lobjBenefitApplication.icdoBenefitApplication.tffr_calculation_method_value.IsNotNullOrEmpty()) // PROD PIR ID 6389
                    lobjBenefitApplication.idtTerminationDate = lobjBenefitApplication.icdoBenefitApplication.termination_date;
                lobjBenefitApplication.SetOrgIdAsLatestEmploymentOrgId();
                //this Load methods are used for correspondence 
                if (lobjBenefitApplication.ibusPersonAccount.ibusPerson.icolPersonEmployment == null)
                    lobjBenefitApplication.ibusPersonAccount.ibusPerson.LoadPersonEmployment();
                //UCS-60
                if (lobjBenefitApplication.icdoBenefitApplication.pre_rtw_payeeaccount_id > 0)
                {
                    lobjBenefitApplication.iblnIsRTWMember = true;
                    lobjBenefitApplication.LoadPensionPlanAccounts();
                    if (lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.Total_PSC.Equals(0.0M))
                    {
                        lobjBenefitApplication.ibusPersonAccount.LoadTotalPSC(lobjBenefitApplication.icdoBenefitApplication.retirement_date);
                    }
                    lobjBenefitApplication.SetIsRTWFlagLessThan2Years(lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.Total_PSC);
                }
                lobjBenefitApplication.icdoBenefitApplication.idecTVSC = lobjBenefitApplication.GetRoundedTVSC();
                lobjBenefitApplication.LoadPersonAccountDetailsInUpdateMode();

                lobjBenefitApplication.LoadBenefitCalculationID();
                lobjBenefitApplication.LoadBenefitCalculation();
                lobjBenefitApplication.LoadBenefitApplicationForCorrespondance(); //PIR 15831 - APP 7007 template to be associated with case management screen too

            }
            return lobjBenefitApplication;
        }

        public busRetirementDisabilityApplication NewRetirementDisabilityApplication(int AintPersonId, int AintRecipientMemberID, string astrBenefitType, int aintPlanID)
        {
            busRetirementDisabilityApplication lobjBenefitApplication = new busRetirementDisabilityApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = astrBenefitType;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value);
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1901, lobjBenefitApplication.icdoBenefitApplication.action_status_value);
            lobjBenefitApplication.icdoBenefitApplication.early_reduction_waived_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aintPlanID;
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = AintPersonId;
            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = AintPersonId;
            lobjBenefitApplication.LoadPerson();
            lobjBenefitApplication.LoadPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.ibusPerson = lobjBenefitApplication.ibusPerson;
            lobjBenefitApplication.ibusPersonAccount.LoadPlan();

            lobjBenefitApplication.LoadRTWPersonAccount();
            lobjBenefitApplication.LoadRecipient();
            lobjBenefitApplication.icdoBenefitApplication.idecTVSC = lobjBenefitApplication.GetRoundedTVSC();
            lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();

            if (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                lobjBenefitApplication.LoadOtherDisabilityBenefits();
            }
            else
            {
                //to avoid null error collection is intialised when benefit account type is other than Disability
                lobjBenefitApplication.iclcBenAppOtherDisBenefit = new Collection<cdoBenAppOtherDisBenefit>();
            }
            //PIR - UAT 1207
            ////Set SSLI Age for Job Service as it will be always 62
            //if (lobjBenefitApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            //{
            //    lobjBenefitApplication.icdoBenefitApplication.ssli_age = busConstant.SSLIAgeForJobService;
            //}
            lobjBenefitApplication.SetOrgIdAsLatestEmploymentOrgId();
            lobjBenefitApplication.SetQDROFlag();

            //UCS-060
            //PIR fix 1738
            if (lobjBenefitApplication.iblnIsRTWMember)
            {
                lobjBenefitApplication.ibusPersonAccount.LoadTotalPSC(lobjBenefitApplication.icdoBenefitApplication.retirement_date);
                lobjBenefitApplication.SetIsRTWFlagLessThan2Years(lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount.Total_PSC);
            }
            //pir 1497            
            //this will be set only once in new mode.                      
            lobjBenefitApplication.LoadDeferralDate();
            //PIR 15421
            lobjBenefitApplication.EvaluateInitialLoadRules();

            return lobjBenefitApplication;
        }

        public busPreRetirementDeathApplication NewPreRetirementDeathApplication(int AintPersonId, int AintRecipientMemberID, string AintApplicantOrgCodeID, string astrBenefitType, int aintPlanID)
        {
            busPreRetirementDeathApplication lobjBenefitApplication = new busPreRetirementDeathApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = astrBenefitType;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value);
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1901, lobjBenefitApplication.icdoBenefitApplication.action_status_value);
            lobjBenefitApplication.ibusPersonAccount = new busPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aintPlanID;
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = AintPersonId;
            lobjBenefitApplication.icdoBenefitApplication.early_reduction_waived_flag = busConstant.Flag_No;
            if (!String.IsNullOrEmpty(AintApplicantOrgCodeID))
            {
                lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = 0;
                lobjBenefitApplication.icdoBenefitApplication.payee_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(AintApplicantOrgCodeID);
                lobjBenefitApplication.LoadApplicantOrganization();
            }
            //Get Active Person account for this Plan and person to load person account object         
            lobjBenefitApplication.LoadPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.LoadPerson();
            lobjBenefitApplication.ibusPersonAccount.LoadPlan();
            lobjBenefitApplication.SetAgeBasedOnDateOfDeath();
            lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();

            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = AintRecipientMemberID;
            lobjBenefitApplication.LoadRecipient();
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipBeneficiary;
            if (lobjBenefitApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            {
                lobjBenefitApplication.SetAccountRelationshipforJobService();
            }
            lobjBenefitApplication.ibusPersonAccount.ibusPerson.LoadBeneficiary();
            lobjBenefitApplication.LoadOtherApplications();
            lobjBenefitApplication.SetFamilyRelationship();

            lobjBenefitApplication.EvaluateInitialLoadRules();

            return lobjBenefitApplication;
        }

        public busPreRetirementDeathApplication FindPreRetirementDeathApplication(int aintReferenceID, string astrBenefitType)
        {
            busPreRetirementDeathApplication lobjBenefitApplication = new busPreRetirementDeathApplication();
            if (lobjBenefitApplication.FindBenefitApplication(aintReferenceID))
            {
                lobjBenefitApplication.LoadPersonAccount();
                lobjBenefitApplication.ibusPersonAccount.LoadPerson();
                lobjBenefitApplication.ibusPersonAccount.LoadPlan();
                lobjBenefitApplication.LoadRecipient();
                lobjBenefitApplication.LoadErrors();
                lobjBenefitApplication.SetAgeBasedOnDateOfDeath();
                lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();
                lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
                lobjBenefitApplication.ibusPersonAccount.ibusPerson.LoadBeneficiary();
                if (lobjBenefitApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                {
                    lobjBenefitApplication.SetAccountRelationshipforJobService();
                }
                lobjBenefitApplication.LoadOtherApplications();
                lobjBenefitApplication.SetOrgIdAsLatestEmploymentOrgId();
                lobjBenefitApplication.LoadBenefitCalculationID();
                lobjBenefitApplication.LoadApplicantOrganization();

                //PIR 14391
                lobjBenefitApplication.LoadBenefitApplication();
            }
            return lobjBenefitApplication;
        }
        public busBenefitRefundApplication NewRefundApplication(int AintPersonId, int AintRecipientMemberID, string astrBenefitType, int aintPlanID)
        {
            busBenefitRefundApplication lobjBenefitApplication = new busBenefitRefundApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = astrBenefitType;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value);
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1901, lobjBenefitApplication.icdoBenefitApplication.action_status_value);
            lobjBenefitApplication.ibusPersonAccount = new busPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aintPlanID;
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = AintPersonId;
            lobjBenefitApplication.LoadPerson();
            lobjBenefitApplication.LoadPersonAccount();
            lobjBenefitApplication.LoadPlan();
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lobjBenefitApplication.icdoBenefitApplication.account_relationship_value);

            lobjBenefitApplication.icdoBenefitApplication.family_relationship_value = busConstant.AccountRelationshipMember;
            lobjBenefitApplication.icdoBenefitApplication.family_relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, lobjBenefitApplication.icdoBenefitApplication.family_relationship_value);
            lobjBenefitApplication.SetTerminationDate();
            lobjBenefitApplication.icdoBenefitApplication.uniform_income_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.sick_leave_purchase_indicated_flag = busConstant.Flag_No;
            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = AintPersonId;

            lobjBenefitApplication.EvaluateInitialLoadRules();
            return lobjBenefitApplication;
        }

        public busBenefitRefundApplication FindRefundApplication(int aintReferenceID, string astrBenefitType)
        {
            busBenefitRefundApplication lobjBenefitApplication = new busBenefitRefundApplication();
            if (lobjBenefitApplication.FindBenefitApplication(aintReferenceID))
            {
                lobjBenefitApplication.LoadPersonAccount();
                lobjBenefitApplication.LoadPerson();
                lobjBenefitApplication.LoadPlan();
                lobjBenefitApplication.LoadErrors();
                lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();
                lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
                lobjBenefitApplication.LoadBenefitApplicationDbTffrTransfer();
                lobjBenefitApplication.LoadRefundBenefitCalculation();
                lobjBenefitApplication.LoadPayeeAccountID();
                lobjBenefitApplication.SetTerminationDate();
            }
            return lobjBenefitApplication;
        }

        public busBenefitApplicationLookup LoadBenefitApplications(DataTable adtbSearchResult)
        {
            busBenefitApplicationLookup lobjBenefitApplicationLookup = new busBenefitApplicationLookup();
            lobjBenefitApplicationLookup.LoadApplications(adtbSearchResult);
            return lobjBenefitApplicationLookup;
        }

        public busDeathNotificationLookup LoadDeathNotifications(DataTable adtbSearchResult)
        {
            busDeathNotificationLookup lobjDeathNotificationLookup = new busDeathNotificationLookup();
            lobjDeathNotificationLookup.LoadDeathNotification(adtbSearchResult);
            return lobjDeathNotificationLookup;
        }

        public busDeathNotification FindDeathNotification(int Aintdeathnotificationid)
        {
            busDeathNotification lobjDeathNotification = new busDeathNotification();
            if (lobjDeathNotification.FindDeathNotification(Aintdeathnotificationid))
            {
                lobjDeathNotification.LoadErrors();
                if (lobjDeathNotification.icdoDeathNotification.person_id != 0)
                {
                    lobjDeathNotification.LoadPerson();
                    lobjDeathNotification.LoadPersonRelatedDetails();
                    lobjDeathNotification.LoadAllRHICCombine();
                    // BPM Death Automation
                    lobjDeathNotification.LoadAllLatestRHICCombine();
                    lobjDeathNotification.LoadEstateOrgCodeID();
                    lobjDeathNotification.LoadDeceasedPayeeAccount();
                    lobjDeathNotification.LoadCorTracking();
                    if (lobjDeathNotification.ibusDeceasedPayeeAccount.icdoPayeeAccount.payee_account_id > 0)// Added if condition to load payment details only if payee account exists,for improving performance.
                    {
                        lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPayee();
                        lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPaymentDetails();
                    }
                    if (lobjDeathNotification.ibusPerson.icolPersonEmployment.IsNull())
                        lobjDeathNotification.ibusPerson.LoadPersonEmployment();

                    //foreach (var lobjPersonEmployment in lobjDeathNotification.ibusPerson.icolPersonEmployment)
                    //{
                    //    if (lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                    //    {
                    //        lobjPersonEmployment.icdoPersonEmployment.iblnEmploymentEndDateFromDeath = true;
                    //    }
                    //}
                    
                }
            }
            return lobjDeathNotification;
        }

        public busDeathNotification NewDeathNotification(int aintPersonId, DateTime adtDateofDeath)
        {
            busDeathNotification lobjDeathNotification = new busDeathNotification();
            lobjDeathNotification.icdoDeathNotification = new cdoDeathNotification();
            lobjDeathNotification.icdoDeathNotification.person_id = aintPersonId;
            lobjDeathNotification.icdoDeathNotification.date_of_death = adtDateofDeath;
            lobjDeathNotification.icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusInProgress;
            lobjDeathNotification.icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, busConstant.DeathNotificationActionStatusInProgress);
            if (lobjDeathNotification.icdoDeathNotification.person_id != 0)
            {
                lobjDeathNotification.LoadPerson();
                lobjDeathNotification.icdoDeathNotification.first_name = lobjDeathNotification.ibusPerson.icdoPerson.first_name;
                lobjDeathNotification.icdoDeathNotification.last_name = lobjDeathNotification.ibusPerson.icdoPerson.last_name;
                lobjDeathNotification.LoadPersonRelatedDetails();
                lobjDeathNotification.LoadAllRHICCombine();
                // BPM Death Automation
                lobjDeathNotification.LoadAllLatestRHICCombine();
                lobjDeathNotification.LoadEstateOrgCodeID();
                lobjDeathNotification.LoadDeceasedPayeeAccount();
                lobjDeathNotification.LoadCorTracking();
                if (lobjDeathNotification.ibusDeceasedPayeeAccount.icdoPayeeAccount.payee_account_id > 0)// Added if condition to load payment details only if payee account exists,for improving performance.
                {
                    lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPayee();
                    lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPaymentDetails();
                }
                if (lobjDeathNotification.ibusPerson.icolPersonEmployment.IsNull())
                    lobjDeathNotification.ibusPerson.LoadPersonEmployment();

                //foreach (var lobjPersonEmployment in lobjDeathNotification.ibusPerson.icolPersonEmployment)
                //{
                //    if (lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                //    {
                //        lobjPersonEmployment.icdoPersonEmployment.iblnEmploymentEndDateFromDeath = true;
                //    }
                //}
            }
            return lobjDeathNotification;
        }

        public busBenefitApplicationDbTffrTransfer FindBenefitApplicationDbTffrTransfer(int AintBenefitApplicationDbTffrTransferid)
        {
            busBenefitApplicationDbTffrTransfer lobjBenefitApplicationDbTffrTransfer = new busBenefitApplicationDbTffrTransfer();
            if (lobjBenefitApplicationDbTffrTransfer.FindBenefitApplicationDbTffrTransfer(AintBenefitApplicationDbTffrTransferid))
            {
                lobjBenefitApplicationDbTffrTransfer.LoadbenefitApplication();
                lobjBenefitApplicationDbTffrTransfer.ibusBenefitApplication.LoadPersonAccount();
            }
            return lobjBenefitApplicationDbTffrTransfer;
        }

        public busBenefitApplicationDbTffrTransfer NewBenefitApplicationDbTffrTransfer(int AintBenefitApplicationid)
        {
            busBenefitApplicationDbTffrTransfer lobjBenefitApplicationDbTffrTransfer = new busBenefitApplicationDbTffrTransfer();
            lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer = new cdoBenefitApplicationDbTffrTransfer();
            lobjBenefitApplicationDbTffrTransfer.icdoBenefitApplicationDbTffrTransfer.benefit_application_id = AintBenefitApplicationid;
            lobjBenefitApplicationDbTffrTransfer.LoadbenefitApplication();
            lobjBenefitApplicationDbTffrTransfer.ibusBenefitApplication.LoadPersonAccount();
            return lobjBenefitApplicationDbTffrTransfer;
        }

        public busBenefitDroApplication NewBenefitDroApplication(int AintPersonId, string astrDroModel, int aintPlanID, int aintAlternatePayeeID)
        {
            busBenefitDroApplication lobjBenefitDroApplication = new busBenefitDroApplication();
            lobjBenefitDroApplication.icdoBenefitDroApplication = new cdoBenefitDroApplication();
            lobjBenefitDroApplication.icdoBenefitDroApplication.member_perslink_id = AintPersonId;
            lobjBenefitDroApplication.icdoBenefitDroApplication.alternate_payee_perslink_id = aintAlternatePayeeID;
            lobjBenefitDroApplication.icdoBenefitDroApplication.plan_id = aintPlanID;
            lobjBenefitDroApplication.icdoBenefitDroApplication.person_account_id = busPersonAccountHelper.GetPersonAccountID
                (lobjBenefitDroApplication.icdoBenefitDroApplication.plan_id, lobjBenefitDroApplication.icdoBenefitDroApplication.member_perslink_id);
            lobjBenefitDroApplication.LoadPersonAccount();

            lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value = busConstant.DROApplicationStatusRecieved;
            lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_description
                        = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2402, lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value);
            lobjBenefitDroApplication.icdoBenefitDroApplication.dro_model_value = astrDroModel;
            lobjBenefitDroApplication.icdoBenefitDroApplication.dro_model_description
                        = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2401, lobjBenefitDroApplication.icdoBenefitDroApplication.dro_model_value);
            lobjBenefitDroApplication.LoadDurationOfBenefitOptionByDROModel();
            lobjBenefitDroApplication.LoadMember();
            lobjBenefitDroApplication.LoadAlternatePayee();
            lobjBenefitDroApplication.LoadPlan();
            //UCS - 086 Method to load Receiving Payee account for Member
            if (lobjBenefitDroApplication.LoadMemberPayeeAccount())
            {
                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPayeeAccountPaymentItemType();
                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentItemType();
                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentHistoryHeader();
                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentHistoryDetail();
                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadNexBenefitPaymentDate();
            }
            lobjBenefitDroApplication.EvaluateInitialLoadRules();

            return lobjBenefitDroApplication;
        }

        public busBenefitDroApplication FindBenefitDroApplication(int Aintdroapplicationid)
        {
            busBenefitDroApplication lobjBenefitDroApplication = new busBenefitDroApplication();
            if (lobjBenefitDroApplication.FindBenefitDroApplication(Aintdroapplicationid))
            {
                lobjBenefitDroApplication.LoadDurationOfBenefitOptionByDROModel();
                lobjBenefitDroApplication.LoadMember();
                lobjBenefitDroApplication.LoadAlternatePayee();
                lobjBenefitDroApplication.LoadPlan();
                //UCS - 086 Method to load Receiving Payee account for Member
                if (lobjBenefitDroApplication.LoadMemberPayeeAccountUpdateMode())
                {
                    lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentItemType();
                    lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentHistoryHeader();
                    lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadPaymentHistoryDetail();
                    lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadNexBenefitPaymentDate();
                }
                lobjBenefitDroApplication.LoadPersonAccount();
                lobjBenefitDroApplication.LoadErrors();
                //Method to load DRO Beneficiary Details
                //PIR - 1323 : Beneficiaries of Alternate payee should be displayed
                lobjBenefitDroApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
                lobjBenefitDroApplication.LoadDROCalculation();
                if (lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                {
                    lobjBenefitDroApplication.CalculateAdditionalInterest(busInterestCalculationHelper.GetInterestBatchLastRunDate());
                    //UCS - 086 -- Loading gross monthly benefit, taxable, non taxable amounts
                    lobjBenefitDroApplication.LoadPostRetirementDROCalculation();
                    //ucs - 086 : loading balance min guarantee amount
                    lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.LoadBalanceMinimumGuarantee
                       (lobjBenefitDroApplication.ibusMemberPayeeAccount, new DateTime(lobjBenefitDroApplication.icdoBenefitDroApplication.date_of_divorce.Year,
                        lobjBenefitDroApplication.icdoBenefitDroApplication.date_of_divorce.Month, 1));
                    //ucs - 086 : loading balance non taxable amount
                    lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.LoadBalanceNonTaxableAmount
                        (lobjBenefitDroApplication.ibusMemberPayeeAccount,
                        lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date);
                    if ((lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified ||
                        lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusPendingNullified ||
                        lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusNullified) &&
                        lobjBenefitDroApplication.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                    {
                        lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(
                                    lobjBenefitDroApplication.ibusMemberPayeeAccount.idtNextBenefitPaymentDate,
                                    lobjBenefitDroApplication.ibusMemberPayeeAccount);
                    }
                    if (lobjBenefitDroApplication.iclbPayeeAccount == null)
                        lobjBenefitDroApplication.LoadPayeeAccount();
                    if (lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusApproved ||
                        lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusProcessed)
                    {
                        lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest =
                            lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount;
                        //uat pir 1590
                        busPayeeAccount lobjPayeeAccount = lobjBenefitDroApplication.iclbPayeeAccount
                            .Where(o => o.icdoPayeeAccount.dro_calculation_id == lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id)
                            .FirstOrDefault();
                        decimal ldecMonthlyNontaxable = 0.0m, ldecMonthlyTaxable = 0.0m;
                        if (lobjPayeeAccount != null)
                        {
                            lobjBenefitDroApplication.CalculateMonthlyPaymentItems(lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                                                                                    lobjPayeeAccount.icdoPayeeAccount.benefit_option_value,
                                                                                    lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value,
                                                                                    ref ldecMonthlyTaxable, ref ldecMonthlyNontaxable);
                        }
                    }
                    if (lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.icdoBenefitDroCalculation.payment_status_value ==
                        busConstant.DROApplicationPaymentStatusApproved)
                    {
                        /*if (lobjBenefitDroApplication.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                        {                            
                            lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.idecMemberTaxableAmount =
                                lobjBenefitDroApplication.ibusMemberPayeeAccount.LoadLatestTaxableNonTaxableAmount(busConstant.PAPITTaxableAmount);                                
                        }*/
                        foreach (busPayeeAccount lobjPayeeAccount in lobjBenefitDroApplication.iclbPayeeAccount)
                        {
                            if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id == lobjBenefitDroApplication.ibusAlternatePayee.icdoPerson.person_id &&
                                lobjBenefitDroApplication.icdoBenefitDroApplication.dro_status_value != busConstant.DROApplicationStatusNullified)
                            {
                                lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.idecAltPayeeStartingNonTaxableAmount =
                                    lobjPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
                                lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                                lobjPayeeAccount.LoadPaymentItemType();
                                /*lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.idecAltPayeeNonTaxableAmt =
                                                               lobjPayeeAccount.LoadLatestTaxableNonTaxableAmount(
                                                               busConstant.PAPITTaxalbeAmount);
                                lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.idecAltPayeeTaxableAmt =
                                                               lobjPayeeAccount.LoadLatestTaxableNonTaxableAmount(
                                                               busConstant.PAPITTaxableAmount);  */
                            }
                        }
                        decimal ldecGrossMonthlyBenefit = lobjBenefitDroApplication.icdoBenefitDroApplication.overridden_member_gross_monthly_amount == 0.0M ?
                        lobjBenefitDroApplication.icdoBenefitDroApplication.computed_member_gross_monthly_amount :
                        lobjBenefitDroApplication.icdoBenefitDroApplication.overridden_member_gross_monthly_amount;
                        lobjBenefitDroApplication.idecAPGrossMonthlyAmount = (lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount > 0 ?
                            lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.monthly_benefit_amount :
                        (ldecGrossMonthlyBenefit * lobjBenefitDroApplication.icdoBenefitDroApplication.monthly_benefit_percentage / 100));
                        lobjBenefitDroApplication.idecUpdatedMonthlyAmount = ldecGrossMonthlyBenefit - lobjBenefitDroApplication.icdoBenefitDroApplication.monthly_benefit_amount;
                    }
                }
                else
                {
                    lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation = new busBenefitPostRetirementDROCalculation { icdoBenefitDroCalculation = new cdoBenefitDroCalculation() };
                    //ucs - 086 : loading balance min guarantee amount
                    lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.LoadBalanceMinimumGuarantee
                       (lobjBenefitDroApplication.ibusMemberPayeeAccount, new DateTime(lobjBenefitDroApplication.icdoBenefitDroApplication.date_of_divorce.Year,
                        lobjBenefitDroApplication.icdoBenefitDroApplication.date_of_divorce.Month, 1));
                    //ucs - 086 : loading balance non taxable amount
                    lobjBenefitDroApplication.ibusBenefitPostRetirementDROCalculation.LoadBalanceNonTaxableAmount
                        (lobjBenefitDroApplication.ibusMemberPayeeAccount,
                        lobjBenefitDroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.benefit_begin_date);
                }
                if (lobjBenefitDroApplication.iclbPayeeAccount == null)
                    lobjBenefitDroApplication.LoadPayeeAccount();
            }
            return lobjBenefitDroApplication;
        }

        public busBenefitDroApplicationLookup LoadDroApplications(DataTable adtbSearchResult)
        {
            busBenefitDroApplicationLookup lobjDroApplicationLookup = new busBenefitDroApplicationLookup();
            lobjDroApplicationLookup.LoadBenefitDroApplications(adtbSearchResult);
            return lobjDroApplicationLookup;
        }

        # region UCS -54
        public busPostRetirementDeathApplication NewPostRetirementDeathApplication(int AintPersonId, int AintRecipientMemberID, string AintApplicantOrgCodeID, string astrBenefitType,
            int aintPlanID, int aintPayeeAccountId)
        {
            busPostRetirementDeathApplication lobjBenefitApplication = new busPostRetirementDeathApplication();
            lobjBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value = astrBenefitType;
            lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value);
            lobjBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
            lobjBenefitApplication.icdoBenefitApplication.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1901, lobjBenefitApplication.icdoBenefitApplication.action_status_value);
            lobjBenefitApplication.ibusPersonAccount = new busPersonAccount();
            lobjBenefitApplication.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            lobjBenefitApplication.icdoBenefitApplication.plan_id = aintPlanID;
            lobjBenefitApplication.icdoBenefitApplication.member_person_id = AintPersonId;
            lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = AintRecipientMemberID;
            lobjBenefitApplication.LoadRecipient();

            lobjBenefitApplication.icdoBenefitApplication.originating_payee_account_id = aintPayeeAccountId;

            if (!String.IsNullOrEmpty(AintApplicantOrgCodeID))
            {
                lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = 0;
                lobjBenefitApplication.icdoBenefitApplication.payee_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(AintApplicantOrgCodeID);
            }
            lobjBenefitApplication.LoadApplicantOrganization();

            //Get Active Person account for this Plan and person to load person account object
            lobjBenefitApplication.LoadPerson();
            lobjBenefitApplication.ibusPersonAccount = lobjBenefitApplication.ibusPerson.LoadActivePersonAccountByPlan(aintPlanID);
            lobjBenefitApplication.ibusPersonAccount.ibusPerson = lobjBenefitApplication.ibusPerson;
            lobjBenefitApplication.ibusPersonAccount.LoadPlan();

            string lstrDeathReasonValue = string.Empty;
            DateTime ldtTerminationDate = DateTime.MinValue;
            string lstrBenefitOptionValue = string.Empty;
            string lstrAccountRelationshipValue = string.Empty;
            lobjBenefitApplication.ValidatePostRetirementDeathApplication(AintPersonId, AintRecipientMemberID, lobjBenefitApplication.ibusApplicantOrganization.icdoOrganization.org_id,
                                                                            lobjBenefitApplication.icdoBenefitApplication.originating_payee_account_id,
                                                                            lobjBenefitApplication.icdoBenefitApplication.plan_id, lobjBenefitApplication.ibusPersonAccount.ibusPerson.IsMarried,
                                                                            ref lstrDeathReasonValue, ref lstrBenefitOptionValue, ref ldtTerminationDate, ref lstrAccountRelationshipValue);
            lobjBenefitApplication.icdoBenefitApplication.post_retirement_death_reason_type_value = lstrDeathReasonValue;
            lobjBenefitApplication.icdoBenefitApplication.post_retirement_death_reason_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2701, lstrDeathReasonValue);
            lobjBenefitApplication.icdoBenefitApplication.benefit_option_value = lstrBenefitOptionValue;
            lobjBenefitApplication.icdoBenefitApplication.benefit_option_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2216, lstrBenefitOptionValue);
            lobjBenefitApplication.icdoBenefitApplication.termination_date = ldtTerminationDate;
            if (lstrDeathReasonValue == busConstant.PostRetirementFirstBeneficiaryDeath)
                lstrAccountRelationshipValue = busConstant.AccountRelationshipBeneficiary;
            lobjBenefitApplication.SetAgeBasedOnDateOfDeath();
            lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();

            lobjBenefitApplication.icdoBenefitApplication.account_relationship_value = lstrAccountRelationshipValue;
            lobjBenefitApplication.icdoBenefitApplication.account_relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2225, lstrAccountRelationshipValue);
            lobjBenefitApplication.icdoBenefitApplication.family_relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(321, lobjBenefitApplication.icdoBenefitApplication.family_relationship_value);
            lobjBenefitApplication.SetFieldsForRTWMember();
            /* PIR 14238 && PIR 13365 - Permanent fix - if joint annuitant Perslink id is not set here while creating post retirement application by batch, 
             * 6714 hard error was being thrown on verify click button on post retirement death application maintenance, change made in postretirement account owner death batch*/
            if (lobjBenefitApplication.icdoBenefitApplication.joint_annuitant_perslink_id == 0
                && lobjBenefitApplication.icdoBenefitApplication.account_relationship_value == busConstant.AccountRelationshipJointAnnuitant
                && lobjBenefitApplication.icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
            {
                lobjBenefitApplication.icdoBenefitApplication.joint_annuitant_perslink_id = lobjBenefitApplication.icdoBenefitApplication.recipient_person_id;
            }
            lobjBenefitApplication.EvaluateInitialLoadRules();

            return lobjBenefitApplication;
        }

        public busPostRetirementDeathApplication FindPostRetirementDeathApplication(int aintReferenceID, string astrBenefitType)
        {
            busPostRetirementDeathApplication lobjBenefitApplication = new busPostRetirementDeathApplication();
            if (lobjBenefitApplication.FindBenefitApplication(aintReferenceID))
            {
                lobjBenefitApplication.SetPersonAccount();
                lobjBenefitApplication.ibusPersonAccount.LoadPerson();
                lobjBenefitApplication.ibusPersonAccount.LoadPlan();
                lobjBenefitApplication.LoadRecipient();
                lobjBenefitApplication.LoadErrors();
                lobjBenefitApplication.SetAgeBasedOnDateOfDeath();
                lobjBenefitApplication.ibusPersonEmploymentDtl = lobjBenefitApplication.ibusPersonAccount.GetLatestEmploymentDetail();
                lobjBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
                lobjBenefitApplication.ibusPersonAccount.ibusPerson.LoadBeneficiary();

                lobjBenefitApplication.LoadOtherApplications();
                lobjBenefitApplication.SetOrgIdAsLatestEmploymentOrgId();
                lobjBenefitApplication.LoadBenefitCalculationID();
                lobjBenefitApplication.LoadApplicantOrganization();
                lobjBenefitApplication.LoadBenefitCalculationID();
                //PIR 14391
                lobjBenefitApplication.LoadBenefitApplication();
            }
            return lobjBenefitApplication;
        }

        # endregion
    }
}
