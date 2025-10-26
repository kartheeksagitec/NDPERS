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
using System.Linq;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using Sagitec.Bpm;

using System.Collections.Generic;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDeathNotification : busDeathNotificationGen
    {
        public bool iblnIsNewMode { get; set; }
        private ObjectState istrObjectState;
        public string istrFromBatch;
        public string istrRetirementAccountVisibility { get; set; }// PIR 7940
        # region button Logic
        public ArrayList btnNonResponsiveClicked()

        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusNonResponsive;

                icdoDeathNotification.Update();

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();

                //refresh cdo
                icdoDeathNotification.Select();
                icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, icdoDeathNotification.action_status_value);
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnCancelClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes)
            {
                lobjError = AddError(1980, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iarrErrors.Count == 0)
            {
                icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusCancelled;
                icdoDeathNotification.Update();

                if (ibusPerson == null)
                    LoadPerson();

                ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
                ibusPerson.icdoPerson.Update();

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();

                //refresh cdo
                icdoDeathNotification.Select();
                icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, icdoDeathNotification.action_status_value);
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnErroneousClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes)
            {
                lobjError = AddError(1980, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (icdoDeathNotification.suppress_warnings_flag != busConstant.Flag_Yes
                && (Convert.ToString(icdoDeathNotification.ihstOldValues[enmDeathNotification.action_status_value.ToString()]) != busConstant.DeathNotificationActionStatusErroneous
                || IsDateOfDateChangedOrStatusToErroneous()))
            {
                lobjError = AddError(10479, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iarrErrors.Count == 0)
            {
                icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusErroneous;
                icdoDeathNotification.Update();

                if (ibusPerson == null)
                    LoadPerson();

                ibusPerson.icdoPerson.date_of_death = DateTime.MinValue;
                ibusPerson.icdoPerson.Update();

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();

                //refresh cdo
                icdoDeathNotification.Select();
                icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, icdoDeathNotification.action_status_value);
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnCompleteClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (icdoDeathNotification.death_certified_flag != busConstant.Flag_Yes)
            {
                lobjError = AddError(2020, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            //Check whether all the beneficiaries dependents contacts are end dated
            //else raise errors.
            if (!IsAllPersonRelatedDatasEnded())
            {
                lobjError = AddError(1982, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            //********* this rule is changed to warning - PIR 1736
            //UCS-054 
            //check if any valid beneficiary exists
            //if (!IsValidBeneficiaryExists())
            //{
            //    lobjError = AddError(7717, "");
            //    alReturn.Add(lobjError);
            //    return alReturn;
            //}
            //UCS-054 
            //check if any payee account exists with status other than Payment Complete, Suspended, Cancelled
            //then throw error
            if (!IsPayeeAccountStatusValid())
            {
                lobjError = AddError(7716, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iarrErrors.Count == 0)
            {
                icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusCompleted;
                icdoDeathNotification.Update();

                //When Death Notification is Completed and there is a first activity for WFL Process IDs 249,250,282,283,284 
                //(Activity IDs 58,59,144,148,152) in a Suspended status it should be updated to RESU.
                //
                //Venkat - modify query
                DataTable ldtSuspendedInstance = busBase.Select("entSolBpmActivityInstance.LoadSuspendedInstancesByPersonWhenDeathNotificationComplete",
                                    new object[1] { icdoDeathNotification.person_id });

                foreach (DataRow dr in ldtSuspendedInstance.Rows)
                {
                     busSolBpmActivityInstance lobjActivityInstance = new busSolBpmActivityInstance();
                     lobjActivityInstance.icdoBpmActivityInstance.LoadData(dr);
                     lobjActivityInstance.icdoBpmActivityInstance.status_value = busConstant.ActivityStatusResumed;
                     lobjActivityInstance.icdoBpmActivityInstance.Update();
                } 

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();
                //refresh cdo
                icdoDeathNotification.Select();
                icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, icdoDeathNotification.action_status_value);
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnInProgressClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoDeathNotification.action_status_value = busConstant.DeathNotificationActionStatusInProgress;
                icdoDeathNotification.Update();

                //validate errors
                base.ValidateSoftErrors();
                LoadErrors();
                base.UpdateValidateStatus();

                //refresh cdo
                icdoDeathNotification.Select();
                icdoDeathNotification.action_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2321, icdoDeathNotification.action_status_value);
                alReturn.Add(this);

                //Load intinial rules
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnReadyToGenerateCorrespodenceClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusDeceasedPayeeAccount.IsNull())
                    LoadDeceasedPayeeAccount();
                if (ibusPerson.icolPersonAccount.IsNull())
                    ibusPerson.LoadPersonAccount();
                if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                    ibusPerson.LoadMemberPlusAlternatePayeeAccounts();

                //delete all Records if exists against death_notification_id
                DBFunction.DBNonQuery("entDeathNotification.DeleteAllCorrRelatedTableData", new object[1] { icdoDeathNotification.death_notification_id },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                busCorDeathNotification lbusCorDeathNotification = new busCorDeathNotification() { icdoCorDeathNotification = new DataObjects.doCorDeathNotification() };
                lbusCorDeathNotification.icdoCorDeathNotification.death_notification_id = icdoDeathNotification.death_notification_id;
                lbusCorDeathNotification.icdoCorDeathNotification.person_id = icdoDeathNotification.person_id;
                lbusCorDeathNotification.icdoCorDeathNotification.last_name = ibusPerson.icdoPerson.last_name;
                lbusCorDeathNotification.icdoCorDeathNotification.first_name = ibusPerson.icdoPerson.first_name;
                lbusCorDeathNotification.icdoCorDeathNotification.middle_name = ibusPerson.icdoPerson.middle_name;

                lbusCorDeathNotification.icdoCorDeathNotification.is_active = busConstant.Flag_Yes;
                lbusCorDeathNotification.icdoCorDeathNotification.is_retiree = busConstant.Flag_No;
                if (ibusDeceasedPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                {
                    lbusCorDeathNotification.icdoCorDeathNotification.is_retiree = busConstant.Flag_Yes;
                    lbusCorDeathNotification.icdoCorDeathNotification.is_active = busConstant.Flag_No;
                }
                bool lblnIsInsurancePlan = false;
                bool lblnIsRetirementPlan = false;
                if (ibusPerson.icolPersonAccount.Count > 0)
                {
                    foreach (busPersonAccount lobjPersonAccount in ibusPerson.icolPersonAccount)
                    {
                        if (lobjPersonAccount.ibusPlan.IsInsurancePlan())
                        {
                            busCorInsurance lbusCorInsurance = new busCorInsurance() { icdoCorInsurance = new DataObjects.doCorInsurance() };
                            lbusCorInsurance.icdoCorInsurance.death_notification_id = icdoDeathNotification.death_notification_id;
                            lbusCorInsurance.icdoCorInsurance.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                            lbusCorInsurance.icdoCorInsurance.Insert();
                            lblnIsInsurancePlan = true;
                        }
                        else if (lobjPersonAccount.ibusPlan.IsRetirementPlan())
                        {
                            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement() { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
                            lbusPersonAccountRetirement.FindPersonAccountRetirement(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lbusPersonAccountRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPersonAccountRetirement.ibusPerson.icdoPerson = ibusPerson.icdoPerson;
                            //lbusPersonAccountRetirement.LoadLTDSummary();
                            if (lbusPersonAccountRetirement.ibusPlan == null)
                                lbusPersonAccountRetirement.LoadPlan();
                            lbusPersonAccountRetirement.LoadTotalVSC();
                            lbusPersonAccountRetirement.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                            lbusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount = lbusPersonAccountRetirement.icdoPersonAccount;

                            //calculate person age
                            int lintMemberAgeInMonths = 0;
                            int lintMemberAgeInYears = 0;
                            decimal ldecMonthAndYear = 0m;
                            int lintMonths = 0;
                            busPersonBase.CalculateAge(ibusPerson.icdoPerson.date_of_birth,
                                icdoDeathNotification.date_of_death, ref lintMonths, ref ldecMonthAndYear, 2,
                                ref lintMemberAgeInYears, ref lintMemberAgeInMonths);

                            if (busPersonBase.CheckIsPersonVested(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id
                                , lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_code,
                                lbusPersonAccountRetirement.ibusPlan.icdoPlan.benefit_provision_id,
                                busConstant.ApplicationBenefitTypeRetirement,
                                lbusPersonAccountRetirement.icdoPersonAccount.Total_VSC,
                                ldecMonthAndYear, icdoDeathNotification.date_of_death.GetFirstDayofNextMonth(), false, icdoDeathNotification.date_of_death, lbusPersonAccountRetirement, iobjPassInfo)) //PIR 14646 - Vesting logic changes
                            {
                                ibusPerson.istrIsDeceasedEmployeeVested = busConstant.Flag_Yes;
                            }

                            busCorRetirement lbusCorRetirement = new busCorRetirement() { icdoCorRetirement = new DataObjects.doCorRetirement() };
                            lbusCorRetirement.icdoCorRetirement.death_notification_id = icdoDeathNotification.death_notification_id;
                            lbusCorRetirement.icdoCorRetirement.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                            lbusCorRetirement.icdoCorRetirement.is_vested = ibusPerson.istrIsDeceasedEmployeeVested;
                            lbusCorRetirement.icdoCorRetirement.is_db_retirement = lbusPersonAccountRetirement.ibusPlan.IsDBRetirementPlan() ? busConstant.Flag_Yes : busConstant.Flag_No;
                            lbusCorRetirement.icdoCorRetirement.is_dc_retirement = (lbusPersonAccountRetirement.ibusPlan.IsDCRetirementPlan() || lbusPersonAccountRetirement.ibusPlan.IsHBRetirementPlan()) ? busConstant.Flag_Yes : busConstant.Flag_No;
                            lbusCorRetirement.icdoCorRetirement.Insert();
                            lblnIsRetirementPlan = true;
                        }
                    }                    
                }
                lbusCorDeathNotification.icdoCorDeathNotification.is_insurance_plan = lblnIsInsurancePlan ? busConstant.Flag_Yes : busConstant.Flag_No;
                lbusCorDeathNotification.icdoCorDeathNotification.is_retirement_plan = lblnIsRetirementPlan ? busConstant.Flag_Yes : busConstant.Flag_No;
                lbusCorDeathNotification.icdoCorDeathNotification.Insert();

                DataTable ldtbBeneficiaryandDependent = Select("entPerson.LoadBeneficiaryandDependentDataForCor", new object[1] { icdoDeathNotification.person_id });
                foreach (DataRow ldrBeneficiaryandDependent in ldtbBeneficiaryandDependent.Rows)
                {
                    busCorBeneDep lbusCorBeneDep = new busCorBeneDep() { icdoCorBeneDep = new DataObjects.doCorBeneDep() };
                    lbusCorBeneDep.icdoCorBeneDep.death_notification_id = icdoDeathNotification.death_notification_id;
                    lbusCorBeneDep.icdoCorBeneDep.person_id = Convert.ToInt32(ldrBeneficiaryandDependent["ORG_ID"]) > 0 ? 0 : Convert.ToInt32(ldrBeneficiaryandDependent["PERSON_ID"]);
                    lbusCorBeneDep.icdoCorBeneDep.org_id = Convert.ToInt32(ldrBeneficiaryandDependent["ORG_ID"]);
                    lbusCorBeneDep.icdoCorBeneDep.person_dependent_id = Convert.ToInt32(ldrBeneficiaryandDependent["PERSON_DEPENDENT_ID"]);
                    lbusCorBeneDep.icdoCorBeneDep.beneficiary_id = Convert.ToInt32(ldrBeneficiaryandDependent["BENEFICIARY_ID"]);
                    lbusCorBeneDep.icdoCorBeneDep.person_account_id = Convert.ToInt32(ldrBeneficiaryandDependent["PERSON_ACCOUNT_ID"]);
                    lbusCorBeneDep.icdoCorBeneDep.beneficiary_relationship = ldrBeneficiaryandDependent["BENEFICIARY_RELATIONSHIP"].ToString();
                    lbusCorBeneDep.icdoCorBeneDep.dependent_relationship = ldrBeneficiaryandDependent["DEPENDENT_RELATIONSHIP"].ToString();
                    //lbusCorBeneDep.icdoCorBeneDep.family_relationship = "";
                    //lbusCorBeneDep.icdoCorBeneDep.account_relationship = "";
                    lbusCorBeneDep.icdoCorBeneDep.Insert();
                }
                if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Count > 0)
                {
                    foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
                    {
                        busCorPayeeAccount lbusCorPayeeAccount = new busCorPayeeAccount() { icdoCorPayeeAccount = new DataObjects.doCorPayeeAccount() };
                        lbusCorPayeeAccount.icdoCorPayeeAccount.death_notification_id = icdoDeathNotification.death_notification_id;
                        lbusCorPayeeAccount.icdoCorPayeeAccount.payee_account_id = lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                        lbusCorPayeeAccount.icdoCorPayeeAccount.Insert();
                    }
                }

                DataTable ldtbBeneficiaryandDependentTo = Select("entPerson.LoadBeneficiaryandDependentToDataForCor", new object[1] { icdoDeathNotification.person_id });
                foreach (DataRow ldrBeneficiaryandDependentTo in ldtbBeneficiaryandDependentTo.Rows)
                {
                    busCorBeneDepTo lbusCorBeneDepTo = new busCorBeneDepTo() { icdoCorBeneDepTo = new DataObjects.doCorBeneDepTo() };
                    lbusCorBeneDepTo.icdoCorBeneDepTo.death_notification_id = icdoDeathNotification.death_notification_id;
                    lbusCorBeneDepTo.icdoCorBeneDepTo.person_account_id = Convert.ToInt32(ldrBeneficiaryandDependentTo["PERSON_ACCOUNT_ID"]);
                    lbusCorBeneDepTo.icdoCorBeneDepTo.dependent_to = ldrBeneficiaryandDependentTo["DEPENDENT_TO"].ToString();
                    lbusCorBeneDepTo.icdoCorBeneDepTo.beneficiary_to = ldrBeneficiaryandDependentTo["BENEFICIARY_TO"].ToString();
                    lbusCorBeneDepTo.icdoCorBeneDepTo.beneficiary_relationship = ldrBeneficiaryandDependentTo["BENEFICIARY_RELATIONSHIP"].ToString();
                    lbusCorBeneDepTo.icdoCorBeneDepTo.dependent_relationship = ldrBeneficiaryandDependentTo["DEPENDENT_RELATIONSHIP"].ToString();
                    lbusCorBeneDepTo.icdoCorBeneDepTo.person_account_beneficiary_id = Convert.ToInt32(ldrBeneficiaryandDependentTo["PERSON_ACCOUNT_BENEFICIARY_ID"].ToString());
                    lbusCorBeneDepTo.icdoCorBeneDepTo.person_account_dependent_id = Convert.ToInt32(ldrBeneficiaryandDependentTo["PERSON_ACCOUNT_DEPENDENT_ID"].ToString());
                    lbusCorBeneDepTo.icdoCorBeneDepTo.Insert();
                }
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        #endregion

        public bool iblnIsDatesAreInsertedForDepeTo { get; set; }
        public bool iblnIsDatesAreInsertedForBeneTo { get; set; }
        public bool iblnIsPersonEmploymentWithoutEndDate { get; set; }
        public bool iblnIsPersonEmploymentFutureEndDate { get; set; }
        public bool iblnIsFlexCompSuspended { get; set; }
        public bool iblnIsEmploymentEnded { get; set; }
        public bool iblnIsDateOfDeathEqualsToEmploymentEndDate { get; set; }
        public Collection<busCorTracking> iclbAllGeneratedCorByPerson { get; set; }

        # region LoadMethods
        //Find Death Notification by Person ID
        public bool FindDeathNotificationByPersonId(int AintPersonId)
        {
            icdoDeathNotification = new cdoDeathNotification();
            DataTable ldtbDeathNotification = Select<cdoDeathNotification>(new string[1] { "person_id" }, new object[1] { AintPersonId }, null, null);
            if (ldtbDeathNotification.Rows.Count > 0)
            {
                icdoDeathNotification.LoadData(ldtbDeathNotification.Rows[0]);
                return true; ;
            }
            return false;
        }
        public int iintCountOfOpenEmployment
        {
            get
            {
                if (ibusPerson.IsNotNull())
                {
                    ibusPerson.LoadPersonEmployment();
                    return ibusPerson.icolPersonEmployment.Count(obj => obj.icdoPersonEmployment.end_date == DateTime.MinValue);
                }
                return 0;
            }
        }
        //load all methods to populate all tabs related to concerned person id 
        public void LoadPersonRelatedDetails()
        {
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPerson.iclbPersonBeneficiary == null)
                ibusPerson.LoadBeneficiaryForDeceased();

            if (ibusPerson.iclbPersonAccountBeneficiary == null)
                LoadPersonAccountBeneficiary();

            if (ibusPerson.icolPersonContact == null)
                ibusPerson.LoadContacts();

            if (ibusPerson.iclbPersonContactTo == null)
                ibusPerson.LoadPersonContactTo();

            if (ibusPerson.iclbPersonDependent == null)
                ibusPerson.LoadDependent();

            if (ibusPerson.iclbPersonAccountDependent == null)
                LoadPersonAccountDependent();

            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();

            if (ibusPerson.iclbActivePensionAccounts == null)
                ibusPerson.LoadActivePlanDetails();

            //UCS - 54 
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();

            //PIR 1863
            if (ibusPerson.iclbBeneficiaryApplication.IsNull())
                ibusPerson.LoadBeneficiaryApplication(true);
            if (ibusPerson.iclbBeneficiaryApplicationForBeneficiaryTo.IsNull())
                ibusPerson.LoadApplicationsForBeneficiaryTo();

            // Notice of Death - 3rd Party Payor
            if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdJobService3rdPartyPayor))
            {
                if (ibusPerson.iclbActiveBeneForGivenPlan.IsNull()) ibusPerson.LoadActiveBeneForGivenPlan(busConstant.PlanIdJobService3rdPartyPayor, icdoDeathNotification.date_of_death);
                if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
            }
            else
            {
                if (ibusPerson.iclbActiveBeneForGivenPlan.IsNull())
                    ibusPerson.iclbActiveBeneForGivenPlan = new Collection<busPersonBeneficiary>();
            }
            if (ibusPerson.iclbDeathNoticeForGrid.IsNull())
                ibusPerson.LoadDeathNoticeForGrid();
        }

        public void LoadPersonAccountBeneficiary()
        {
            if (ibusPerson.iclbPersonAccountBeneficiary == null)
                ibusPerson.iclbPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();

            ibusPerson.LoadPersonAccountBeneficiary();
            foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in ibusPerson.iclbPersonAccountBeneficiary)
            {
                lobjPersonAccountBeneficiary.LoadPersonAccount();
                lobjPersonAccountBeneficiary.FindPersonBeneficiary(lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_id);
                lobjPersonAccountBeneficiary.LoadPerson();
            }
        }

        // Load Person Account Dependent
        public void LoadPersonAccountDependent()
        {
            if (ibusPerson.iclbPersonAccountDependent == null)
                ibusPerson.iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();
            ibusPerson.LoadPersonAccountDependent();
            foreach (busPersonAccountDependent lobjPersonAccountDependent in ibusPerson.iclbPersonAccountDependent)
            {
                lobjPersonAccountDependent.FindPersonDependent(lobjPersonAccountDependent.icdoPersonAccountDependent.person_dependent_id);
                lobjPersonAccountDependent.LoadPerson();
            }
        }
        # endregion

        # region Rules

        //BR-053-11
        //Check if any Purchase credit exists for this deceased member
        //with status In payment, Pending, Approved
        //Load All the service credit for deceased member 
        //and check status is In payment, Pending, Approved
        //if yes then raise information message.
        public bool IsSerPurWithStatusInPayPendAppExistForPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.iclbServicePurchaseHeader == null)
                ibusPerson.LoadServicePurchase(false);
            // BPM death automation - Removed Pending action status
            foreach (busServicePurchaseHeader lobjServicePurchase in ibusPerson.iclbServicePurchaseHeader)
            {
                if ((lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
                    //|| (lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Pending)
                    || (lobjServicePurchase.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Approved))
                {
                    return false;
                }
            }
            return true;
        }

        //
        private bool IsAllPersonRelatedDatasEnded()
        {
            if (ibusPerson == null)
                LoadPerson();

            //1. Check if any beneficiary record is open
            //2. Check all dependent records are ended    
            //3. check if all employment is ended for person
            //4. check if any plan is enrolled
            //5. Check all contacts are inactive status
            //6. check if Contact to is inactive status
            //7. check if the beneficiary to is ended
            //8. check if the dependent to is ended
            //9. check rhic exists with status 'Pending Approval' and 'Approved'.
            if (CheckIfAllBeneficiariesEndDated()
                && (CheckAllDependentsAreAlive())
                && (CheckIfAllEmploymentEndedForPerson())
                && (CheckIfAllPlanParticipationIsEnded())
                && (CheckIfAllContactToInActive())
                //&& (CheckIfAllContactsInActive()) // Exclude Contact Status under BPM Death Notification PIR
                && (CheckIfAllBeneficiaryToEndDated())
                && (CheckIfAllDependentToEndDated())
                && (!CheckIfRHICinPendingApprovalORApprovedStatus()))
            {
                return true;
            }
            return false;
        }

        private bool CheckIfRHICinPendingApprovalORApprovedStatus()
        {
            if (iclbAllRHICombine.IsNull())
                LoadAllRHICCombine();

            var lenuList = iclbAllRHICombine.Where(i => i.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved
                                                    || i.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusPendingApproval);

            //UAT PIR 2106: We need to exclude the RHIC Record which is getting created for spouse (after the death date) in the validation 
            return lenuList.Any(i => i.icdoBenefitRhicCombine.start_date <= icdoDeathNotification.date_of_death.GetLastDayofMonth());
        }

        private bool CheckIfAllPlanParticipationIsEnded()
        {
            if (ibusPerson.iclbActivePensionAccounts == null)
                ibusPerson.LoadActivePlanDetails();
            foreach (busPersonAccount lobjPersonAcc in ibusPerson.iclbActivePensionAccounts)
            {
                if ((lobjPersonAcc.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)
                    || (lobjPersonAcc.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    || (lobjPersonAcc.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfAllEmploymentEndedForPerson()
        {
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            {
                if (lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckAllDependentsAreAlive()
        {
            if (ibusPerson.iclbPersonDependent == null)
                ibusPerson.LoadDependent();
            foreach (busPersonDependent lobjPersonDependent in ibusPerson.iclbPersonDependent)
            {
                if (lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfAllBeneficiariesEndDated()
        {
            if (ibusPerson.iclbPersonBeneficiary == null)
                ibusPerson.LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPerson.iclbPersonBeneficiaryForDeceased)
            {
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                {
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                        lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull())
                        lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();

                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsInsurancePlan())
                    {
                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled
                            || lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                        {
                            if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
                            {
                                return false;
                            }
                        }
                    }
                    else if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsRetirementPlan())
                    {
                        if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended
                            || lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                            || lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
                        {
                            if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
                            {
                                return false;
                            }
                        }
                    }

                }
            }
            return true;
        }

        private bool CheckIfAllContactsInActive()
        {
            if (ibusPerson.icolPersonContact == null)
                ibusPerson.LoadContacts();

            foreach (busPersonContact lobjPersonContact in ibusPerson.icolPersonContact)
            {
                if (lobjPersonContact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfAllContactToInActive()
        {
            if (ibusPerson.iclbContactTo == null)
                ibusPerson.LoadContactTo();
            foreach (busPersonContact lobjPersonContact in ibusPerson.iclbPersonContactTo)
            {
                if ((lobjPersonContact.icdoPersonContact.contact_person_id == icdoDeathNotification.person_id)
                    && (lobjPersonContact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfAllDependentToEndDated()
        {
            if (ibusPerson.iclbPersonAccountDependent == null)
                LoadPersonAccountDependent();
            foreach (busPersonAccountDependent lobjPersonAccountDependent in ibusPerson.iclbPersonAccountDependent)
            {
                if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfAllBeneficiaryToEndDated()
        {
            if (ibusPerson.iclbPersonAccountBeneficiary == null)
                LoadPersonAccountBeneficiary();
            foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in ibusPerson.iclbPersonAccountBeneficiary)
            {
                if (lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsDependentEnrolledIfLife()
        {
            if (ibusPerson.iclbDependentTo == null)
                ibusPerson.LoadDependentTo();

            foreach (busPerson lobjPersonDependentTo in ibusPerson.iclbDependentTo)
            {
                if (IsPersonEnrollinLife(lobjPersonDependentTo.icdoPerson.person_id) && (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, DateTime.Now) <= 26))
                    return true;
            }
            return false;
        }

        public bool IsPersonEnrolledInLifeAsSpouse()
        {
            if (ibusPerson.iclbDependentTo == null)
                ibusPerson.LoadDependentTo();

            DataTable ldtbContact = Select<cdoPersonContact>(new string[2] { "contact_person_id", "relationship_value" }, new object[2] { icdoDeathNotification.person_id, busConstant.PersonContactTypeSpouse }, null, null);
            if (ldtbContact.Rows.Count > 0)
            {
                int lintMemberPersonId = Convert.ToInt32(ldtbContact.Rows[0]["person_id"]);
                if (IsPersonEnrollinLife(lintMemberPersonId) && (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, DateTime.Now) <= 26))
                    return true;
            }
            return false;
        }

        private bool IsPersonEnrollinLife(int lintMemberPersonId)
        {
            busPerson lbusPerson = new busPerson();
            lbusPerson.FindPerson(lintMemberPersonId);
            lbusPerson.LoadLifePersonAccount();

            if (lbusPerson.ibusLifePersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lbusPerson.ibusLifePersonAccount.LoadPersonAccountLife();
                if (lbusPerson.ibusLifePersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.person_account_id > 0)
                {
                    lbusPerson.ibusLifePersonAccount.ibusPersonAccountLife.LoadLifeOptionData();
                    foreach (busPersonAccountLifeOption lobjPerAcclifeOpt in lbusPerson.ibusLifePersonAccount.ibusPersonAccountLife.iclbLifeOption)
                    {
                        if ((lobjPerAcclifeOpt.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                            && (lobjPerAcclifeOpt.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)
                            && (lobjPerAcclifeOpt.icdoPersonAccountLifeOption.coverage_amount != 0.00M))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // PIR 1480
        // check if all the beneficiaries have active address else throw warning.
        public bool IsAllBeneficiarieshaveActiveAddress()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbPersonBeneficiary == null)
                ibusPerson.LoadBeneficiaryForDeceased();

            foreach (busPersonBeneficiary lobjPersonBeneficiary in ibusPerson.iclbPersonBeneficiary)
            {
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.istrIsAddressActive != busConstant.Flag_Yes)
                    return false;
            }
            return true;
        }

        # endregion

        private Collection<busBenefitDroApplication> _iclbBenefitDROApplication;
        public Collection<busBenefitDroApplication> iclbBenefitDROApplication
        {
            get { return _iclbBenefitDROApplication; }
            set { _iclbBenefitDROApplication = value; }
        }

        public void LoadDROApplication()
        {
            _iclbBenefitDROApplication = new Collection<busBenefitDroApplication>();
            DataTable ldtbResultMembApp = Select<cdoBenefitDroApplication>(
                                    new string[1] { "member_perslink_id" },
                                    new object[1] { icdoDeathNotification.person_id }, null, null);

            DataTable ldtbResultAltApp = Select<cdoBenefitDroApplication>(
                                  new string[1] { "alternate_payee_perslink_id" },
                                  new object[1] { icdoDeathNotification.person_id }, null, null);

            foreach (DataRow lobjDROApp in ldtbResultMembApp.Rows)
            {
                LoadDRO(lobjDROApp);
            }
            foreach (DataRow lobjDROApp in ldtbResultAltApp.Rows)
            {
                LoadDRO(lobjDROApp);
            }
        }

        private void LoadDRO(DataRow aobjDROApp)
        {
            busBenefitDroApplication lobjDRO = new busBenefitDroApplication();
            lobjDRO.icdoBenefitDroApplication = new cdoBenefitDroApplication();
            lobjDRO.icdoBenefitDroApplication.LoadData(aobjDROApp);
            iclbBenefitDROApplication.Add(lobjDRO);
        }

        public override void BeforePersistChanges()
        {
            //F/W Upgrade PIR 23229 - Objectstate comes as none when no changes are made on screen, when death notification screen is launched 
            //via 248 process initiated by copy-closed saved death contact ticket, refer to 248 process initiation in contac ticket save logic.
            istrObjectState = icdoDeathNotification.death_notification_id == 0 ? ObjectState.Insert : icdoDeathNotification.ienuObjectState;
            if (icdoDeathNotification.death_notification_id == 0)
                iblnIsNewMode = true;
            else
                iblnIsNewMode = false;
            bool lblnUpdate = false;
            if (icdoDeathNotification.person_id != 0)
            {
                //bus person object must be loaded every time when person get changed.
                //thats why null check not done. it has to be refreshed.
                LoadPerson();
                icdoDeathNotification.last_name = ibusPerson.icdoPerson.last_name;
                icdoDeathNotification.first_name = ibusPerson.icdoPerson.first_name;
                LoadPersonRelatedDetails();
                LoadAllRHICCombine();
            }
            if (icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes)
            {
                
                icdoDeathNotification.date_of_death_certified_on = DateTime.Today;
                if (iclbBenefitDROApplication == null)
                    LoadDROApplication();
                foreach (busBenefitDroApplication lobjBenefitDROAppilcation in iclbBenefitDROApplication)
                {
                    if (!lobjBenefitDROAppilcation.IsDROApplicationCancelledOrDenied())
                    {
                        if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                        {
                            //Missing rule in UCS - 059
                            //Implemented as per Satya on 13/April/10
                            //If any Active Job Service DRO exists for person id as member perslink id populate the user date with 1st of the month following date of death
                            //only if dro calc. status is either null or pending
                            //--Start--//
                            if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel &&
                                lobjBenefitDROAppilcation.icdoBenefitDroApplication.member_perslink_id == icdoDeathNotification.person_id)
                            {
                                busBenefitDroCalculation lobjBenefitDroCalculation = new busBenefitDroCalculation();
                                lobjBenefitDroCalculation.FindBenefitDroCalculationByApplicationID(lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_application_id);
                                if (lobjBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == null ||
                                    lobjBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusPending)
                                {
                                    lobjBenefitDROAppilcation.icdoBenefitDroApplication.benefit_receipt_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                                    lblnUpdate = true;
                                }
                            }
                            //--End--//
                            if ((lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag == busConstant.Flag_No) || (lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag == null))
                            {
                                lobjBenefitDROAppilcation.InitiateDROWorkflow();
                                lobjBenefitDROAppilcation.icdoBenefitDroApplication.work_flow_intiated_flag = busConstant.Flag_Yes;
                                lblnUpdate = true;
                            }
                            if (lblnUpdate)
                            {
                                lobjBenefitDROAppilcation.icdoBenefitDroApplication.Update();
                                lblnUpdate = false;
                            }
                        }
                    }
                }
                //UCS - 079 : Initiating Popup Benefit Workflow
                //--Start--//
                if (ibusPerson == null)
                    LoadPerson();
                ibusPerson.LoadBenefitApplicationForJointAnnuitant();
                busBenefitApplication lobjJointAnnuitantApplication = ibusPerson.iclbJointAnnuitantApplications
                                                                                .Where(o => o.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)
                                                                                .FirstOrDefault();
                if (lobjJointAnnuitantApplication.IsNotNull())
                {
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.FindPerson(lobjJointAnnuitantApplication.icdoBenefitApplication.member_person_id);
                    lobjPerson.CheckPayeeAccount();
                }
                
                //--End--//
            }
            //PIR 22733
            if (icdoDeathNotification.person_id > 0 && icdoDeathNotification.death_notification_id == 0)
            {
                if (IsRecoveryinInprocessandApprovedstatus())
                {
                    InitializeWorkFlow(icdoDeathNotification.person_id, busConstant.Map_Initialize_Process_PA_Death_Notification_Workflow);
                }
            }
            if (icdoDeathNotification.ienuObjectState == ObjectState.None && istrObjectState == ObjectState.Insert)
                icdoDeathNotification.ienuObjectState = ObjectState.Insert;
            base.BeforePersistChanges();
        }
        public bool IsRecoveryinInprocessandApprovedstatus()
        {
            DataTable ldtRunningInstance = busBase.Select("entDeathNotification.GetCountofRecoveryInApprovedandInprocess",
                                    new object[1] { icdoDeathNotification.person_id });
            if (ldtRunningInstance != null && ldtRunningInstance.Rows.Count > 0 && ldtRunningInstance.Rows[0]["TOTALCOUNT"] != DBNull.Value)
            {
                int lintCount = Convert.ToInt32(ldtRunningInstance.Rows[0]["TOTALCOUNT"]);
                if (lintCount > 0)
                    return true;
            }
            return false;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            if (icdoDeathNotification.person_id != 0)
            {
                ibusPerson.icdoPerson.date_of_death = icdoDeathNotification.date_of_death;
                if (icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes)
                    ibusPerson.icdoPerson.ndpers_login_id = null;
                ibusPerson.icdoPerson.Update();

                //UCS -057 BR -07
                if (ibusPerson.iclbPayeeAccount == null)
                    ibusPerson.LoadPayeeAccount();
                foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount)
                {
                    // PROD PIR ID 6069
                    //if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                        lobjPayeeAccount.LoadActivePayeeStatus();
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved() ||
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusInPayment())
                        lobjPayeeAccount.CreateReviewPayeeAccountStatus();

                    lobjPayeeAccount.iblnDeathNotificationIndicator = true;
                    // PIR ID 1773
                    if (lobjPayeeAccount.icdoPayeeAccount.dro_application_id != 0)
                    {
                        if (lobjPayeeAccount.ibusMember.IsNull())
                            lobjPayeeAccount.LoadMember();
                        if (lobjPayeeAccount.ibusMember.IsDeathNotified())
                            lobjPayeeAccount.iblnIsMembersDeathNotified = true;
                    }
                    if (lobjPayeeAccount.ibusSoftErrors == null)
                        lobjPayeeAccount.LoadErrors();
                    lobjPayeeAccount.iblnClearSoftErrors = false;
                    lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id == icdoDeathNotification.person_id)
                        lobjPayeeAccount.iblnIsPayeesDeathNotified = true;
                    lobjPayeeAccount.ValidateSoftErrors();
                    lobjPayeeAccount.UpdateValidateStatus();
                }

                //PIR - 1845
                if (ibusPerson.iclbAlternatePayeeAccounts.IsNull())
                    ibusPerson.LoadAlternatePayeePayeeAccounts();
                foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbAlternatePayeeAccounts)
                {
                    if (busGlobalFunctions.GetData2ByCodeValue(2216, lobjPayeeAccount.icdoPayeeAccount.benefit_option_value, iobjPassInfo) == busConstant.BenefitAccountOwner) // PIR 7703
                    {
                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull()) lobjPayeeAccount.LoadActivePayeeStatus();
                        if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusApproved() ||
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusInPayment())
                            lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                    }
                    lobjPayeeAccount.iblnDeathNotificationIndicator = true;

                    if (lobjPayeeAccount.icdoPayeeAccount.dro_application_id != 0)
                    {
                        if (lobjPayeeAccount.ibusMember.IsNull())
                            lobjPayeeAccount.LoadMember();
                        if (lobjPayeeAccount.ibusMember.IsDeathNotified())
                            lobjPayeeAccount.iblnIsMembersDeathNotified = true;
                    }
                    if (lobjPayeeAccount.ibusSoftErrors == null)
                        lobjPayeeAccount.LoadErrors();
                    lobjPayeeAccount.iblnClearSoftErrors = false;
                    lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id == icdoDeathNotification.person_id)
                        lobjPayeeAccount.iblnIsPayeesDeathNotified = true;
                    lobjPayeeAccount.ValidateSoftErrors();
                    lobjPayeeAccount.UpdateValidateStatus();
                }

                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
                if (istrObjectState == ObjectState.Insert)
                {
                    SetPayeeAccountStatusAsReview();
                }
                //if date of death marked as certified
                if (icdoDeathNotification.death_certified_flag == busConstant.Flag_Yes)
                {
                    SetBenefitEndDateForPayee();
                    SetBenefitEndDateForMember();
                }

                LoadPersonRelatedDetails();

                //reload the payee account after status change
                ibusPerson.LoadPayeeAccount();
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();

                LoadEstateOrgCodeID();

                //PIR 14346 - RHIC Changes Start
                // Start Death Automation Process
                if (iblnIsNewMode)
                {
                    if (ibusPerson.ibusLatestBenefitRhicCombine.IsNull()) ibusPerson.LoadLatestBenefitRhicCombine();
                    if (ibusPerson.ibusLatestBenefitRhicCombine.IsNotNull()) //The deceased member is receiver and has approved RHIC combine record
                    {
                        if (ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.IsNull()) ibusPerson.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();
                        foreach (busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail in ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail)
                        {
                            lbusBenefitRhicCombineDetail.LoadPayeeAccount();
                        }
                        if (ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count <= 1) //Conversion records may have 0 details
                        {
                            //We can simply end this record
                            ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoDeathNotification.date_of_death.GetLastDayofMonth());
                            CreateRHICForJointAnnuitant();
                        }
                        //All donor payee accounts are of the deceased member in which case we need to simply end this combine record
                        else if (ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Count ==
                            ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                            .Where(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id == icdoDeathNotification.person_id).Count())
                        {
                            ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoDeathNotification.date_of_death.GetLastDayofMonth());
                            CreateRHICForJointAnnuitant();
                        }
                        //One of the donor is spouse of the deceased member     
                        else if (ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                            .Any(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoDeathNotification.person_id))
                        {
                            busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                                .Where(i => i.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoDeathNotification.person_id
                                        && i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                                .FirstOrDefault();
                            if (lbusBenefitRhicCombineDetail.IsNotNull()) //donors contain spouse and is combined
                            {
                                int lintRHICSpousePerslinkID = lbusBenefitRhicCombineDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                                //Before Rhic Record gets created, spouse may go inactive.. so, lets find out the spouse of each other and create an automatic RHIC for that spouse
                                int lintSpousePersonID = 0;
                                if (ibusPerson.iclbAllSpouse == null)
                                    ibusPerson.LoadAllSpouse();
                                foreach (busPerson lbusSpouse in ibusPerson.iclbAllSpouse)
                                {
                                    if (lbusSpouse.iclbAllSpouse == null)
                                        lbusSpouse.LoadAllSpouse();

                                    if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == ibusPerson.icdoPerson.person_id))
                                    {
                                        lintSpousePersonID = lbusSpouse.icdoPerson.person_id;
                                        break;
                                    }
                                }
                                if (lintSpousePersonID > 0 && lintSpousePersonID == lintRHICSpousePerslinkID)
                                {
                                    ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoDeathNotification.date_of_death.GetLastDayofMonth());
                                    busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine() { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lintRHICSpousePerslinkID;
                                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                                    lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.death_notification_save;
                                    lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                                }
                                else
                                {
                                    ibusPerson.ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
                                }
                            }
                            else
                            {
                                //donor details contain spouse's payee account, but not combined, simply end this combined record
                                ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoDeathNotification.date_of_death.GetLastDayofMonth());
                                CreateRHICForJointAnnuitant();
                            }
                        }
                    }
                    else //The deceased member may be a donor for spouse
                    {
                        ibusPerson.LoadPayeeAccount();
                        foreach (busPayeeAccount lbusPayeeAccount in ibusPerson.iclbPayeeAccount)
                        {
                            lbusPayeeAccount.LoadLatestBenefitRhicCombine(true);
                        }
                        busBenefitRhicCombine lbusLatestApprovedBenefitRhicCombine = null;
                        if (ibusPerson.iclbPayeeAccount.Any(i => i.ibusLatestBenefitRhicCombine.IsNotNull()))
                        {
                            lbusLatestApprovedBenefitRhicCombine = ibusPerson.iclbPayeeAccount
                                                                           .Where(i => i.ibusLatestBenefitRhicCombine.IsNotNull())
                                                                           .Select(i => i.ibusLatestBenefitRhicCombine)
                                                                           .FirstOrDefault();
                        }
                        if (lbusLatestApprovedBenefitRhicCombine.IsNotNull())
                        {
                            lbusLatestApprovedBenefitRhicCombine.EndRHICCombine(icdoDeathNotification.date_of_death.GetLastDayofMonth());
                            busBenefitRhicCombine lNewbusBenefitRhicCombine = new busBenefitRhicCombine() { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                            lNewbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lbusLatestApprovedBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
                            lNewbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                            lNewbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.death_notification_save;
                            lNewbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                        }
                    }
                    //End all person contact to records
                    if (!ibusPerson.iclbPersonContactTo.IsNullOrEmpty())
                    {
                        foreach (busPersonContact lbusPersonContact in ibusPerson.iclbPersonContactTo)
                        {
                            int lintContactToSpousePersonID = ibusPerson.iclbPersonContactTo.Select(i => i.icdoPersonContact.person_id).FirstOrDefault();

                            if (lbusPersonContact.icdoPersonContact.relationship_value == busConstant.FamilyRelationshipSpouse && 
                                lbusPersonContact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive)
                            {
                                if (lintContactToSpousePersonID > 0)
                                {
                                    DataTable ldtbPersonAccount = Select<cdoPersonAccount>(
                                                  new string[3] { enmPersonAccount.person_id.ToString(), enmPersonAccount.plan_id.ToString(), enmPersonAccount.plan_participation_status_value.ToString() },
                                                  new object[3] { lintContactToSpousePersonID, busConstant.PlanIdGroupLife, busConstant.PlanParticipationStatusInsuranceEnrolled }, null, null);
                                    busPersonAccount lbusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
                                    if (ldtbPersonAccount.Rows.Count > 0)
                                    {
                                        lbusPersonAccount.icdoPersonAccount.LoadData(ldtbPersonAccount.Rows[0]);
                                        UpdateLifePlan(lbusPersonAccount);
                                    }
                                }
                            }
                            lbusPersonContact.icdoPersonContact.status_value = busConstant.PersonContactStatusInActive;
                            lbusPersonContact.icdoPersonContact.Update();

                        }
                    }

                    //End all dependent to records
                    if (!ibusPerson.iclbPersonAccountDependent.IsNullOrEmpty())
                    {
                        DataTable ldtbPersonDependent = Select<cdoPersonDependent>(new string[1] { "DEPENDENT_PERSLINK_ID" },
                                                            new object[1] { icdoDeathNotification.person_id }, null, null);
                        if (ldtbPersonDependent.Rows.Count > 0)
                        {
                            foreach (DataRow ldtPersonDepeRow in ldtbPersonDependent.Rows)
                            {
                                int lintChildDependentCount = 0;
                                busPersonDependent lbusPersonDependent = new busPersonDependent() { icdoPersonDependent = new cdoPersonDependent() };
                                lbusPersonDependent.icdoPersonDependent.LoadData(ldtPersonDepeRow);
                                lbusPersonDependent.LoadPerson();
                                lbusPersonDependent.LoadOtherDependents();
                                lbusPersonDependent.LoadDependentInfo();
                                lbusPersonDependent.LoadPersonAccountDependentUpdate();
                                lbusPersonDependent.LoadPersonAccountDependent();
                                lbusPersonDependent.iblnIsFromDeathNotification = true;
                                lbusPersonDependent.icdoPersonDependent.Suppress_Warning = icdoDeathNotification.suppress_warnings_flag;
                                foreach (busPersonAccountDependent lobjPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                                {
                                    if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue
                                         || lobjPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth())
                                    {
                                        if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth())
                                            lobjPersonAccountDependent.iblnIsNeedToValidate = false;
                                        // 10 & 11. Dental & Vision – if Spouse death is reported and Dental/Vision is enrolled as ‘Family’, update to ‘Individual & Children’ or if ‘Individual & Spouse’, update to ‘Individual Only’, etc.  
                                        //if (lbusPersonDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse)
                                        {
                                            if (lobjPersonAccountDependent.ibusPersonAccount.IsNull())
                                                lobjPersonAccountDependent.LoadPersonAccount();
                                            if (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan == null)
                                                lobjPersonAccountDependent.ibusPersonAccount.LoadPlan();
                                            lobjPersonAccountDependent.ibusPersonAccount.LoadPersonAccountDependent();



                                            if (lobjPersonAccountDependent.ibusPersonAccount.iclbPersonAccountDependent.IsNotNull())
                                            {
                                                lobjPersonAccountDependent.ibusPersonAccount.iclbPersonAccountDependent.ForEach(lobjParentPersonAccountDependent => lobjParentPersonAccountDependent.FindPersonDependent(lobjParentPersonAccountDependent.icdoPersonAccountDependent.person_dependent_id));

                                                lintChildDependentCount = lobjPersonAccountDependent.ibusPersonAccount.iclbPersonAccountDependent.Where(lobjParentPersonAccountDependent =>
                                                (lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                                                lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                                                lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild ||
                                                lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                                                lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipLegalGuardian ||
                                                lobjParentPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild)
                                                && lobjParentPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue
                                                || lobjParentPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth()).Count();
                                            }



                                            if (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                               (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental || lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                                               && (lbusPersonDependent.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse)
                                                    || (lintChildDependentCount == 1 && lbusPersonDependent.IsChildDependent())
                                               )
                                            {
                                                UpdateGHDVPlans(lobjPersonAccountDependent.ibusPersonAccount, lbusPersonDependent.icdoPersonDependent.relationship_value);
                                            }
                                        }
                                        lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = icdoDeathNotification.date_of_death.GetLastDayofMonth();
                                        iblnIsDatesAreInsertedForDepeTo = true;
                                    }
                                }
                                if (iblnIsDatesAreInsertedForDepeTo)
                                {
                                    lbusPersonDependent.BeforeValidate(utlPageMode.All);
                                    lbusPersonDependent.ValidateHardErrors(utlPageMode.All);
                                    if (lbusPersonDependent.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjErr in lbusPersonDependent.iarrErrors)
                                        {
                                            if (!iarrErrors.Contains(lobjErr))
                                                iarrErrors.Add(lobjErr);
                                        }
                                    }
                                    else
                                    {
                                        lbusPersonDependent.BeforePersistChanges();
                                        lbusPersonDependent.PersistChanges();
                                    }
                                    SetValuesForSoftErrorsDependent(lbusPersonDependent, null);
                                }
                            }
                        }
                    }
                    // End all Beneficiary to records
                    if (!ibusPerson.iclbPersonAccountBeneficiary.IsNullOrEmpty())
                    {
                        busPersonBeneficiary lbusPersonBeneficiary = new busPersonBeneficiary() { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                        DataTable ldtbPersonBeneficiary = Select<cdoPersonBeneficiary>(new string[1] { "BENEFICIARY_PERSON_ID" },
                                                            new object[1] { icdoDeathNotification.person_id }, null, null);
                        if (ldtbPersonBeneficiary.Rows.Count > 0)
                        {
                            foreach (DataRow ldtPersonBeneRow in ldtbPersonBeneficiary.Rows)
                            {
                                lbusPersonBeneficiary.icdoPersonBeneficiary.LoadData(ldtPersonBeneRow);
                                lbusPersonBeneficiary.LoadPerson();
                                lbusPersonBeneficiary.LoadPersonAccountBeneficiary();
                                lbusPersonBeneficiary.LoadBenefitApplicationForBene();
                                lbusPersonBeneficiary.LoadPersonAccountBeneficiaryByID();
                                lbusPersonBeneficiary.LoadPersonAccountBeneficiaryData();
                                lbusPersonBeneficiary.LoadOtherBeneficiaries();
                                lbusPersonBeneficiary.iblnIsFromDeathNotification = true;
                                lbusPersonBeneficiary.icdoPersonBeneficiary.suppress_warning = icdoDeathNotification.suppress_warnings_flag;
                                foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiary)
                                {
                                    if (lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue
                                         || lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date > icdoDeathNotification.date_of_death)
                                    {
                                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = icdoDeathNotification.date_of_death;
                                        iblnIsDatesAreInsertedForBeneTo = true;
                                    }
                                }
                                if (iblnIsDatesAreInsertedForBeneTo)
                                {
                                    lbusPersonBeneficiary.BeforeValidate(utlPageMode.All);
                                    lbusPersonBeneficiary.ValidateHardErrors(utlPageMode.All);
                                    if (lbusPersonBeneficiary.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjErr in lbusPersonBeneficiary.iarrErrors)
                                        {
                                            if (!iarrErrors.Contains(lobjErr))
                                                iarrErrors.Add(lobjErr);
                                        }
                                    }
                                    else
                                    {
                                        lbusPersonBeneficiary.BeforePersistChanges();
                                        lbusPersonBeneficiary.PersistChanges();
                                        lbusPersonBeneficiary.AfterPersistChanges();
                                    }
                                    SetValuesForSoftErrorsDependent(null, lbusPersonBeneficiary);
                                }
                            }
                        }
                    }
                    // End Employment and employment details
                    if (!ibusPerson.icolPersonEmployment.IsNullOrEmpty())
                    {
                        foreach (busPersonEmployment lbusPersonEmployment in ibusPerson.icolPersonEmployment)
                        {
                            if (lbusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue)
                            {
                                lbusPersonEmployment.icdoPersonEmployment.Update();
                                if (lbusPersonEmployment.icolPersonEmploymentDetail == null)
                                    lbusPersonEmployment.LoadPersonEmploymentDetail(false);
                                foreach (busPersonEmploymentDetail lbusPersonEmploymentDetail in lbusPersonEmployment.icolPersonEmploymentDetail)
                                {
                                    if (lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue ||
                                        lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date > lbusPersonEmployment.icdoPersonEmployment.end_date)
                                    {
                                        lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date = lbusPersonEmployment.icdoPersonEmployment.end_date;
                                        lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();
                                        iblnIsEmploymentEnded = true;
                                    }                                   
                                }
                            }                           
                        }
                    }
                    // End date all person dependent
                    if (!ibusPerson.iclbPersonDependent.IsNullOrEmpty())
                    {
                        //busPersonDependent lbusPersonDependent = new busPersonDependent() { icdoPersonDependent = new cdoPersonDependent() };
                        DataTable ldtbPersonDependent = Select<cdoPersonDependent>(new string[1] { "PERSON_ID" },
                                                            new object[1] { icdoDeathNotification.person_id }, null, null);
                        if (ldtbPersonDependent.Rows.Count > 0)
                        {
                            foreach (DataRow drDependent in ldtbPersonDependent.Rows)
                            {
                                busPersonDependent lbusPersonDependent = new busPersonDependent() { icdoPersonDependent = new cdoPersonDependent() };
                                bool lblnIsEndDateUpdate = false;
                                lbusPersonDependent.icdoPersonDependent.LoadData(drDependent);
                                lbusPersonDependent.LoadPerson();
                                lbusPersonDependent.LoadOtherDependents();
                                lbusPersonDependent.LoadDependentInfo();
                                lbusPersonDependent.LoadPersonAccountDependentUpdate();
                                lbusPersonDependent.LoadPersonAccountDependent();
                                lbusPersonDependent.iblnIsFromDeathNotification = true;
                                lbusPersonDependent.icdoPersonDependent.Suppress_Warning = icdoDeathNotification.suppress_warnings_flag;
                                foreach (busPersonAccountDependent lobjPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                                {
                                    lobjPersonAccountDependent.LoadPersonAccountGhdv();
                                    lobjPersonAccountDependent.LoadPersonAccount();
                                    if (lobjPersonAccountDependent.ibusPersonAccountGhdv.IsNotNull())
                                    {
                                        if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue
                                                  || lobjPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth())
                                        {
                                            if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth())
                                                lobjPersonAccountDependent.iblnIsNeedToValidate = false;
                                            if (((lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                                            ((lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState ||
                                            lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                                            (lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty())))
                                            || (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental && lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)
                                            || (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision && lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive))
                                            {
                                                lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth().GetLastDayofMonth();
                                                lblnIsEndDateUpdate = true;
                                            }
                                            else if (((lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                                             (((lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState ||
                                             lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                                             (lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())) ||
                                             (lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)))
                                            || (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental && lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeActive)
                                            || (lobjPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision && lobjPersonAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeActive))
                                            {
                                                lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = icdoDeathNotification.date_of_death.GetLastDayofMonth();
                                                lblnIsEndDateUpdate = true;
                                            }
                                        }
                                    }
                                }
                                if (lblnIsEndDateUpdate)
                                {
                                    lbusPersonDependent.BeforeValidate(utlPageMode.All);
                                    lbusPersonDependent.ValidateHardErrors(utlPageMode.All);
                                    if (lbusPersonDependent.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjErr in lbusPersonDependent.iarrErrors)
                                        {
                                            if (!iarrErrors.Contains(lobjErr))
                                                iarrErrors.Add(lobjErr);
                                        }
                                    }
                                    else
                                    {
                                        lbusPersonDependent.BeforePersistChanges();
                                        lbusPersonDependent.PersistChanges();
                                    }
                                    SetValuesForSoftErrorsDependent(lbusPersonDependent, null);
                                }
                            }
                            
                        }
                    }

                    // End all Beneficiary records
                    if (!ibusPerson.iclbPersonPlanBeneficiaryForDeceased.IsNullOrEmpty())
                    {
                        busPersonBeneficiary lbusPersonBeneficiary = new busPersonBeneficiary() { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                        DataTable ldtbPersonBeneficiary = Select<cdoPersonBeneficiary>(new string[1] { "PERSON_ID" },
                                                            new object[1] { icdoDeathNotification.person_id }, null, null);
                        if (ldtbPersonBeneficiary.Rows.Count > 0)
                        {
                            foreach (DataRow drBeneficiary in ldtbPersonBeneficiary.Rows)
                            {
                                bool lblnIsDatesAreInsertedForBene = false;
                                lbusPersonBeneficiary.icdoPersonBeneficiary.LoadData(drBeneficiary);
                                lbusPersonBeneficiary.LoadPerson();
                                lbusPersonBeneficiary.LoadPersonAccountBeneficiaryByID();
                                lbusPersonBeneficiary.LoadPersonAccountBeneficiaryData();
                                lbusPersonBeneficiary.LoadOtherBeneficiaries();
                                lbusPersonBeneficiary.LoadBenefitApplicationsBeneficiary();
                                lbusPersonBeneficiary.iblnIsFromDeathNotification = true;
                                lbusPersonBeneficiary.icdoPersonBeneficiary.suppress_warning = icdoDeathNotification.suppress_warnings_flag;

                                foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in lbusPersonBeneficiary.iclbPersonAccountBeneficiary)
                                {
                                    if (lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue
                                         || lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date > icdoDeathNotification.date_of_death)
                                    {
                                        lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date = icdoDeathNotification.date_of_death;
                                        lblnIsDatesAreInsertedForBene = true;
                                    }
                                }
                                if (lblnIsDatesAreInsertedForBene)
                                {
                                    lbusPersonBeneficiary.BeforeValidate(utlPageMode.All);
                                    lbusPersonBeneficiary.ValidateHardErrors(utlPageMode.All);
                                    if (lbusPersonBeneficiary.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjErr in lbusPersonBeneficiary.iarrErrors)
                                        {
                                            if (!iarrErrors.Contains(lobjErr))
                                                iarrErrors.Add(lobjErr);
                                        }
                                    }
                                    else
                                    {
                                        lbusPersonBeneficiary.BeforePersistChanges();
                                        lbusPersonBeneficiary.PersistChanges();
                                        lbusPersonBeneficiary.AfterPersistChanges();
                                    }
                                    SetValuesForSoftErrorsDependent(null, lbusPersonBeneficiary);
                                }
                            }
                            
                        }
                    }

                    //End All Plans
                    if (!ibusPerson.iclbActivePensionAccounts.IsNullOrEmpty() && !iblnIsEmploymentEnded)
                    {
                        //Update SEQ Fix : To Avoid Record Changed Since last modified.
                        ibusPerson.icolPersonAccount = null;
                        ibusPerson.LoadActivePlanDetails(); 

                        foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbActivePensionAccounts)
                        {
                            if (lbusPersonAccount.ibusPlan == null)
                                lbusPersonAccount.LoadPlan();
                            if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && lbusPersonAccount.ibusPersonAccountFlex == null)
                                lbusPersonAccount.LoadPersonAccountFlex();
                            if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && lbusPersonAccount.ibusPersonAccountLife == null)
                                lbusPersonAccount.LoadPersonAccountLife();
                            if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                                lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                                lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) &&
                                (lbusPersonAccount.ibusPersonAccountGHDV == null) &&
                                lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                UpdateGHDVPlans(lbusPersonAccount, string.Empty, true);
                            }
                            else if (lbusPersonAccount.ibusPlan.IsRetirementPlan() && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                            {
                                busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement
                                {
                                    icdoPersonAccount = new cdoPersonAccount(),
                                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                                };
                                if (lobjPARetirement.FindPersonAccountRetirement(lbusPersonAccount.icdoPersonAccount.person_account_id))
                                {
                                    //lobjPARetirement.ibusPerson = ibusPerson;
                                    lobjPARetirement.ibusPlan = lbusPersonAccount.ibusPlan;
                                    lobjPARetirement.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                                    lobjPARetirement.LoadPerson();
                                    lobjPARetirement.LoadPlanEffectiveDate();
                                    lobjPARetirement.icdoPersonAccount.person_employment_dtl_id = lobjPARetirement.GetAssociatedEmploymentDetailID();
                                    lobjPARetirement.LoadPersonEmploymentDetail();
                                    lobjPARetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                    lobjPARetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                    lobjPARetirement.LoadOrgPlan();

                                    // Change the status to Suspended.
                                    lobjPARetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetimentSuspended;
                                    lobjPARetirement.icdoPersonAccount.history_change_date = GetHistoryChangeDateofSuspendedAccount(lbusPersonAccount);
                                    lobjPARetirement.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                                    lobjPARetirement.icdoPersonAccount.reason_id = 332;
                                    //lobjPARetirement.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;

                                    // Calling the Save operation, that will insert the History.
                                    lobjPARetirement.iarrChangeLog.Add(lobjPARetirement.icdoPersonAccount);
                                    lobjPARetirement.iarrChangeLog.Add(lobjPARetirement.icdoPersonAccountRetirement);
                                    lobjPARetirement.BeforeValidate(utlPageMode.All);
                                    lobjPARetirement.BeforePersistChanges();
                                    lobjPARetirement.PersistChanges();
                                    lobjPARetirement.icdoPersonAccount.iblnIsEmploymentEnded = true;//PIR 20259
                                    lobjPARetirement.AfterPersistChanges();
                                    busGlobalFunctions.PostESSMessage(lobjPARetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, lbusPersonAccount.ibusPlan.icdoPlan.plan_id, iobjPassInfo); // PIR 9115 
                                }
                            }
                            else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                busPersonAccountEAP lobjPAEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
                                lobjPAEAP.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPAEAP.ibusPlan = lbusPersonAccount.ibusPlan;
                                lobjPAEAP.LoadPerson(); //.ibusPerson = ibusPerson;
                                lobjPAEAP.LoadPlanEffectiveDate();
                                lobjPAEAP.icdoPersonAccount.person_employment_dtl_id = lobjPAEAP.GetAssociatedEmploymentDetailID();
                                lobjPAEAP.LoadPersonEmploymentDetail();
                                lobjPAEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                lobjPAEAP.LoadOrgPlan();

                                // Change the status to Suspended.
                                lobjPAEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                                lobjPAEAP.icdoPersonAccount.history_change_date = GetHistoryChangeDateofSuspendedAccount(lbusPersonAccount); //idtelastDateOfService != DateTime.MinValue ? idtelastDateOfService.GetFirstDayofNextMonth().AddMonths(1) :
                                lobjPAEAP.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                                lobjPAEAP.icdoPersonAccount.reason_id = 332;
                                //lobjPAEAP.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;

                                // Calling the Save operation, that will insert the History.
                                lobjPAEAP.iarrChangeLog.Add(lobjPAEAP.icdoPersonAccount);
                                lobjPAEAP.BeforeValidate(utlPageMode.All);
                                lobjPAEAP.BeforePersistChanges();
                                lobjPAEAP.PersistChanges();
                                lobjPAEAP.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                                lobjPAEAP.AfterPersistChanges();
                                busGlobalFunctions.PostESSMessage(lobjPAEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, busConstant.PlanIdEAP, iobjPassInfo);                               
                            }
                            else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife &&
                                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                UpdateLifePlan(lbusPersonAccount, true);
                            }
                            else if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457) &&
                                      (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled))
                            {
                                busPersonAccountDeferredComp lobjPADefComp = new busPersonAccountDeferredComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
                                lobjPADefComp.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPADefComp.LoadPerson(); //ibusPerson = ibusPerson;
                                lobjPADefComp.ibusPlan = lbusPersonAccount.ibusPlan;
                                lobjPADefComp.LoadPlanEffectiveDate();
                                lobjPADefComp.icdoPersonAccount.person_employment_dtl_id = lobjPADefComp.GetAssociatedEmploymentDetailID();
                                lobjPADefComp.LoadPersonEmploymentDetail();
                                lobjPADefComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                lobjPADefComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                lobjPADefComp.LoadOrgPlan();
                                lobjPADefComp.FindPersonAccountDeferredComp(lobjPADefComp.icdoPersonAccount.person_account_id);

                                // Change the status to Suspended.
                                lobjPADefComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompSuspended;
                                lobjPADefComp.icdoPersonAccount.history_change_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                                lobjPADefComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                                lobjPADefComp.icdoPersonAccountDeferredComp.ienuObjectState = ObjectState.Update;
                                lobjPADefComp.icdoPersonAccount.reason_id = 332;
                                lobjPADefComp.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;

                                // Calling the Save operation, that will insert the History.
                                lobjPADefComp.iarrChangeLog.Add(lobjPADefComp.icdoPersonAccount);
                                lobjPADefComp.iarrChangeLog.Add(lobjPADefComp.icdoPersonAccountDeferredComp);
                                lobjPADefComp.iblnIsFromTerminationPost = true;
                                lobjPADefComp.idtmProviderEndDateWhenTEEM = icdoDeathNotification.date_of_death; //idtmDefCompProviderEndDateWhenTEEM;
                                lobjPADefComp.BeforeValidate(utlPageMode.Update);
                                lobjPADefComp.BeforePersistChanges();
                                lobjPADefComp.PersistChanges();
                                lobjPADefComp.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                                lobjPADefComp.AfterPersistChanges();
                                busGlobalFunctions.PostESSMessage(lobjPADefComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, busConstant.PlanIdDeferredCompensation, iobjPassInfo); // PIR 9115
                            }
                            else if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex) &&
                                    (lbusPersonAccount.ibusPersonAccountFlex.icdoPersonAccountFlexComp.flex_comp_type_value != busConstant.FlexCompTypeValueCOBRA) &&
                                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)
                            {
                                busPersonAccountFlexComp lobjPAFlex = new busPersonAccountFlexComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp() };
                                lobjPAFlex.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPAFlex.LoadPerson(); //ibusPerson = ibusPerson;
                                lobjPAFlex.ibusPlan = lbusPersonAccount.ibusPlan;
                                lobjPAFlex.LoadPlanEffectiveDate();
                                lobjPAFlex.icdoPersonAccount.person_employment_dtl_id = lobjPAFlex.GetAssociatedEmploymentDetailID();
                                lobjPAFlex.LoadPersonEmploymentDetail();
                                lobjPAFlex.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                lobjPAFlex.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                                lobjPAFlex.LoadOrgPlan();
                                lobjPAFlex.FindPersonAccountFlexComp(lobjPAFlex.icdoPersonAccount.person_account_id);
                                lobjPAFlex.LoadFlexCompOptionUpdate();

                                // Change the status to Suspended.
                                lobjPAFlex.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexSuspended;
                                lobjPAFlex.icdoPersonAccount.history_change_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                                lobjPAFlex.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                                lobjPAFlex.icdoPersonAccountFlexComp.ienuObjectState = ObjectState.Update;
                                lobjPAFlex.icdoPersonAccount.reason_id = 332;
                                lobjPAFlex.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;
                                lobjPAFlex.icdoPersonAccountFlexComp.reason_id = 332;
                                lobjPAFlex.icdoPersonAccountFlexComp.reason_value = busConstant.BCBSFileCodeDeath;

                                // Calling the Save operation, that will insert the History.
                                lobjPAFlex.iarrChangeLog.Add(lobjPAFlex.icdoPersonAccount);
                                lobjPAFlex.iarrChangeLog.Add(lobjPAFlex.icdoPersonAccountFlexComp);
                                lobjPAFlex.iblnIsFromTerminationPost = true;
                                lobjPAFlex.BeforeValidate(utlPageMode.Update);
                                lobjPAFlex.BeforePersistChanges();
                                lobjPAFlex.PersistChanges();
                                lobjPAFlex.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                                lobjPAFlex.AfterPersistChanges();
                                iblnIsFlexCompSuspended = Select("entWssPersonAccountEnrollmentRequest.IsMCRAorDCRAExistWithAnnualPedgeAmount", new object[2] { DateTime.Now.Year, lbusPersonAccount.icdoPersonAccount.person_account_id }).Rows.Count > 0;
                                busGlobalFunctions.PostESSMessage(lobjPAFlex.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, busConstant.PlanIdFlex, iobjPassInfo); 
                            }
                            else if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD) &&
                                lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                            {
                                busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
                                if (lobjPersonAccountMedicarePartDHistory.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id))
                                {
                                    if (lobjPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id))
                                    {
                                        lobjPersonAccountMedicarePartDHistory.LoadPerson();
                                        lobjPersonAccountMedicarePartDHistory.LoadPlan();
                                        lobjPersonAccountMedicarePartDHistory.LoadMedicarePartDMembers();//For the grid
                                        lobjPersonAccountMedicarePartDHistory.LoadHistory();
                                        lobjPersonAccountMedicarePartDHistory.LoadPersonAccountMedicarePartDHistory(lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_id);
                                        if (lobjPersonAccountMedicarePartDHistory.FindByPrimaryKey(lbusPersonAccount.icdoPersonAccount.person_account_id))
                                        {
                                            lobjPersonAccountMedicarePartDHistory.GetTotalPremiumAmountForMedicareForPapit();
                                        }
                                        if (lobjPersonAccountMedicarePartDHistory.FindByPrimaryKey(lbusPersonAccount.icdoPersonAccount.person_account_id))
                                        {
                                            lobjPersonAccountMedicarePartDHistory.GetMonthlyPremiumAmountForMedicarePartD();
                                        }
                                        lobjPersonAccountMedicarePartDHistory.LoadPaymentElection();
                                        lobjPersonAccountMedicarePartDHistory.LoadPersonAccountAchDetail();
                                        lobjPersonAccountMedicarePartDHistory.LoadBillingOrganization();
                                        lobjPersonAccountMedicarePartDHistory.LoadInsuranceYTD();
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.reason_id = 332;
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.reason_value = busConstant.BCBSFileCodeDeath;
                                        lobjPersonAccountMedicarePartDHistory.LoadActiveProviderOrgPlan(lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.current_plan_start_date_no_null);
                                        lobjPersonAccountMedicarePartDHistory.LoadPaymentElectionHistory();
                                        lobjPersonAccountMedicarePartDHistory.LoadPersonEmploymentDetail();
                                        lobjPersonAccountMedicarePartDHistory.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                        lobjPersonAccountMedicarePartDHistory.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                                        // Change the status to Suspended.
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.history_change_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.reason_id = 332;
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;

                                        // Calling the Save operation, that will insert the History.
                                        lobjPersonAccountMedicarePartDHistory.iarrChangeLog.Add(lobjPersonAccountMedicarePartDHistory.icdoPersonAccount);
                                        lobjPersonAccountMedicarePartDHistory.iblnIsFromTerminationPost = true;
                                        lobjPersonAccountMedicarePartDHistory.BeforeValidate(utlPageMode.Update);
                                        lobjPersonAccountMedicarePartDHistory.BeforePersistChanges();
                                        lobjPersonAccountMedicarePartDHistory.PersistChanges();
                                        lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                                        lobjPersonAccountMedicarePartDHistory.AfterPersistChanges();
                                        busGlobalFunctions.PostESSMessage(lobjPersonAccountMedicarePartDHistory.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, busConstant.PlanIdMedicarePartD, iobjPassInfo);

                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (!ibusPerson.iclbActivePensionAccounts.IsNullOrEmpty())
                        {
                            if (ibusPerson.icolPersonEmployment.IsNullOrEmpty())
                                ibusPerson.LoadPersonEmployment();

                            if (ibusPerson.icolPersonEmployment.Count > 0)
                            {
                                busPersonEmployment lbusPersonEmployment = new busPersonEmployment()
                                {
                                    icdoPersonEmployment = new cdoPersonEmployment(),
                                    ibusPerson = new busPerson() { icdoPerson = new cdoPerson() }
                                };

                                lbusPersonEmployment = ibusPerson.icolPersonEmployment.FirstOrDefault();
                                lbusPersonEmployment.ibusPerson = ibusPerson;
                                lbusPersonEmployment.ibusPerson.icolPersonAccount = null;  //Update SEQ Fix : To Avoid Record Changed Since last modified.
                                lbusPersonEmployment.idtmProviderEndDateFromDeathNotif = icdoDeathNotification.date_of_death;
                                lbusPersonEmployment.UpdateTerminateEmployment(true);
                            }
                        }

                    }
                    if (ibusPerson.icdoPerson.person_id > 0)
                    {
                        if (ibusPerson.icolPersonContact.Count == 0)
                            ibusPerson.LoadSpouseContact();
                        Collection<busPersonContact> lclbPersonContact = ibusPerson.icolPersonContact.Where(contact => contact.icdoPersonContact.relationship_value != busConstant.FamilyRelationshipSpouse
                                                                            && contact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive).ToList().ToCollection();
                        foreach (busPersonContact lbusPersonContact in lclbPersonContact)
                        {
                            lbusPersonContact.icdoPersonContact.status_value = busConstant.PersonContactStatusInActive;
                            lbusPersonContact.icdoPersonContact.Update();
                        }
                        //10 & 11 - Update Spouse Life, end the Spouse Supplemental coverage
                        int lintSpousePersonID = ibusPerson.icolPersonContact.Where(contact => contact.icdoPersonContact.relationship_value == busConstant.FamilyRelationshipSpouse
                                                                        && contact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive).Select(i => i.icdoPersonContact.contact_person_id).FirstOrDefault();
                        if (lintSpousePersonID > 0)
                        {
                            DataTable ldtbPersonAccount = Select<cdoPersonAccount>(
                                          new string[3] { enmPersonAccount.person_id.ToString(), enmPersonAccount.plan_id.ToString(), enmPersonAccount.plan_participation_status_value.ToString() },
                                          new object[3] { lintSpousePersonID, busConstant.PlanIdGroupLife, busConstant.PlanParticipationStatusInsuranceEnrolled }, null, null);
                            busPersonAccount lbusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() };
                            if (ldtbPersonAccount.Rows.Count > 0)
                            {
                                lbusPersonAccount.icdoPersonAccount.LoadData(ldtbPersonAccount.Rows[0]);
                                UpdateLifePlan(lbusPersonAccount);
                            }
                        }
                    }

                    ValidateHardErrors(utlPageMode.All);
                    LoadErrors();
                    iblnClearSoftErrors = true;
                    if (ibusSoftErrors != null)
                        ibusSoftErrors.iblnClearError = true;
                    this.ValidateSoftErrors();

                    base.UpdateValidateStatus();
                }
                //PIR - 2142
                //UCS 054 21,22,23,24,25, 25a
                InitializeWorkflowBasedonPaymentHistoryStatus();
            }
        }
        private void SetValuesForSoftErrorsDependent(busPersonDependent abusPersonDependent, busPersonBeneficiary abusPersonBeneficiary)
        {
            if (abusPersonDependent.IsNotNull())
            {
                if ((abusPersonDependent.icdoPersonDependent.dependent_perslink_id == 0) &&
                    (abusPersonDependent.icdoPersonDependent.address_country_value == "0001") &&
                    (abusPersonDependent.icdoPersonDependent.address_validate_flag == "N") &&
                    (abusPersonDependent.iblnIsFromDeathNotification))
                    iblnIsPersonDependentAddressInValid = true;
            }
            if (abusPersonBeneficiary.IsNotNull())
            {
                if (!(abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0) &&
                    (abusPersonBeneficiary.icdoPersonBeneficiary.address_country_value == "0001") &&
                    (abusPersonBeneficiary.icdoPersonBeneficiary.address_validate_flag == "N") &&
                    (abusPersonBeneficiary.iblnIsFromDeathNotification))
                    iblnIsPersonBeneficiaryAddressInValid = true;
                if ((abusPersonBeneficiary.AintValidatedAddress == 5) && (abusPersonBeneficiary.iblnIsFromDeathNotification))
                    iblnIsPersonBeneficiaryAddressInValid = true;
            }
        }
        private void UpdateLifePlan(busPersonAccount lbusPersonAccount, bool aIsCallFromEndAllPlans = false)
        {
            busPersonAccountLife lobjPALife = new busPersonAccountLife { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
            lobjPALife.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPALife.LoadPerson(); //.ibusPerson = ibusPerson;
            lobjPALife.ibusPlan = lbusPersonAccount.ibusPlan;
            lobjPALife.FindPersonAccountLife(lobjPALife.icdoPersonAccount.person_account_id);
            lobjPALife.LoadLifeOptionData();
            bool lblnIsGoingToUpdatePlan = false;
            if ((!aIsCallFromEndAllPlans && lobjPALife.iclbLifeOption.Count(i => i.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                                                                                i.icdoPersonAccountLifeOption.coverage_amount > 0) > 0)
                || aIsCallFromEndAllPlans)
            {
                lblnIsGoingToUpdatePlan = true;
            }

            if (lblnIsGoingToUpdatePlan)
            {
                lobjPALife.LoadPlanEffectiveDate();
                lobjPALife.icdoPersonAccount.person_employment_dtl_id = lobjPALife.GetAssociatedEmploymentDetailID();
                lobjPALife.LoadPersonEmploymentDetail();
                lobjPALife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPALife.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPALife.LoadOrgPlan();

                lobjPALife.LoadPaymentElection();
                lobjPALife.LoadProviderOrgPlan();

                if (aIsCallFromEndAllPlans)
                {
                    // Change the status to Suspended.
                    lobjPALife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                }
                else
                {
                    //10 & 11 - For Life, end the Spouse Supplemental coverage 
                    foreach (busPersonAccountLifeOption lobjPAOption in lobjPALife.iclbLifeOption.Where(i => i.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental))
                    {
                        lobjPAOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                        lobjPAOption.icdoPersonAccountLifeOption.coverage_amount = 0.0m;

                        if ((lobjPAOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue) &&
                            (lobjPAOption.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue))
                        {
                            lobjPAOption.icdoPersonAccountLifeOption.effective_end_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                        }
                    }
                }

                lobjPALife.icdoPersonAccount.history_change_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                lobjPALife.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                lobjPALife.icdoPersonAccountLife.ienuObjectState = ObjectState.Update;

                lobjPALife.icdoPersonAccount.reason_id = 332;
                lobjPALife.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;
                lobjPALife.icdoPersonAccountLife.reason_id = 332;
                lobjPALife.icdoPersonAccountLife.reason_value = busConstant.BCBSFileCodeDeath;

                // Calling the Save operation, that will insert the History.
                lobjPALife.iarrChangeLog.Add(lobjPALife.icdoPersonAccount);
                lobjPALife.iarrChangeLog.Add(lobjPALife.icdoPersonAccountLife);
                lobjPALife.BeforeValidate(utlPageMode.Update);
                lobjPALife.BeforePersistChanges();
                lobjPALife.PersistChanges();
                lobjPALife.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                lobjPALife.AfterPersistChanges();
                busGlobalFunctions.PostESSMessage(lobjPALife.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, busConstant.PlanIdGroupLife, iobjPassInfo); // PIR 9115
            }
        }

        private void UpdateGHDVPlans(busPersonAccount abusPersonAccount, string astRrelationship_value, bool aIsCallFromEndAllPlans = false)
        {
            abusPersonAccount.LoadPersonAccountGHDV();
            busPersonAccountGhdv lobjPAGHDV = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            lobjPAGHDV.FindPersonAccount(abusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPAGHDV.ibusPlan = abusPersonAccount.ibusPlan;
            lobjPAGHDV.LoadPerson();
            lobjPAGHDV.FindGHDVByPersonAccountID(lobjPAGHDV.icdoPersonAccount.person_account_id);

            bool lblnIsGoingToUpdatePlan = false;

            if ((!aIsCallFromEndAllPlans &&
              ((lobjPAGHDV.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision && lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value != busConstant.VisionLevelofCoverageIndividual)
                || (lobjPAGHDV.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental && lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value != busConstant.DentalLevelofCoverageIndividual)
              )
              ) || aIsCallFromEndAllPlans)
            {
                lblnIsGoingToUpdatePlan = true;
            }

            if (lblnIsGoingToUpdatePlan)
            {
                lobjPAGHDV.LoadPlanEffectiveDate();
                lobjPAGHDV.icdoPersonAccount.person_employment_dtl_id = lobjPAGHDV.GetAssociatedEmploymentDetailID();
                lobjPAGHDV.LoadPersonEmploymentDetail();
                lobjPAGHDV.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPAGHDV.icdoPersonAccount.history_change_date = GetHistoryChangeDateofSuspendedAccount(abusPersonAccount);
                //this is exception for dependent death plan enrollment effective from next month 
                if (!aIsCallFromEndAllPlans && lobjPAGHDV.icdoPersonAccount.history_change_date >= icdoDeathNotification.date_of_death.GetFirstDayofNextMonth().AddMonths(1))
                    lobjPAGHDV.icdoPersonAccount.history_change_date = lobjPAGHDV.icdoPersonAccount.history_change_date.AddMonths(-1);
                lobjPAGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPAGHDV.LoadOrgPlan();

                // Change the status to Suspended.
                if (aIsCallFromEndAllPlans)
                {
                    lobjPAGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                }
                else
                {
                    if (lobjPAGHDV.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                    {
                        if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = astRrelationship_value == busConstant.DependentRelationshipSpouse ?
                                    busConstant.VisionLevelofCoverageIndividualChild : busConstant.VisionLevelofCoverageIndividualSpouse;
                        else if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                        else if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                    }
                    else if (lobjPAGHDV.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental)
                    {
                        if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.DentalLevelofCoverageFamily)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = astRrelationship_value == busConstant.DependentRelationshipSpouse ?
                                busConstant.DentalLevelofCoverageIndividualChild : busConstant.DentalLevelofCoverageIndividualSpouse;
                        else if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;
                        else if (lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild)
                            lobjPAGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;
                    }
                }
                lobjPAGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                lobjPAGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Update;
                lobjPAGHDV.icdoPersonAccount.reason_id = 332;
                lobjPAGHDV.icdoPersonAccount.reason_value = busConstant.BCBSFileCodeDeath;
                lobjPAGHDV.icdoPersonAccountGhdv.reason_id = 332;
                lobjPAGHDV.icdoPersonAccountGhdv.reason_value = busConstant.BCBSFileCodeDeath;

                // Calling the Save operation, that will insert the History.                               
                lobjPAGHDV.iarrChangeLog.Add(lobjPAGHDV.icdoPersonAccount);
                lobjPAGHDV.iarrChangeLog.Add(lobjPAGHDV.icdoPersonAccountGhdv);
                lobjPAGHDV.iblnIsFromTerminationPost = true;
                lobjPAGHDV.BeforeValidate(utlPageMode.Update);
                lobjPAGHDV.BeforePersistChanges();
                lobjPAGHDV.PersistChanges();
                lobjPAGHDV.icdoPersonAccount.iblnIsEmploymentEnded = true; //PIR 20259
                lobjPAGHDV.AfterPersistChanges();
                //if (lobjPAGHDV.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && lobjPAGHDV.IsHistoryEntryRequired &&
                //                    ((lobjPAGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && !(lobjPAGHDV.icdoPersonAccountGhdv.is_health_cobra || lobjPAGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree))
                //                    || (lobjPAGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdDental && !(lobjPAGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA || lobjPAGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree))
                //                    || (lobjPAGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdVision && !(lobjPAGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA || lobjPAGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)))
                //                    && lobjPAGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes)
                //{
                //    lobjPAGHDV.InsertIntoEnrollmentData();
                //}
                busGlobalFunctions.PostESSMessage(lobjPAGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, abusPersonAccount.icdoPersonAccount.plan_id, iobjPassInfo);
            }
        }

        private DateTime GetHistoryChangeDateofSuspendedAccount(busPersonAccount abusPersonAccount)
        {
            DateTime History_Change_Date = DateTime.MinValue;
            if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
               abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
               abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                if (abusPersonAccount.ibusPersonAccountGHDV == null)
                    abusPersonAccount.LoadPersonAccountGHDV();
                if ((((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                                                ((abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState || abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                                                (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()))) ||
                                                ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) &&
                                                (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive)) ||
                                                ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) &&
                                                (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive))))
                {
                    if (IsCoverageSingle(abusPersonAccount.icdoPersonAccount.plan_id, abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value))
                        History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                    else
                        History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth().AddMonths(1);

                }
                else if ((((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) &&
                 ((abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeNonState || abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeState) &&
                 (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())) ||
                 (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)) ||
                 ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental) &&
                 (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeActive)) ||
                 ((abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision) &&
                 (abusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeActive))))
                {
                    History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                }
            }
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife ||
                abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation ||
                abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457 ||
                abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdFlex ||
                abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
            }
            else if (abusPersonAccount.ibusPlan.IsRetirementPlan())
            {
                if (abusPersonAccount.icdoPersonAccount.person_employment_dtl_id == 0)
                    abusPersonAccount.icdoPersonAccount.person_employment_dtl_id = abusPersonAccount.GetAssociatedEmploymentDetailID();
                if (abusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                    abusPersonAccount.LoadPersonEmploymentDetail();
                if (abusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    abusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();

                //if (abusPersonAccount.ibusPlan.IsRetirementPlan())
                //    History_Change_Date = abusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.GetFirstDayofNextMonth();
                //else
                    History_Change_Date = abusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date.GetFirstDayofNextMonth();

            }
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
            {
                if (Convert.ToInt32(DBFunction.DBExecuteScalar("entDeathNotification.LoadDependent", new object[2]
                { icdoDeathNotification.date_of_death, icdoDeathNotification.person_id  }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                {
                    History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth().AddMonths(1);
                }
                else
                {
                    History_Change_Date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                }
            }
            return History_Change_Date;
        }

        public bool IsPASuspendedWithFutureDates() => ibusPerson.iclbActivePensionAccounts.Any(i => i.IsSuspended() && i.icdoPersonAccount.history_change_date > GetHistoryChangeDateofSuspendedAccount(i));
        public bool IsCoverageSingle(int aintPlanID, string astrCoverageCode, string astrLevelOfCoverageValue)
        {
            bool lblnResult = false;
            if (aintPlanID == busConstant.PlanIdGroupHealth)
            {
                if (!string.IsNullOrEmpty(astrCoverageCode))
                {
                    if (astrCoverageCode.Contains("0001")
                        || astrCoverageCode.Contains("0004")
                        || astrCoverageCode.Contains("0006")
                        || astrCoverageCode.Contains("0021")
                        || astrCoverageCode.Contains("0024")
                        || astrCoverageCode.Contains("0044")
                        || astrCoverageCode.Contains("0041")
                        || astrCoverageCode.Contains("0043")
                        || astrCoverageCode.Contains("0046")
                        || astrCoverageCode.Contains("0048"))
                        lblnResult = true;
                }
            }
            else
            {
                if (aintPlanID == busConstant.PlanIdVision)
                {
                    if (!string.IsNullOrEmpty(astrLevelOfCoverageValue))
                    {
                        if (astrLevelOfCoverageValue == busConstant.VisionLevelofCoverageIndividual)
                            lblnResult = true;
                    }
                }
                if (aintPlanID == busConstant.PlanIdDental)
                {
                    if (!string.IsNullOrEmpty(astrLevelOfCoverageValue))
                    {
                        if (astrLevelOfCoverageValue == busConstant.DentalLevelofCoverageIndividual)
                            lblnResult = true;
                    }
                }
                if (aintPlanID == busConstant.PlanIdHMO)
                {
                    if (!string.IsNullOrEmpty(astrLevelOfCoverageValue))
                    {
                        if (astrLevelOfCoverageValue == busConstant.HMOLevelOfCoverageSingle)
                            lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }
        public bool IsBeneToSpouseEnrolledInLife()
        {
            if (ibusPerson.iclbPersonAccountBeneficiary.IsNull())
                ibusPerson.LoadPersonAccountBeneficiary();
            if (ibusPerson.iclbPersonAccountBeneficiary.IsNotNull() && iblnIsDatesAreInsertedForBeneTo)
            {
                foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in ibusPerson.iclbPersonAccountBeneficiary)
                {
                    lobjPersonAccountBeneficiary.FindPersonBeneficiary(lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_id);
                    //lobjPersonAccountBeneficiary.LoadPersonAccount();
                    //if(lobjPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                    //   // iblIsWorkFlowCreateLifeInsuranceClaims = true;
                    if (lobjPersonAccountBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse && Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.CheckIfBeneficiaryToIsEnrolledInLife", new object[2]
                            { lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id, ibusPerson.icdoPerson.date_of_death }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsDependentToSpouseEnrolledInLife()
        {
            if (ibusPerson.iclbPersonAccountDependent.IsNull())
                ibusPerson.LoadPersonAccountDependent();
            if (ibusPerson.iclbPersonAccountDependent.IsNotNull() && iblnIsDatesAreInsertedForDepeTo)
            {
                foreach (busPersonAccountDependent lobjPersonAccountDependent in ibusPerson.iclbPersonAccountDependent)
                {
                    lobjPersonAccountDependent.FindPersonDependent(lobjPersonAccountDependent.icdoPersonAccountDependent.person_dependent_id);
                    if (lobjPersonAccountDependent.icdoPersonDependent.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse && Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.CheckIfBeneficiaryToIsEnrolledInLife", new object[2]
                            { lobjPersonAccountDependent.icdoPersonAccountDependent.person_account_id, ibusPerson.icdoPerson.date_of_death }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CreateRHICForJointAnnuitant()
        {
            foreach (busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail in ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail)
            {
                lbusBenefitRhicCombineDetail.ibusPayeeAccount.LoadBenfitAccount();
                lbusBenefitRhicCombineDetail.ibusPayeeAccount.LoadApplication();
                                
            }
            if(ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                .Any(i=>i.ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.RHICOptionReduced100 ||
                    i.ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.RHICOptionReduced50))
            {
                busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = ibusPerson.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail
                .Where(i => i.ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.RHICOptionReduced100 ||
                    i.ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.RHICOptionReduced50)
                .OrderByDescending(i=>i.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id)
                .FirstOrDefault();
                if (lbusBenefitRhicCombineDetail.IsNotNull())
                {
                    int lintJointAnnuitantPerslinkID = lbusBenefitRhicCombineDetail.ibusPayeeAccount.ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id;
                    if (lintJointAnnuitantPerslinkID > 0)
                    {
                        busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = lintJointAnnuitantPerslinkID;
                        lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth();
                        lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                    }
                }
            }
                        
        }

        //override delete to delete soft errors
        public override int Delete()
        {
            DBFunction.DBNonQuery("cdoDeathNotification.DeleteErrors", new object[1] { icdoDeathNotification.death_notification_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return base.Delete();
        }

        public override busBase GetCorPerson()
        {
            busPerson lobjPerson = new busPerson();
            if (string.IsNullOrEmpty(istrFromBatch) && string.IsNullOrEmpty(istrIsFromEmployeeDeathBatch))
            {
                if (ibusPerson == null)
                    LoadPerson();
                lobjPerson = ibusPerson;
            }
            else
            {
                if (string.IsNullOrEmpty(istrIsFromEmployeeDeathBatch))
                {
                    if (IsPayeeDeathLetter)
                    {
                        if (ibusPersonToBeneficiary.IsNotNull() && ibusPersonToBeneficiary.ibusBeneficiaryPerson.IsNotNull())
                            lobjPerson = ibusPersonToBeneficiary.ibusBeneficiaryPerson;
                    }
                    else
                    {
                        //this is for non employee death batch
                        if (ibusPersonToBeneficiary.IsNotNull())
                            lobjPerson = ibusPersonToBeneficiary.ibusPerson;
                        else if (ibusPersonToDependent.IsNotNull())
                            lobjPerson = ibusPersonToDependent.ibusPerson;
                    }
                }
                else
                {   //this is for employee death batch
                    //if the bene is not having current address  then the letter need to be send to deceased address
                    if (ibusPersonBeneficiary.IsNotNull())
                    {
                        lobjPerson = ibusPersonBeneficiary.ibusBeneficiaryPerson;
                    }
                }
            }
            return lobjPerson;
        }

        private void LoadEmployeeDeathBeneAddress()
        {
            if (ibusPersonBeneficiary.IsNotNull())
            {
                ibusEmployeeDeathBeneAddress = new busPersonAddress
                {
                    icdoPersonCurrentAddress = new cdoPersonAddress(),
                    icdoPersonAddress = new cdoPersonAddress()
                };
                if (ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.IsNull())
                {
                    ibusPersonBeneficiary.ibusBeneficiaryPerson.LoadPersonCurrentAddress();
                }
                if (ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.IsNotNull())
                    ibusEmployeeDeathBeneAddress.icdoPersonAddress = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress;
                else
                {
                    // ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress = new cdoPersonAddress();
                    if (ibusPerson.ibusPersonCurrentAddress.IsNull())
                        ibusPerson.LoadPersonCurrentAddress();
                    ibusEmployeeDeathBeneAddress.icdoPersonAddress = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress;
                    //ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress = ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress;
                }

                //ibusEmployeeDeathBeneAddress.icdoPersonAddress = ibusPersonBeneficiary.ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress;
                if (ibusEmployeeDeathBeneAddress.icdoPersonAddress.IsNotNull())
                    ibusEmployeeDeathBeneAddress.icdoPersonAddress.addr_zip_code += ibusEmployeeDeathBeneAddress.icdoPersonAddress.addr_zip_4_code.IsNullOrEmpty() ?
                       String.Empty : " - " + ibusEmployeeDeathBeneAddress.icdoPersonAddress.addr_zip_4_code;
            }
        }

        public busPersonAddress ibusEmployeeDeathBeneAddress { get; set; }

        # region cor 55

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadPersonAccountForLife();
            LoadPersonAccountLifeOptions();
            if (ibusDeceasedPayeeAccount.IsNotNull())
            {
                if (ibusDeceasedPayeeAccount.ibusBenefitAccount == null)
                    ibusDeceasedPayeeAccount.LoadBenfitAccount();
            }

            if (astrTemplateName == "APP-7050")
                LoadEmployeeDeathBeneAddress();
        }
        //Property to get the person account id for life plan
        private busPersonAccount _ibusPersonAccountForLife;
        public busPersonAccount ibusPersonAccountForLife
        {
            get { return _ibusPersonAccountForLife; }
            set { _ibusPersonAccountForLife = value; }
        }

        public void LoadPersonAccountForLife()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPersonAccountForLife == null)
                ibusPersonAccountForLife = new busPersonAccount();
            ibusPersonAccountForLife = ibusPerson.LoadActivePersonAccountByPlan(busConstant.PlanIdGroupLife);
        }

        public void LoadPersonAccountLifeOptions()
        {
            if (iclbPersonAccountLifeOption == null)
                iclbPersonAccountLifeOption = new Collection<busPersonAccountLifeOption>();
            if (ibusPersonAccountForLife.icdoPersonAccount.person_account_id != 0)
            {
                DataTable ldtbList = Select<cdoPersonAccountLifeOption>(
                    new string[1] { "person_account_id" },
                    new object[1] { ibusPersonAccountForLife.icdoPersonAccount.person_account_id }, null, null);
                iclbPersonAccountLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbList, "icdoPersonAccountLifeOption");
            }
        }

        //Property to contain all life options for the member
        private Collection<busPersonAccountLifeOption> _iclbPersonAccountLifeOption;
        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeOption
        {
            get { return _iclbPersonAccountLifeOption; }
            set { _iclbPersonAccountLifeOption = value; }
        }

        //Property to store total Life insurance policy value
        //only basic and supplemental as per PIR 2487
        public decimal idecLifeInsurancePolicyValue
        {
            get
            {
                decimal ldecLifePolicyValue = 0.0M;
                foreach (busPersonAccountLifeOption lobjLifeOption in iclbPersonAccountLifeOption)
                {
                    if (lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental
                        || lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        ldecLifePolicyValue += lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                }
                return Math.Round(ldecLifePolicyValue, MidpointRounding.AwayFromZero);
            }
        }
        # endregion

        # region UCS -054

        //UCS 54 02
        //check is any payee account exists with status other than Payment complete, Cancelled, suspended
        private bool IsPayeeAccountStatusValid()
        {
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
            var lenumPayeeAccountExceptRefund = ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Where(lobj => lobj.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund);
            foreach (busPayeeAccount lobjPayeeAccount in lenumPayeeAccountExceptRefund)
            {
                //Don't check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatus(); //PIR 2106
                if (!String.IsNullOrEmpty(lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value))
                {
                    string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                    if (lstrStatus != busConstant.PayeeAccountStatusCancelled &&
                                         lstrStatus != busConstant.PayeeAccountStatusPaymentComplete &&
                                         lstrStatus != busConstant.PayeeAccountStatusSuspended)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //BR-054-03
        //only in new mode
        //check if payee account is in status Suspended, review, Approved
        //then create a review status for Payee account
        //this will make **********ACCOUNT OWNER PAYEE ACCOUNT************** to review
        private void ChangePAStatusIfStatusSuspendedReviewApproved()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();

            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
            {
                //Don't check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatus();
                string lstrPayeeStatusValue = busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if ((lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusApproved)
                    || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusSuspended)
                        || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirementReview))
                {
                    lobjPayeeAccount.idtStatusEffectiveDate = icdoDeathNotification.created_date;
                    lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                }
            }
        }

        /// UCS-054 - 04 && UAT PIR ID 1230
        /// There should be minimum of one Beneficiary should be active at the time of death per plan.
        /// This validation warning message will be fired only once in New mode.
        public bool IsValidBeneficiaryExists()
        {
            //if (ibusPerson.iclbPersonBeneficiary == null)
                ibusPerson.LoadBeneficiary();

            // Get Distinct Plans
            Collection<busPersonBeneficiary> lclbPersonBene = new Collection<busPersonBeneficiary>();
            foreach (busPersonBeneficiary lobjBene in ibusPerson.iclbPersonBeneficiary)
            {
                lobjBene.ibusPersonAccountBeneficiary.LoadPersonAccount();
                if (!(lclbPersonBene.Where(lobj => lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id ==
                                        lobjBene.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id).Any()))
                    lclbPersonBene.Add(lobjBene);
            }

            // Check whether atleast one beneficiary is active per plan as on Date of Death
            foreach (busPersonBeneficiary lobjPA in lclbPersonBene)
            {
                if (!ibusPerson.iclbPersonBeneficiary.Any(lobj => lobj.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary
                                                         && lobj.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date >= icdoDeathNotification.date_of_death))
                    return false;
            }
            return true;
        }

        //UCS -054- 05
        //check if any payee account with status other than cancelled exists
        //check account relationship is member
        //check if the benefit begin date and date of death date difference is less than 15 days
        //throw error
        public bool IsFirstPaymentDateIsValid()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
            {
                //Don't check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            }
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Count > 0)
            {
                if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Count(lobjPA => lobjPA.
                   icdoPayeeAccount.account_relation_value == busConstant.PayeeAccountAccountRelationshipOwner
                     && (icdoDeathNotification.date_of_death.Subtract(lobjPA.icdoPayeeAccount.benefit_begin_date)).Days < 15
                     && (!lobjPA.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value.Equals(busConstant.PayeeAccountStatusCancelled))) > 0)
                    return false;
            }
            return true;
        }

        //UCS-054 -06
        //for DRO Payee Accounts
        //check if beneficiary of relation as Estate. if not exists then raise error
        public bool IsBeneficiaryEstateExistsForDROpayee()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icdoPerson.person_id > 0 && icdoDeathNotification.date_of_death != DateTime.MinValue)
            {
                DataTable ldtbPayeeAccounts = Select("entDeathNotification.LoadAlternativePayeeAccounts", new object[1] { icdoDeathNotification.person_id });
                Collection<busPayeeAccount> lclcAlternatePayeeAccounts = GetCollection<busPayeeAccount>(ldtbPayeeAccounts, "icdoPayeeAccount");
                foreach (busPayeeAccount lbusPayeeAccount in lclcAlternatePayeeAccounts)
                {
                    lbusPayeeAccount.LoadActivePayeeStatus();
                    lbusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = busGlobalFunctions.GetData2ByCodeValue(2203, lbusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                }
                lclcAlternatePayeeAccounts = lclcAlternatePayeeAccounts.Where(lobjPA => lobjPA.ibusPayeeAccountActiveStatus.IsStatusApproved() || lobjPA.ibusPayeeAccountActiveStatus.IsStatusReceiving() || lobjPA.ibusPayeeAccountActiveStatus.IsStatusReview() || lobjPA.ibusPayeeAccountActiveStatus.IsStatusSuspended()).ToList().ToCollection();
                //If alternate payee payee accounts exists 
                if (lclcAlternatePayeeAccounts.Count() > 0)
                {
                    foreach (busPayeeAccount lbusPayeeAccount in lclcAlternatePayeeAccounts)
                    {
                        lbusPayeeAccount.LoadDROApplication();
                        if ((lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel) ||
                       (lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel) ||
                       (lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel))
                        {
                            string lstrIsBeneNeededFlag = busGlobalFunctions.GetData2ByCodeValue(
                                            lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.benefit_duration_option_id,
                                            lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);

                            if (lstrIsBeneNeededFlag == busConstant.Flag_Yes || lbusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.alternate_payee_death_percentage > 0)
                            {
                                if (lbusPayeeAccount.iclbPaymentHistoryHeader.IsNull())
                                {
                                    lbusPayeeAccount.LoadPaymentHistoryHeader();
                                }
                                //If the alternate payee dies prior to receipt of benefits under the QDRO, the alternate payee’s estate receives a lump sum payment.
                                //Check whether a valid beneficiary exists or not
                                if (lbusPayeeAccount.iclbPaymentHistoryHeader.Count == 0 || lbusPayeeAccount.iclbPaymentHistoryHeader.All(his => icdoDeathNotification.date_of_death < his.icdoPaymentHistoryHeader.payment_date))
                                {
                                    if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.IsNull())
                                        lbusPayeeAccount.ibusDROApplication.LoadAlternatePayee();
                                    if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.icdoPerson.person_id > 0)
                                    {
                                        if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.IsNull())
                                            lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
                                        //No ben exists or no primary ben exists or primary ben dis percent sum is not 100
                                        if ((lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Count == 0) ||
                                            (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary
                                            .Count(perben => perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && busGlobalFunctions
                                            .CheckDateOverlapping(icdoDeathNotification.date_of_death, perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date)) == 0) ||
                                            (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.
                                            Where(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && busGlobalFunctions
                                            .CheckDateOverlapping(icdoDeathNotification.date_of_death, personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date))
                                            .Sum(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent) != 100))
                                            return false;
                                        else
                                        {
                                            busPersonBeneficiary lbusPersonBeneficiary = lbusPayeeAccount
                                                                                        .ibusDROApplication
                                                                                        .ibusAlternatePayee
                                                                                        .iclbPersonBeneficiary.
                                                                                        FirstOrDefault(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary &&
                                                                                        personben.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate &&
                                                                                        busGlobalFunctions
                                                                                        .CheckDateOverlapping(icdoDeathNotification.date_of_death, personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                                                                        personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date));
                                            if (lbusPersonBeneficiary.IsNotNull())
                                            {
                                                int lintBenOrgId = lbusPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
                                                busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                                                if (lbusOrganization.FindOrganization(lintBenOrgId))
                                                {

                                                    if (lbusOrganization.icdoOrganization.org_type_value != busConstant.OrganizationTypeEstate)
                                                        return false;
                                                    else
                                                    {
                                                        istrEstateOrgCodeID = lbusOrganization.icdoOrganization.org_code;
                                                    }
                                                }
                                                else
                                                    return false;
                                            }
                                            else
                                                return false;
                                        }
                                    }
                                }
                                else //If the alternate payee dies after receipt of benefits under the QDRO, payments continue to the alternate payee’s designated beneficiary
                                {
                                    if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.IsNull())
                                        lbusPayeeAccount.ibusDROApplication.LoadAlternatePayee();
                                    if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.icdoPerson.person_id > 0)
                                    {
                                        if (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.IsNull())
                                            lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
                                        lbusPayeeAccount.LoadPaymentDetails();
                                        decimal ldecAccountBalance = lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount + lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.ee_post_tax_amount +
                                                                    lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount + lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.er_vested_amount +
                                                                    lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.interest_amount + lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.capital_gain +
                                                                    lbusPayeeAccount.ibusDROCalculation.icdoBenefitDroCalculation.additional_interest_amount;
                                        decimal ldecRemMinGuarantee = ldecAccountBalance - lbusPayeeAccount.idecpaidgrossamount;
                                        //No ben exists or no primary ben exists or primary ben (not estate) dis percent sum is not 100
                                        if (((ldecRemMinGuarantee > 0) || (lbusPayeeAccount.icdoPayeeAccount.IsTermCertainBenefitOption() && icdoDeathNotification.date_of_death < lbusPayeeAccount.icdoPayeeAccount.term_certain_end_date)) && 
                                            ((lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Count == 0) ||
                                            (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary
                                            .Count(perben => perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && busGlobalFunctions
                                            .CheckDateOverlapping(icdoDeathNotification.date_of_death, perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date)) == 0) ||
                                            (lbusPayeeAccount.ibusDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.
                                            Where(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary &&
                                            personben.icdoPersonBeneficiary.relationship_value != busConstant.PersonBeneficiaryRelationshipEstate &&
                                            busGlobalFunctions
                                            .CheckDateOverlapping(icdoDeathNotification.date_of_death, personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date))
                                            .Sum(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent) != 100)))
                                            return false;
                                    }
                                }
                            }
                        }
                    }
                }
                //Load DRO applications without payee account
                if (ibusPerson.iclbAlternatePayeeDROApplication.IsNull())
                    ibusPerson.LoadAlternatePayeeDROApplicaitons();
                Collection<busBenefitDroApplication> lenuValidBenDroApps = ibusPerson.iclbAlternatePayeeDROApplication.Where(droapp => droapp.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved ||
                                                                            droapp.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified ||
                                                                            droapp.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved).ToList().ToCollection();
                foreach (busBenefitDroApplication lbusBenefitDroApplication in lenuValidBenDroApps)
                {
                    DataTable ldtbDROPayeeAccounts = Select<cdoPayeeAccount>(new string[1] { enmPayeeAccount.dro_application_id.ToString() }, new object[1] { lbusBenefitDroApplication.icdoBenefitDroApplication.dro_application_id }, null, null);

                    if (ldtbDROPayeeAccounts.Rows.Count == 0)
                    {
                        if ((lbusBenefitDroApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel) ||
                           (lbusBenefitDroApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel) ||
                           (lbusBenefitDroApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel))
                        {
                            string lstrIsBeneNeededFlag = busGlobalFunctions.GetData2ByCodeValue(
                                            lbusBenefitDroApplication.icdoBenefitDroApplication.benefit_duration_option_id,
                                            lbusBenefitDroApplication.icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                            if (lstrIsBeneNeededFlag == busConstant.Flag_Yes || lbusBenefitDroApplication.icdoBenefitDroApplication.alternate_payee_death_percentage > 0)
                            {

                                if (lbusBenefitDroApplication.ibusAlternatePayee.IsNull())
                                    lbusBenefitDroApplication.LoadAlternatePayee();
                                if (lbusBenefitDroApplication.ibusAlternatePayee.icdoPerson.person_id > 0)
                                {
                                    if (lbusBenefitDroApplication.ibusAlternatePayee.iclbPersonBeneficiary.IsNull())
                                        lbusBenefitDroApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);
                                    //No ben exists or no primary ben exists or primary ben dis percent sum is not 100
                                    if ((lbusBenefitDroApplication.ibusAlternatePayee.iclbPersonBeneficiary.Count == 0) ||
                                        (lbusBenefitDroApplication.ibusAlternatePayee.iclbPersonBeneficiary
                                        .Count(perben => perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && busGlobalFunctions
                                        .CheckDateOverlapping(icdoDeathNotification.date_of_death, perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                        perben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date)) == 0) ||
                                        (lbusBenefitDroApplication.ibusAlternatePayee.iclbPersonBeneficiary.
                                        Where(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary && busGlobalFunctions
                                        .CheckDateOverlapping(icdoDeathNotification.date_of_death, personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                        personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date))
                                        .Sum(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent) != 100))
                                        return false;
                                    else
                                    {
                                        busPersonBeneficiary lbusPersonBeneficiary = lbusBenefitDroApplication
                                                                                    .ibusAlternatePayee
                                                                                    .iclbPersonBeneficiary.
                                                                                    FirstOrDefault(personben => personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary &&
                                                                                    personben.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate &&
                                                                                    busGlobalFunctions
                                                                                    .CheckDateOverlapping(icdoDeathNotification.date_of_death, personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                                                                    personben.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date));
                                        if (lbusPersonBeneficiary.IsNotNull())
                                        {
                                            int lintBenOrgId = lbusPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
                                            busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                                            if (lbusOrganization.FindOrganization(lintBenOrgId))
                                            {

                                                if (lbusOrganization.icdoOrganization.org_type_value != busConstant.OrganizationTypeEstate)
                                                    return false;
                                                else
                                                {
                                                    istrEstateOrgCodeID = lbusOrganization.icdoOrganization.org_code;
                                                }
                                            }
                                            else
                                                return false;
                                        }
                                        else
                                            return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool IsBeneficiaryEstateOrBeneNotExists()
        {
            foreach (busBenefitDroApplication lobjDROApplication in ibusPerson.iclbAlternatePayeeDROApplication)
            {
                if ((lobjDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBFormerModel) ||
                       (lobjDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveJobServiceModel) ||
                       (lobjDROApplication.icdoBenefitDroApplication.dro_model_value == busConstant.DROApplicationModelActiveDBModel))
                {
                    //PIR - 1393
                    //if the DRO application needs the bene
                    //then only throw this error
                    string lstrIsBeneNeededFlag = busGlobalFunctions.GetData2ByCodeValue(
                                            lobjDROApplication.icdoBenefitDroApplication.benefit_duration_option_id,
                                            lobjDROApplication.icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);

                    if (lstrIsBeneNeededFlag == busConstant.Flag_Yes)
                    {
                        {
                            if (lobjDROApplication.ibusBenefitDroCalculation.IsNull()) lobjDROApplication.LoadDROCalculation();

                            if (lobjDROApplication.ibusBenefitDroCalculation.IsNotNull())
                            {
                                if (lobjDROApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                                {
                                    if (lobjDROApplication.ibusAlternatePayee.IsNull())
                                        lobjDROApplication.LoadAlternatePayee();

                                    //load only beneficiaries that are entered for this DRO application
                                    if (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.IsNull())
                                        lobjDROApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);

                                    //if (ibusPerson.iclbPersonBeneficiaryForDeceased.IsNull())
                                    //    ibusPerson.LoadBeneficiaryForDeceased();
                                    if ((lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Count == 0) ||
                                        (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                            lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate).Count() == 0)
                                        || (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBene =>
                                            busGlobalFunctions.CheckDateOverlapping(icdoDeathNotification.date_of_death,
                                            lobjBene.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            lobjBene.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date)).Count() == 0)) // UAT PIR ID 1234
                                        return false;
                                    else
                                    {
                                        if (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                            lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate).Count() > 0)
                                        {
                                            busPersonBeneficiary lobjPersonBeneficiary = lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                                 lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate
                                                 && lobjBen.icdoPersonBeneficiary.benificiary_org_id > 0).FirstOrDefault();

                                            if (lobjPersonBeneficiary.IsNotNull())
                                            {
                                                lobjPersonBeneficiary.LoadBeneficiaryInfo();

                                                istrEstateOrgCodeID = lobjPersonBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.org_code;
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (lobjDROApplication.icdoBenefitDroApplication.alternate_payee_death_percentage > 0.00M) //check alternate payee percentage - if entered then beneficiaries must be there
                    {
                        if (lobjDROApplication.ibusAlternatePayee.IsNull())
                            lobjDROApplication.LoadAlternatePayee();

                        //load only beneficiaries that are entered for this DRO application
                        if (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.IsNull())
                            lobjDROApplication.ibusAlternatePayee.LoadApplicationBeneficiary(ablnFromDRO: true);

                        //if (ibusPerson.iclbPersonBeneficiaryForDeceased.IsNull())
                        //    ibusPerson.LoadBeneficiaryForDeceased();
                        if ((lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Count == 0) ||
                            (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate).Count() == 0)
                                || (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBene =>
                                            busGlobalFunctions.CheckDateOverlapping(icdoDeathNotification.date_of_death,
                                            lobjBene.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date,
                                            lobjBene.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.end_date)).Count() == 0))  // UAT PIR ID 1234
                            return false;
                        else
                        {
                            if (lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate).Count() > 0)
                            {
                                busPersonBeneficiary lobjPersonBeneficiary = lobjDROApplication.ibusAlternatePayee.iclbPersonBeneficiary.Where(lobjBen =>
                                     lobjBen.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate
                                     && lobjBen.icdoPersonBeneficiary.benificiary_org_id > 0).FirstOrDefault();

                                if (lobjPersonBeneficiary.IsNotNull())
                                {
                                    lobjPersonBeneficiary.LoadBeneficiaryInfo();

                                    istrEstateOrgCodeID = lobjPersonBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.org_code;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        //UCS -054 -041
        //create Payee account status with review
        //for the following DRO Models
        //while saving for first time
        private void SetPayeeAccountStatusAsReview()
        {
            if (istrObjectState == ObjectState.Insert)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                    ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
                var lclbDROPayeeAccounts = ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Where(lobjPa => lobjPa.icdoPayeeAccount.dro_application_id > 0
                    && lobjPa.icdoPayeeAccount.payee_perslink_id != icdoDeathNotification.person_id);
                foreach (busPayeeAccount lobjPayAcc in lclbDROPayeeAccounts)
                {
                    lobjPayAcc.LoadDROApplication();
                    string lstrBenefitOptionValueData3 = busGlobalFunctions.GetData3ByCodeValue(2404, lobjPayAcc.ibusDROApplication.icdoBenefitDroApplication.benefit_duration_option_value, iobjPassInfo);
                    if (((lstrBenefitOptionValueData3 == busConstant.BenefitDurationValueLifeOfBenAccOwnerSingleLifeAnnuity)
                        || (lstrBenefitOptionValueData3 == busConstant.BenefitDurationValueLifeOfBenAccOwner15YrTermCertainAndLifeOption)
                        || (lstrBenefitOptionValueData3 == busConstant.BenefitDurationValueLifeOfBenAccOwner20YrTermCertainAndLifeOption)
                        || (lstrBenefitOptionValueData3 == busConstant.BenefitDurationValueLifeOfBenAccOwner10YrTermCertainAndLifeOption)
                        || (lstrBenefitOptionValueData3 == busConstant.BenefitDurationValueLifeOfBenAccOwner5YrTermCertainAndLifeOption))
                        && (lobjPayAcc.icdoPayeeAccount.term_certain_end_date < icdoDeathNotification.date_of_death))
                    {
                        lobjPayAcc.idtStatusEffectiveDate = icdoDeathNotification.date_of_death;

                        // PROD PIR ID 6069
                        if (lobjPayAcc.ibusPayeeAccountActiveStatus.IsNull()) lobjPayAcc.LoadActivePayeeStatus();
                        if (lobjPayAcc.ibusPayeeAccountActiveStatus.IsStatusApproved() || //PIR 17987 - The payee accounts should be put into review only when they are in approved or receiving status
                            lobjPayAcc.ibusPayeeAccountActiveStatus.IsStatusInPayment())
                            lobjPayAcc.CreateReviewPayeeAccountStatus();
                    }
                }
            }
            ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
        }

        //UCS -054 - 26 && 27
        //for plan PlanIdPriorJudges end benefit with date as date of death month last day
        //else set benefit begin date as last day one month after date of death 
        //when death notification is marked as certified
        //and set payee account status value as payment complete
        private void SetBenefitEndDateForPayee()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
            var lenumPayeeAccountExceptRefund = ibusPerson.iclbMemberPlusAlternatePayeeAccounts.Where(lobj => lobj.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund);
            foreach (busPayeeAccount lobjPA in lenumPayeeAccountExceptRefund)
            {
                //PIR - 2132
                //don't set payment complete payee account already with the status as payment complete
                //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                //if (lobjPA.ibusPayeeAccountActiveStatus.IsNull())
                lobjPA.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

                if (!lobjPA.ibusPayeeAccountActiveStatus.IsStatusCompleted())
                {
                    if (lobjPA.icdoPayeeAccount.payee_perslink_id == icdoDeathNotification.person_id)
                    {
                        DateTime ldtTempDate = DateTime.MinValue;
                        if (lobjPA.icdoPayeeAccount.dro_application_id > 0)
                        {
                            lobjPA.LoadDROApplication();
                            if (lobjPA.ibusDROApplication.icdoBenefitDroApplication.plan_id == busConstant.PlanIdPriorJudges)
                                ldtTempDate = icdoDeathNotification.date_of_death.AddMonths(2);
                            else
                                ldtTempDate = icdoDeathNotification.date_of_death.AddMonths(1);
                        }
                        else
                        {
                            lobjPA.LoadApplication();

                            if (lobjPA.ibusApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdPriorJudges)
                                ldtTempDate = icdoDeathNotification.date_of_death.AddMonths(2);
                            else
                                ldtTempDate = icdoDeathNotification.date_of_death.AddMonths(1);
                        }
                        ldtTempDate = new DateTime(ldtTempDate.Year,
                                                        ldtTempDate.Month, 1);
                        ldtTempDate = ldtTempDate.AddDays(-1);
                        lobjPA.icdoPayeeAccount.benefit_end_date = ldtTempDate;
                        lobjPA.icdoPayeeAccount.Update();
    
                        // PROD PIR ID 6069
                        // Death was notified to the member who didn't even get any payment.
                        // Changing the payment status to complete will be enable the Post retirement Acct owner death batch to select and create a Post retirement application.
                        if (!lobjPA.ibusPayeeAccountActiveStatus.IsStatusCancelPending() &&
                            !lobjPA.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                            lobjPA.CreatePaymentCompletePayeeAccountStatus();
                    }
                }
            }
            ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
        }

        //UCS -054 - 44 45 49 50
        //set benefit end date for the member Payee account
        public void SetBenefitEndDateForMember()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
            foreach (busPayeeAccount lobjPA in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
            {
                if (lobjPA.icdoPayeeAccount.payee_perslink_id != icdoDeathNotification.person_id)
                {
                    if (lobjPA.icdoPayeeAccount.dro_application_id > 0)
                    {                   //BR 44 & 49     
                        if ((lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwnerSingleLifeAnnuity)
                            || (lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwnerStraightLife))
                        {
                            //SET TO LAST DAY OF MONTH DEATH OCCURED
                            DateTime ldtTempDate = new DateTime(icdoDeathNotification.date_of_death.AddMonths(1).Year,
                                icdoDeathNotification.date_of_death.AddMonths(1).Month, 1);
                            lobjPA.icdoPayeeAccount.benefit_end_date = ldtTempDate.AddDays(-1);
                            lobjPA.icdoPayeeAccount.Update();
                            break;
                        }
                        else if (((lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner15YrTermCertainAndLifeOption)
                            || (lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner20YrTermCertainAndLifeOption)
                            || (lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner10YrTermCertainAndLifeOption)
                            || (lobjPA.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner5YrTermCertainAndLifeOption)
                             ))
                        {
                            if (lobjPA.icdoPayeeAccount.term_certain_end_date != DateTime.MinValue)
                            {
                                if (lobjPA.icdoPayeeAccount.term_certain_end_date < icdoDeathNotification.date_of_death)
                                {           //BR 50
                                    DateTime ldtTempDate = new DateTime(icdoDeathNotification.date_of_death.AddMonths(1).Year,
                                      icdoDeathNotification.date_of_death.AddMonths(1).Month, 1);
                                    lobjPA.icdoPayeeAccount.benefit_end_date = ldtTempDate.AddDays(-1);
                                    lobjPA.icdoPayeeAccount.Update();
                                    break;
                                }
                                else if (lobjPA.icdoPayeeAccount.term_certain_end_date >= icdoDeathNotification.date_of_death)
                                {   // BR 45
                                    lobjPA.icdoPayeeAccount.benefit_end_date = lobjPA.icdoPayeeAccount.term_certain_end_date;
                                    lobjPA.icdoPayeeAccount.Update();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
        }

        //BR- 54 - 07
        //check if any recovery exists not in satisfied status
        public bool IsNonSatisfiedRecoveryExists()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbPayeeAccount.IsNull())
                ibusPerson.LoadPayeeAccount();

            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount)
            {
                if (lobjPayeeAccount.iclbPaymentRecovery.IsNull())
                    lobjPayeeAccount.LoadPaymentRecovery();

                if (lobjPayeeAccount.iclbPaymentRecovery.Count > 0)
                {
                    var lintCountOfSatisfiedRecovery = lobjPayeeAccount.iclbPaymentRecovery
                                                        .Where(lobjReco => lobjReco.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusSatisfied);

                    if (lintCountOfSatisfiedRecovery.Count() == 0)
                        return true;
                }
            }
            return false;
        }

        //BR-054-21, BR-054-24
        //-- for payment date greater than the date of death
        //--for outstanding
        //BR-054-25, BR-054-22
        //-- for payment date greater than the date of death
        //--Cleared
        //BR-054-23, BR-054-25a
        //-- for payment date less than or equal to the date of death
        //--outstanding
        public bool iblIsWorkFlowCreateLifeInsuranceClaims => Convert.ToInt32(DBFunction.DBExecuteScalar("entDeathNotification.IsWorkFlowCreateLifeInsuranceClaims",
            new object[2] { icdoDeathNotification.person_id, icdoDeathNotification.date_of_death }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0;
        public void InitializeWorkflowBasedonPaymentHistoryStatus()
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbMemberPlusAlternatePayeeAccounts.IsNull())
                ibusPerson.LoadMemberPlusAlternatePayeeAccounts();
            //    bool lblnIsWorkFlowInitiated = false;
            bool lblnIsWorkFlowChangePaymentInitiated = false;
            bool lblnIsWorkFlowCreateReceivableInitiated = false;
            bool lblnIsWorkFlowCancelPaymentInitiated = false;

            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbMemberPlusAlternatePayeeAccounts)
            {
                if (lobjPayeeAccount.iclbPaymentHistoryHeader.IsNull())
                    lobjPayeeAccount.LoadPaymentHistoryHeader();

                foreach (busPaymentHistoryHeader lobjPaymentHistory in lobjPayeeAccount.iclbPaymentHistoryHeader)
                {
                    int lintCountOfOutStandingDistribution = 0;
                    int lintCountOfClearedDistribution = 0;
                    int lintPaymentHistoryDistribution = 0;

                    if (lobjPaymentHistory.iclbPaymentHistoryDistribution.IsNull())
                        lobjPaymentHistory.LoadPaymentHistoryDistribution();

                    lintPaymentHistoryDistribution = lobjPaymentHistory.iclbPaymentHistoryDistribution.Count();
                    lintCountOfOutStandingDistribution = lobjPaymentHistory.iclbPaymentHistoryDistribution.Where(lobjDis => lobjDis.isOutStandingStatus()).Count();
                    lintCountOfClearedDistribution = lobjPaymentHistory.iclbPaymentHistoryDistribution.Where(lobjDis => lobjDis.isClearedStatus()).Count();

                    // PROD PIR ID 5585 -- Do not cancel the payment to be made on the same month of date of death
                    // Workflow should only initiated when the payment date is after date of death and in next month.
                    if (lobjPaymentHistory.icdoPaymentHistoryHeader.payment_date > icdoDeathNotification.date_of_death.GetLastDayofMonth())
                    {
                        //BR-054-21, BR-054-24                       
                        if ((lintCountOfOutStandingDistribution > 0) && (lintCountOfOutStandingDistribution == lintPaymentHistoryDistribution))
                        {
                            if (!lblnIsWorkFlowCancelPaymentInitiated)
                            {
                                if (!IsWorkflowAlreadyExistForPerson(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Cancel_Payment_History))
                                {
                                    InitializeWorkFlow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Cancel_Payment_History);
                                    lblnIsWorkFlowCancelPaymentInitiated = true;
                                }
                            }
                        }
                        if (lintCountOfClearedDistribution > 0)  //BR-054-22, BR-054-25                       
                        {
                            if (!lblnIsWorkFlowCreateReceivableInitiated)
                            {
                                //if (!IsWorkflowAlreadyExistForPerson(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Creating_Receivable_Against_Payment_History))
                                //{
                                    //InitializeWorkFlow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Creating_Receivable_Against_Payment_History);
                                    lblnIsWorkFlowCreateReceivableInitiated = true;
                                //}
                            }
                        }
                        if (lintCountOfOutStandingDistribution > 0)  //BR-054-23, BR-054-25a                       
                        {
                            if (!lblnIsWorkFlowChangePaymentInitiated)
                            {
                                if (!IsWorkflowAlreadyExistForPerson(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Change_Payment_Distribution_Status))
                                {
                                    InitializeWorkFlow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, busConstant.Map_Process_Change_Payment_Distribution_Status);
                                    lblnIsWorkFlowChangePaymentInitiated = true;
                                }
                            }
                        }
                    }
                }
                //if (lblnIsWorkFlowInitiated)
                //    break;
            }
            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (lbusActivityInstance.ibusBpmActivity == null)
                {
                    lbusActivityInstance.LoadBpmActivity();
                }
                if (lbusActivityInstance.ibusBpmActivity.icdoBpmActivity.name.Equals("Enter Death Notification Data"))
                {
                    if (lbusActivityInstance.ibusBpmProcessInstance == null)
                    {
                        lbusActivityInstance.LoadBpmProcessInstance();
                    }
                    if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Initialize_Process_Death_Notification_Workflow)
                    {
                        lbusActivityInstance.UpdateParameter("IsWorkFlowCreateReceivableInitiated", lblnIsWorkFlowCreateReceivableInitiated.ToString());
                        lbusActivityInstance.UpdateParameter("IsWorkFlowCreateLifeInsuranceClaims", iblIsWorkFlowCreateLifeInsuranceClaims.ToString());
                    }
                }
            }
        }

        public void InitializeWorkFlow(int aintPersonID, int aintProcessId)
        {
            
            busWorkflowHelper.InitiateBpmRequest(aintProcessId, aintPersonID, 0, 0, iobjPassInfo);
        }

        //Check if any workflow already initiated for this person and workflow id
        private bool IsWorkflowAlreadyExistForPerson(int aintPersonID, int aintProcessId)
        {

            DataTable ldtbList = Select("entSolBpmActivityInstance.LoadRunningInstancesByPersonAndProcess", new object[2] { aintPersonID, aintProcessId });

            if (ldtbList.Rows.Count > 0)
                return true;
            return false;
        }

        # endregion

        public busPersonBeneficiary ibusPersonToBeneficiary { get; set; } //Used in NonEmployee Death batch, Re-used in Payee death batch too.
        public busPersonDependent ibusPersonToDependent { get; set; }
        public string istrMemberEnrolledInDefComp { get; set; }
        public string istrMemberIsDependentSupplementalLifeInsurance { get; set; }
        public string istrMemberIsSpouseSupplementalLifeInsurance { get; set; }

        /// UAT PIR ID 1360
        /// Validation msg for BR-053-18 should be thrown in Death notification screen too.
        public bool IsAnyBeneficiaryDeathInProgressorNonResponsive()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            return ibusPerson.IsAnyBeneficiaryDeathInProgressorNonResponsive();
        }

        /// UAT PIR ID 1360
        /// Validation msg for BR-053-19 should be thrown in Death notification screen too.
        public bool IsAnyDependentDeathInProgressorNonResponsive()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            return ibusPerson.IsAnyDependentDeathInProgressorNonResponsive();
        }

        public busPayeeAccount ibusDeceasedPayeeAccount { get; set; }

        public void LoadDeceasedPayeeAccount()
        {
            if (ibusDeceasedPayeeAccount.IsNull())
                ibusDeceasedPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };

            DataTable ldtbResult = Select<cdoPayeeAccount>(new string[1] { "PAYEE_PERSLINK_ID" }, new object[1] { icdoDeathNotification.person_id }, null, null);
            if (ldtbResult.Rows.Count > 0) ibusDeceasedPayeeAccount.icdoPayeeAccount.LoadData(ldtbResult.Rows[0]);
        }

        # region Payee Death Letter

        public bool IsPayeeDeathLetter { get; set; }

        public override busBase GetCorOrganization()
        {
            busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (ibusPersonToBeneficiary.IsNotNull())
            {
                if (ibusPersonToBeneficiary.ibusBeneficiaryOrganization.IsNotNull())
                    lobjOrganization = ibusPersonToBeneficiary.ibusBeneficiaryOrganization;
            }
            return lobjOrganization;
        }

        public string IsBeneSpouse
        {
            get
            {
                if (ibusPersonToBeneficiary.IsNotNull())
                {
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        public string IsLifeTermALPD
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if (ibusDeceasedPayeeAccount.IsBenefitOptionSingleLife || ibusDeceasedPayeeAccount.IsBenefitOptionStraightLife || ibusDeceasedPayeeAccount.IsTermCertainOption)
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        public string IsDeceasedDCRetired
        {
            get
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
                if (ibusPerson.icolPersonAccount.Where(lobj => (lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                               lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 || //PIR 25920
                               lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) && //PIR 20232
                               lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired).Any())
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsACODSingleOrStraightLife
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwnerStraightLife ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwnerSingleLifeAnnuity)
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        public string IsACODTermCertainOption
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner5YrTermCertainAndLifeOption ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner10YrTermCertainAndLifeOption ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner15YrTermCertainAndLifeOption ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitDurationValueLifeOfBenAccOwner20YrTermCertainAndLifeOption)
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        public string IsACODandBeneSpouse
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (ibusPersonToBeneficiary.IsNotNull())
                    {
                        if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                            return busConstant.Flag_Yes;
                    }
                }
                return busConstant.Flag_No;
            }
        }

        public string FirstDayofNextMonthToDOD
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (ibusPerson.IsNull()) LoadPerson();
                    if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        return ibusPerson.icdoPerson.date_of_death.GetFirstDayofNextMonth().ToString(busConstant.DateFormatLongDate);
                }
                return string.Empty;
            }
        }

        public string MemberDateofDeathPlus31Days
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (ibusPerson.IsNull()) LoadPerson();
                    if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                        return ibusPerson.icdoPerson.date_of_death.AddDays(31).ToString(busConstant.DateFormatLongDate);
                }
                return string.Empty;
            }
        }

        public string IsInsuranceOnly { get; set; }

        public string IsMemberinDC { get; set; }

        public string IsMemberinDCandSpouseBeneficiary { get; set; }

        public string LatestHealthHistoryEndDate { get; set; }

        public string IsBeneSpouseEligibleForHealth { get; set; }

        public string IsSpouseAgeGreaterThan65 { get; set; }

        public string IsMemberInHealthAndNoSpouse { get; set; }

        public string IsNoHealthPlanEnrolled { get; set; }

        public string IsMemberinDCandSpouseLessthan65 { get; set; }

        public string IsMemberNOTinHealthandSpouseGreaterThan65 { get; set; }

        public string IsMemberinMedicare { get; set; }

        public string IsMemberInDental { get; set; }

        public string IsMemberInDentalAndSpouse { get; set; }

        public string IsMemberInDentalAndNotDependent { get; set; }

        public string IsDentalMemberInDC { get; set; }

        public string IsMemberInVision { get; set; }

        public string IsMemberInVisionAndSpouse { get; set; }

        public string IsMemberInVisionAndNoSpouse { get; set; }

        public string IsVisionMemberInDC { get; set; }

        public string IsMemberInLifeandNotIBS { get; set; }

        public string IsPayeeEnrolledAsBeneinLife { get; set; }

        public decimal idecLifePercentage { get; set; }

        public string IsPayeeAgeLessThan18 { get; set; }

        public decimal idecTotalLifeCoverageAmount { get; set; }

        public string IsPayeeRelationEstateorTrust { get; set; }

        public string PayeeRelationship { get; set; }

        public string IsMemberandSpouseInLTC { get; set; }

        public string IsMemberandSpouseNOTInLTC { get; set; }

        public string IsMemberOnlyInLTC { get; set; }

        public string IsSpouseOnlyInLTC { get; set; }

        public string IsLTCVisible { get; set; }

        public string IsMemberInDeferredComp { get; set; }

        public string IsSpouseMedicareClaimNumberExists { get; set; }

        public decimal idecHealthPremiumAmount { get; set; }

        public decimal idecMedicarePremiumAmount { get; set; }

        public decimal idecDentalPremiumAmount { get; set; }

        public decimal idecVisionPremiumAmount { get; set; }

        public void LoadPayeeDeathLetterInsuranceProperties(DateTime adteBatchRunDate)
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
            if (ibusPersonToBeneficiary.IsNotNull())
            {
                if (ibusPersonToBeneficiary.ibusBeneficiaryPerson.IsNull()) ibusPersonToBeneficiary.LoadBeneficiaryPerson();
                int lintPayeeAgeinMonths = 0;
                if (ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_DOB != DateTime.MinValue)
                    lintPayeeAgeinMonths = busGlobalFunctions.DateDiffByMonth(ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_DOB, ibusPerson.icdoPerson.date_of_death);

                if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
                bool iblnIsMemberInDC = ibusPerson.icolPersonAccount.Where(lobj => lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                                                                                   lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 || //PIR 25920
                                                                                   lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020).Any(); //PIR 20232
                if (iblnIsMemberInDC)
                    IsMemberinDC = busConstant.Flag_Yes;
                if (iblnIsMemberInDC && ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    IsMemberinDCandSpouseBeneficiary = busConstant.Flag_Yes;

                bool lblnIsSpouseDependentonMember = false;
                if (ibusPerson.iclbPersonDependent.IsNull()) ibusPerson.LoadDependent();
                if (ibusPerson.iclbPersonDependent.Where(lobj =>
                        lobj.icdoPersonDependent.dependent_perslink_id == ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id).Any())
                    lblnIsSpouseDependentonMember = true;

                // ********** HEALTH INSURANCE ****************
                int lintPAHealthID = GetEnrolledPersonAccountID(busConstant.PlanIdGroupHealth);
                if (lintPAHealthID > 0)
                {
                    if (ibusPerson.iclbPersonBeneficiary.IsNull()) ibusPerson.LoadBeneficiary();
                    if (ibusPerson.iclbPersonBeneficiary.Where(lobj =>
                            lobj.icdoPersonBeneficiary.beneficiary_person_id == ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id).Any())
                        lblnIsSpouseDependentonMember = true;

                    // No Spouse Exists
                    if (!(ibusPerson.iclbPersonDependent.Where(lobj => lobj.icdoPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse).Any()))
                        IsMemberInHealthAndNoSpouse = busConstant.Flag_Yes;

                    if (ibusPerson.ibusHealthPersonAccount.IsNull()) ibusPerson.LoadHealthPersonAccount();
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.IsNull()) ibusPerson.ibusHealthPersonAccount.LoadPersonAccountGHDV();
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNull())
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                    busPersonAccountGhdvHistory lbusGhdvHistory = ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(ibusPerson.icdoPerson.date_of_death);
                    ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV = lbusGhdvHistory.LoadGHDVObject(ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV);
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.ibusPerson == null) ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadPerson();
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.ibusPlan == null) ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadPlan();
                    //Initialize the Org Object to Avoid the NULL error
                    ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.InitializeObjects();
                    ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate = ibusPerson.icdoPerson.date_of_death;
                    LatestHealthHistoryEndDate = icdoDeathNotification.date_of_death.GetFirstDayofNextMonth().ToString(busConstant.DateFormatLongDate); // SYS PIR 2602
                    if ((ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdMedicarePartD) && (lblnIsSpouseDependentonMember))) // Medicare Part D
                        IsMemberinMedicare = busConstant.Flag_Yes;

                    IsBeneSpouseEligibleForHealth = "N";
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    {
                        // PROD PIR ID 5459 -- The Spouse is eligilible to enroll as of the current's date.
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate = adteBatchRunDate;
                        if (lintPayeeAgeinMonths < 780)
                        {
                            IsSpouseAgeGreaterThan65 = "N";
                            if (iblnIsMemberInDC)
                                IsMemberinDCandSpouseLessthan65 = busConstant.Flag_Yes;
                        }
                        else
                            IsSpouseAgeGreaterThan65 = "Y";

                        if ((lblnIsSpouseDependentonMember) && (lintPayeeAgeinMonths < 780) &&
                            (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no.IsNullOrEmpty()))
                            IsBeneSpouseEligibleForHealth = "Y";

                        if (ibusPersonToBeneficiary.ibusBeneficiaryPerson.ibusHealthPersonAccount.IsNull())
                            ibusPersonToBeneficiary.ibusBeneficiaryPerson.LoadHealthPersonAccount();
                        if (ibusPersonToBeneficiary.ibusBeneficiaryPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.IsNull())
                            ibusPersonToBeneficiary.ibusBeneficiaryPerson.ibusHealthPersonAccount.LoadPersonAccountGHDV();
                        if ((ibusPersonToBeneficiary.ibusBeneficiaryPerson.ibusHealthPersonAccount.icdoPersonAccount.person_account_id > 0) &&
                            (ibusPersonToBeneficiary.ibusBeneficiaryPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no.IsNotNullOrEmpty()))
                            IsSpouseMedicareClaimNumberExists = busConstant.Flag_Yes;
                    }

                    // ******** UAT PIR ID 2628 Calculate Health Premium amount
                    if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree)
                    {
                        if (lblnIsSpouseDependentonMember)
                        {
                            if (lintPayeeAgeinMonths < 780)
                            {
                                ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0021";
                            }
                            else
                            {
                                ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0041";
                            }
                        }
                        if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                        else
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                    }
                    else
                    {
                        if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_id > 0 &&
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty() &&
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date > adteBatchRunDate)
                        {
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";
                            if (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                            else
                                ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();
                            ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID(
                                                    ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date);
                        }
                    }
                    idecHealthPremiumAmount = ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                }
                else
                {
                    // ******** UAT PIR ID 2628 Calculate Health Premium amount
                    if (lintPayeeAgeinMonths < 780)
                    {
                        ibusPerson.ibusHealthPersonAccount = new busPersonAccount
                        {
                            ibusPersonAccountGHDV = new busPersonAccountGhdv
                            {
                                ibusPlan = new busPlan { icdoPlan = new cdoPlan { plan_id = busConstant.PlanIdGroupHealth } },
                                icdoPersonAccount = new cdoPersonAccount { plan_id = busConstant.PlanIdGroupHealth },
                                icdoPersonAccountGhdv = new cdoPersonAccountGhdv
                                {
                                    health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree,
                                    coverage_code = "0021"
                                },
                                idtPlanEffectiveDate = icdoDeathNotification.date_of_death
                            }
                        };
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();
                        ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                        idecHealthPremiumAmount = ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                    }

                    IsNoHealthPlanEnrolled = busConstant.Flag_Yes;
                    // SYS PIR ID 2623
                    if (ibusDeceasedPayeeAccount.ibusApplication.IsNull()) ibusDeceasedPayeeAccount.LoadApplication();
                    if ((lintPayeeAgeinMonths < 780) &&
                        (ibusPerson.ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no.IsNullOrEmpty()) &&
                        (ibusDeceasedPayeeAccount.IsBenefitOptionJandS || ibusDeceasedPayeeAccount.IsTermCertainOption ||
                         ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit ||
                         ibusDeceasedPayeeAccount.ibusApplication.IsReducedRHICSelected()))
                        IsBeneSpouseEligibleForHealth = "Y";

                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                    {
                        if (lintPayeeAgeinMonths > 780)
                            IsMemberNOTinHealthandSpouseGreaterThan65 = busConstant.Flag_Yes;
                    }
                }

                // ********** DENTAL INSURANCE ****************
                int lintPADentalID = GetEnrolledPersonAccountID(busConstant.PlanIdDental);
                if (lintPADentalID > 0)
                {
                    IsMemberInDental = busConstant.Flag_Yes;
                    if (ibusPerson.iclbPersonDependent.IsNull()) ibusPerson.LoadDependent();
                    if (ibusPerson.iclbPersonDependent.Where(lobj => lobj.icdoPersonDependent.dependent_perslink_id == ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id).Any())
                    {
                        if (iblnIsMemberInDC)
                            IsDentalMemberInDC = busConstant.Flag_Yes;
                    }
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    {
                        IsMemberInDentalAndNotDependent = busConstant.Flag_Yes;
                        if (lblnIsSpouseDependentonMember)
                            IsMemberInDentalAndSpouse = busConstant.Flag_Yes;
                    }
                }
                else
                {
                    IsMemberInDental = busConstant.Flag_No;
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse &&
                        (ibusDeceasedPayeeAccount.IsBenefitOptionJandS || ibusDeceasedPayeeAccount.IsTermCertainOption ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit))
                        IsMemberInDentalAndSpouse = busConstant.Flag_Yes;
                }
                DataTable ldtbRateTable = null;
                int lintDentalOrgPlanID = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdDental, icdoDeathNotification.date_of_death); // PROD PIR ID 7066
                idecDentalPremiumAmount = busRateHelper.GetDentalPremiumAmount(lintDentalOrgPlanID, busConstant.DentalInsuranceTypeRetiree, busConstant.DentalLevelofCoverageIndividual,
                                                                                            icdoDeathNotification.date_of_death, ldtbRateTable, iobjPassInfo);

                // ********** VISION INSURANCE ****************
                int lintPAVisionID = GetEnrolledPersonAccountID(busConstant.PlanIdVision);
                if (lintPAVisionID > 0)
                {
                    if (ibusPerson.iclbPersonDependent.IsNull()) ibusPerson.LoadDependent();
                    if (ibusPerson.iclbPersonDependent.Where(lobj => lobj.icdoPersonDependent.dependent_perslink_id == ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id).Any())
                    {
                        IsMemberInVision = busConstant.Flag_Yes;
                        if (iblnIsMemberInDC)
                            IsVisionMemberInDC = busConstant.Flag_Yes;
                    }
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    {
                        IsMemberInVisionAndNoSpouse = busConstant.Flag_Yes;
                        if (lblnIsSpouseDependentonMember)
                            IsMemberInVisionAndSpouse = busConstant.Flag_Yes;
                    }
                }
                else
                {
                    IsMemberInVision = busConstant.Flag_No;
                    if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse &&
                        (ibusDeceasedPayeeAccount.IsBenefitOptionJandS || ibusDeceasedPayeeAccount.IsTermCertainOption ||
                        ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit))
                        IsMemberInVisionAndSpouse = busConstant.Flag_Yes;
                }
                // Calculate Premium Amount
                DataTable ldtbVision = null;
                int lintVisionOrgPlanID = ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdVision, icdoDeathNotification.date_of_death); // PROD PIR ID 7066
                idecVisionPremiumAmount = busRateHelper.GetVisionPremiumAmount(lintVisionOrgPlanID, busConstant.VisionInsuranceTypeRetiree, busConstant.VisionLevelofCoverageIndividual,
                                                                                    icdoDeathNotification.date_of_death, ldtbVision, iobjPassInfo);

                // ********** GROUP LIFE INSURANCE ****************
                int lintPALifeID = GetEnrolledPersonAccountIDForLife();
                if (lintPALifeID > 0)
                {
                    // Member in Life
                    busPersonAccountLife lobjLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife(), icdoPersonAccount = new cdoPersonAccount() };
                    lobjLife.FindPersonAccountLife(lintPALifeID);
                    lobjLife.FindPersonAccount(lintPALifeID);
                    lobjLife.LoadPersonAccount();
                    lobjLife.LoadPaymentElection();
                    lobjLife.LoadPerson();
                    lobjLife.ibusPerson.LoadPersonOverviewBeneficiary();
                    lobjLife.LoadLifeOptionData();
                    idecTotalLifeCoverageAmount = lobjLife.iclbLifeOption.Where(o => o.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue &&
                                                                                 o.icdoPersonAccountLifeOption.effective_end_date == DateTime.MinValue).
                                                                                 Sum(o => o.icdoPersonAccountLifeOption.coverage_amount);
                    
                    if (lobjLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                        IsMemberInLifeandNotIBS = busConstant.Flag_Yes;
                    // Payee Enrolled as Bene for Member
                    bool lblnFlag = false;
                    foreach (busPersonBeneficiary lobjPersonBene in lobjLife.ibusPerson.iclbPersonBeneficiary)
                    {
                        if (lobjPersonBene.ibusPersonAccountBeneficiary.IsNull()) lobjPersonBene.LoadPersonAccountBeneficiary();
                        if (lobjPersonBene.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull()) lobjPersonBene.ibusPersonAccountBeneficiary.LoadPersonAccount();
                        if (lobjPersonBene.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull()) lobjPersonBene.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();
                        if (lobjPersonBene.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                        {
                            if (lobjPersonBene.icdoPersonBeneficiary.beneficiary_person_id == ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id)
                            {
                                lblnFlag = true;
                                idecLifePercentage = lobjPersonBene.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent;
                                break;
                            }
                        }
                        if (lobjPersonBene.icdoPersonBeneficiary.benificiary_org_id == ibusPersonToBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.org_id)
                        {
                            // Payee in Life as Estate or Trustee
                            if ((lobjPersonBene.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate) ||
                                (lobjPersonBene.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipTrustee))
                            {
                                IsPayeeRelationEstateorTrust = busConstant.Flag_Yes;
                                PayeeRelationship = lobjPersonBene.icdoPersonBeneficiary.relationship_description;
                                break;
                            }
                        }
                    }

                    if (lblnFlag)
                        IsPayeeEnrolledAsBeneinLife = busConstant.Flag_Yes;
                    else
                        IsPayeeEnrolledAsBeneinLife = busConstant.Flag_No;

                    if ((lintPayeeAgeinMonths <= 216) && (ibusPersonToBeneficiary.ibusBeneficiaryPerson.icdoPerson.person_id > 0))
                    {
                        IsPayeeAgeLessThan18 = busConstant.Flag_Yes;
                    }
                }

                // ********** LONG TERM CARE ****************                     
                if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                {
                    IsLTCVisible = busConstant.Flag_Yes;
                    if (ibusPerson.icolPersonAccount.IsNull()) ibusPerson.LoadPersonAccount();
                    int lintMemberLTCID = GetEnrolledLTCPersonAccountID(ibusPerson.icolPersonAccount);

                    if (ibusPersonToBeneficiary.ibusBeneficiaryPerson.icolPersonAccount.IsNull()) ibusPersonToBeneficiary.ibusBeneficiaryPerson.LoadPersonAccount();
                    int lintSpouseLTCID = GetEnrolledLTCPersonAccountID(ibusPersonToBeneficiary.ibusBeneficiaryPerson.icolPersonAccount);

                    if (lintMemberLTCID > 0)
                    {
                        if (lintSpouseLTCID > 0)
                            IsMemberandSpouseInLTC = busConstant.Flag_Yes;
                        else
                            IsMemberOnlyInLTC = busConstant.Flag_Yes;
                    }
                    else if (lintSpouseLTCID > 0)
                        IsSpouseOnlyInLTC = busConstant.Flag_Yes;
                    else
                        IsMemberandSpouseNOTInLTC = busConstant.Flag_Yes;
                }

                // ********** DEFERRED COMPENSATION ****************         
                int lintDeferredID = ibusPerson.icolPersonAccount.Where(lobj =>
                                        ((lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled) ||
                                        (lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended))
                                        && ((lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                                        || (lobj.icdoPersonAccount.plan_id == busConstant.PlanIdOther457)))
                                        .Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();
                if (lintDeferredID > 0)
                    IsMemberInDeferredComp = busConstant.Flag_Yes;
                else
                    IsMemberInDeferredComp = busConstant.Flag_No;
            }
            else
            {
                IsLTCVisible = busConstant.Flag_No;
            }
        }

        public int GetEnrolledPersonAccountIDForLife()
        {
            if (ibusPerson.IsNotNull() && ibusPerson.icolPersonAccount.IsNotNull())
            {
                busPersonAccount lobjPA = ibusPerson.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife).FirstOrDefault();
                if (lobjPA != null)
                {
                    if (lobjPA.ibusPersonAccountLife == null) lobjPA.LoadPersonAccountLife();
                    busPersonAccountLifeHistory lobjLifeHistory = lobjPA.ibusPersonAccountLife.LoadHistoryByDateWithOutOption(icdoDeathNotification.date_of_death);
                    if (lobjLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        return lobjLifeHistory.icdoPersonAccountLifeHistory.person_account_id;
                    else
                        return 0;
                }
            }
            return 0;
        }

        public int GetEnrolledPersonAccountID(int aintPlanID)
        {
            if (ibusPerson.IsNotNull() && ibusPerson.icolPersonAccount.IsNotNull())
            {
                foreach (busPersonAccount lobjPA in ibusPerson.icolPersonAccount)
                {
                    if (lobjPA.icdoPersonAccount.plan_id == aintPlanID)
                    {
                        if (lobjPA.ibusPersonAccountGHDV.IsNull()) lobjPA.LoadPersonAccountGHDV();
                        if (lobjPA.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNull()) lobjPA.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                        if (lobjPA.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(lobj =>
                                        lobj.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                        busGlobalFunctions.CheckDateOverlapping(icdoDeathNotification.date_of_death,
                                        lobj.icdoPersonAccountGhdvHistory.start_date, lobj.icdoPersonAccountGhdvHistory.end_date)).Any())
                            return lobjPA.icdoPersonAccount.person_account_id;
                    }
                }
            }
            return 0;
        }

        public int GetEnrolledLTCPersonAccountID(Collection<busPersonAccount> aclbPersonAccount)
        {
            if (aclbPersonAccount.IsNotNull())
            {
                foreach (busPersonAccount lobjPA in aclbPersonAccount)
                {
                    if (lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
                    {
                        if (lobjPA.ibusPersonAccountLtc.IsNull()) lobjPA.LoadPersonAccountLtc();
                        if (lobjPA.ibusPersonAccountLtc.iclbLtcHistory.IsNull()) lobjPA.ibusPersonAccountLtc.LoadLtcOptionHistory(false);

                        if (lobjPA.ibusPersonAccountLtc.iclbLtcHistory.Where(lobj =>
                                        lobj.icdoPersonAccountLtcOptionHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                        busGlobalFunctions.CheckDateOverlapping(icdoDeathNotification.date_of_death,
                                        lobj.icdoPersonAccountLtcOptionHistory.start_date, lobj.icdoPersonAccountLtcOptionHistory.end_date)).Any())

                            return lobjPA.icdoPersonAccount.person_account_id;
                    }
                }
            }
            return 0;
        }

        public string IsLifeAPTermEndDatePastDate
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if (ibusDeceasedPayeeAccount.icdoPayeeAccount.term_certain_end_date < DateTime.Today)
                        return busConstant.Flag_Yes;
                    else
                        return busConstant.Flag_No;
                }
                return string.Empty;
            }
        }

        public string IsRetrDisaTermEndDatePastDate
        {
            get
            {
                if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                if ((ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                   (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                {
                    if (ibusDeceasedPayeeAccount.IsTermCertainOption)
                    {
                        if (ibusDeceasedPayeeAccount.icdoPayeeAccount.term_certain_end_date <= DateTime.Today)
                            return busConstant.Flag_Yes;
                        else
                            return busConstant.Flag_No;
                    }
                }
                return string.Empty;
            }
        }

        public string IsRetrDisaSingleRemMGexists
        {
            get
            {
                if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                if ((ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                   (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                {
                    if (ibusDeceasedPayeeAccount.IsBenefitOptionSingleLife)
                    {
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount == 0M) ibusDeceasedPayeeAccount.LoadMinimumGuaranteeAmount();
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount > 0M)
                            return busConstant.Flag_Yes;
                        else
                            return busConstant.Flag_No;
                    }
                }
                return string.Empty;
            }
        }

        public string IsRetrDisaSingleRemMGnotExistsOrJS
        {
            get
            {
                if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                if ((ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                   (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                {
                    if (ibusDeceasedPayeeAccount.IsBenefitOptionSingleLife)
                    {
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount == 0M) ibusDeceasedPayeeAccount.LoadMinimumGuaranteeAmount();
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount <= 0M)
                            return busConstant.Flag_Yes;
                    }
                }
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdJobService) && (ibusDeceasedPayeeAccount.IsBenefitOptionStraightLife))
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsRetrDisaOrJSDisa
        {
            get
            {
                if (ibusDeceasedPayeeAccount.IsNotNull())
                {
                    if ((ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                       (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                    {
                        if ((ibusDeceasedPayeeAccount.IsBenefitOptionJandS) ||
                            (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value.Equals(busConstant.BenefitOptionNormalRetBenefit)) ||
                            (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value.Equals(busConstant.BenefitOptionDisability)))
                            return busConstant.Flag_Yes;
                    }
                }
                return busConstant.Flag_No;
            }
        }

        public string IsRemainingMGExistsALPD
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if ((ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                       (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                    {
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount == 0M) ibusDeceasedPayeeAccount.LoadMinimumGuaranteeAmount();
                        if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount > 0M)
                            return busConstant.Flag_Yes;
                        else
                            return busConstant.Flag_No;
                    }
                }
                return string.Empty;
            }
        }

        public string IsFBEDTermCertainEndDatePastDate
        {
            get
            {
                if (icdoDeathNotification.death_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                {
                    if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                    if (ibusDeceasedPayeeAccount.icdoPayeeAccount.term_certain_end_date < DateTime.Today)
                        return busConstant.Flag_Yes;
                    else
                        return busConstant.Flag_No;
                }
                return string.Empty;
            }
        }

        public string IsPriorJudge
        {
            get
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdPriorJudges))
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsPriorService
        {
            get
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdPriorService))
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsRHICOptionReduced
        {
            get
            {
                if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
                if (ibusDeceasedPayeeAccount.ibusApplication.IsNull()) ibusDeceasedPayeeAccount.LoadApplication();
                if ((ibusDeceasedPayeeAccount.IsBenefitOptionSingleLife) && (ibusDeceasedPayeeAccount.ibusApplication.IsReducedRHICSelected()))
                    return busConstant.Flag_Yes;
                else if ((ibusDeceasedPayeeAccount.IsBenefitOptionJandS) &&
                    (ibusDeceasedPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard))
                    return busConstant.Flag_Yes;
                else if (ibusDeceasedPayeeAccount.IsTermCertainOption)
                {
                    if (ibusDeceasedPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value == busConstant.RHICOptionStandard)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoDeathNotification.date_of_death,
                                                ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_begin_date, ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_end_date))
                            return busConstant.Flag_Yes;
                    }
                    if (ibusDeceasedPayeeAccount.ibusApplication.IsReducedRHICSelected())
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        public decimal idecMonthlyBenefitPaymentAmount { get; set; }

        public decimal idecTaxableMonthlyBenefitPaymentAmount { get; set; }

        public decimal idecNonTaxableMonthlyBenefitPaymentAmount { get; set; }

        public decimal idecTaxableMonthlyBenefitPaymentAmount20Percent
        {
            get
            {
                return idecTaxableMonthlyBenefitPaymentAmount * 0.2M;
            }
        }

        public decimal idecTaxableMonthlyBenefitPaymentAmount10Percent
        {
            get
            {
                return idecTaxableMonthlyBenefitPaymentAmount * 0.1M;
            }
        }

        public void LoadTaxableandNonTaxablePaymentAmount()
        {
            if (ibusPersonToBeneficiary.IsNotNull() && ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.IsNotNull())
            {
                decimal ldecMGamount = 0M;
                ldecMGamount = ibusDeceasedPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ibusDeceasedPayeeAccount.idecpaidgrossamount;
                ibusDeceasedPayeeAccount.LoadBalanceNontaxableAmount();
                if (ldecMGamount < ibusDeceasedPayeeAccount.idecBalanceNontaxableAmount)
                {
                    idecNonTaxableMonthlyBenefitPaymentAmount = ldecMGamount;
                    idecTaxableMonthlyBenefitPaymentAmount = 0;
                }
                else
                {
                    idecTaxableMonthlyBenefitPaymentAmount = (ldecMGamount - ibusDeceasedPayeeAccount.idecBalanceNontaxableAmount) *
                                                            (ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent / 100M);
                    idecNonTaxableMonthlyBenefitPaymentAmount = ibusDeceasedPayeeAccount.idecBalanceNontaxableAmount *
                                                            (ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent / 100M);
                }
            }
        }

        public void LoadMonthlyBenefitPaymentAmount()
        {
            idecMonthlyBenefitPaymentAmount = 0M;
            if (ibusPersonToBeneficiary.IsNotNull() && ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.IsNotNull())
            {
                if (ibusDeceasedPayeeAccount.idecBalanceNontaxableAmount == 0M) ibusDeceasedPayeeAccount.LoadBalanceNontaxableAmount();
                if (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount == 0M) ibusDeceasedPayeeAccount.LoadMinimumGuaranteeAmount();
                idecMonthlyBenefitPaymentAmount = ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent *
                                                                    (ibusDeceasedPayeeAccount.idecMinimumGuaranteeAmount / 100);
            }
        }

        public decimal idecMonthlyGrossBenefitAmount { get; set; }

        public decimal idecMonthlyGrossPercentPaymentAmount { get; set; }

        public void LoadMonthlyGrossBenefitAmount()
        {
            if (ibusDeceasedPayeeAccount.IsNull()) LoadDeceasedPayeeAccount();
            if (ibusDeceasedPayeeAccount.ibusPlan.IsNull()) ibusDeceasedPayeeAccount.LoadPlan();
            ibusDeceasedPayeeAccount.LoadBenefitProvisionBenefitOption(ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
                            ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value, icdoDeathNotification.date_of_death,
                            ibusDeceasedPayeeAccount.ibusPlan.icdoPlan.benefit_provision_id);
            if (ibusDeceasedPayeeAccount.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor > 0M)
                idecMonthlyGrossBenefitAmount = ibusDeceasedPayeeAccount.idecGrossBenfitAmount *
                    ibusDeceasedPayeeAccount.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor;
            else
                idecMonthlyGrossBenefitAmount = ibusDeceasedPayeeAccount.idecGrossBenfitAmount;
            if (ibusPersonToBeneficiary.IsNotNull() && ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.IsNotNull())
                idecMonthlyGrossPercentPaymentAmount = ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent *
                                                            (idecMonthlyGrossBenefitAmount / 100);
        }

        # endregion

        //to be used in Employee death
        public busPersonBeneficiary ibusPersonBeneficiary { get; set; }
        public string istrIsFromEmployeeDeathBatch { get; set; }

        //Systest PIR 2626
        //load RHIC combine records
        public Collection<busBenefitRhicCombine> iclbAllRHICombine { get; set; }
        public void LoadAllRHICCombine()
        {
            if (iclbAllRHICombine.IsNull())
                iclbAllRHICombine = new Collection<busBenefitRhicCombine>();

            if (ibusPerson.iclbBenefitRhicCombine.IsNull())
                ibusPerson.LoadBenefitRhicCombine(true);

            //Load rhic in which deceased is donor
            if (ibusPerson.iclbBenefitRHICCombineAsDonor.IsNull())
                ibusPerson.LoadRhicCombineForPersonAsDonor();
            //receiver
            foreach (busBenefitRhicCombine lobjRHICCombine in ibusPerson.iclbBenefitRhicCombine)
            {
                if (lobjRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusPendingApproval
                    || lobjRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved)
                    iclbAllRHICombine.Add(lobjRHICCombine);
            }
            //donor
            foreach (busBenefitRhicCombine lobjRHICCombine in ibusPerson.iclbBenefitRHICCombineAsDonor)
            {
                if (lobjRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusPendingApproval
                    || lobjRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved)
                {
                    //UAT PIR 2106: We need to exclude the RHIC Record which is getting created for spouse (after the death date) in the validation 
                    if (lobjRHICCombine.icdoBenefitRhicCombine.start_date <= icdoDeathNotification.date_of_death.GetLastDayofMonth())
                    {
                        if (iclbAllRHICombine.Where(lobjRHIC => lobjRHIC.icdoBenefitRhicCombine.benefit_rhic_combine_id == lobjRHICCombine.icdoBenefitRhicCombine.benefit_rhic_combine_id).Count() == 0)
                        {
                            lobjRHICCombine.LoadPerson();
                            iclbAllRHICombine.Add(lobjRHICCombine);
                        }
                    }
                }
            }
        }
        // BPM Death Automation - load latest RHIC combine records
        public Collection<busBenefitRhicCombine> iclbAllLatestRHICombine { get; set; }
        public void LoadAllLatestRHICCombine()
        {
            if (iclbAllLatestRHICombine.IsNull())
                iclbAllLatestRHICombine = new Collection<busBenefitRhicCombine>();

            if (ibusPerson.iclbBenefitRhicCombine.IsNull())
                ibusPerson.LoadBenefitRhicCombine(true);

            //Load rhic in which deceased is donor
            if (ibusPerson.iclbBenefitRHICCombineAsDonor.IsNull())
                ibusPerson.LoadRhicCombineForPersonAsDonor();
            //receiver
            if (ibusPerson.iclbBenefitRhicCombine.Count > 0)
            {
                busBenefitRhicCombine lbusBenefitRhicCombine = ibusPerson.iclbBenefitRhicCombine.OrderByDescending(i => i.icdoBenefitRhicCombine.benefit_rhic_combine_id).FirstOrDefault();
                iclbAllLatestRHICombine.Add(lbusBenefitRhicCombine);
            }

            //donor
            foreach (busBenefitRhicCombine lobjRHICCombine in ibusPerson.iclbBenefitRHICCombineAsDonor)
            {
                if (ibusPerson.iclbBenefitRHICCombineAsDonor.Count > 0)
                {
                    busBenefitRhicCombine lbusBenefitRhicCombine = ibusPerson.iclbBenefitRHICCombineAsDonor.OrderByDescending(i => i.icdoBenefitRhicCombine.benefit_rhic_combine_id).FirstOrDefault();
                    //UAT PIR 2106: We need to exclude the RHIC Record which is getting created for spouse (after the death date) in the validation 
                    if (lobjRHICCombine.icdoBenefitRhicCombine.start_date <= icdoDeathNotification.date_of_death.GetLastDayofMonth())
                    {
                        if (iclbAllRHICombine.Where(lobjRHIC => lobjRHIC.icdoBenefitRhicCombine.benefit_rhic_combine_id == lobjRHICCombine.icdoBenefitRhicCombine.benefit_rhic_combine_id).Count() == 0)
                        {
                            lobjRHICCombine.LoadPerson();
                            iclbAllRHICombine.Add(lobjRHICCombine);
                        }
                    }
                }
            }
        }
        public string istrEstateOrgCodeID { get; set; }

        public void LoadEstateOrgCodeID()
        {
            //this is called as the condition need to satisfy 
            //based on condition only the estate org code needs to be populated
            IsBeneficiaryEstateExistsForDROpayee();
        }
        public bool IsDeceasedDependentHaveMedicarePartDEnrolled()
        {
            busBase lbusbase = new busBase();
            Collection<busPersonAccountMedicarePartDHistory> lclbPersonAccountMedicarePartDHistory = new Collection<busPersonAccountMedicarePartDHistory>();
            DataTable ldtbResult = SelectWithOperator<cdoPersonAccountMedicarePartDHistory>(
                                        new string[3] { "MEMBER_PERSON_ID", "PLAN_PARTICIPATION_STATUS_VALUE", "PERSON_ID" },
                                        new string[3] { "=", "=", "<>" },
                                        new object[3] { icdoDeathNotification.person_id, busConstant.PlanParticipationStatusInsuranceEnrolled, icdoDeathNotification.person_id }, null);
            lclbPersonAccountMedicarePartDHistory = lbusbase.GetCollection<busPersonAccountMedicarePartDHistory>(ldtbResult, "icdoPersonAccountMedicarePartDHistory");
            if (lclbPersonAccountMedicarePartDHistory.Count > 0 && lclbPersonAccountMedicarePartDHistory.Any(i => i.icdoPersonAccountMedicarePartDHistory.end_date == DateTime.MinValue))
                return true;
            return false;
        }
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.AsEnumerable().Where(i => i.icdoPersonEmployment.end_date == DateTime.MinValue).Count() > 0)
                iblnIsPersonEmploymentWithoutEndDate = true;
            if (ibusPerson.icolPersonEmployment.AsEnumerable().Where(i => i.icdoPersonEmployment.end_date > icdoDeathNotification.date_of_death.GetLastDayofMonth()).Count() > 0)
                iblnIsPersonEmploymentFutureEndDate = true;
            base.ValidateHardErrors(aenmPageMode);  
        }
        public override bool ValidateSoftErrors()
        {
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.AsEnumerable().Where(i => i.icdoPersonEmployment.iblnEmploymentEndDateFromDeath).Any(i => i.icdoPersonEmployment.end_date.Date != icdoDeathNotification.date_of_death.Date))
                iblnIsDateOfDeathEqualsToEmploymentEndDate = true;
            return base.ValidateSoftErrors();
        }
        public void ResetFlagForNextCorrespondence()
        {
            istrMemberIsSpouseSupplementalLifeInsurance = busConstant.Flag_No;
            istrMemberEnrolledInDefComp = busConstant.Flag_No;
            istrMemberIsDependentSupplementalLifeInsurance = busConstant.Flag_No;
            ibusPerson.IsPersonBeneficiaryForDBOrDCPlan = busConstant.Flag_No;
            if(ibusPersonToDependent.IsNotNull())
                ibusPersonToDependent.ibusPerson.IsDeceasedIsSpouseToPerson = busConstant.Flag_No;
            if(ibusPersonToBeneficiary.IsNotNull())
                ibusPersonToBeneficiary.ibusPerson.IsDeceasedIsSpouseToPerson = busConstant.Flag_No;
        }
        public void SetPABeneficiaryCorrespondenceProperties(Collection<busPerson> aobjDummyPerson, busPersonAccountBeneficiary aobjpersonAccountBeneficiary)
        {
            ibusPersonToBeneficiary = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
            ibusPersonToBeneficiary.icdoPersonBeneficiary = aobjpersonAccountBeneficiary.icdoPersonBeneficiary;
            ibusPersonToBeneficiary.LoadPerson();

            //check if the correspondence is already generated for this person
            if (aobjDummyPerson.Where(o => o.icdoPerson.person_id == ibusPersonToBeneficiary.ibusPerson.icdoPerson.person_id).Count() == 0)
            {
                if (busGlobalFunctions.CheckDateOverlapping(ibusPerson.icdoPerson.date_of_death,
                                              aobjpersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                              aobjpersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date))
                {
                    if (ibusPersonToBeneficiary.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
                    {
                        if (aobjpersonAccountBeneficiary.ibusPersonAccount.IsNull())
                            aobjpersonAccountBeneficiary.LoadPersonAccount();
                        if (aobjpersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull())
                            aobjpersonAccountBeneficiary.ibusPersonAccount.LoadPlan();

                        //check if the dependent is spouse to person
                        if (ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonContactTypeSpouse)
                        {
                            ibusPersonToBeneficiary.ibusPerson.IsDeceasedIsSpouseToPerson = busConstant.Flag_Yes;
                        }

                        //check is the deceased person is spouse in life plan
                        if (ibusPersonToBeneficiary.ibusPerson.IsDeceasedIsSpouseToPerson == busConstant.Flag_Yes)
                        {
                            if (aobjpersonAccountBeneficiary.ibusPersonAccount.ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupLife)
                            {
                                ibusPerson.isDeceasedSpouseInLife = busConstant.Flag_Yes;
                            }
                        }

                        //check is member enrolled in Life Spouse supplemental Life Insurance
                        if (ibusPersonToBeneficiary.ibusPerson.istrMemberIsSpouseSupplementalLifeInsurance == busConstant.Flag_Yes)
                            istrMemberIsSpouseSupplementalLifeInsurance = busConstant.Flag_Yes;

                        //check if the member is enrolled in Def comp in enrolled status and not end dated
                        if (ibusPersonToBeneficiary.ibusPerson.istrMemberEnrolledInDefComp == busConstant.Flag_Yes)
                            istrMemberEnrolledInDefComp = busConstant.Flag_Yes;

                        //is member enrolled in Life with coverage level as  dependent supplemental Life Insurance
                        if (ibusPersonToBeneficiary.ibusPerson.istrMemberIsDependentSupplementalLifeInsurance == busConstant.Flag_Yes)
                            istrMemberIsDependentSupplementalLifeInsurance = busConstant.Flag_Yes;

                        //check if the deceased is beneficiary for DB/DC plan
                        if ((aobjpersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDCRetirementPlan()) ||
                                aobjpersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDBRetirementPlan() ||
                                aobjpersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                            ibusPerson.IsPersonBeneficiaryForDBOrDCPlan = busConstant.Flag_Yes;

                        //this flag is set to generate the letter to whom the deceased is beneficiary to
                        //else it will generate to deceased person
                        istrFromBatch = busConstant.Flag_No;

                        //generate the corr
                        GenerateCorrespondence(this, "APP-7500");

                        //reset the flag for next bene
                        ResetFlagForNextCorrespondence();

                        //add to the dummy person colln to avoid multiple generation of the letter to the same person
                        aobjDummyPerson.Add(ibusPersonToBeneficiary.ibusPerson);
                    }
                }
            }
        }
        public void SetPADependentCorrespondenceProperties(Collection<busPerson> aobjDummyPerson, busPersonAccountDependent aobjPersonAccountDependent)
        {
            ibusPersonToDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };
            ibusPersonToDependent.icdoPersonDependent = aobjPersonAccountDependent.icdoPersonDependent;

            if (ibusPersonToDependent.ibusPerson.IsNull())
                ibusPersonToDependent.LoadPerson();
            if (aobjDummyPerson.Where(o => o.icdoPerson.person_id == ibusPersonToDependent.ibusPerson.icdoPerson.person_id).Count() == 0)
            {
                if (busGlobalFunctions.CheckDateOverlapping(ibusPerson.icdoPerson.date_of_death,
                                              aobjPersonAccountDependent.icdoPersonAccountDependent.start_date,
                                              aobjPersonAccountDependent.icdoPersonAccountDependent.end_date))
                {
                    if (ibusPersonToDependent.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue)
                    {
                        if (aobjPersonAccountDependent.ibusPersonAccount.IsNull())
                            aobjPersonAccountDependent.LoadPersonAccount();
                        if (aobjPersonAccountDependent.ibusPersonAccount.ibusPlan.IsNull())
                            aobjPersonAccountDependent.ibusPersonAccount.LoadPlan();

                        //check if the dependent is spouse to person
                        if (ibusPersonToDependent.icdoPersonDependent.relationship_value == busConstant.PersonContactTypeSpouse)
                        {
                            ibusPersonToDependent.ibusPerson.IsDeceasedIsSpouseToPerson = busConstant.Flag_Yes;
                        }

                        //check is member enrolled in Life Spouse supplemental Life Insurance
                        if (ibusPersonToDependent.ibusPerson.IsDeceasedIsSpouseToPerson == busConstant.Flag_Yes)
                        {
                            if (ibusPersonToDependent.ibusPerson.istrMemberIsSpouseSupplementalLifeInsurance == busConstant.Flag_Yes)
                                istrMemberIsSpouseSupplementalLifeInsurance = busConstant.Flag_Yes;
                        }

                        //check if the member is enrolled in Def comp in enrolled status and not end dated
                        if (ibusPersonToDependent.ibusPerson.istrMemberEnrolledInDefComp == busConstant.Flag_Yes)
                            istrMemberEnrolledInDefComp = busConstant.Flag_Yes;

                        //is member enrolled in Life with coverage level as  dependent supplemental Life Insurance
                        if (ibusPersonToDependent.ibusPerson.istrMemberIsDependentSupplementalLifeInsurance == busConstant.Flag_Yes)
                            istrMemberIsDependentSupplementalLifeInsurance = busConstant.Flag_Yes;

                        //check if the deceased is beneficiary for DB/DC plan
                        if ((aobjPersonAccountDependent.ibusPersonAccount.ibusPlan.IsDCRetirementPlan()) ||
                                aobjPersonAccountDependent.ibusPersonAccount.ibusPlan.IsDBRetirementPlan() ||
                                aobjPersonAccountDependent.ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                            ibusPerson.IsPersonBeneficiaryForDBOrDCPlan = busConstant.Flag_Yes;


                        //this flag is set to generate the letter to whom the deceased is dependent to
                        //else it will generate to deceased person
                        istrFromBatch = busConstant.Flag_No;

                        //generate the corr
                        GenerateCorrespondence(this, "APP-7500");

                        //reset the flag for next depe to
                        ResetFlagForNextCorrespondence();

                        //add to the dummy person colln to avoid multiple generation of the letter to the same person
                        aobjDummyPerson.Add(ibusPersonToDependent.ibusPerson);
                    }
                }
            }
        }
        public void GenerateCorrespondence(busDeathNotification aobjDeathNotification, string astrTemplateName)
        {
            ArrayList larrlist = new ArrayList();
            larrlist.Add(aobjDeathNotification);

            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence(astrTemplateName, aobjDeathNotification, lhstDummyTable);
            Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML();
            lobjCorBuilder.InstantiateWord();
            lobjCorBuilder.CreateCorrespondenceFromTemplate(astrTemplateName, lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
            lobjCorBuilder.CloseWord();
        }

        public void CreateLetter(DataTable adtbResults, string astrGroupType, ref Collection<busPersonBeneficiary> aclbPersonBeneficiary, ref Collection<busDeathNotification> aclbDeathNotification)
        {
            int lintLastDeceasedPersonID = 0;
            foreach (DataRow ldtr in adtbResults.Rows)
            {
                ibusPerson = new busPerson { icdoPerson = new CustomDataObjects.cdoPerson(), iclbActiveBeneForGivenPlan = new Collection<busPersonBeneficiary>() };
                ibusDeceasedPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new CustomDataObjects.cdoPayeeAccount() };
                ibusPersonToBeneficiary = new busPersonBeneficiary
                {
                    ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary
                    {
                        icdoPersonAccountBeneficiary = new CustomDataObjects.cdoPersonAccountBeneficiary()
                    },
                    icdoPersonBeneficiary = new CustomDataObjects.cdoPersonBeneficiary(),
                    ibusBeneficiaryPerson = new busPerson { icdoPerson = new CustomDataObjects.cdoPerson() },
                    ibusBeneficiaryOrganization = new busOrganization { icdoOrganization = new CustomDataObjects.cdoOrganization() }
                };

                icdoDeathNotification.LoadData(ldtr);
                ibusPerson.icdoPerson.LoadData(ldtr);
                ibusDeceasedPayeeAccount.icdoPayeeAccount.LoadData(ldtr);
                ibusPersonToBeneficiary.icdoPersonBeneficiary.LoadData(ldtr);
                ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(ldtr);
                ibusPersonToBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.LoadData(ldtr);

                if (busGlobalFunctions.CheckDateOverlapping(ibusPerson.icdoPerson.date_of_death,
                                                         ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                         ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date))
                {
                    if (((ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0) &&
                         (!aclbPersonBeneficiary.Where(lobj =>
                                lobj.icdoPersonBeneficiary.beneficiary_person_id == ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_person_id &&
                                lobj.icdoPersonBeneficiary.person_id == icdoDeathNotification.person_id).Any())) ||
                        ((ibusPersonToBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0) &&
                         (!aclbPersonBeneficiary.Where(lobj =>
                                lobj.icdoPersonBeneficiary.benificiary_org_id == ibusPersonToBeneficiary.icdoPersonBeneficiary.benificiary_org_id &&
                                lobj.icdoPersonBeneficiary.person_id == icdoDeathNotification.person_id).Any()))) // SYS PIR ID 2619
                    {
                        istrFromBatch = busConstant.Flag_Yes;
                        IsPayeeDeathLetter = true;
                        ibusPersonToBeneficiary.LoadBeneficiaryInfo();
                        ibusPersonToBeneficiary.LoadBeneficiaryAddress(true);
                        ibusPersonToBeneficiary.LoadBeneficiaryPerson();
                        ibusDeceasedPayeeAccount.LoadPayeePerson();
                        ibusDeceasedPayeeAccount.LoadBenfitAccount();
                        LoadMonthlyBenefitPaymentAmount();
                        ibusDeceasedPayeeAccount.LoadPaymentDetails();
                        // UAT PIR: 2132 While fetching the Monthly Benefit Amount, Fetch the amount as of 1st of the month following date of death, 
                        // here retiremetn date or Go Live date which ever is later.
                        ibusDeceasedPayeeAccount.LoadGrossBenefitAmount(
                                busGlobalFunctions.GetMax(icdoDeathNotification.date_of_death, busPayeeAccountHelper.GetPERSLinkGoLiveDate()));
                        LoadMonthlyGrossBenefitAmount();
                        LoadPayeeDeathLetterInsuranceProperties(DateTime.Now);// batch date
                        LoadTaxableandNonTaxablePaymentAmount();
                        ibusDeceasedPayeeAccount.LoadLatestPaymentHistory();

                        if (astrGroupType == "INSU")
                            IsInsuranceOnly = busConstant.Flag_Yes;
                        else
                            IsInsuranceOnly = busConstant.Flag_No;

                        bool lblnGenerateLetter = true;
                        if ((astrGroupType == busConstant.PostRetirementAccountOwnerDeath) &&
                            (ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS))
                        {
                            if ((ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value != busConstant.DependentRelationshipSpouse) ||
                                (ibusDeceasedPayeeAccount.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor == 0M))
                                lblnGenerateLetter = false;
                        }

                        if (lblnGenerateLetter)
                        {
                            //ArrayList larrList = new ArrayList();
                            //larrList.Add(lobjDeathNotification);
                            Hashtable lshtTemp = new Hashtable();
                            lshtTemp.Add("FormTable", "Batch");
                            GenerateCorrespondence(this, "APP-7252");


                            // Update letter sent flag
                            // The query is not grouped by DeathNotification. Possibilities of occurence of same Death notification for different beneficiaries.
                            if (lintLastDeceasedPersonID != icdoDeathNotification.person_id)
                                aclbDeathNotification.Add(this);
                            lintLastDeceasedPersonID = icdoDeathNotification.person_id;

                            aclbPersonBeneficiary.Add(ibusPersonToBeneficiary);
                        }
                    }
                }
            }
        }
        #region BPM Death Automation - Correspondence Changes
        public Collection<busPersonBeneficiary> iclbCorPersonBeneficiary { get; set; }
        #endregion

        public busCorBeneDep LoadCorBeneficiaryData(int aintBeneficiaryID)
        {
            DataTable ldtbdoCorBeneDep = busBase.SelectWithOperator<doCorBeneDep>(
                   new string[2] { enmCorBeneDep.death_notification_id.ToString(), enmCorBeneDep.beneficiary_id.ToString() },
                   new string[2] { "=", "=" },
                   new object[2] { icdoDeathNotification.death_notification_id, aintBeneficiaryID },
                   enmCorBeneDep.cor_bene_dep_id.ToString() + " asc");
            busCorBeneDep lbusCorBeneDep = new busCorBeneDep() { icdoCorBeneDep = new doCorBeneDep() };
            if(ldtbdoCorBeneDep.Rows.Count > 0)
                lbusCorBeneDep.icdoCorBeneDep.LoadData(ldtbdoCorBeneDep.Rows[0]);
            return lbusCorBeneDep;
        }
        public bool IsDateOfDateChangedOrStatusToErroneous()
        {
            if ((icdoDeathNotification.date_of_death != DateTime.MinValue && Convert.ToDateTime(icdoDeathNotification.ihstOldValues[enmDeathNotification.date_of_death.ToString()]) != icdoDeathNotification.date_of_death))
            {
                return true;
            }
            return false;
        }
        public busCorBeneDep LoadCorDependentData(int aintPersonDependentID)
        {
            DataTable ldtbdoCorBeneDep = busBase.SelectWithOperator<doCorBeneDep>(
                   new string[2] { enmCorBeneDep.death_notification_id.ToString(), enmCorBeneDep.person_dependent_id.ToString() },
                   new string[2] { "=", "=" },
                   new object[2] { icdoDeathNotification.death_notification_id, aintPersonDependentID },
                   enmCorBeneDep.cor_bene_dep_id.ToString() + " asc");
            busCorBeneDep lbusCorBeneDep = new busCorBeneDep() { icdoCorBeneDep = new doCorBeneDep() };
            if (ldtbdoCorBeneDep.Rows.Count > 0)
                lbusCorBeneDep.icdoCorBeneDep.LoadData(ldtbdoCorBeneDep.Rows[0]);
            return lbusCorBeneDep;
        }

        public void LoadCorPersonBeneficiary()
        {
            DataTable ldtbBeneficiaries = busNeoSpinBase.Select("entPerson.LoadCorPersonBeneficiary", new object[1] { this.icdoDeathNotification.death_notification_id });
            iclbCorPersonBeneficiary = new Collection<busPersonBeneficiary>();
            foreach (DataRow drBeneficiary in ldtbBeneficiaries.Rows)
            {
                busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
                lobjPersonBeneficiary.FindPersonBeneficiary(Convert.ToInt32(drBeneficiary[enmCorBeneDep.beneficiary_id.ToString()]));
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(drBeneficiary);
                busPerson lbusBenPerson = null;
                busOrganization lbusBenOrg = null;
                //PIR - 2568 Person object is not loading in the query thats why lbusBenPerson is set to null
                //beneficiary person is loaded confilcting with person id, name fields                
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0)
                {
                    lbusBenOrg = new busOrganization { icdoOrganization = new CustomDataObjects.cdoOrganization() };
                    lbusBenOrg.icdoOrganization.LoadData(drBeneficiary);
                }
                lobjPersonBeneficiary.LoadBeneficiaryInfo(lbusBenPerson, lbusBenOrg);
                iclbCorPersonBeneficiary.Add(lobjPersonBeneficiary);
            }
            iclbCorPersonBeneficiary = busGlobalFunctions.Sort<busPersonBeneficiary>(
                                "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                                iclbCorPersonBeneficiary);
        }
        public Collection<busPersonDependent> LoadPersonDependets()
        {
            DataTable ldtbCorPersonDependent = Select("entPerson.LoadCorPersonDepedent", new object[1] { icdoDeathNotification.death_notification_id });
            
            Collection<busPersonDependent> lclbPersonDependent = new Collection<busPersonDependent>();
            //person account will not be loaded in the loadDependent method
            foreach (DataRow lobjPDependent in ldtbCorPersonDependent.Rows)
            {
                busPersonDependent lbusPersonDependent = new busPersonDependent() { icdoPersonDependent = new cdoPersonDependent(), ibusPeronAccountDependent = new busPersonAccountDependent() { icdoPersonAccountDependent = new cdoPersonAccountDependent() } };
                lbusPersonDependent.FindPersonDependent(Convert.ToInt32(lobjPDependent[enmCorBeneDep.person_dependent_id.ToString()]));
                lbusPersonDependent.ibusPeronAccountDependent.FindPersonAccountDependent(lbusPersonDependent.icdoPersonDependent.person_dependent_id, Convert.ToInt32(lobjPDependent[enmCorBeneDep.person_account_id.ToString()]));
                lbusPersonDependent.ibusPeronAccountDependent.LoadPersonAccount();
                lbusPersonDependent.LoadDependentInfo();
                lclbPersonDependent.Add(lbusPersonDependent);
            }
            return lclbPersonDependent;
        }
        #region Death Letter - APP-7050 
        public busDBCacheData ibusDBCacheData { get; set; }
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public DataTable idtbPALifeOptionHistory { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }
        DataTable idtbBenProvisionBenType;
        DataTable idtbBenOptionFactor;
        DataTable idtbBenefitProvisionExclusion;
        DataTable idtbAgeEstimate;
        DateTime idtLastIntertestPostDate;
        DataTable idtbDeathBenOptionCodeValue;
        DataTable idtbBenOptionCodeValue;
        private busPersonAccountRetirement SetBeneficiaryRelatedProperties(busDeathNotification lobjDeathNotification, busPersonBeneficiary abusPersonBeneficiary,
             Collection<busPersonBeneficiary> aclbPersonBeneficiary, decimal ldecBeneficiaryMonthAndYear, ref bool lblnIsPersonDepedentInVision,
             ref bool lblnIsPersonDepedentInDental, ref bool lblnIsPersonDepedentInHealth, bool ablnMultipleBeneExists)
        {
            //check if the recipient is not beneficiary in any of DB/DC plan -- PIR systest 2644
            if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsRetirementPlan()))
            {
                lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_No;

                //relationship is estate in Db / DC plan
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate)
                {
                    lobjDeathNotification.ibusPerson.istrIsEstateInDBDCPlan = busConstant.Flag_Yes;
                }
            }
            else
            {
                lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_Yes;
            }

            if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                                               i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
            {
                lobjDeathNotification.ibusPerson.istrIsMemberHadDCPlan = busConstant.Flag_Yes;
            }

            //relationship is trustee
            if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipTrustee)
            {
                lobjDeathNotification.ibusPerson.istrIsRelationshipTrustee = busConstant.Flag_Yes;
            }
            busPersonAccountRetirement lbusPersonAccountRetirement = null;
            if (lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan == busConstant.Flag_No)
            {
                //check is vested or not           
                lbusPersonAccountRetirement = CheckEmployeeVestedOrNot(lobjDeathNotification);

                //is realtionship is spouse
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse = busConstant.Flag_Yes;
                else lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse = busConstant.Flag_No;


                if (abusPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary)
                {//PIR 7940- Display only for primary beneficiary and not contingent beneficiary
                    //if multiple benes and vested
                    //or non vested
                    //or vested and non spouse
                    if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    if (ablnMultipleBeneExists && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    if ((!ablnMultipleBeneExists) && (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_No) &&
                        (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes))
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    //  vested and only spouse is beneficiary
                    if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                        && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse = busConstant.Flag_Yes;
                }
                //beneficiary enrolled in DC and relation is spouse
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                {
                    if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                                                       i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
                    {
                        lobjDeathNotification.ibusPerson.istrIsDCBeneficiarySpouse = busConstant.Flag_Yes;
                    }
                }
            }

            //Beneficiary under 18 years of age
            if (abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
            {
                if (ldecBeneficiaryMonthAndYear < 18)
                    lobjDeathNotification.ibusPerson.istrIsBeneficiaryAgeLessThan18 = busConstant.Flag_Yes;
            }

            //******************************HEALTH********************************************************//

            //check if beneficiairy is health dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
            if (lenumDependent.Count() > 0)
            {
                lblnIsPersonDepedentInHealth = true;
            }

            //deceased is not vested and enrolled in health and person is health dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                // if (lblnIsPersonDepedentInHealth) //-- systest 2637
                {
                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                        || ((lblnIsPersonDepedentInHealth)
                        && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse != busConstant.Flag_Yes)) //--systest 2644
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInHealth = busConstant.Flag_Yes;
                    }
                }
            }

            //deceased is not in health and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInGroupHealth()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInHealth = busConstant.Flag_Yes;

            //deceased has health insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumHealthDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
                if (lenumHealthDependent.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInHealth
                && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInHealth = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_Yes;
            }
            else
            {
                if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                    && !lblnIsPersonDepedentInHealth
                    && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInHealth == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInHealth == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInHealth == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisHealthVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
                || lblnIsPersonDepedentInHealth)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInHealth = busConstant.Flag_Yes;

            //******************************DENTAL********************************************************//                            
            //check if beneficiairy is Dental dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDentalDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental);
            if (lenumDentalDependent.Count() > 0)
            {

                lblnIsPersonDepedentInDental = true;
            }

            //deceased is not vested and enrolled in dental and person is dental dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                if (lblnIsPersonDepedentInDental)
                    //if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                    //&& lblnIsPersonDepedentInDental)

                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                       || (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)) //--systest 2644                
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInDental = busConstant.Flag_Yes;
                    }
            }

            //deceased is not in health and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInDental()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInDental = busConstant.Flag_Yes;

            //deceased has health insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumdentalDependents = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental);
                if (lenumdentalDependents.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInDental)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInDental = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInDentalRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInDentalRelationshipMarried == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInDental == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInDental == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInDental == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisDentalVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
              || lblnIsPersonDepedentInDental)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInDental = busConstant.Flag_Yes;

            //******************************VISION********************************************************//

            //check if beneficiairy is vision dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumVisionDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision);
            if (lenumVisionDependent.Count() > 0)
            {

                lblnIsPersonDepedentInVision = true;
            }

            //deceased is not vested and enrolled in vision and person is vision dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                if (lblnIsPersonDepedentInVision)
                    //if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                    //&& lblnIsPersonDepedentInVision)
                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                      || (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)) //--systest 2644                
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInVision = busConstant.Flag_Yes;
                    }
            }

            //deceased is not in vision and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInVision()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInVision = busConstant.Flag_Yes;

            //deceased has vision insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumdentalDependents = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision);
                if (lenumdentalDependents.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInVision)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInVision = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInVisionRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInVisionRelationshipMarried == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInVision == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInVision == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInVision == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisVisionVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
             || lblnIsPersonDepedentInVision)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInVision = busConstant.Flag_Yes;

            //******************************LIFE********************************************************//
            bool lblnIsBeneInLife = false;
            //check if beneficiairy is Life                    
            if (aclbPersonBeneficiary.Any(lobjPD => lobjPD.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife))
            {
                lblnIsBeneInLife = true;
            }

            // is bene in life
            if (lblnIsBeneInLife)
                lobjDeathNotification.ibusPerson.istrIsBeneficiaryInLife = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsBeneficiaryNotInLife = busConstant.Flag_Yes;

            //is life bene under 18
            if (lblnIsBeneInLife && ldecBeneficiaryMonthAndYear < 18)
                lobjDeathNotification.ibusPerson.istrIsLifeBeneAgeUnder18 = busConstant.Flag_Yes;

            //check if the bene is estate or trustee
            if (lblnIsBeneInLife)
            {
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipTrustee)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneTrustee = busConstant.Flag_Yes;
                //estate
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneEstate = busConstant.Flag_Yes;

                if (lobjDeathNotification.ibusPerson.istrIsLifeBeneTrustee == busConstant.Flag_Yes
                    || lobjDeathNotification.ibusPerson.istrIsLifeBeneEstate == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneTrusteeOrEstate = busConstant.Flag_Yes;
            }

            //*************************************LTC***************************************
            //checkif the LTC account exists for the deceased
            if (lobjDeathNotification.ibusPerson.istrPersonInLTC == busConstant.Flag_Yes
                && abusPersonBeneficiary.ibusBeneficiaryPerson.istrPersonInLTC != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased = busConstant.Flag_Yes;

            //checkif the LTC account exists for the spouse
            if (abusPersonBeneficiary.ibusBeneficiaryPerson.istrPersonInLTC == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.istrPersonInLTC != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForBoth = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased != busConstant.Flag_Yes
               && lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForNone = busConstant.Flag_Yes;

            //********************************************deferred comp account exists
            if (lobjDeathNotification.ibusPerson.IsPersonInDeferredComp())
                lobjDeathNotification.ibusPerson.istrIsdefCompPlanExits = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsdefCompPlanNotExits = busConstant.Flag_Yes;

            //*********************************************Other 457
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdOther457))
                lobjDeathNotification.ibusPerson.istrIsOther457PlanExits = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsOther457PlanNotExits = busConstant.Flag_Yes;

            //*********************************************Is DC Plan Exists
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC))
                lobjDeathNotification.ibusPerson.istrIsDCPlanExits = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC2020)) //PIR 20232
                lobjDeathNotification.ibusPerson.istrIsDCPlanExits = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInVisionDependentsEligible = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInDentalDependentsEligible = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInHealthDependentsEligible = busConstant.Flag_Yes;

            //DC non vested or single death 
            //OR
            //dc vested and married death
            if (lobjDeathNotification.ibusPerson.istrIsDCPlanExits == busConstant.Flag_Yes)
            {
                if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes
                    || lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                    ||
                    (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                    && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried))
                    lobjDeathNotification.ibusPerson.istrIsDCNonVestedOrSingleDeathOrDCVestedAndMarriedDeath = busConstant.Flag_Yes;
            }

            if (lbusPersonAccountRetirement == null)
            {
                lbusPersonAccountRetirement = new busPersonAccountRetirement
                {
                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement(),
                    icdoPersonAccount = new cdoPersonAccount()
                };
            }
            return lbusPersonAccountRetirement;
        }

        private busPersonAccountRetirement CheckEmployeeVestedOrNot(busDeathNotification lobjDeathNotification)
        {
            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            if (lobjDeathNotification.ibusPerson.iclbRetirementAccount == null)
                lobjDeathNotification.ibusPerson.LoadRetirementAccount();

            var lenumDBPersonAccount = lobjDeathNotification.ibusPerson.iclbRetirementAccount
                .Where(lobjRA => !lobjRA.IsWithDrawn());

            if (lenumDBPersonAccount.Count() > 0)
            {
                //Load the Account Balance                                    
                lbusPersonAccountRetirement.FindPersonAccountRetirement(lenumDBPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id);
                lbusPersonAccountRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccountRetirement.ibusPerson.icdoPerson = lobjDeathNotification.ibusPerson.icdoPerson;
                lbusPersonAccountRetirement.LoadLTDSummary();
                if (lbusPersonAccountRetirement.ibusPlan == null)
                    lbusPersonAccountRetirement.LoadPlan();
                lbusPersonAccountRetirement.LoadTotalVSC();
                lbusPersonAccountRetirement.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount = lbusPersonAccountRetirement.icdoPersonAccount;

                //calculate person age
                int lintMemberAgeInMonths = 0;
                int lintMemberAgeInYears = 0;
                decimal ldecMonthAndYear = 0m;
                int lintMonths = 0;
                busPersonBase.CalculateAge(lobjDeathNotification.ibusPerson.icdoPerson.date_of_birth,
                    lobjDeathNotification.icdoDeathNotification.date_of_death, ref lintMonths, ref ldecMonthAndYear, 2,
                    ref lintMemberAgeInYears, ref lintMemberAgeInMonths);

                if (busPersonBase.CheckIsPersonVested(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id
                    , lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_code,
                    lbusPersonAccountRetirement.ibusPlan.icdoPlan.benefit_provision_id,
                    busConstant.ApplicationBenefitTypeRetirement,
                    lbusPersonAccountRetirement.icdoPersonAccount.Total_VSC,
                    ldecMonthAndYear, lobjDeathNotification.icdoDeathNotification.date_of_death.GetFirstDayofNextMonth(), false, lobjDeathNotification.icdoDeathNotification.date_of_death, lbusPersonAccountRetirement, iobjPassInfo)) //PIR 14646 - Vesting logic changes
                {
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested = busConstant.Flag_Yes;
                }
                else
                {
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested = busConstant.Flag_Yes;
                }
            }
            return lbusPersonAccountRetirement;
        }

        public void LoadAmountFields(busPerson aobjPerson)
        {
            if (aobjPerson.icolPersonAccount.IsNull())
                aobjPerson.LoadPersonAccount();

            bool lblnErrorFound = false;
            decimal ldecMemberPremiumAmt = 0.00M;
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;

            GetHealthPremiumAmountBasedOnConditions(aobjPerson, ref lblnErrorFound, ref lintGHDVHistoryID, ref lstrGroupNumber);

            int lintPADentalID = aobjPerson.icolPersonAccount.Where(lobj =>
                                            //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                            lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDental).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();
            if (lintPADentalID > 0)
            {
                decimal ldecSinglePremium = 0M;
                decimal ldecFamilyPremium = 0M;
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPADentalID);
                lobjPA.LoadPersonAccountGHDV();

                // if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                {
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;
                    {
                        ldecSinglePremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                        aobjPerson.idecDentalSinglePolicyCOBRAPremium = ldecSinglePremium;
                    }
                    //}
                    //  if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                    //{
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageFamily;
                    {
                        ldecFamilyPremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                        aobjPerson.idecDentalFamilyPolicyCOBRAPremium = ldecFamilyPremium;
                    }
                }
            }

            int lintPAVisionID = aobjPerson.icolPersonAccount.Where(lobj =>
                                            //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                            lobj.icdoPersonAccount.plan_id == busConstant.PlanIdVision).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();
            if (lintPAVisionID > 0)
            {
                decimal ldecSinglePremium = 0M;
                decimal ldecFamilyPremium = 0M;
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPAVisionID);
                lobjPA.LoadPersonAccountGHDV();

                // if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                {

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                    ldecSinglePremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                    aobjPerson.idecVisionSinglePolicyCOBRAPremium = ldecSinglePremium;

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageFamily;
                    ldecFamilyPremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                    aobjPerson.idecVisionFamilyPolicyCOBRAPremium = ldecFamilyPremium;

                    ////if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual)
                    //{
                    //    aobjPerson.idecVisionSinglePolicyCOBRAPremium = ldecPremium;
                    //}
                    //// else if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                    //{
                    //    aobjPerson.idecVisionFamilyPolicyCOBRAPremium = ldecPremium;
                    //}
                }
            }

            busPersonAccount lbusPersonAccount = aobjPerson.icolPersonAccount.Where(lobj =>
                                           //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                           lobj.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife).FirstOrDefault();
            if ((lbusPersonAccount != null) && (lbusPersonAccount.icdoPersonAccount.person_account_id > 0))
            {
                busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                lobjPersonAccountLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id);
                lobjPersonAccountLife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;

                lobjPersonAccountLife.LoadLifeOptionData();

                //include only basic and supplemental level of coverage
                var lCoverage = lobjPersonAccountLife.iclbLifeOption.Where(lobjLife => lobjLife.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled
                    && (lobjLife.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic
                    || lobjLife.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental))
                    .Sum(lobjLife => lobjLife.icdoPersonAccountLifeOption.coverage_amount);

                aobjPerson.idecLifeInsurancePolicyValue = Convert.ToDecimal(lCoverage);
            }
        }
        private void GetHealthPremiumAmountBasedOnConditions(busPerson aobjPerson, ref bool lblnErrorFound, ref int lintGHDVHistoryID, ref string lstrGroupNumber)
        {
            int lintPAHealthID = aobjPerson.icolPersonAccount.Where(lobj =>
                                         //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                         lobj.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();

            //Loading the RHIC Upfront.. since Retiree Block also uses this bookmark
            //Load the Accured RHIC Benefit Amount
            aobjPerson.idecHealthRHIC = 0.00M;
            aobjPerson.LoadPensionSummary(); //Can be done Better later
            if (aobjPerson.iclbPensionAccounts != null && aobjPerson.iclbPensionAccounts.Count > 0)
                aobjPerson.idecHealthRHIC = aobjPerson.iclbPensionAccounts.First().idecAccruedRHICBenefit;

            if (lintPAHealthID > 0)
            {
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPAHealthID);
                lobjPA.LoadPersonAccountGHDV();

                lobjPA.ibusPersonAccountGHDV.ibusPerson = aobjPerson;
                lobjPA.ibusPersonAccountGHDV.LoadPlan();

                lobjPA.ibusPersonAccountGHDV.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;


                busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
                if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
                {
                    //idlgUpdateProcessLog(
                    //    "Error : No History Record Found for Person Account = " +
                    //    lobjPA.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                    lblnErrorFound = true;
                }
                else
                {
                    lobjPA.ibusPersonAccountGHDV = lobjPAGhdvHistory.LoadGHDVObject(lobjPA.ibusPersonAccountGHDV);

                    lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                    lstrGroupNumber = lobjPA.ibusPersonAccountGHDV.GetGroupNumber();

                    //Initialize the Org Object to Avoid the NULL error
                    lobjPA.ibusPersonAccountGHDV.InitializeObjects();
                    lobjPA.ibusPersonAccountGHDV.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType36Month;

                    lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(lobjPA.ibusPersonAccountGHDV.idtPlanEffectiveDate);

                    if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                        lobjPA.ibusPersonAccountGHDV.LoadHealthPlanOption();
                        //To Get the Rate Structure Code (Derived Field)
                        lobjPA.ibusPersonAccountGHDV.LoadRateStructure(aobjPerson.icdoPerson.date_of_death);
                    }

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";
                    aobjPerson.idecHealthSinglePolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0005";
                    aobjPerson.idecHealthFamilyPolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0026";
                    aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);
                }

                aobjPerson.idecHealthSinglePolicyCOBRANetAmt = aobjPerson.idecHealthSinglePolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
                aobjPerson.idecHealthFamilyPolicyCOBRANetAmt = aobjPerson.idecHealthFamilyPolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
                aobjPerson.idecHealthFamilyOf3PolicyCOBRANetAmt = aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
            }
        }
        private decimal GetSpousePremiumsForCOBRA(string lstrCoverageCode, busPerson aobjPerson, busPersonAccountGhdv aobjPersonAccountGHDV)
        {
            busPersonAccountGhdv lobjPAGHDV = new busPersonAccountGhdv
            {
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPAGHDV.icdoPersonAccountGhdv = aobjPersonAccountGHDV.icdoPersonAccountGhdv;
            lobjPAGHDV.icdoPersonAccount = aobjPersonAccountGHDV.icdoPersonAccount;
            lobjPAGHDV.ibusPlan = aobjPersonAccountGHDV.ibusPlan;
            lobjPAGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType36Month;
            lobjPAGHDV.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
            lobjPAGHDV.icdoPersonAccountGhdv.coverage_code = lstrCoverageCode;
            lobjPAGHDV.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;
            lobjPAGHDV.icdoPersonAccount.cobra_expiration_date = aobjPerson.icdoPerson.date_of_death.AddMonths(36);

            lobjPAGHDV.LoadCoverageRefID();
            lobjPAGHDV.GetMonthlyPremiumAmountByRefID();
            return lobjPAGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
        }
        
        private decimal GetPremiumForDentalAndVision(busPerson aobjPerson, busPersonAccount lobjPA)
        {
            busBase lbusBase = new busBase();
            bool lblnErrorFound = false;
            decimal ldecPremiumAmt = 0m;

            //lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
            lobjPA.ibusPersonAccountGHDV.ibusPerson = aobjPerson;
            lobjPA.ibusPersonAccountGHDV.LoadPlan();

            lobjPA.ibusPersonAccountGHDV.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            //Get the GHDV History Object By Billing Month Year
            busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
            if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
            {
                //idlgUpdateProcessLog(
                //    "Error : No History Record Found for Person Account = " +
                //    lobjPA.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                lblnErrorFound = true;
            }

            if (!lblnErrorFound)
            {
                if (iclbProviderOrgPlan != null)
                {
                    busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjPA.ibusPersonAccountGHDV.icdoPersonAccount.plan_id);
                    if (lbusProviderOrgPlan != null)
                    {
                        lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan = lbusProviderOrgPlan;
                    }
                    else
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(aobjPerson.icdoPerson.date_of_death);
                    }
                }
                else
                {
                    lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(aobjPerson.icdoPerson.date_of_death);
                }

                if (lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                {
                    ldecPremiumAmt =
                        busRateHelper.GetDentalPremiumAmount(
                            lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjPerson.icdoPerson.date_of_death,
                            ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                }
                else if (lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    ldecPremiumAmt =
                        busRateHelper.GetVisionPremiumAmount(
                            lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjPerson.icdoPerson.date_of_death,
                            ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);
                }
            }
            return ldecPremiumAmt;
        }
        private void GetPremiumForHealthRetiree(busPerson aobjPerson)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPersonAccountGhdv.icdoPersonAccount.plan_id = busConstant.PlanIdGroupHealth;
            lobjPersonAccountGhdv.icdoPersonAccount.person_id = aobjPerson.icdoPerson.person_id;

            lobjPersonAccountGhdv.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccountGhdv.ibusPerson = aobjPerson;

            lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
            lobjPersonAccountGhdv.icdoPersonAccount.start_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.history_change_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value = busConstant.PersonJobTypePermanent;

            //for coverage code single
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0021";
            aobjPerson.idecHealthSinglePolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);

            //for coverage code family
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0022";
            aobjPerson.idecHealthFamilyPolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);

            //for coverage code family 3+
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0023";
            aobjPerson.idecHealthFamily3PolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);
        }
        private decimal GetPremiumBasedOnCoverage(busPerson aobjPerson, busPersonAccountGhdv lobjPersonAccountGhdv)
        {
            bool lblnErrorFound = false;
            string lstrGroupNumber = string.Empty;

            lobjPersonAccountGhdv.LoadPlan();

            lobjPersonAccountGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjPersonAccountGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjPersonAccountGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjPersonAccountGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjPersonAccountGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjPersonAccountGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjPersonAccountGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;


            lstrGroupNumber = lobjPersonAccountGhdv.GetGroupNumber();

            //Initialize the Org Object to Avoid the NULL error
            lobjPersonAccountGhdv.InitializeObjects();
            lobjPersonAccountGhdv.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;

            lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(lobjPersonAccountGhdv.idtPlanEffectiveDate);

            if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
            {
                lobjPersonAccountGhdv.LoadRateStructureForUserStructureCode();
            }
            else
            {
                lobjPersonAccountGhdv.LoadHealthParticipationDate();
                lobjPersonAccountGhdv.LoadHealthPlanOption();
                //To Get the Rate Structure Code (Derived Field)
                lobjPersonAccountGhdv.LoadRateStructure(aobjPerson.icdoPerson.date_of_death);
            }

            //Get the Coverage Ref ID
            lobjPersonAccountGhdv.LoadCoverageRefID();

            //Get the Premium Amount
            lobjPersonAccountGhdv.GetMonthlyPremiumAmountByRefID(aobjPerson.icdoPerson.date_of_death);

            if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID == 0)
            {
                //idlgUpdateProcessLog(
                //    "Error : Invalid Coverage Ref ID for Person Account = " +
                //    lobjPersonAccountGhdv.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                lblnErrorFound = true;
            }
            if (!lblnErrorFound)
            {
                return lobjPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            return 0.00M;
        }
        private void GetPremiumForDentalVisionRetiree(busPerson aobjPerson, int aintPlanId)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPersonAccountGhdv.icdoPersonAccount.plan_id = aintPlanId;
            lobjPersonAccountGhdv.icdoPersonAccount.person_id = aobjPerson.icdoPerson.person_id;

            lobjPersonAccountGhdv.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccountGhdv.ibusPerson = aobjPerson;

            if (aintPlanId == busConstant.PlanIdDental)
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
            if (aintPlanId == busConstant.PlanIdVision)
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
            lobjPersonAccountGhdv.icdoPersonAccount.start_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.history_change_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value = busConstant.PersonJobTypePermanent;

            //load the org plan provider         
            {
                lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(lobjPersonAccountGhdv.icdoPersonAccount.current_plan_start_date_no_null);
            }

            if (aintPlanId == busConstant.PlanIdDental)
            {
                //for single dental LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;

                aobjPerson.idecDentalSinglePolicyPremium =
                    busRateHelper.GetDentalPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                //for family dental LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageFamily;

                aobjPerson.idecDentalFamilyPolicyPremium =
                    busRateHelper.GetDentalPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);
            }
            else if (aintPlanId == busConstant.PlanIdVision)
            {
                //for single vision LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                aobjPerson.idecVisionSinglePolicyPremium =
                     busRateHelper.GetVisionPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                         aobjPerson.icdoPerson.date_of_death,
                         ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);

                //for family vision LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageFamily;
                aobjPerson.idecVisionFamilyPolicyPremium =
                    busRateHelper.GetVisionPremiumAmount(
                        lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);
            }
        }
        //get the benefit options for deceased
        private void SetBenefitOptionAmount(busPerson aobjPerson, busPlan aobjPlan, busPersonAccount aobjPersonAccount)
        {
            //check for this plan whether 50 percent or 100 % JS is available

            Collection<cdoCodeValue> lclbbenefitOptions = busPersonBase.LoadBenefitOptionsBasedOnPlans(aobjPlan.icdoPlan.plan_id, busConstant.ApplicationBenefitTypePreRetirementDeath);

            var lGetRequired50BenefitOption = lclbbenefitOptions.Where(lCode => lCode.code_value == busConstant.BenefitOption50Percent);
            var lGetRequired100JSBenefitOption = lclbbenefitOptions.Where(lCode => lCode.code_value == busConstant.BenefitOption100PercentJS);
            if (lGetRequired50BenefitOption.Count() > 0
                && lGetRequired100JSBenefitOption.Count() > 0)
            {

                busPreRetirementDeathBenefitCalculation lobjPreRetirementDeath = new busPreRetirementDeathBenefitCalculation
                {
                    icdoBenefitCalculation = new cdoBenefitCalculation(),
                    ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                };
                lobjPreRetirementDeath.ibusMember = aobjPerson;
                lobjPreRetirementDeath.ibusPlan = aobjPlan;
                lobjPreRetirementDeath.ibusPersonAccount = aobjPersonAccount;
                lobjPreRetirementDeath.ibusPersonAccount.ibusPerson = aobjPerson;
                lobjPreRetirementDeath.ibusPersonAccount.ibusPlan = aobjPlan;
                lobjPreRetirementDeath.idtbBenOptionFactor = idtbBenOptionFactor;
                lobjPreRetirementDeath.idtbBenefitProvisionExclusion = idtbBenefitProvisionExclusion;
                lobjPreRetirementDeath.iblnUseDataTableForBenOptionFactor = true;
                lobjPreRetirementDeath.idtbBenOptionCodeValue = idtbBenOptionCodeValue;
                lobjPreRetirementDeath.idtbDeathBenOptionCodeValue = idtbDeathBenOptionCodeValue;
                lobjPreRetirementDeath.idtLastIntertestPostDate = idtLastIntertestPostDate;
                lobjPreRetirementDeath.LoadLastContributedDate();
                lobjPreRetirementDeath.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypePreRetirementDeath;
                lobjPreRetirementDeath.icdoBenefitCalculation.person_id = lobjPreRetirementDeath.ibusMember.icdoPerson.person_id;
                lobjPreRetirementDeath.icdoBenefitCalculation.plan_id = lobjPreRetirementDeath.ibusPlan.icdoPlan.plan_id;
                lobjPreRetirementDeath.icdoBenefitCalculation.created_date = DateTime.Now;
                lobjPreRetirementDeath.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal; // No Projections
                //lobjRetirementCalculation.iblnIncludeJobServiceSickLeave = true;
                lobjPreRetirementDeath.icdoBenefitCalculation.termination_date = aobjPerson.icdoPerson.date_of_death;
                lobjPreRetirementDeath.icdoBenefitCalculation.date_of_death = aobjPerson.icdoPerson.date_of_death;
                lobjPreRetirementDeath.icdoBenefitCalculation.retirement_date = aobjPerson.icdoPerson.date_of_death.AddMonths(1);
                lobjPreRetirementDeath.iblnIsFromEmployeeDeathBatch = true;
                lobjPreRetirementDeath.icdoBenefitCalculation.benefit_option_value = busConstant.BenefitOption50Percent;
                //lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                //lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                lobjPreRetirementDeath.SetNormalRetirementDate(ibusDBCacheData);
                lobjPreRetirementDeath.GetNormalEligibilityDetails(ibusDBCacheData);
                lobjPreRetirementDeath.LoadBenefitCalculationPayeeForNewMode();
                lobjPreRetirementDeath.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
                lobjPreRetirementDeath.CalculatePreRetirementDeathBenefit();

                //get benefit option amount for 50 JS
                decimal ldec50PercentBenefitAmt = 0.00M;
                decimal ldec100JSPercentBenefitAmt = 0.00M;
                var lBenefit50PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                    == busConstant.BenefitOption50Percent);
                var lBenefit100PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                   == busConstant.BenefitOption100PercentJS);
                if (lBenefit50PercentOPtionAmount.Count() > 0)
                    ldec50PercentBenefitAmt = lBenefit50PercentOPtionAmount.FirstOrDefault().icdoBenefitCalculationOptions.benefit_option_amount;

                //lobjPreRetirementDeath.icdoBenefitCalculation.benefit_option_value = busConstant.BenefitOption100PercentJS;
                ////lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                ////lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                //lobjPreRetirementDeath.SetNormalRetirementDate(ibusDBCacheData);
                //lobjPreRetirementDeath.GetNormalEligibilityDetails(ibusDBCacheData);
                //lobjPreRetirementDeath.LoadBenefitCalculationPayeeForNewMode();
                //lobjPreRetirementDeath.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
                //lobjPreRetirementDeath.CalculatePreRetirementDeathBenefit();

                //get benefit option amount for 100 JS
                //var lBenefit100PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                //    == busConstant.BenefitOption100PercentJS);

                if (lBenefit100PercentOPtionAmount.Count() > 0)
                    ldec100JSPercentBenefitAmt = lBenefit100PercentOPtionAmount.FirstOrDefault().icdoBenefitCalculationOptions.benefit_option_amount;

                aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = ldec100JSPercentBenefitAmt;
                if (ldec100JSPercentBenefitAmt < ldec50PercentBenefitAmt)
                    aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = ldec50PercentBenefitAmt;
            }
        }
        public void LoadAllCacheDataAndOtherTableData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            LoadDBCacheData();
            LoadInitialData();
            idtbBenProvisionBenType = busBase.Select("cdoBenefitProvisionBenefitType.GetAllBenefitProvision", new object[0] { });
            ibusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);

            idtbBenOptionFactor = busBase.Select<cdoBenefitOptionFactor>(
                           new string[1] { "benefit_type" },
                           new object[1] { "DETH" }, null, null);

            idtbBenefitProvisionExclusion = busBase.Select<cdoBenefitProvisionExclusion>(
                              new string[0] { },
                              new object[0] { }, null, null);

            idtbAgeEstimate = busBase.Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 1311 }, null, null);
            idtLastIntertestPostDate = busInterestCalculationHelper.GetInterestBatchLastRunDate();
            idtbDeathBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2406);
            idtbBenOptionCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1903);
        }
        public void LoadDBCacheData()
        {
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
        }
        private void LoadInitialData()
        {
            //idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            LoadActiveProviders(DateTime.Now);
        }
        public void LoadActiveProviders(DateTime adtEffectiveChangeDate)
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllActiveProviders", new object[1] { adtEffectiveChangeDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }
        public void LoadBeneficiaryAndCorPropertiesData(busDeathNotification lobjDeathNotification, busPersonBeneficiary lobjPersonBeneficiary)
        {
            //this is used in correspondence generation
            lobjDeathNotification.ibusPersonBeneficiary = lobjPersonBeneficiary;
            lobjDeathNotification.ibusPersonBeneficiary.ibusBeneficiaryPerson = lobjPersonBeneficiary.ibusBeneficiaryPerson;
            lobjDeathNotification.istrIsFromEmployeeDeathBatch = busConstant.Flag_Yes;

            var lenuTempBeneficiary = lobjDeathNotification.ibusPerson.iclbPersonBeneficiary
                                                          .Where(i => i.icdoPersonBeneficiary.beneficiary_person_id == lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                                                          && busGlobalFunctions.CheckDateOverlapping(lobjDeathNotification.ibusPerson.icdoPerson.date_of_death,
                                                          i.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                          i.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date));

            if (lenuTempBeneficiary.Count() > 0)
            {
                var lclbTempBeneficiary = lenuTempBeneficiary.ToList().ToCollection();
                //calculate beneficiary agec    
                int lintBeneficiaryAgeInMonths = 0;
                int lintBeneficiaryAgeInYears = 0;
                decimal ldecBeneficiaryMonthAndYear = 0m;
                int lintBeneficiaryMonths = 0;

                bool lblnIsPersonDepedentInVision = false;
                bool lblnIsPersonDepedentInDental = false;
                bool lblnIsPersonDepedentInHealth = false;

                lobjDeathNotification.ibusPerson.istrRelationshipToBeneficiary = lobjPersonBeneficiary.icdoPersonBeneficiary.relationship_description;

                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                {
                    busPersonBase.CalculateAge(lobjPersonBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_birth,
                        lobjDeathNotification.icdoDeathNotification.date_of_death, ref lintBeneficiaryMonths, ref ldecBeneficiaryMonthAndYear, 2,
                        ref lintBeneficiaryAgeInYears, ref lintBeneficiaryAgeInMonths);
                }

                foreach (busPersonBeneficiary lbusPersonBeneficiary in lenuTempBeneficiary)
                {
                    if (lbusPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                        lbusPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();

                    if (lbusPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull())
                        lbusPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();
                }

                bool lblnMultipleBeneExists = false;
                //check if multiple bene exists for retirement plan
                if (lenuTempBeneficiary.Where(lobjPB => lobjPB.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDBRetirementPlan()).Count() > 1)
                    lblnMultipleBeneExists = true;
                busPersonAccountRetirement lbusPersonAccountRetirement = SetBeneficiaryRelatedProperties(lobjDeathNotification, lobjPersonBeneficiary,
                     lclbTempBeneficiary, ldecBeneficiaryMonthAndYear, ref lblnIsPersonDepedentInVision,
                     ref lblnIsPersonDepedentInDental, ref lblnIsPersonDepedentInHealth, lblnMultipleBeneExists);

                busPersonBeneficiary lbusRetirementBeneficiary = lenuTempBeneficiary.Where(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsRetirementPlan()).FirstOrDefault();
                if (lbusRetirementBeneficiary == null)
                {
                    lbusRetirementBeneficiary = new busPersonBeneficiary
                    {
                        icdoPersonBeneficiary = new cdoPersonBeneficiary(),
                        ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary { icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary() }
                    };
                }

                lobjDeathNotification.ibusPerson.idec20PercentOfTaxableAmount = 0.00M;
                lobjDeathNotification.ibusPerson.idecDeceasedMemberBalanceAmount = lbusPersonAccountRetirement.Member_Account_Balance_ltd;
                lobjDeathNotification.ibusPerson.idecTaxableMemberBalanceAmount = Math.Round((lbusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd * lbusRetirementBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent) / 100, 2, MidpointRounding.AwayFromZero);
                lobjDeathNotification.ibusPerson.idecNonTaxableMemberBalanceAmount = Math.Round((lbusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd * lbusRetirementBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent) / 100, 2, MidpointRounding.AwayFromZero);
                lobjDeathNotification.ibusPerson.idec20PercentOfTaxableAmount = Math.Round((20 * lobjDeathNotification.ibusPerson.idecTaxableMemberBalanceAmount) / 100, 2, MidpointRounding.AwayFromZero);
                lobjDeathNotification.ibusPerson.idecBeneRetPercentage = Math.Round(lbusRetirementBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent, 2); // PROD PIR 7067

                // Get Amount Fields
                LoadAmountFields(lobjDeathNotification.ibusPerson);

                //load the retiree premium amounts for health plan
                GetPremiumForHealthRetiree(lobjDeathNotification.ibusPerson);

                lobjDeathNotification.ibusPerson.idecHealthSinglePolicyNetAmount =
                    lobjDeathNotification.ibusPerson.idecHealthSinglePolicyPremium - lobjDeathNotification.ibusPerson.idecHealthRHIC;
                lobjDeathNotification.ibusPerson.idecHealthFamilyPolicyNetAmount =
                    lobjDeathNotification.ibusPerson.idecHealthFamilyPolicyPremium - lobjDeathNotification.ibusPerson.idecHealthRHIC;
                lobjDeathNotification.ibusPerson.idecHealthFamilyOf3PolicyNetAmount =
                    lobjDeathNotification.ibusPerson.idecHealthFamily3PolicyPremium - lobjDeathNotification.ibusPerson.idecHealthRHIC;

                //if (lobjDeathNotification.ibusPerson.IsPersonInDental())
                //load the retiree premium amounts for vision plan
                GetPremiumForDentalVisionRetiree(lobjDeathNotification.ibusPerson, busConstant.PlanIdDental);

                //if (lobjDeathNotification.ibusPerson.IsPersonInVision())
                //load the retiree premium amounts for dental plan
                GetPremiumForDentalVisionRetiree(lobjDeathNotification.ibusPerson, busConstant.PlanIdVision);

                if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id > 0)
                {
                    //get amount for 50 & 100 JS percent
                    SetBenefitOptionAmount(lobjDeathNotification.ibusPerson, lbusPersonAccountRetirement.ibusPlan, lbusPersonAccountRetirement.ibusPersonAccount);
                }

                if (lenuTempBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife))
                {
                    busPersonBeneficiary lbusLifePersonBeneficiary = lenuTempBeneficiary.First(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife);
                    lobjDeathNotification.ibusPerson.idecBeneLifePercentage =
                        Math.Round(lbusLifePersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent, 2, MidpointRounding.ToEven);
                }


                //PIR 7940- To set the visibility of the Retirement Account heading in APP-7050
                if (lobjDeathNotification.ibusPerson.istrIsBeneficiaryAgeLessThan18 == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsDCBeneficiarySpouse == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsMemberHadDCPlan == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneTrustee == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsEstateInDBDCPlan == busConstant.Flag_Yes ||
                    lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan == busConstant.Flag_Yes)
                    lobjDeathNotification.istrRetirementAccountVisibility = "Y";
                GenerateCorrespondence(lobjDeathNotification, "APP-7050");
            }
            RefreshCorrespondencePropertyValues(lobjDeathNotification.ibusPerson);
        }
        private void RefreshCorrespondencePropertyValues(busPerson aobjPerson)
        {
            aobjPerson.istrRelationshipToBeneficiary = busConstant.Flag_No;
            aobjPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_No;
            aobjPerson.istrIsEstateInDBDCPlan = busConstant.Flag_No;
            aobjPerson.istrIsRelationshipTrustee = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEmployeeVested = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEmployeeNonVested = busConstant.Flag_No;
            aobjPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_No;
            aobjPerson.istrIsRelationshipSpouse = busConstant.Flag_No;
            aobjPerson.istrIsVestedRelationshipSpouse = busConstant.Flag_No;
            aobjPerson.istrIsBeneficiaryAgeLessThan18 = busConstant.Flag_No;
            aobjPerson.istrIsDCBeneficiarySpouse = busConstant.Flag_No;
            aobjPerson.istrIsMemberHadDCPlan = busConstant.Flag_No;
            //health
            aobjPerson.istrIsBeneficiaryDependentInHealth = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInHealth = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInHealthNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInHealth = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_No;

            //dental
            aobjPerson.istrIsBeneficiaryDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInDental = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInDentalNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInDentalRelationshipMarried = busConstant.Flag_No;

            //Vision
            aobjPerson.istrIsBeneficiaryDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInVision = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInVisionNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInVisionRelationshipMarried = busConstant.Flag_No;

            //Life
            aobjPerson.istrIsBeneficiaryInLife = busConstant.Flag_No;
            aobjPerson.istrIsBeneficiaryNotInLife = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneAgeUnder18 = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneTrusteeOrEstate = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneTrustee = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneEstate = busConstant.Flag_No;

            //LTC
            aobjPerson.istrIsLTCExtistsForBoth = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForNone = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForDeceased = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForSpouse = busConstant.Flag_No;

            //Def comp  
            aobjPerson.istrIsdefCompPlanExits = busConstant.Flag_No;
            aobjPerson.istrIsdefCompPlanNotExits = busConstant.Flag_No;

            //Other 457 
            aobjPerson.istrIsOther457PlanExits = busConstant.Flag_No;
            aobjPerson.istrIsOther457PlanNotExits = busConstant.Flag_No;

            //DC
            aobjPerson.istrIsDCPlanExits = busConstant.Flag_No;

            aobjPerson.istrIsMemberInHealthDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsMemberInDentalDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsMemberInVisionDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsDCNonVestedOrSingleDeathOrDCVestedAndMarriedDeath = busConstant.Flag_No;

            //Amounts
            aobjPerson.idecDeceasedMemberBalanceAmount = 0.00M;
            aobjPerson.idecTaxableMemberBalanceAmount = 0.00M;
            aobjPerson.idecNonTaxableMemberBalanceAmount = 0.00M;
            aobjPerson.idec20PercentOfTaxableAmount = 0.00M;
            aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = 0.00M;

            aobjPerson.idecHealthSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecHealthFamilyPolicyCOBRAPremium = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium = 0.00M;

            aobjPerson.idecHealthSinglePolicyCOBRANetAmt = 0.00M;

            aobjPerson.idecHealthFamilyPolicyCOBRANetAmt = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyCOBRANetAmt = 0.00M;

            aobjPerson.idecHealthRHIC = 0.00M;
            aobjPerson.idecHealthSinglePolicyNetAmount = 0.00M;
            aobjPerson.idecHealthFamilyPolicyNetAmount = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyNetAmount = 0.00M;

            aobjPerson.idecHealthSinglePolicyPremium = 0.00M;
            aobjPerson.idecHealthFamilyPolicyPremium = 0.00M;
            aobjPerson.idecHealthFamily3PolicyPremium = 0.00M;

            aobjPerson.idecDentalSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecDentalFamilyPolicyCOBRAPremium = 0.00M;

            aobjPerson.idecDentalSinglePolicyPremium = 0.00M;
            aobjPerson.idecDentalFamilyPolicyPremium = 0.00M;

            aobjPerson.idecVisionSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecVisionFamilyPolicyCOBRAPremium = 0.00M;

            aobjPerson.idecVisionSinglePolicyPremium = 0.00M;
            aobjPerson.idecVisionFamilyPolicyPremium = 0.00M;
            //public decimal idecSinglePolicyPremium { get; set; }
            //public decimal idecFamilyPolicyPremium { get; set; }
            aobjPerson.idecLifeInsurancePolicyValue = 0.00M;
            aobjPerson.idecFamilyOfThreePolicyPremium = 0.00M;

            aobjPerson.istrisHealthVisible = busConstant.Flag_No;
            aobjPerson.istrisDentalVisible = busConstant.Flag_No;
            aobjPerson.istrisVisionVisible = busConstant.Flag_No;

            aobjPerson.istrIsSpouseOrDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsSpouseOrDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsSpouseOrDependentInHealth = busConstant.Flag_No;

        }
        public ArrayList GenerateDeathCorrespondences()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (alReturn.Count == 0)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (icdoDeathNotification.non_employee_death_batch_flag != busConstant.Flag_Yes && 
                    !(icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusErroneous 
                    || icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusCancelled))
                {
                    if (ibusPerson.iclbPersonAccountBeneficiary == null)
                        LoadPersonAccountBeneficiary();
                    LoadPersonAccountDependent();

                    Collection<busPerson> lobjDummyPerson = new Collection<busPerson>();

                    foreach (busPersonAccountDependent lobjPersonAccountDependent in ibusPerson.iclbPersonAccountDependent)
                    {
                        SetPADependentCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountDependent);
                    }
                    foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in ibusPerson.iclbPersonAccountBeneficiary)
                    {
                        SetPABeneficiaryCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountBeneficiary);
                    }
                    icdoDeathNotification.non_employee_death_batch_flag = busConstant.Flag_Yes;
                }
                if (icdoDeathNotification.employee_death_batch_letter_sent != busConstant.Flag_Yes)
                {
                    DataTable ldtbGetEmployeeDeathRecords = busBase.Select("entBenefitApplication.EmployeeDeathBatchFromScreen", new object[1] { icdoDeathNotification.person_id });
                    if (ldtbGetEmployeeDeathRecords.Rows.Count > 0)
                    {
                        LoadAllCacheDataAndOtherTableData();
                        if (ibusPerson.iclbPersonBeneficiary.IsNull())
                            ibusPerson.LoadBeneficiary();
                        Collection<busPersonBeneficiary> icolPersonBeneficiary = new Collection<busPersonBeneficiary>();
                        foreach (busPersonBeneficiary lobj in ibusPerson.iclbPersonBeneficiary)
                        {
                            if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull()) lobj.ibusPersonAccountBeneficiary.LoadPersonAccount();
                            if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull()) lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();

                            if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDBRetirementPlan())
                                icolPersonBeneficiary.Add(lobj);
                        }

                        var lenuDistinctBeneficiary = icolPersonBeneficiary.GroupBy(i => i.icdoPersonBeneficiary.beneficiary_person_id).Select(i => i.First());
                        if (ibusPerson.IsNull())
                            LoadPerson();
                        foreach (busPersonBeneficiary lobjPersonBeneficiary in lenuDistinctBeneficiary)
                        {
                            LoadBeneficiaryAndCorPropertiesData(this, lobjPersonBeneficiary);
                        }
                    }
                    icdoDeathNotification.employee_death_batch_letter_sent = busConstant.Flag_Yes;

                }
                if (icdoDeathNotification.is_payee_death_letter_sent_flag != busConstant.Flag_Yes)
                {
                    Collection<busPersonBeneficiary> lclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
                    Collection<busDeathNotification> lclbDeathNotification = new Collection<busDeathNotification>();
                    DataTable ldtbPayeeDeathACOD = busBase.Select("cdoDeathNotification.PayeeDeathLetterACODFromScreen", new object[1] { icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathACOD.Rows.Count > 0)
                    {
                        CreateLetter(ldtbPayeeDeathACOD, busConstant.PostRetirementAccountOwnerDeath, ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathFBED = busBase.Select("cdoDeathNotification.PayeeDeathLetterFBEDFromScreen", new object[1] { icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathFBED.Rows.Count > 0)
                    {
                        CreateLetter(ldtbPayeeDeathFBED, string.Empty, ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathALPD = busBase.Select("cdoDeathNotification.PayeeDeathLetterALPDFromScreen", new object[1] { icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathALPD.Rows.Count > 0)
                    {
                        CreateLetter(ldtbPayeeDeathALPD, string.Empty, ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathInsurance = busBase.Select("cdoDeathNotification.PayeeDeathLetterInsuranceFromScreen", new object[1] { icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathInsurance.Rows.Count > 0)
                    {
                        CreateLetter(ldtbPayeeDeathInsurance, "INSU", ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }
                    if (lclbDeathNotification.Count > 0)
                    {
                        foreach (busDeathNotification lobjDeathNotification in lclbDeathNotification)
                        {
                            lobjDeathNotification.icdoDeathNotification.is_payee_death_letter_sent_flag = busConstant.Flag_Yes;
                            lobjDeathNotification.icdoDeathNotification.Update();
                        }
                    }
                }
                icdoDeathNotification.Update();
                // Refresh the cdo - avoid update_seq issue
                icdoDeathNotification.Select();
                LoadCorTracking();
                alReturn.Add(this);
                return alReturn;
            }
            else
                return alReturn;
        }

        public Collection<busCorTracking> LoadCorTracking()
        {
            iclbAllGeneratedCorByPerson = new Collection<busCorTracking>();
            DataTable ldtbList = busNeoSpinBase.Select("entCorTracking.LoadCorTrackByPerson", new object[1] { icdoDeathNotification.person_id });
            foreach (DataRow ldtrResult in ldtbList.Rows)
            {
                busCorTracking ibusCorTracking = new busCorTracking() { icdoCorTracking = new cdoCorTracking() };
                ibusCorTracking.icdoCorTracking.LoadData(ldtrResult);
                ibusCorTracking.ibusCorTemplates = new busCorTemplates() { icdoCorTemplates = new cdoCorTemplates() };
                ibusCorTracking.ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);
                ibusCorTracking.ibusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                ibusCorTracking.ibusOrganization.icdoOrganization.LoadData(ldtrResult);
                ibusCorTracking.ibusOrgContact = new busOrgContact() { icdoOrgContact = new cdoOrgContact() };
                ibusCorTracking.ibusOrgContact.icdoOrgContact.LoadData(ldtrResult);
                ibusCorTracking.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                ibusCorTracking.ibusPerson.icdoPerson.LoadData(ldtrResult);
                ibusCorTracking.ibusPlan = new busPlan() { icdoPlan = new cdoPlan() };
                ibusCorTracking.ibusPlan.icdoPlan.LoadData(ldtrResult);
                iclbAllGeneratedCorByPerson.Add(ibusCorTracking);
            }
            return iclbAllGeneratedCorByPerson;
        }
        #endregion

    }
}