#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Linq;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvOrganization : srvNeoSpin
    {
        public srvOrganization()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private int iintOrgID;
        private int iintContactID;
        private bool iblnIsPAG;


        private void SetWebParameters()
        {
            if (iobjPassInfo.idictParams.ContainsKey("ContactID"))
                iintContactID = (int)iobjPassInfo.idictParams["ContactID"];

            if (iobjPassInfo.idictParams.ContainsKey("OrgID"))
                iintOrgID = (int)iobjPassInfo.idictParams["OrgID"];

            if (iobjPassInfo.idictParams.ContainsKey("IsPAG"))
                iblnIsPAG = (bool)iobjPassInfo.idictParams["IsPAG"];
        }

        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmOrganizationLookup")
                {
                    busOrganizationLookup lbusOrganization = new busOrganizationLookup();
                    larrErrors = lbusOrganization.ValidateNew(ahstParam);
                }
            }
            finally
            {
               // iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }

        public busOrganization NewOrganization(string astrOrgTypeValue, string astrEmployerTypeValue)
        {
            busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjOrganization.icdoOrganization.org_type_value = astrOrgTypeValue;
            lobjOrganization.icdoOrganization.emp_category_value = astrEmployerTypeValue;
            lobjOrganization.icdoOrganization.org_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(201, astrOrgTypeValue);
            lobjOrganization.icdoOrganization.emp_category_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(203, astrEmployerTypeValue);
            if (astrOrgTypeValue == busConstant.OrganizationTypeEmployer &&
                astrEmployerTypeValue == busConstant.EmployerCategoryState)
                lobjOrganization.icdoOrganization.org_code = lobjOrganization.GetNewOrgCodeRangeID();
            lobjOrganization.EvaluateInitialLoadRules();
            return lobjOrganization;
        }

        public busOrganizationRelease1 NewOrganizationRelease1()
        {
            busOrganizationRelease1 lobjOrganization = new busOrganizationRelease1();
            lobjOrganization.icdoOrganization = new cdoOrganization();
            return lobjOrganization;
        }
        public busOrganizationRelease1 FindOrganizationRelease1(int Aintorgid)
        {
            busOrganizationRelease1 lobjOrganization = new busOrganizationRelease1();
            if (lobjOrganization.FindOrganization(Aintorgid))
            {
                lobjOrganization.LoadOrgContact();
                lobjOrganization.LoadContactsAppointments();
                lobjOrganization.LoadSeminarAttendance();
            }
            return lobjOrganization;
        }

        //pir 7183
        public busOrganization FindOrgContactAppointment(int Aintorgid)
        {
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(Aintorgid))
            {
                lobjOrganization.LoadContactsAppointments();
            }
            return lobjOrganization;
        }

        //pir 7183
        public busOrganization FindWorkFlowProcessHistory(int Aintorgid)
        {
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(Aintorgid))
            {
                lobjOrganization.LoadWorkflowProcessHistory();
            }
            return lobjOrganization;
        }

        public busOrganization FindOrganization(int Aintorgid)
        {
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(Aintorgid))
            {
                lobjOrganization.LoadOrgAddresses();
                lobjOrganization.LoadOrgPrimaryAddress();
                lobjOrganization.LoadOrgContact();
                if (lobjOrganization.iclbOrgContact?.Count > 0)
                {
                    lobjOrganization.iclbOrgContact = lobjOrganization.iclbOrgContact.OrderBy(oc => oc.icdoOrgContact.status_value).ThenBy(oc => oc.ibusContact.icdoContact.first_name).ToList().ToCollection();
                }
                lobjOrganization.LoadOrgBank();
                lobjOrganization.LoadOrgPlan();
                //lobjOrganization.LoadContactsAppointments(); --- 7183
                lobjOrganization.LoadSeminarAttendance();
                lobjOrganization.LoadNotes();
                //lobjOrganization.LoadWorkflowProcessHistory(); --- 7183
                if (lobjOrganization.icdoOrganization.org_type_value == busConstant.OrganizationTypeVendor)
                    lobjOrganization.LoadOrgDeductionTypeByOrgID();
                lobjOrganization.LoadPaymentDetails();
                lobjOrganization.LoadActiveOrgContactsLOB();
                lobjOrganization.LoadInactiveOrgContactsLOB();
            }
            return lobjOrganization;
        }
        public busOrgContactAddress NewOrgAddress(int AintOrgid)
        {
            busOrgContactAddress lobjOrgContactAddress = new busOrgContactAddress();
            lobjOrgContactAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            lobjOrgContactAddress.icdoOrgContactAddress.org_id = AintOrgid;
            lobjOrgContactAddress.LoadOrganization();
            lobjOrgContactAddress.LoadOtherOrgAddress();
            return lobjOrgContactAddress;
        }
        public busOrgContactAddress FindOrgAddress(int AintOrgContactAddressid)
        {
            busOrgContactAddress lobjOrgContactAddress = new busOrgContactAddress();
            if (lobjOrgContactAddress.FindOrgContactAddress(AintOrgContactAddressid))
            {
                lobjOrgContactAddress.LoadOrganization();
                if (lobjOrgContactAddress.ibusOrganization.icdoOrganization.primary_address_id == AintOrgContactAddressid)
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressYes;
                else
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressNo;
                lobjOrgContactAddress.LoadOtherOrgAddress();
                lobjOrgContactAddress.LoadCounty();
            }
            return lobjOrgContactAddress;
        }
        public busOrgContactAddress NewContactAddress(int AintContactid)
        {
            busOrgContactAddress lobjOrgContactAddress = new busOrgContactAddress();
            lobjOrgContactAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            lobjOrgContactAddress.icdoOrgContactAddress.contact_id = AintContactid;
            lobjOrgContactAddress.LoadContact();
            lobjOrgContactAddress.LoadOtherContactAddress();
            return lobjOrgContactAddress;
        }
        public busOrgContactAddress FindContactAddress(int AintContactAddressid)
        {
            busOrgContactAddress lobjOrgContactAddress = new busOrgContactAddress();
            if (lobjOrgContactAddress.FindOrgContactAddress(AintContactAddressid))
            {
                lobjOrgContactAddress.LoadContact();
                if (lobjOrgContactAddress.ibusContact.icdoContact.primary_address_id == AintContactAddressid)
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressYes;
                else
                    lobjOrgContactAddress.primary_address_flag = busConstant.PrimaryAddressNo;

                lobjOrgContactAddress.LoadOtherContactAddress();
                lobjOrgContactAddress.LoadCounty();
            }
            return lobjOrgContactAddress;
        }
        public busOrganizationLookup LoadOrganizations(DataTable adtbSearchResult)
        {
            busOrganizationLookup lobjOrganizationLookup = new busOrganizationLookup();
            lobjOrganizationLookup.LoadOrganization(adtbSearchResult);
            return lobjOrganizationLookup;
        }

        public busPlanLookup LoadPlans(DataTable adtbSearchResult)
        {
            busPlanLookup lobjPlanLookup = new busPlanLookup();
            lobjPlanLookup.LoadPlan(adtbSearchResult);
            return lobjPlanLookup;
        }
        public busPlan NewPlan()
        {
            busPlan lobjPlan = new busPlan();
            lobjPlan.icdoPlan = new cdoPlan();
            return lobjPlan;
        }
        public busPlan FindPlan(int Aintplanid)
        {
            busPlan lobjPlan = new busPlan();
            if (lobjPlan.FindPlan(Aintplanid))
            {
                lobjPlan.LoadRetirementRates();
            }

            return lobjPlan;
        }
        public busOrgContact NewOrgContact(int AintOrgid)
        {
            busOrgContact lobjOrgContact = new busOrgContact();
            lobjOrgContact.icdoOrgContact = new cdoOrgContact();
            lobjOrgContact.icdoOrgContact.org_id = AintOrgid;
            lobjOrgContact.iclbOrgContactRole = new utlCollection<cdoOrgContactRole>();
            lobjOrgContact.LoadOrganization();
            //lobjOrgContact.LoadOtherOrgContacts();
            lobjOrgContact.LoadCodeValuePlanAndRole();
            lobjOrgContact.EvaluateInitialLoadRules();
            return lobjOrgContact;
        }
        public busOrgContact FindOrgContact(int AintOrgContactid)
        {
            busOrgContact lobjOrgContact = new busOrgContact();
            lobjOrgContact.iclbOrgContactRole = new utlCollection<cdoOrgContactRole>();
            if (lobjOrgContact.FindOrgContact(AintOrgContactid))
            {
                lobjOrgContact.LoadOrganization();
                lobjOrgContact.LoadContact();
                lobjOrgContact.LoadOrgAndContactAddresses();
                //lobjOrgContact.LoadOtherOrgContacts();
                lobjOrgContact.setConsolidatStatusValue();
                lobjOrgContact.LoadCodeValuePlanAndRole();
                lobjOrgContact.LoadContactTypes();
                lobjOrgContact.LoadScreenNotes(busConstant.OrgContactMaintenanceScreenDifferentiator, lobjOrgContact.icdoOrgContact.contact_id);
                lobjOrgContact.LoadOrgContactGroupByRoles();
                lobjOrgContact.IsContactAffiliatedWithNoOrg();
                lobjOrgContact.GetDisctinctContactRolesFromContactID();
            }
            return lobjOrgContact;
        }
        public busOrgContactLookup LoadOrgContacts(DataTable adtbSearchResult)
        {
            busOrgContactLookup lobjOrgContactLookup = new busOrgContactLookup();
            lobjOrgContactLookup.LoadOrgContact(adtbSearchResult);
            return lobjOrgContactLookup;
        }
        public busOrgBank NewOrgBank(int AintOrgid)
        {
            busOrgBank lobjOrgBank = new busOrgBank();
            lobjOrgBank.icdoOrgBank = new cdoOrgBank();
            lobjOrgBank.icdoOrgBank.org_id = AintOrgid;
            lobjOrgBank.LoadOrganization();
            lobjOrgBank.LoadOtherOrgBank();
            return lobjOrgBank;
        }
        public busOrgBank FindOrgBank(int Aintorgbankid)
        {
            busOrgBank lobjOrgBank = new busOrgBank();
            if (lobjOrgBank.FindOrgBank(Aintorgbankid))
            {
                lobjOrgBank.icdoOrgBank.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjOrgBank.icdoOrgBank.bank_org_id);
                lobjOrgBank.LoadBankOrg();
                lobjOrgBank.LoadOrganization();
                lobjOrgBank.LoadOtherOrgBank();
            }

            return lobjOrgBank;
        }

        public busOrgPlan NewOrgPlan(int AintOrgid)
        {
            busOrgPlan lobjOrgPlan = new busOrgPlan();
            lobjOrgPlan.icdoOrgPlan = new cdoOrgPlan();
            lobjOrgPlan.icdoOrgPlan.org_id = AintOrgid;
            lobjOrgPlan.LoadOrganization();
            lobjOrgPlan.LoadOtherOrgPlans();
            lobjOrgPlan.LoadOrgPlanProviders();
            lobjOrgPlan.EvaluateInitialLoadRules();
            return lobjOrgPlan;
        }
        public busOrgPlan FindOrgPlan(int Aintorgplanid)
        {
            busOrgPlan lobjOrgPlan = new busOrgPlan();
            if (lobjOrgPlan.FindOrgPlan(Aintorgplanid))
            {
                lobjOrgPlan.LoadOrganization();
                lobjOrgPlan.LoadOtherOrgPlans();
                lobjOrgPlan.LoadOrgPlanProviders();
                lobjOrgPlan.LoadPlanInfo();
                lobjOrgPlan.LoadRates();
            }
            return lobjOrgPlan;
        }

        public busOrgPlanProvider NewOrgPlanProvider(int AintOrgid, int AintOrgPlanid)
        {
            busOrgPlanProvider lobjOrgPlanProvider = new busOrgPlanProvider();
            lobjOrgPlanProvider.icdoOrgPlanProvider = new cdoOrgPlanProvider();
            lobjOrgPlanProvider.icdoOrgPlanProvider.org_plan_id = AintOrgPlanid;
            lobjOrgPlanProvider.LoadOrganization(AintOrgid);
            lobjOrgPlanProvider.LoadOrgPlan(AintOrgPlanid);
            lobjOrgPlanProvider.LoadOtherOrgPlanProviders();
            return lobjOrgPlanProvider;
        }

        public busOrgPlanProvider FindOrgPlanProvider(int AintOrgid, int AintOrgPlanid, int AintorgplanProviderid)
        {
            busOrgPlanProvider lobjOrgPlanProvider = new busOrgPlanProvider();
            if (lobjOrgPlanProvider.FindOrgPlanProvider(AintorgplanProviderid))
            {
                lobjOrgPlanProvider.LoadOrganization(AintOrgid);
                lobjOrgPlanProvider.LoadOrgPlan(AintOrgPlanid);
                lobjOrgPlanProvider.LoadOtherOrgPlanProviders();
                lobjOrgPlanProvider.LoadProviderOrg();
                //PIR -336
                lobjOrgPlanProvider.icdoOrgPlanProvider.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjOrgPlanProvider.icdoOrgPlanProvider.provider_org_id);
            }
            return lobjOrgPlanProvider;
        }
        public busContact NewContact()
        {
            busContact lobjContact = new busContact();
            lobjContact.icdoContact = new cdoContact();
            return lobjContact;
        }
        public busContact FindContact(int AintContactid)
        {
            busContact lobjContact = new busContact();
            if (lobjContact.FindContact(AintContactid))
            {
                lobjContact.LoadContactAddress();
                lobjContact.LoadContactPrimaryAddress();
                lobjContact.LoadSeminarAttendance();
                lobjContact.LoadOrgContact();
                lobjContact.LoadAffiliatedOrgs();
                //if (lobjContact.iclbAffliatedOrgs.Count() == 0)
                if (!lobjContact.iclbOrgContact.Any(lobjOrgContact => lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusActive))
                        lobjContact.iblnIsContactAffiliatedWithNoOrg = true;
                lobjContact.LoadContactScreenNotes(); //Load screen notes PIR 13155
            }
            return lobjContact;
        }
        public busContactLookup LoadContacts(DataTable adtbSearchResult)
        {
            busContactLookup lobjContactLookup = new busContactLookup();
            lobjContactLookup.LoadContact(adtbSearchResult);
            return lobjContactLookup;
        }
        public busPlanRetirementRate NewPlanRetirementRate(int AintPlanid)
        {
            busPlanRetirementRate lobjPlanRetirementRate = new busPlanRetirementRate();
            lobjPlanRetirementRate.icdoPlanRetirementRate = new cdoPlanRetirementRate();
            lobjPlanRetirementRate.icdoPlanRetirementRate.plan_id = AintPlanid;
            lobjPlanRetirementRate.LoadPlan();
            //PIR 25920 New DC Plan need to declare collection to new for checkbox list bind purpose 
            lobjPlanRetirementRate.iclbPlanMemberTypeCrossref = new utlCollection<cdoPlanMemberTypeCrossref>();
            lobjPlanRetirementRate.icdoPlanRetirementRate.ienuObjectState = ObjectState.Insert;
            return lobjPlanRetirementRate;
        }
        public busPlanRetirementRate FindPlanRetirementRate(int Aintplanrateid)
        {
            busPlanRetirementRate lobjPlanRetirementRate = new busPlanRetirementRate();
            if (lobjPlanRetirementRate.FindPlanRetirementRate(Aintplanrateid))
            {
                lobjPlanRetirementRate.LoadPlan();
                //PIR 25920 New Plan DC 2025
                lobjPlanRetirementRate.LoadPlanMemberTypeCrossref();
                lobjPlanRetirementRate.icdoPlanRetirementRate.ienuObjectState = ObjectState.Update; 		//PIR 25920 New DC Plan
            }
            else
                lobjPlanRetirementRate.iclbPlanMemberTypeCrossref = new utlCollection<cdoPlanMemberTypeCrossref>(); //PIR 25920 New DC Plan
            return lobjPlanRetirementRate;
        }
        public busOrgPlanDentalRate NewOrgPlanDentalRate(int AintOrgPlanid)
        {
            busOrgPlanDentalRate lobjOrgPlanDentalRate = new busOrgPlanDentalRate();
            lobjOrgPlanDentalRate.icdoOrgPlanDentalRate = new cdoOrgPlanDentalRate();
            lobjOrgPlanDentalRate.icdoOrgPlanDentalRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanDentalRate.LoadOrgPlan();
            lobjOrgPlanDentalRate.LoadPlan();
            lobjOrgPlanDentalRate.LoadOrganization();
            return lobjOrgPlanDentalRate;
        }
        public busOrgPlanDentalRate FindOrgPlanDentalRate(int Aintorgplandentalrateid)
        {
            busOrgPlanDentalRate lobjOrgPlanDentalRate = new busOrgPlanDentalRate();
            if (lobjOrgPlanDentalRate.FindOrgPlanDentalRate(Aintorgplandentalrateid))
            {
                lobjOrgPlanDentalRate.LoadOrgPlan();
                lobjOrgPlanDentalRate.LoadPlan();
                lobjOrgPlanDentalRate.LoadOrganization();
            }
            return lobjOrgPlanDentalRate;
        }
        public busOrgPlanEapRate NewOrgPlanEapRate(int AintOrgPlanid)
        {
            busOrgPlanEapRate lobjOrgPlanEapRate = new busOrgPlanEapRate();
            lobjOrgPlanEapRate.icdoOrgPlanEapRate = new cdoOrgPlanEapRate();
            lobjOrgPlanEapRate.icdoOrgPlanEapRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanEapRate.LoadOrgPlan();
            lobjOrgPlanEapRate.LoadPlan();
            lobjOrgPlanEapRate.LoadOrganization();
            return lobjOrgPlanEapRate;
        }
        public busOrgPlanEapRate FindOrgPlanEapRate(int Aintorgplaneaprateid)
        {
            busOrgPlanEapRate lobjOrgPlanEapRate = new busOrgPlanEapRate();
            if (lobjOrgPlanEapRate.FindOrgPlanEapRate(Aintorgplaneaprateid))
            {
                lobjOrgPlanEapRate.LoadOrgPlan();
                lobjOrgPlanEapRate.LoadPlan();
                lobjOrgPlanEapRate.LoadOrganization();
            }
            return lobjOrgPlanEapRate;
        }
        public busOrgPlanVisionRate NewOrgPlanVisionRate(int AintOrgPlanid)
        {
            busOrgPlanVisionRate lobjOrgPlanVisionRate = new busOrgPlanVisionRate();
            lobjOrgPlanVisionRate.icdoOrgPlanVisionRate = new cdoOrgPlanVisionRate();
            lobjOrgPlanVisionRate.icdoOrgPlanVisionRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanVisionRate.LoadOrgPlan();
            lobjOrgPlanVisionRate.LoadPlan();
            lobjOrgPlanVisionRate.LoadOrganization();
            return lobjOrgPlanVisionRate;
        }
        public busOrgPlanVisionRate FindOrgPlanVisionRate(int Aintorgplanvisionrateid)
        {
            busOrgPlanVisionRate lobjOrgPlanVisionRate = new busOrgPlanVisionRate();
            if (lobjOrgPlanVisionRate.FindOrgPlanVisionRate(Aintorgplanvisionrateid))
            {
                lobjOrgPlanVisionRate.LoadOrgPlan();
                lobjOrgPlanVisionRate.LoadPlan();
                lobjOrgPlanVisionRate.LoadOrganization();
            }
            return lobjOrgPlanVisionRate;
        }
        public busOrgPlanHmoRate NewOrgPlanHmoRate(int AintOrgPlanid)
        {
            busOrgPlanHmoRate lobjOrgPlanHmoRate = new busOrgPlanHmoRate();
            lobjOrgPlanHmoRate.icdoOrgPlanHmoRate = new cdoOrgPlanHmoRate();
            lobjOrgPlanHmoRate.icdoOrgPlanHmoRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanHmoRate.LoadOrgPlan();
            lobjOrgPlanHmoRate.LoadPlan();
            lobjOrgPlanHmoRate.LoadOrganization();
            return lobjOrgPlanHmoRate;
        }
        public busOrgPlanHmoRate FindOrgPlanHmoRate(int Aintorgplanhmorateid)
        {
            busOrgPlanHmoRate lobjOrgPlanHmoRate = new busOrgPlanHmoRate();
            if (lobjOrgPlanHmoRate.FindOrgPlanHmoRate(Aintorgplanhmorateid))
            {
                lobjOrgPlanHmoRate.LoadOrgPlan();
                lobjOrgPlanHmoRate.LoadPlan();
                lobjOrgPlanHmoRate.LoadOrganization();
            }
            return lobjOrgPlanHmoRate;
        }
        public busOrgPlanMemberType NewOrgPlanMemberType(int AintOrgPlanid)
        {
            busOrgPlanMemberType lobjOrgPlanMemberType = new busOrgPlanMemberType();
            lobjOrgPlanMemberType.icdoOrgPlanMemberType = new cdoOrgPlanMemberType();
            lobjOrgPlanMemberType.icdoOrgPlanMemberType.org_plan_id = AintOrgPlanid;
            lobjOrgPlanMemberType.LoadOrgPlan();
            lobjOrgPlanMemberType.LoadPlan();
            return lobjOrgPlanMemberType;
        }
        public busOrgPlanMemberType FindOrgPlanMemberType(int AintOrgPlanMemberTypeID)
        {
            busOrgPlanMemberType lobjOrgPlanMemberType = new busOrgPlanMemberType();
            if (lobjOrgPlanMemberType.FindOrgPlanMemberType(AintOrgPlanMemberTypeID))
            {
                lobjOrgPlanMemberType.LoadOrgPlan();
                lobjOrgPlanMemberType.LoadPlan();
            }
            return lobjOrgPlanMemberType;
        }
        public busOrgPlanLtcRate NewOrgPlanLtcRate(int AintOrgPlanid)
        {
            busOrgPlanLtcRate lobjOrgPlanLtcRate = new busOrgPlanLtcRate();
            lobjOrgPlanLtcRate.icdoOrgPlanLtcRate = new cdoOrgPlanLtcRate();
            lobjOrgPlanLtcRate.icdoOrgPlanLtcRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanLtcRate.LoadOrgPlan();
            lobjOrgPlanLtcRate.LoadPlan();
            lobjOrgPlanLtcRate.LoadOrganization();
            return lobjOrgPlanLtcRate;
        }
        public busOrgPlanLtcRate FindOrgPlanLtcRate(int Aintorgplanltcrateid)
        {
            busOrgPlanLtcRate lobjOrgPlanLtcRate = new busOrgPlanLtcRate();
            if (lobjOrgPlanLtcRate.FindOrgPlanLtcRate(Aintorgplanltcrateid))
            {
                lobjOrgPlanLtcRate.LoadOrgPlan();
                lobjOrgPlanLtcRate.LoadPlan();
                lobjOrgPlanLtcRate.LoadOrganization();
            }
            return lobjOrgPlanLtcRate;
        }
        public busOrgPlanLifeRate NewOrgPlanLifeRate(int AintOrgPlanid)
        {
            busOrgPlanLifeRate lobjOrgPlanLifeRate = new busOrgPlanLifeRate();
            lobjOrgPlanLifeRate.icdoOrgPlanLifeRate = new cdoOrgPlanLifeRate();
            lobjOrgPlanLifeRate.icdoOrgPlanLifeRate.org_plan_id = AintOrgPlanid;
            lobjOrgPlanLifeRate.LoadOrgPlan();
            lobjOrgPlanLifeRate.LoadPlan();
            lobjOrgPlanLifeRate.LoadOrganization();
            return lobjOrgPlanLifeRate;
        }
        public busOrgPlanLifeRate FindOrgPlanLifeRate(int Aintorgplanliferateid)
        {
            busOrgPlanLifeRate lobjOrgPlanLifeRate = new busOrgPlanLifeRate();
            if (lobjOrgPlanLifeRate.FindOrgPlanLifeRate(Aintorgplanliferateid))
            {
                lobjOrgPlanLifeRate.LoadOrgPlan();
                lobjOrgPlanLifeRate.LoadPlan();
                lobjOrgPlanLifeRate.LoadOrganization();
            }
            return lobjOrgPlanLifeRate;
        }

        public busOrgPlanGroupHealthMedicarePartDCoverageRef FindOrgPlanGroupHealthMedicarePartDCoverageRef(int Aintorgplangrouphealthmedicarepartdcoveragerefid)
        {
            busOrgPlanGroupHealthMedicarePartDCoverageRef lobjOrgPlanGroupHealthMedicarePartDCoverageRef = new busOrgPlanGroupHealthMedicarePartDCoverageRef();
            if (lobjOrgPlanGroupHealthMedicarePartDCoverageRef.FindOrgPlanGroupHealthMedicarePartDCoverageRef(Aintorgplangrouphealthmedicarepartdcoveragerefid))
            {
            }

            return lobjOrgPlanGroupHealthMedicarePartDCoverageRef;
        }

        public busOrgPlanGroupHealthMedicarePartDRateRef FindOrgPlanGroupHealthMedicarePartDRateRef(int Aintorgplangrouphealthmedicarepartdraterefid)
        {
            busOrgPlanGroupHealthMedicarePartDRateRef lobjOrgPlanGroupHealthMedicarePartDRateRef = new busOrgPlanGroupHealthMedicarePartDRateRef();
            if (lobjOrgPlanGroupHealthMedicarePartDRateRef.FindOrgPlanGroupHealthMedicarePartDRateRef(Aintorgplangrouphealthmedicarepartdraterefid))
            {
            }

            return lobjOrgPlanGroupHealthMedicarePartDRateRef;
        }
        public busOrgPlanHealthMedicarePartDRate NewOrgPlanHealthMedicarePartDRate(int Aintorgplanid)
        {
            busOrgPlanHealthMedicarePartDRate lobjOrgPlanHealthMedicarePartDRate = new busOrgPlanHealthMedicarePartDRate();
            lobjOrgPlanHealthMedicarePartDRate.icdoOrgPlanHealthMedicarePartDRate = new cdoOrgPlanHealthMedicarePartDRate();
            lobjOrgPlanHealthMedicarePartDRate.icdoOrgPlanHealthMedicarePartDRate.org_plan_id = Aintorgplanid;
            lobjOrgPlanHealthMedicarePartDRate.LoadOrgPlan();
            lobjOrgPlanHealthMedicarePartDRate.LoadPlan();
            lobjOrgPlanHealthMedicarePartDRate.LoadOrganization();
            return lobjOrgPlanHealthMedicarePartDRate;
        }
        public busOrgPlanHealthMedicarePartDRate FindOrgPlanHealthMedicarePartDRate(int Aintorgplanhealthmedicarepartdrateid)
        {
            busOrgPlanHealthMedicarePartDRate lobjOrgPlanHealthMedicarePartDRate = new busOrgPlanHealthMedicarePartDRate();
            lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDCoverageRef = new busOrgPlanGroupHealthMedicarePartDCoverageRef();
            lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDRateRef = new busOrgPlanGroupHealthMedicarePartDRateRef();
            if (lobjOrgPlanHealthMedicarePartDRate.FindOrgPlanHealthMedicarePartDRate(Aintorgplanhealthmedicarepartdrateid))
            {
                lobjOrgPlanHealthMedicarePartDRate.LoadOrgPlan();
                lobjOrgPlanHealthMedicarePartDRate.LoadPlan();
                lobjOrgPlanHealthMedicarePartDRate.LoadOrganization();
                if (lobjOrgPlanHealthMedicarePartDRate.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id != 0)
                {
                    int lintMedicarePartDCoverageRefID = lobjOrgPlanHealthMedicarePartDRate.icdoOrgPlanHealthMedicarePartDRate.org_plan_group_health_medicare_part_d_coverage_ref_id;
                    if (lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDCoverageRef.FindOrgPlanGroupHealthMedicarePartDCoverageRef(lintMedicarePartDCoverageRefID))
                    {
                        if (lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDCoverageRef.icdoOrgPlanGroupHealthMedicarePartDCoverageRef.org_plan_group_health_medicare_part_d_rate_ref_id != 0)
                        {
                            int lintMedicarePartDRateRefID = lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDCoverageRef.icdoOrgPlanGroupHealthMedicarePartDCoverageRef.org_plan_group_health_medicare_part_d_rate_ref_id;
                            lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDRateRef.FindOrgPlanGroupHealthMedicarePartDRateRef(lintMedicarePartDRateRefID);
                            lobjOrgPlanHealthMedicarePartDRate.istrMemberType = lobjOrgPlanHealthMedicarePartDRate.ibusMedicarePartDRateRef.icdoOrgPlanGroupHealthMedicarePartDRateRef.health_insurance_type_value;
                        }
                    }
                }
            }
            return lobjOrgPlanHealthMedicarePartDRate;
        }
        public busProviderReportDataBatchRequest NewProviderReportDataBatchRequest()
        {
            busProviderReportDataBatchRequest lobjProviderReportDataBatchRequest = new busProviderReportDataBatchRequest();
            lobjProviderReportDataBatchRequest.icdoProviderReportDataBatchRequest = new cdoProviderReportDataBatchRequest();
            lobjProviderReportDataBatchRequest.icdoProviderReportDataBatchRequest.status_value = busConstant.Vendor_Payment_Status_NotProcessed;// PIR 24921 
            lobjProviderReportDataBatchRequest.EvaluateInitialLoadRules();
            return lobjProviderReportDataBatchRequest;
        }
        public busProviderReportDataBatchRequest FindProviderReportDataBatchRequest(int Aintproviderreportdatabatchrequestid)
        {
            busProviderReportDataBatchRequest lobjProviderReportDataBatchRequest = new busProviderReportDataBatchRequest();
            if (lobjProviderReportDataBatchRequest.FindProviderReportDataBatchRequest(Aintproviderreportdatabatchrequestid))
            {
                lobjProviderReportDataBatchRequest.LoadOrganizationByOrgId();
                lobjProviderReportDataBatchRequest.icdoProviderReportDataBatchRequest.org_code = lobjProviderReportDataBatchRequest.ibusOrganization.icdoOrganization.org_code;
                lobjProviderReportDataBatchRequest.LoadProviderReportDataDC();
                lobjProviderReportDataBatchRequest.LoadProviderReportDataDeffcompDetails();
                lobjProviderReportDataBatchRequest.LoadProviderReportDataInsurance();
                lobjProviderReportDataBatchRequest.LoadProviderReportDataMedicarePartD();
            }
            return lobjProviderReportDataBatchRequest;
        }
        public busProviderReportDataBatchRequestLookup LoadProviderReportDataBatchRequests(DataTable adtbSearchResult)
        {
            busProviderReportDataBatchRequestLookup lobjProviderReportDataBatchRequestLookup = new busProviderReportDataBatchRequestLookup();
            lobjProviderReportDataBatchRequestLookup.LoadProviderReportDataBatchRequests(adtbSearchResult);
            return lobjProviderReportDataBatchRequestLookup;
        }
        public busProviderReportDataDC FindProviderReportDataDC(int Aintproviderreportdatadcid)
        {
            busProviderReportDataDC lobjProviderReportDataDC = new busProviderReportDataDC();
            if (lobjProviderReportDataDC.FindProviderReportDataDC(Aintproviderreportdatadcid))
            {
            }
            return lobjProviderReportDataDC;
        }

        public busProviderReportDataDeffComp FindProviderReportDataDeffComp(int Aintproviderreportdatadeffcompid)
        {
            busProviderReportDataDeffComp lobjProviderReportDataDeffComp = new busProviderReportDataDeffComp();
            if (lobjProviderReportDataDeffComp.FindProviderReportDataDeffComp(Aintproviderreportdatadeffcompid))
            {
            }

            return lobjProviderReportDataDeffComp;
        }

        public busProviderReportDataInsurance FindProviderReportDataInsurance(int Aintproviderreportdatadeffcompid)
        {
            busProviderReportDataInsurance lobjProviderReportDataInsurance = new busProviderReportDataInsurance();
            if (lobjProviderReportDataInsurance.FindProviderReportDataInsurance(Aintproviderreportdatadeffcompid))
            {
            }

            return lobjProviderReportDataInsurance;
        }

        public busMergeEmployerHeader NewMergeEmployerHeader()
        {
            busMergeEmployerHeader lobjMergeEmployerHeader = new busMergeEmployerHeader();
            lobjMergeEmployerHeader.icdoMergeEmployerHeader = new cdoMergeEmployerHeader();
            lobjMergeEmployerHeader.icdoMergeEmployerHeader.merge_status_value = busConstant.EmployerMergeStatusQueued;
            lobjMergeEmployerHeader.icdoMergeEmployerHeader.merge_status_description =
                 iobjPassInfo.isrvDBCache.GetCodeDescriptionString(414, lobjMergeEmployerHeader.icdoMergeEmployerHeader.merge_status_value);
            return lobjMergeEmployerHeader;
        }

        public busMergeEmployerHeader FindMergeEmployerHeader(int Aintmergeemployerheaderid)
        {
            busMergeEmployerHeader lobjMergeEmployerHeader = new busMergeEmployerHeader();
            if (lobjMergeEmployerHeader.FindMergeEmployerHeader(Aintmergeemployerheaderid))
            {
                lobjMergeEmployerHeader.LoadMergeEmployerDetails();
                lobjMergeEmployerHeader.LoadFromEmployer();
                lobjMergeEmployerHeader.LoadToEmployer();
                lobjMergeEmployerHeader.LoadFromOrgPlans();
                lobjMergeEmployerHeader.LoadToOrgPlans();
                lobjMergeEmployerHeader.istrFromOrgCodeID = lobjMergeEmployerHeader.ibusFromEmployer.icdoOrganization.org_code;
                lobjMergeEmployerHeader.istrToOrgCodeID = lobjMergeEmployerHeader.ibusToEmployer.icdoOrganization.org_code;
                lobjMergeEmployerHeader.LoadErrors();
            }
            return lobjMergeEmployerHeader;
        }

        public busMergeEmployerDetail NewMergeEmployerDetail(int Aintmergeemployerheaderid)
        {
            busMergeEmployerDetail lobjMergeEmployerDetail = new busMergeEmployerDetail();
            lobjMergeEmployerDetail.icdoMergeEmployerDetail = new cdoMergeEmployerDetail();
            lobjMergeEmployerDetail.icdoMergeEmployerDetail.merge_employer_header_id = Aintmergeemployerheaderid;
            lobjMergeEmployerDetail.LoadPerson();
            lobjMergeEmployerDetail.LoadMergeEmployerHeader();
            lobjMergeEmployerDetail.ibusMergeEmployerHeader.istrFromOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(
                      lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader.from_employer_id);
            lobjMergeEmployerDetail.ibusMergeEmployerHeader.istrToOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(
                    lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader.to_employer_id);
            return lobjMergeEmployerDetail;
        }

        public busMergeEmployerDetail FindMergeEmployerDetail(int Aintmergeemployerdetailid)
        {
            busMergeEmployerDetail lobjMergeEmployerDetail = new busMergeEmployerDetail();
            if (lobjMergeEmployerDetail.FindMergeEmployerDetail(Aintmergeemployerdetailid))
            {
                lobjMergeEmployerDetail.LoadPerson();
                lobjMergeEmployerDetail.LoadMergeEmployerHeader();
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.istrFromOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(
                          lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader.from_employer_id);
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.istrToOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(
                        lobjMergeEmployerDetail.ibusMergeEmployerHeader.icdoMergeEmployerHeader.to_employer_id);
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.LoadFromEmployer();
                lobjMergeEmployerDetail.ibusMergeEmployerHeader.LoadToEmployer();
                lobjMergeEmployerDetail.LoadErrors();
            }
            return lobjMergeEmployerDetail;
        }

        public busMergeEmployerHeaderLookup LoadMergeEmployerHeaders(DataTable adtbSearchResult)
        {
            busMergeEmployerHeaderLookup lobjMergeEmployerHeaderLookup = new busMergeEmployerHeaderLookup();
            lobjMergeEmployerHeaderLookup.LoadMergeEmployerHeaders(adtbSearchResult);
            return lobjMergeEmployerHeaderLookup;
        }

        public busMergeEmployerDetailLookup LoadMergeEmployerDetails(DataTable adtbSearchResult)
        {
            busMergeEmployerDetailLookup lobjMergeEmployerDetailLookup = new busMergeEmployerDetailLookup();
            lobjMergeEmployerDetailLookup.LoadMergeEmployerDetails(adtbSearchResult);
            return lobjMergeEmployerDetailLookup;
        }

        public busOrgDeductionType NewOrgDeductionType(int AintOrgID)
        {
            busOrgDeductionType lobjOrgDeductionType = new busOrgDeductionType();
            lobjOrgDeductionType.icdoOrgDeductionType = new cdoOrgDeductionType();
            lobjOrgDeductionType.FindOrganization(AintOrgID);
            lobjOrgDeductionType.icdoOrgDeductionType.org_id = AintOrgID;
            return lobjOrgDeductionType;
        }

        public busOrgDeductionType FindOrgDeductionType(int Aintorgdeductiontypeid)
        {
            busOrgDeductionType lobjOrgDeductionType = new busOrgDeductionType();
            if (lobjOrgDeductionType.FindOrgDeductionType(Aintorgdeductiontypeid))
            {
                lobjOrgDeductionType.FindOrganization(lobjOrgDeductionType.icdoOrgDeductionType.org_id);
            }
            return lobjOrgDeductionType;
        }

        public busOrganization ESSFindOrganization(int aintOrgID, int aintContactID, bool ablnPAG)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSOrganizationMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;               
                ablnPAG = iblnIsPAG; //FW Upgrade PIR-11383           
            }
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(aintOrgID))
            {
                lobjOrganization.LoadOrgPrimaryAddress();
                lobjOrganization.iblnIsPAA = ablnPAG;

                //PIR-13171
                lobjOrganization.iblnIsFromESS = true;

                lobjOrganization.LoadESSPrimaryAuthorizedContact();

                //------ Load Contact List
                //lobjOrganization.iintContactID = aintContactID;
                lobjOrganization.LoadOrgContact();

                lobjOrganization.iclbESSOrgContact = new System.Collections.ObjectModel.Collection<busOrgContact>();

                //PIR - 13171 - displaying all active contacts
                List<busOrgContact> DistinctOrgContact = lobjOrganization.iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusActive).GroupBy(CurrentEmployee => CurrentEmployee.icdoOrgContact.contact_id)
                                                    .Select(OrgContactRecords => OrgContactRecords.First())
                                                    .ToList();
                foreach (busOrgContact lobjOrgContact in DistinctOrgContact)
                {
                    lobjOrgContact.iclbOrgContactRole = new utlCollection<cdoOrgContactRole>();
                    //prod pir 5574
                    if (lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusActive)
                    {
                        lobjOrgContact.ibusContact.LoadContactPrimaryAddress();
                        lobjOrgContact.iblnPAAG = ablnPAG;
                        lobjOrgContact.iintPAAG = ablnPAG == true ? 1 : 0;
                        lobjOrgContact.iintContactId = aintContactID;
                        lobjOrganization.iclbESSOrgContact.Add(lobjOrgContact);
                    }
                }
                //------ Load Plan List
                lobjOrganization.LoadOrgPlan();
                //PIR - 13905
                lobjOrganization.iclbOrgPlan = lobjOrganization.iclbOrgPlan.Where(item => item.icdoOrgPlan.participation_end_date == DateTime.MinValue
                   || item.icdoOrgPlan.participation_end_date > DateTime.Now).ToList().ToCollection<busOrgPlan>();
            }
            return lobjOrganization;
        }

        public busOrganization ESSFindOrganizationContactList(int aintOrgID, int aintContactID, bool ablnPAG)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSContactListMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
                ablnPAG = ablnPAG.IsNotNull() ? ablnPAG : iblnIsPAG;
            }
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(aintOrgID))
            {
                lobjOrganization.iblnIsPAA = ablnPAG;
                lobjOrganization.iintContactID = aintContactID;
                lobjOrganization.LoadOrgContact();
                lobjOrganization.iclbESSOrgContact = new System.Collections.ObjectModel.Collection<busOrgContact>();
                foreach (busOrgContact lobjOrgContact in lobjOrganization.iclbOrgContact)
                {
                    //prod pir 5574
                    if (lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusActive)
                    {
                        lobjOrgContact.ibusContact.LoadContactPrimaryAddress();
                        lobjOrgContact.iblnPAAG = ablnPAG;
                        lobjOrgContact.iintPAAG = ablnPAG == true ? 1 : 0;
                        lobjOrgContact.iintContactId = aintContactID;
                        lobjOrganization.iclbESSOrgContact.Add(lobjOrgContact);
                    }
                }
            }
            return lobjOrganization;
        }

        public busOrganization ESSFindOrganizationPlanList(int aintOrgID, int aintContactID, bool ablnPAG)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSPlanListMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
                ablnPAG = ablnPAG.IsNotNull() ? ablnPAG : iblnIsPAG;
            }
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(aintOrgID))
            {
                lobjOrganization.LoadOrgPlan();
                lobjOrganization.iintContactID = aintContactID;
            }
            return lobjOrganization;
        }

        public busOrganization ESSFindOrganizationBankInfo(int aintOrgID, int aintContactID, bool ablnPAG)
        {
            /*
             * FMUpgrade: Changes for set Navigation parameter from menu item click
             */
            SetWebParameters();
            if (iobjPassInfo.istrFormName == "wfmESSOrgBankMaintenance")
            {
                aintOrgID = aintOrgID != 0 ? aintOrgID : iintOrgID;
                aintContactID = aintContactID != 0 ? aintContactID : iintContactID;
                ablnPAG = ablnPAG.IsNotNull() ? ablnPAG : iblnIsPAG;
            }
            busOrganization lobjOrganization = new busOrganization();
            if (lobjOrganization.FindOrganization(aintOrgID))
            {
                lobjOrganization.iintContactID = aintContactID;
                lobjOrganization.iblnIsPAA = ablnPAG;
                lobjOrganization.CheckFinanceRoleAndLoadOrgBank();
            }
            return lobjOrganization;
        }

        public busOrgPlan ESSFindOrgPlan(int Aintorgplanid, int aintContactID)
        {
            busOrgPlan lobjOrgPlan = new busOrgPlan();
            if (lobjOrgPlan.FindOrgPlan(Aintorgplanid))
            {
                lobjOrgPlan.LoadOrganization();
                lobjOrgPlan.LoadOtherOrgPlans();
                lobjOrgPlan.LoadOrgPlanProviders();
                lobjOrgPlan.LoadPlanInfo();
                if (lobjOrgPlan.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdLTC)
                    lobjOrgPlan.LoadESSLTCRatesLink();
                else if (lobjOrgPlan.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife)
                    lobjOrgPlan.LoadESSLifeRatesLink();
                else
                    lobjOrgPlan.LoadRates();
                lobjOrgPlan.ESSFilterRates();
                lobjOrgPlan.iintContactID = aintContactID;
                //lobjOrgPlan.LoadEmployeesEnrolledInPlan();
                foreach (busOrgPlanProvider lobjOrgPlanProvider in lobjOrgPlan.iclbOrgPlanProvider)
                {
                    lobjOrgPlanProvider.icdoOrgPlanProvider.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjOrgPlanProvider.icdoOrgPlanProvider.provider_org_id);
                    if (lobjOrgPlan.iclcCodeValue != null)
                        lobjOrgPlanProvider.icdoOrgPlanProvider.istrPlanRateLink = lobjOrgPlan.iclcCodeValue.FirstOrDefault().data1;
                }
                ////PIR  13171- Other 457/403 Conversion Provider not to be shown update//
                lobjOrgPlan.iclbOrgPlanProvider = lobjOrgPlan.iclbOrgPlanProvider.Where(i => i.icdoOrgPlanProvider.provider_org_id != 2415).ToList().ToCollection();
            }
            return lobjOrgPlan;
        }

        public busOrgContact ESSFindOrgContact(int AintOrgContactid, int aintContactId, int aintPAAG)
        {
            busOrgContact lobjOrgContact = new busOrgContact();
            lobjOrgContact.iclbOrgContactRole = new utlCollection<cdoOrgContactRole>();
            if (lobjOrgContact.FindOrgContact(AintOrgContactid))
            {
                lobjOrgContact.LoadOrganization();
                lobjOrgContact.LoadContact();
                lobjOrgContact.LoadOrgAndContactAddresses();
                lobjOrgContact.LoadOtherOrgContacts();
                lobjOrgContact.LoadContactTypes();
                lobjOrgContact.LoadContactPlanRoles();
                lobjOrgContact.LoadContactPlanRolesGroupByRole();
                if (lobjOrgContact.iclbOrgContactRole.IsNotNull() && lobjOrgContact.iclbOrgContactRole.Count() > 0)
                {
                    lobjOrgContact.icdoContactRole = lobjOrgContact.iclbOrgContactRole[0];
                }
                lobjOrgContact.LoadPlan();
                lobjOrgContact.setConsolidatStatusValue();
                lobjOrgContact.LoadCodeValuePlanAndRole();
                lobjOrgContact.iblnIsFromESS = true;
                lobjOrgContact.ibusContact.LoadContactPrimaryAddress();
                lobjOrgContact.iintContactId = aintContactId;
                lobjOrgContact.iintPAAG = aintPAAG;
                lobjOrgContact.iblnPAAG = aintPAAG == 1 ? true : false;
                lobjOrgContact.EvaluateInitialLoadRules();
            }
            return lobjOrgContact;
        }

        public busProviderReportDataInsuranceSplit FindProviderReportDataInsuranceSplit(int aintproviderreportdatainsurancesplitid)
        {
            busProviderReportDataInsuranceSplit lobjProviderReportDataInsuranceSplit = new busProviderReportDataInsuranceSplit();
            if (lobjProviderReportDataInsuranceSplit.FindProviderReportDataInsuranceSplit(aintproviderreportdatainsurancesplitid))
            {
            }

            return lobjProviderReportDataInsuranceSplit;
        }

        public busProviderReportDataInsuranceSplit NewProviderReportDataInsuranceSplit()
        {
            busProviderReportDataInsuranceSplit lobjProviderReportDataInsuranceSplit = new busProviderReportDataInsuranceSplit();
            lobjProviderReportDataInsuranceSplit.icdoProviderReportDataInsuranceSplit = new cdoProviderReportDataInsuranceSplit();
            return lobjProviderReportDataInsuranceSplit;
        }

        public busUpdateOrgPlanProviderLookup LoadUpdateOrgPlanProvider(DataTable adtbSearchResult)
        {
            busUpdateOrgPlanProviderLookup lobjUpdateOrgPlanProviderLookup = new busUpdateOrgPlanProviderLookup();
            lobjUpdateOrgPlanProviderLookup.LoadUpdateOrgPlanProviders(adtbSearchResult);
            return lobjUpdateOrgPlanProviderLookup;
        }

        public busUpdateOrgPlanProvider NewUpdateOrgPlanProvider()
        {
            busUpdateOrgPlanProvider lobjUpdateOrgPlanProvider = new busUpdateOrgPlanProvider { icdoUpdateOrgPlanProvider = new cdoUpdateOrgPlanProvider() };
            lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.plan_id = busConstant.PlanIdEAP;
            lobjUpdateOrgPlanProvider.LoadPlan();
            lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.status_id = 1506;
            lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.status_value = busConstant.Vendor_Payment_Status_NotProcessed;
            lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.status_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1506, busConstant.Vendor_Payment_Status_NotProcessed);
            return lobjUpdateOrgPlanProvider;
        }

        public busUpdateOrgPlanProvider FindUpdateOrgPlanProvider(int AintRequestID)
        {
            busUpdateOrgPlanProvider lobjUpdateOrgPlanProvider = new busUpdateOrgPlanProvider();
            if (lobjUpdateOrgPlanProvider.FindUpdateOrgPlanProvider(AintRequestID))
            {
                lobjUpdateOrgPlanProvider.LoadPlan();
                lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.employer_org_code =
                    busGlobalFunctions.GetOrgCodeFromOrgId(lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.employer_org_id);
                lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.from_provider_org_code =
                    busGlobalFunctions.GetOrgCodeFromOrgId(lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.from_provider_org_id);
                lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.to_provider_org_code =
                    busGlobalFunctions.GetOrgCodeFromOrgId(lobjUpdateOrgPlanProvider.icdoUpdateOrgPlanProvider.to_provider_org_id);
            }
            return lobjUpdateOrgPlanProvider;
        }
        //PIR 1758
        public busLowIncomeCreditRefLookup LoadLowIncomeCreditRefs(DataTable adtbSearchResult)
        {
            busLowIncomeCreditRefLookup lobjLowIncomeCreditRefLookup = new busLowIncomeCreditRefLookup();
            lobjLowIncomeCreditRefLookup.LoadLowIncomeCreditRefs(adtbSearchResult);
            return lobjLowIncomeCreditRefLookup;
        }
        //PIR 1758
        public busLowIncomeCreditRef FindLowIncomeCreditRefs(int AintLowIncomeCreditRefsid)
        {
            busLowIncomeCreditRef lobjLowIncomeCreditRef = new busLowIncomeCreditRef();
            if (lobjLowIncomeCreditRef.FindLowIncomeCreditRef(AintLowIncomeCreditRefsid))
            { }
            return lobjLowIncomeCreditRef;
        }
        public Tuple<string, string> GetOrgCodeOrgNameById(int aintOrgID)
        {
            Tuple<string, string> ltupOrgCodeOrgName = null;
            busOrganization lbusOrganization = new busOrganization();
            if(lbusOrganization.FindOrganization(aintOrgID))
            {
                ltupOrgCodeOrgName = new Tuple<string, string>(lbusOrganization.icdoOrganization.org_code, lbusOrganization.icdoOrganization.org_name);
            }
            return ltupOrgCodeOrgName ?? new Tuple<string, string>(string.Empty, string.Empty);
        }
    }
}
