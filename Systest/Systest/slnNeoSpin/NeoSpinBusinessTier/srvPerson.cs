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
using Sagitec.DBUtility;
using NeoSpin.DataObjects;
using System.IO;
using System.Web.Script.Serialization;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvPerson : srvNeoSpin
    {
        public srvPerson()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //FMUpgrade : Added Global Parameter for srvMethods
        private int iintpersonid;
        private int iintOrgID;
        private int iintContactID;
        
        
        private void SetWebParameters()
        {
            // iintContactID = (int)iobjPassInfo.idictParams["personid"];
            if (iobjPassInfo.idictParams.ContainsKey("ContactID"))
                iintContactID = (int)iobjPassInfo.idictParams["ContactID"];
            if (iobjPassInfo.idictParams.ContainsKey("OrgID"))
                iintOrgID = (int)iobjPassInfo.idictParams["OrgID"];
        }
        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
            //iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmPersonMaintenance")
                {
                    busPerson lbusPerson = new busPerson();
                    larrErrors = lbusPerson.ValidateNew(ahstParam);
                }

                if (astrFormName == "wfmServicePurchaseHeaderLookup")
                {
                    busServicePurchaseHeaderLookup lobjServicePurchaseHeaderLookup =
                        new busServicePurchaseHeaderLookup();
                    larrErrors = lobjServicePurchaseHeaderLookup.ValidateNew(ahstParam);
                }
                if (astrFormName == "wfmFlexCompMaintenance")
                {
                    busPersonAccountFlexComp lobjFlexComp = new busPersonAccountFlexComp();
                    larrErrors = lobjFlexComp.ValidateNew(ahstParam);
                }
            }
            finally
            {
               // iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }
        public busPerson NewPerson()
        {
            busPerson lobjPerson = new busPerson();
            lobjPerson.icdoPerson = new cdoPerson();
            lobjPerson.EvaluateInitialLoadRules();
            return lobjPerson;
        }

        public busPersonRelease1 NewPersonRelease1()
        {
            busPersonRelease1 lobjPerson = new busPersonRelease1();
            lobjPerson.icdoPerson = new cdoPerson();
            return lobjPerson;
        }

        public busPersonRelease1 FindPersonRelease1(int Aintpersonid)
        {
            busPersonRelease1 lobjPerson = new busPersonRelease1();
            lobjPerson.FindPerson(Aintpersonid);
            return lobjPerson;
        }
        public busPerson FindPerson(int Aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(Aintpersonid))
            {
                lobjPerson.LoadAddresses();
                lobjPerson.LoadContacts();
                lobjPerson.LoadPersonCurrentAddress();
                lobjPerson.LoadBeneficiary();
                lobjPerson.LoadPersonEmployment();
                lobjPerson.LoadDependent();
                lobjPerson.LoadUser();
                lobjPerson.LoadNotes();
                lobjPerson.LoadBeneficiaryTo();
                lobjPerson.LoadDependentTo();
                lobjPerson.LoadContactTo();
                lobjPerson.LoadPersonAccount();
                lobjPerson.LoadDROApplications();
                lobjPerson.LoadPersonTypes();
                lobjPerson.LoadIBSMemberSummary(false);
                lobjPerson.LoadTffrTiaaService();
                lobjPerson.LoadPlanEnrollmentVisibility();

                //PIR 1863
                lobjPerson.LoadBeneficiaryApplication(true);
                lobjPerson.LoadApplicationsForBeneficiaryTo();
                if (!(String.IsNullOrEmpty(lobjPerson.icdoPerson.peoplesoft_id)))
                {
                    lobjPerson.icdoPerson.peoplesoft_id = lobjPerson.icdoPerson.peoplesoft_id.Trim();
                    lobjPerson.icdoPerson.peoplesoft_id = lobjPerson.icdoPerson.peoplesoft_id.PadLeft(7, '0');
                }
                lobjPerson.LoadDeathNotification();
                lobjPerson.LoadPaymentDetails();
                lobjPerson.icdoPerson.temp_ssn = lobjPerson.icdoPerson.ssn;
                lobjPerson.SetFlagIs1099RExits();
                lobjPerson.EvaluateInitialLoadRules();

                //UAT PIR - 1660
                //this is loaded :- SFN-53621 is used in both Person and Organization maintenance
                //common object - organization is maaped thru person enployment in person
                // organization
                lobjPerson.LoadCurrentEmployer();
                // pir 6566
                lobjPerson.SetBeneficiaryRequiredFlag();
                //PIR 24755
                lobjPerson.LoadPaymentElectionAdjustment();
                //PIR 26615
                lobjPerson.LoadPersonTffrHeader();
            }
            return lobjPerson;
        }

        public busPerson FindPersonOverview(int Aintpersonid)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(Aintpersonid))
            {
                lobjPerson.LoadAddresses();
                lobjPerson.LoadContacts();
                lobjPerson.LoadPersonCurrentAddress();
                lobjPerson.LoadPersonOverviewBeneficiary();
                lobjPerson.LoadPersonOverviewDependent();
                //lobjPerson.LoadContactsAppointments();
                lobjPerson.LoadSeminarAttendance();
                //lobjPerson.LoadWorkflowProcessHistory();
                lobjPerson.LoadNotes();
                lobjPerson.LoadServicePurchase();
                lobjPerson.LoadPersonTypes();
                lobjPerson.LoadPensionSummary();
                lobjPerson.LoadInsuranceSummary();
                if (!(String.IsNullOrEmpty(lobjPerson.icdoPerson.peoplesoft_id)))
                {
                    lobjPerson.icdoPerson.peoplesoft_id = lobjPerson.icdoPerson.peoplesoft_id.Trim();
                    lobjPerson.icdoPerson.peoplesoft_id = lobjPerson.icdoPerson.peoplesoft_id.PadLeft(7, '0');
                }
                lobjPerson.LoadBenefitApplication();
                lobjPerson.LoadApplicantsbenefitApplication();
                lobjPerson.LoadAllDeathNotificationForPerson();
                lobjPerson.LoadDROApplications();
                lobjPerson.LoadDeathNotification();
                lobjPerson.LoadMemberPlusAlternatePayeeAccounts();
                lobjPerson.LoadMASStatementFile();
                lobjPerson.LoadCase();

                //UAT PIR - 2102
                lobjPerson.LoadBenefitRhicCombine(true);

                lobjPerson.LoadBeneficiaryApplication(true);
            }
            return lobjPerson;
        }

        public busPersonAccountRetirement FindAccruedBenefit(int AintPersonAccountID)
        {
            // PROD PIR ID 4061
            busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement();
            lobjPARetirement.FindPersonAccount(AintPersonAccountID);
            lobjPARetirement.FindPersonAccountRetirement(AintPersonAccountID);
            lobjPARetirement.LoadPerson();
            lobjPARetirement.LoadPlan();
            lobjPARetirement.LoadPersonAccount();
            lobjPARetirement.icdoPersonAccount.person_employment_dtl_id = lobjPARetirement.GetEmploymentDetailID();
            lobjPARetirement.LoadPersonEmploymentDetail();
            lobjPARetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPARetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjPARetirement.LoadPersonAccountForRetirementPlan();
            lobjPARetirement.LoadRetirementContributionAll();
            lobjPARetirement.LoadNewRetirementBenefitCalculation();
            lobjPARetirement.AssignRetirementBenefitCalculationFields(busConstant.CalculationTypeFinal);
            lobjPARetirement.CalculateRetirementBenefitAmount(true);
            lobjPARetirement.CalculateAccruedBenefit(lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.termination_date, lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.retirement_date);

            //Setting the FAS to first available FAS from Retirement or Disability
            //If FAS in Retirement is 0 then set computed FAS from Disability
            lobjPARetirement.idecFAS = lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary == 0 ?
                lobjPARetirement.ibusDisabilityBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary :
                lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.computed_final_average_salary;

            //UAT PIR - 1991
            lobjPARetirement.idecAccruedBenefit = lobjPARetirement.ibusRetirementBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;
            lobjPARetirement.idecDisability = lobjPARetirement.ibusDisabilityBenefitCalculation.icdoBenefitCalculation.unreduced_benefit_amount;

            //Setting the RHIC Benefit to first available RHIC Benefit from Retirement or Disability
            //If RHIC Benefit in Retirement is 0 then set idecReducedRHICAmount from Disability
            lobjPARetirement.idecAccruedRHICBenefit = lobjPARetirement.ibusRetirementBenefitCalculation.idecReducedRHICAmount == 0 ?
                lobjPARetirement.ibusDisabilityBenefitCalculation.idecReducedRHICAmount :
                lobjPARetirement.ibusRetirementBenefitCalculation.idecReducedRHICAmount;

            lobjPARetirement.LoadTotalVSC();
            lobjPARetirement.LoadTotalPSC(); // PIR ID 2187 - Earlier, in Calculation method loads the PSC as of Retirement date
            lobjPARetirement.CalculateERVestedPercentage();//UAT PIR 1239
            return lobjPARetirement;
        }

        public busPersonAddress NewPersonAddress(int Aintpersonid, string ablnIsValidateNewTrue)
        {
            busPersonAddress lobjPersonAddress = new busPersonAddress();
            lobjPersonAddress.icdoPersonAddress = new cdoPersonAddress();
            lobjPersonAddress.icdoPersonAddress.person_id = Aintpersonid;
            lobjPersonAddress.icdoPersonAddress.start_date = DateTime.Now.Date; //PIR 23744
            //systest pir 2323
            lobjPersonAddress.icdoPersonAddress.addr_state_value = busConstant.StateNorthDakota;
            lobjPersonAddress.icdoPersonAddress.addr_country_value = busConstant.US_Code_ID;
            lobjPersonAddress.LoadOtherAddressses();
            lobjPersonAddress.LoadPerson();
            return lobjPersonAddress;
        }
        public busPersonAddress FindPersonAddress(int Aintpersonaddressid)
        {
            busPersonAddress lobjPersonAddress = new busPersonAddress();
            if (lobjPersonAddress.FindPersonAddress(Aintpersonaddressid))
            {
                lobjPersonAddress.LoadPerson();
                lobjPersonAddress.LoadOtherAddressses();
                lobjPersonAddress.LoadCounty();
            }

            return lobjPersonAddress;
        }
        public busPersonContact NewPersonContact(int Aintpersonid, string ablnIsValidateNewTrue)
        {
            busPersonContact lobjPersonContact = new busPersonContact();
            lobjPersonContact.icdoPersonContact = new cdoPersonContact();
            lobjPersonContact.icdoPersonContact.person_id = Aintpersonid;
            lobjPersonContact.LoadOtherContacts();
            lobjPersonContact.LoadPerson();
            lobjPersonContact.LoadContactName();
            return lobjPersonContact;
        }

        public busPersonContact FindPersonContact(int Aintpersoncontactid)
        {
            busPersonContact lobjPersonContact = new busPersonContact();
            if (lobjPersonContact.FindPersonContact(Aintpersoncontactid))
            {
                lobjPersonContact.icdoPersonContact.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjPersonContact.icdoPersonContact.contact_org_id);
                lobjPersonContact.LoadOtherContacts();
                lobjPersonContact.LoadPerson();
                lobjPersonContact.LoadContactName();
                lobjPersonContact.LoadErrors();
            }
            return lobjPersonContact;
        }

        public busPersonLookup LoadPersons(DataTable adtbSearchResult)
        {
            busPersonLookup lobjPersonLookup = new busPersonLookup();
            lobjPersonLookup.LoadPerson(adtbSearchResult);
            return lobjPersonLookup;
        }
        public busPersonBeneficiary NewPersonBeneficiary(int AintPersonID, string AstrParentPageID, int AintDROApplicationID)
        {
            busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
            lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
            lobjPersonBeneficiary.iblnIsNewMode = true;
            lobjPersonBeneficiary.icdoPersonBeneficiary.person_id = AintPersonID;
            lobjPersonBeneficiary.LoadPerson();
            lobjPersonBeneficiary.LoadPersonAccountBeneficiary();
            lobjPersonBeneficiary.LoadBenefitApplicationForBene();
            lobjPersonBeneficiary.LoadOtherBeneficiaries();
            if (AstrParentPageID == busConstant.PersonMaintenance)
            {
                lobjPersonBeneficiary.LoadOtherBeneficiaries();
                lobjPersonBeneficiary.istrParentPage = busConstant.PersonMaintenance;
            }
            else if (AstrParentPageID == busConstant.DROApplicationMaintenance)
            {
                lobjPersonBeneficiary.istrParentPage = busConstant.DROApplicationMaintenance;
                lobjPersonBeneficiary.iintDROApplicationID = AintDROApplicationID;
            }
            lobjPersonBeneficiary.EvaluateInitialLoadRules();
            return lobjPersonBeneficiary;
        }
        public busPersonBeneficiary FindPersonBeneficiary(int AintPersonBeneficiaryID, string AstrParentPageID, int AintDROApplicationID)
        {
            busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
            if (lobjPersonBeneficiary.FindPersonBeneficiary(AintPersonBeneficiaryID))
            {
                lobjPersonBeneficiary.LoadPerson();
                lobjPersonBeneficiary.LoadPersonAccountBeneficiary();
                lobjPersonBeneficiary.LoadPersonAccountBeneficiaryData();
                lobjPersonBeneficiary.LoadOtherBeneficiaries();
                if (AstrParentPageID == busConstant.PersonMaintenance)
                {
                    lobjPersonBeneficiary.istrParentPage = busConstant.PersonMaintenance;
                    //lobjPersonBeneficiary.LoadOtherBeneficiaries();
                }
                else if (AstrParentPageID == busConstant.DROApplicationMaintenance)
                {
                    lobjPersonBeneficiary.istrParentPage = busConstant.DROApplicationMaintenance;
                    lobjPersonBeneficiary.iintDROApplicationID = AintDROApplicationID;
                }
                lobjPersonBeneficiary.LoadBeneficiaryInfo();
                //this is used to generate correspondence
                lobjPersonBeneficiary.ibusPerson.LoadPersonOverviewBeneficiary(true);
                lobjPersonBeneficiary.LoadBenefitApplicationForBene();
            }

            return lobjPersonBeneficiary;
        }

        public busPersonDependent FindPersonDependent(int AintPersonDependentID)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent();
            if (lobjPersonDependent.FindPersonDependent(AintPersonDependentID))
            {
                lobjPersonDependent.LoadPerson();
                lobjPersonDependent.LoadOtherDependents();
                lobjPersonDependent.LoadDependentInfo();
                lobjPersonDependent.LoadPersonAccountDependentUpdate();
            }
            return lobjPersonDependent;
        }
        public busPersonDependent NewPersonDependent(int AintPersonID)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent();
            lobjPersonDependent.icdoPersonDependent = new cdoPersonDependent();
            lobjPersonDependent.icdoPersonDependent.person_id = AintPersonID;
            lobjPersonDependent.LoadPerson();
            lobjPersonDependent.LoadOtherDependents();
            lobjPersonDependent.LoadPersonAccountDependentNew();
            return lobjPersonDependent;
        }

        public busServicePurchaseHeader FindServicePurchaseHeader(int Aintservicepurchaseheaderid)
        {
            busServicePurchaseHeader lobjServicePurchaseHeader = new busServicePurchaseHeader();
            if (lobjServicePurchaseHeader.FindServicePurchaseHeader(Aintservicepurchaseheaderid))
            {
                lobjServicePurchaseHeader.LoadPlan();
                lobjServicePurchaseHeader.LoadServicePurchaseDetail();
                lobjServicePurchaseHeader.LoadErrors();
                lobjServicePurchaseHeader.LoadAmortizationSchedule();
                lobjServicePurchaseHeader.EvaluateInitialLoadRules();
                lobjServicePurchaseHeader.LoadAvailableRemittances();
                lobjServicePurchaseHeader.LoadUnPostedPaymentAllcoation();
                lobjServicePurchaseHeader.LoadPerson();
                lobjServicePurchaseHeader.LoadNotes();
                lobjServicePurchaseHeader.LoadPersonAccount();
                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadBenefitMultiplierTierData();
                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadTierPercentage();
                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadRHICMultiplierData();
                lobjServicePurchaseHeader.LoadInPaymnetServicePurchases(); //PIR 20022
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.suppress_warnings_flag = busConstant.Flag_No;
            }

            return lobjServicePurchaseHeader;
        }

        public busServicePurchaseHeader NewServicePurchaseHeader ( int aintServicePurchaseHeaderId, string astrServiceCreditHeaderType, int aintPersonID)
        
        {
            // This method has been modified for business rules 
            busServicePurchaseHeader lobjServicePurchaseHeader;
            if (aintServicePurchaseHeaderId == 0)
            {
                lobjServicePurchaseHeader = new busServicePurchaseHeader();
                lobjServicePurchaseHeader.icdoServicePurchaseHeader = new cdoServicePurchaseHeader();
                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail = new busServicePurchaseDetail();
                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail =
                    new cdoServicePurchaseDetail();
                lobjServicePurchaseHeader.ibusPersonAccount = new busPersonAccount();
                lobjServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();

                // Populate the default values for this screen
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase = System.DateTime.Now;
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value = astrServiceCreditHeaderType;
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.person_id = aintPersonID;
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, astrServiceCreditHeaderType);

                lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Pending;
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(324, lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value);
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_value = busConstant.Service_Purchase_Status_Review;
                lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(323, lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_value);

                lobjServicePurchaseHeader.icdoServicePurchaseHeader.payor_value = busConstant.Service_Purchase_Payor_Employee;
                lobjServicePurchaseHeader.LoadPerson();
                lobjServicePurchaseHeader.LoadNotes();
                lobjServicePurchaseHeader.EvaluateInitialLoadRules();

            }
            else
            {                
                lobjServicePurchaseHeader = new busServicePurchaseHeader();                
                if (lobjServicePurchaseHeader.FindServicePurchaseHeader(aintServicePurchaseHeaderId))
                {
                    lobjServicePurchaseHeader.LoadPlan();
                    lobjServicePurchaseHeader.LoadServicePurchaseDetail();
                    lobjServicePurchaseHeader.LoadPerson();
                    lobjServicePurchaseHeader.LoadNotes();
                    if (lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail != null)
                    {
                        lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.service_purchase_detail_id = 0;
                        lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.service_purchase_header_id = 0;
                        lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.ienuObjectState = ObjectState.Insert;
                        lobjServicePurchaseHeader.iarrChangeLog.Add(lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail);

                        lobjServicePurchaseHeader.LoadPersonAccount();

                        switch (lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value)
                        {
                            case busConstant.Service_Purchase_Type_Consolidated_Purchase:
                                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailConsolidated();
                                foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                                {
                                    lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_consolidated_detail_id = 0;
                                    lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_detail_id = 0;
                                    lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.ienuObjectState = ObjectState.Insert;
                                }

                                break;
                            case busConstant.Service_Purchase_Type_USERRA_Military_Service:
                                lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.LoadServicePurchaseDetailUSERRA();
                                foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailUserra)
                                {
                                    lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.service_purchase_userra_detail_id = 0;
                                    lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.service_purchase_detail_id = 0;
                                    lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.ienuObjectState = ObjectState.Insert;
                                }
                                break;
                        }
                    }
                    lobjServicePurchaseHeader.LoadInPaymnetServicePurchases();//PIR 20022
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.ienuObjectState = ObjectState.Insert;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id = 0;
                    lobjServicePurchaseHeader.iarrChangeLog.Add(lobjServicePurchaseHeader.icdoServicePurchaseHeader);
                    //PIR 748
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Pending;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(324, lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value);

                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_value = busConstant.Service_Purchase_Status_Review;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(323, lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_status_value);

                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.prorated_psc = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.prorated_vsc = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_service_credit = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_free_service_credit = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_rhic_cost_amount = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_retirement_ee_cost_amount = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_retirement_er_cost_amount = 0M;
                    //lobjServicePurchaseHeader.icdoServicePurchaseHeader.paid_contract_amount_used = 0M;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.delinquent_letter1_sent_flag = busConstant.Flag_No;
                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.delinquent_letter2_sent_flag = busConstant.Flag_No;
                    lobjServicePurchaseHeader.EvaluateInitialLoadRules();
                }
            }

            return lobjServicePurchaseHeader;
        }

        public busServicePurchaseDetail FindServicePurchaseDetail(int Aintservicepurchasedetailid)
        {
            busServicePurchaseDetail lobjServicePurchaseDetail = new busServicePurchaseDetail();
            if (lobjServicePurchaseDetail.FindServicePurchaseDetail(Aintservicepurchasedetailid))
            {
            }
            return lobjServicePurchaseDetail;
        }

        public busServicePurchaseDetailConsolidated FindServicePurchaseDetailConsolidated(int Aintservicepurchaseconsolidateddetailid)
        {
            busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated = new busServicePurchaseDetailConsolidated();
            if (lobjServicePurchaseDetailConsolidated.FindServicePurchaseDetailConsolidated(Aintservicepurchaseconsolidateddetailid))
            {
                lobjServicePurchaseDetailConsolidated.LoadOrganization();
                lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.istrOrgCodeId =
                    lobjServicePurchaseDetailConsolidated.ibusOrganization.icdoOrganization.org_code;
                lobjServicePurchaseDetailConsolidated.LoadServicePurchaseDetail();
                lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.LoadServicePurchaseHeader();
                lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
                lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPlan();
                lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPersonAccount();
            }

            return lobjServicePurchaseDetailConsolidated;
        }

        public busServicePurchaseDetailConsolidated NewServicePurchaseDetailConsolidated(int aintServicePurchaseDetailId)
        {
            busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated =
                new busServicePurchaseDetailConsolidated();
            lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated =
                new cdoServicePurchaseDetailConsolidated();
            lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_detail_id =
                aintServicePurchaseDetailId;
            lobjServicePurchaseDetailConsolidated.LoadServicePurchaseDetail();
            lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.LoadServicePurchaseHeader();
            lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
            lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPlan();
            lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPersonAccount();
            return lobjServicePurchaseDetailConsolidated;
        }

        public busServicePurchaseDetailUserra FindServicePurchaseDetailUserra(int Aintservicepurchaseuserradetailid)
        {
            busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra = new busServicePurchaseDetailUserra();
            if (lobjServicePurchaseDetailUserra.FindServicePurchaseDetailUserra(Aintservicepurchaseuserradetailid))
            {
                lobjServicePurchaseDetailUserra.LoadServicePurchaseDetail();
                lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.LoadServicePurchaseHeader();
                lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
                lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPlan();
                lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPersonAccount();
            }
            return lobjServicePurchaseDetailUserra;
        }

        public busServicePurchaseDetailUserra NewServicePurchaseDetailUserra(int aintServicePurchaseDetailId)
        {
            busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra =
                new busServicePurchaseDetailUserra();
            lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra =
                new cdoServicePurchaseDetailUserra();
            lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.service_purchase_detail_id =
                aintServicePurchaseDetailId;
            lobjServicePurchaseDetailUserra.LoadServicePurchaseDetail();
            lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.LoadServicePurchaseHeader();
            lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
            lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPlan();
            lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPersonAccount();
            lobjServicePurchaseDetailUserra.EvaluateInitialLoadRules();
            return lobjServicePurchaseDetailUserra;
        }


        public busServicePurchaseHeaderLookup LoadServicePurchaseHeaders(DataTable adtbSearchResult)
        {
            busServicePurchaseHeaderLookup lobjServicePurchaseHeaderLookup = new busServicePurchaseHeaderLookup();
            lobjServicePurchaseHeaderLookup.LoadServicePurchaseHeader(adtbSearchResult);
            return lobjServicePurchaseHeaderLookup;
        }

        public busServiceCreditPlanFormulaRef FindServiceCreditPlanFormulaRef(int Aintservicecreditplanformularefid)
        {
            busServiceCreditPlanFormulaRef lobjServiceCreditPlanFormulaRef = new busServiceCreditPlanFormulaRef();
            if (lobjServiceCreditPlanFormulaRef.FindServiceCreditPlanFormulaRef(Aintservicecreditplanformularefid))
            {
            }

            return lobjServiceCreditPlanFormulaRef;
        }
        public busPersonEmployment NewPersonEmployment(int Aintpersonid, string ablnIsValidateNewTrue)
        {
            busPersonEmployment lobjPersonEmployment = new busPersonEmployment();
            lobjPersonEmployment.icdoPersonEmployment = new cdoPersonEmployment();
            lobjPersonEmployment.icdoPersonEmployment.person_id = Aintpersonid;
            lobjPersonEmployment.LoadPerson();
            lobjPersonEmployment.LoadOtherEmployment();
            lobjPersonEmployment.EvaluateInitialLoadRules();
            return lobjPersonEmployment;
        }

        public busPersonEmployment FindPersonEmployment(int Aintpersonemploymentid)
        {
            busPersonEmployment lobjPersonEmployment = new busPersonEmployment();
            if (lobjPersonEmployment.FindPersonEmployment(Aintpersonemploymentid))
            {
                lobjPersonEmployment.LoadPerson();
                lobjPersonEmployment.icdoPersonEmployment.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjPersonEmployment.icdoPersonEmployment.org_id);
                lobjPersonEmployment.LoadOrganization();
                lobjPersonEmployment.LoadPersonEmploymentDetail();
                lobjPersonEmployment.LoadOtherEmployment();
                // PIR-8298
                lobjPersonEmployment.LoadLastPaycheckDate();
                // PIR-8298 end
                lobjPersonEmployment.icdoPersonEmployment.new_start_date = lobjPersonEmployment.icdoPersonEmployment.start_date;
                lobjPersonEmployment.LoadACAEligibilityCertification();
            }
            return lobjPersonEmployment;
        }

        public busPersonEmploymentDetail NewPersonEmploymentDetail(int Aintemploymentid)
        {
            string ldtEmploymentStartDate;
            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id = Aintemploymentid;
            lobjPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonEmploymentDetail.ibusPersonEmployment.LoadPerson();
            lobjPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            //prod pir - 4091
            //lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusContributing;
            lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_description =
                 iobjPassInfo.isrvDBCache.GetCodeDescriptionString(310, lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value);
            ldtEmploymentStartDate = lobjPersonEmploymentDetail.EmploymentDetailsExists();
            if (ldtEmploymentStartDate != string.Empty)
            {
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = Convert.ToDateTime(ldtEmploymentStartDate);
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(1);
            }
            else
            {
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date = lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
            }
            lobjPersonEmploymentDetail.LoadPlansOffered();
            return lobjPersonEmploymentDetail;
        }
        public busPersonEmploymentDetail FindPersonEmploymentDetail(int Aintemploymentdetailid)
        {
            busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail();
            if (lobjPersonEmploymentDetail.FindPersonEmploymentDetail(Aintemploymentdetailid))
            {
                lobjPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonEmploymentDetail.ibusPersonEmployment.LoadPerson();
                lobjPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPersonEmploymentDetail.ibusPersonEmployment.ibusPerson.LoadPersonAccount();
                lobjPersonEmploymentDetail.LoadPlansByEmployer();
                lobjPersonEmploymentDetail.LoadPlansOffered();
                lobjPersonEmploymentDetail.LoadPersonAccountEmploymentDetailWithInsuranceFilter();
                lobjPersonEmploymentDetail.InsertNewOfferedPlansIfAvailable();
                lobjPersonEmploymentDetail.SetActivePlansByEmployer();
                lobjPersonEmploymentDetail.LoadMemberType();
               
                // PIR-8298
                if (lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.date_of_last_regular_paycheck == DateTime.MinValue )
                    lobjPersonEmploymentDetail.LoadLastPaycheckDate();
                // PIR-8298 end

                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.new_job_class_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value;
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.new_official_list_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.official_list_value;
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.new_status_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value;
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.new_type_value = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            }
            return lobjPersonEmploymentDetail;
        }
        public busPersonAccount NewPersonAccount(int Aintpersonid)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccount.icdoPersonAccount.person_id = Aintpersonid;
            return lobjPersonAccount;
        }
        public busPersonAccount FindPersonAccount(int Aintpersonaccountid)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (lobjPersonAccount.FindPersonAccount(Aintpersonaccountid))
            {
            }
            return lobjPersonAccount;
        }
        public busPersonAccountLtc NewPersonAccountLtc(int AintPersonID, int Aintemploymentdtlid)
        {
            busPersonAccountLtc lobjPersonAccountLtc = new busPersonAccountLtc();
            lobjPersonAccountLtc.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountLtc.ibusPaymentElection = new busPersonAccountPaymentElection();
            lobjPersonAccountLtc.ibusPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            lobjPersonAccountLtc.icdoPersonAccount.person_id = AintPersonID;
            lobjPersonAccountLtc.icdoPersonAccount.plan_id = busConstant.PlanIdLTC;
            lobjPersonAccountLtc.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountLtc.LoadPerson();
            lobjPersonAccountLtc.LoadPlan();
            lobjPersonAccountLtc.LoadMemberAge();
            if (Aintemploymentdtlid != 0)
            {
                lobjPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id = Aintemploymentdtlid;
                lobjPersonAccountLtc.LoadPersonEmploymentDetail();
                lobjPersonAccountLtc.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountLtc.LoadOrgPlan();
                lobjPersonAccountLtc.LoadProviderOrgPlan();
            }
            else
            {
                lobjPersonAccountLtc.LoadActiveProviderOrgPlan(lobjPersonAccountLtc.icdoPersonAccount.current_plan_start_date_no_null);
            }
            lobjPersonAccountLtc.LoadLtcOptionNewMember();
            lobjPersonAccountLtc.LoadLtcOptionNewSpouse();
            if (lobjPersonAccountLtc.iintSpousePERSLinkID != 0)
            {
                lobjPersonAccountLtc.LoadSpouseAge(lobjPersonAccountLtc.iintSpousePERSLinkID);
            }
            return lobjPersonAccountLtc;
        }
        public busPersonAccountLtc FindPersonAccountLtc(int AintPersonAccountID)
        {
            busPersonAccountLtc lobjPersonAccountLtc = new busPersonAccountLtc();
            if (lobjPersonAccountLtc.FindPersonAccount(AintPersonAccountID))
            {
                lobjPersonAccountLtc.LoadPerson();
                lobjPersonAccountLtc.LoadPlan();
                lobjPersonAccountLtc.LoadPaymentElection();
                lobjPersonAccountLtc.LoadInsuranceYTD();
                lobjPersonAccountLtc.LoadPlanEffectiveDate();
                lobjPersonAccountLtc.LoadMemberAge(lobjPersonAccountLtc.idtPlanEffectiveDate);
                lobjPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccountLtc.GetEmploymentDetailID();
                if (lobjPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lobjPersonAccountLtc.LoadPersonEmploymentDetail();
                    lobjPersonAccountLtc.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lobjPersonAccountLtc.LoadOrgPlan(lobjPersonAccountLtc.idtPlanEffectiveDate);
                    lobjPersonAccountLtc.LoadProviderOrgPlan(lobjPersonAccountLtc.idtPlanEffectiveDate);
                }
                else
                {
                    lobjPersonAccountLtc.LoadActiveProviderOrgPlan(lobjPersonAccountLtc.idtPlanEffectiveDate);
                }
                lobjPersonAccountLtc.LoadLtcOptionUpdateMember();
                lobjPersonAccountLtc.LoadLtcOptionUpdateSpouse();
                if (lobjPersonAccountLtc.iintSpousePERSLinkID != 0)
                {
                    lobjPersonAccountLtc.LoadSpouseAge(lobjPersonAccountLtc.iintSpousePERSLinkID, lobjPersonAccountLtc.idtPlanEffectiveDate);
                }
                lobjPersonAccountLtc.GetMonthlyPremiumAmount();
                lobjPersonAccountLtc.LoadAllPersonEmploymentDetails();
                lobjPersonAccountLtc.LoadLtcOptionHistory();
                lobjPersonAccountLtc.LoadErrors();
                lobjPersonAccountLtc.LoadPreviousHistory();
                lobjPersonAccountLtc.LoadPersonAccountAchDetail();
                lobjPersonAccountLtc.LoadPersonAccountInsuranceTransfer();
            }
            // UAT PIR ID 997 - Change effective date to be blank
            lobjPersonAccountLtc.icdoPersonAccount.history_change_date = DateTime.MinValue;
            lobjPersonAccountLtc.RefreshValues();

            lobjPersonAccountLtc.LoadPersonAccount();
            return lobjPersonAccountLtc;
        }
        public busPersonAccountRetirement NewPersonAccountRetirement(int AintEmploymentDetailID, string AstrIsDBPlan, string AstrIsDCPlan)
        {
            busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
            lobjPersonAccountRetirement.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
            lobjPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = AintEmploymentDetailID;
            lobjPersonAccountRetirement.LoadPersonEmploymentDetail();
            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPlansOffered();
            lobjPersonAccountRetirement.icdoPersonAccount.person_id = lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lobjPersonAccountRetirement.LoadPerson();
            lobjPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
            if (AstrIsDBPlan == busConstant.Flag_Yes)
            {
                DataTable ldtbList = busPersonAccountHelper.DeterminePlan(lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                    lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value);
                /*LE Plans - There is a chance for returning more than one row. In that case,We need to determin
                 * based on employer offered plans*/
                if (ldtbList.Rows.Count > 1)
                {
                    foreach (DataRow dr in ldtbList.Rows)
                    {
                        var lintPlanId = from lobj in lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrganizationOfferedPlans
                                         where lobj.icdoOrgPlan.plan_id == (int)dr["plan_id"]
                                         select Convert.ToInt32(dr["plan_id"]);
                        if (!lintPlanId.IsNullOrEmpty())
                        {
                            lobjPersonAccountRetirement.icdoPersonAccount.plan_id = lintPlanId.FirstOrDefault();
                            break;
                        }
                    }
                }
                else
                {
                    lobjPersonAccountRetirement.icdoPersonAccount.plan_id = (int)ldtbList.Rows[0]["plan_id"];
                }

                if (lobjPersonAccountRetirement.IsDCPlanEligible() && lobjPersonAccountRetirement.IsDcEligibilityDateRequired() // PIR 11483
                    && (lobjPersonAccountRetirement.IsPopulateDCEligibilityDate(lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date))) // PIR 11891
                {
                    lobjPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date =
                                                                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                }
            }
            else if (AstrIsDCPlan == busConstant.Flag_Yes)
            {
                //PIR 20232
                if (lobjPersonAccountRetirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                    lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();

                //Check the Election Value as Enrolled in Person Employment Detail
                foreach (busPersonAccountEmploymentDetail lobjPAEmploymentDetail in lobjPersonAccountRetirement.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl)
                {
                    if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC &&
                        lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                    {
                        lobjPersonAccountRetirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC;
                    }
                    else if (lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdDC2020 &&
                        lobjPAEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                    {
                        lobjPersonAccountRetirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC2020;
                    }
                }
            }
            //PIR 25920 New HB Button logic.
            else
            {
                lobjPersonAccountRetirement.icdoPersonAccount.plan_id = busConstant.PlanIdDC2025;
            }
            lobjPersonAccountRetirement.LoadPlan();
            lobjPersonAccountRetirement.LoadOrgPlan();
            //UAT PIR - 1661
            lobjPersonAccountRetirement.SetQDRO();
            lobjPersonAccountRetirement.icolPersonAccountRetirementHistory = new Collection<busPersonAccountRetirementHistory>(); //pir 8285
            lobjPersonAccountRetirement.iblnIsEnrollmentValidationApplicable = true;
            lobjPersonAccountRetirement.EvaluateInitialLoadRules();
            return lobjPersonAccountRetirement;
        }
        public busPersonAccountRetirement FindPersonAccountRetirement(int AintPersonAccountID)
        {
            busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
            if (lobjPersonAccountRetirement.FindPersonAccountRetirement(AintPersonAccountID))
            {
                lobjPersonAccountRetirement.LoadPlan();
                lobjPersonAccountRetirement.LoadPlanEffectiveDate();
                lobjPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccountRetirement.GetEmploymentDetailID();
                lobjPersonAccountRetirement.LoadPersonEmploymentDetail();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountRetirement.LoadPerson();
                lobjPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPersonAccountRetirement.LoadOrgPlan(lobjPersonAccountRetirement.idtPlanEffectiveDate);
                lobjPersonAccountRetirement.LoadFYTDSummary();
                lobjPersonAccountRetirement.LoadLTDSummary();
                lobjPersonAccountRetirement.LoadMissedDeposits();
                lobjPersonAccountRetirement.LoadAllPersonEmploymentDetails();
                lobjPersonAccountRetirement.LoadDBDCTransfer();
                lobjPersonAccountRetirement.LoadPersonAccountRetirementHistory();
                lobjPersonAccountRetirement.LoadPersonAccountAdjustment();
                lobjPersonAccountRetirement.LoadPersonAccountDBDBTransfer(true);
                lobjPersonAccountRetirement.LoadErrors();
                lobjPersonAccountRetirement.LoadPreviousHistory();
                lobjPersonAccountRetirement.CalculateERVestedPercentage();
                lobjPersonAccountRetirement.LoadTotalPSC();
                //PIR 9546 - Capital gain to be shown on pension maintenance
                lobjPersonAccountRetirement.LoadCapitalGainContribution();
                lobjPersonAccountRetirement.LoadADECAmountZero();   //PIR 25920 DC 2025 changes
            }
            lobjPersonAccountRetirement.icdoPersonAccount.history_change_date = DateTime.MinValue;  // UAT PIR ID 997
            lobjPersonAccountRetirement.LoadPersonAccountForRetirementPlan();
            lobjPersonAccountRetirement.idtInitialStartDate = lobjPersonAccountRetirement.icdoPersonAccount.start_date;//Display Start date on screen
            //UAT PIR - 1661
            lobjPersonAccountRetirement.SetQDRO();
            lobjPersonAccountRetirement.RefreshValues();
            lobjPersonAccountRetirement.iblnIsEnrollmentValidationApplicable = true;
            return lobjPersonAccountRetirement;
        }
		//PIR 25920 Droplist Refresh from DatePicker
        public object GetADECAmountValuesByEffectiveDate(DateTime astrHistoryChangeDate, int aintPersonEmploymentDtlId, int aintPersonAccountId)
        {
            Collection<cdoCodeValue> lclcADCEAmountOptions = new Collection<cdoCodeValue>();
            Collection<busCustomCodeValue> lclcADECAmountValueOptions = new Collection<busCustomCodeValue>();
            object lobjPersonAccountRetirement = busMainBase.GetObjectFromDB("wfmPensionPlanMaintenance", aintPersonAccountId);
            if (lobjPersonAccountRetirement is busPersonAccountRetirement)
            {
                //((busPersonAccountRetirement)lobjPersonAccountRetirement).icdoPersonAccount.plan_id = aintPlanId;
                lclcADCEAmountOptions = ((busPersonAccountRetirement)lobjPersonAccountRetirement).GetADECAmountValuesByEffectiveDate(astrHistoryChangeDate, aintPersonEmploymentDtlId, aintPersonAccountId);
            }
            foreach (cdoCodeValue lcdocodeValue in lclcADCEAmountOptions)
            {
                lclcADECAmountValueOptions.Add(new busCustomCodeValue() { code_value = Convert.ToString(lcdocodeValue.code_value), description = Convert.ToString(lcdocodeValue.code_id) });
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(lclcADECAmountValueOptions);
        }
        public busPersonAccountRetirementHistory FindPersonAccountRetirementHistory(int Aintpersonaccountretirementhistoryid)
        {
            busPersonAccountRetirementHistory lobjPersonAccountRetirementHistory = new busPersonAccountRetirementHistory();
            if (lobjPersonAccountRetirementHistory.FindPersonAccountRetirementHistory(Aintpersonaccountretirementhistoryid))
            {
            }
            return lobjPersonAccountRetirementHistory;
        }

        public busPersonAccountDeferredComp NewPersonAccountDeferredComp(int AintEmploymentDetailID, string astrIs457)
        {
            busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            lobjPersonAccountDeferredComp.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp();
            lobjPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = AintEmploymentDetailID;
            if (astrIs457 == busConstant.Flag_Yes)
            {
                lobjPersonAccountDeferredComp.icdoPersonAccount.plan_id = busConstant.PlanIdDeferredCompensation;
            }
            else
            {
                lobjPersonAccountDeferredComp.icdoPersonAccount.plan_id = busConstant.PlanIdOther457;
            }
            lobjPersonAccountDeferredComp.LoadPlan();
            lobjPersonAccountDeferredComp.LoadPersonEmploymentDetail();
            lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonAccountDeferredComp.icdoPersonAccount.person_id = lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lobjPersonAccountDeferredComp.LoadPerson();
            lobjPersonAccountDeferredComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusDefCompEnrolled;
            lobjPersonAccountDeferredComp.LoadOrgPlan();
            lobjPersonAccountDeferredComp.Set457Limit();
            lobjPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider = new Collection<busPersonAccountDeferredCompProvider>(); // PROD PIR 8170
            lobjPersonAccountDeferredComp.iblnIsEnrollmentValidationApplicable = true;
            return lobjPersonAccountDeferredComp;
        }
        public busPersonAccountDeferredComp FindPersonAccountDeferredComp(int AintPersonAccountID)
        {
            busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp();
            if (lobjPersonAccountDeferredComp.FindPersonAccountDeferredComp(AintPersonAccountID))
            {
                lobjPersonAccountDeferredComp.LoadPlan();
                lobjPersonAccountDeferredComp.LoadPlanEffectiveDate();
                lobjPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccountDeferredComp.GetEmploymentDetailID();
                lobjPersonAccountDeferredComp.LoadPersonEmploymentDetail();
                lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountDeferredComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPersonAccountDeferredComp.LoadPerson();
                lobjPersonAccountDeferredComp.LoadOrgPlan(lobjPersonAccountDeferredComp.idtPlanEffectiveDate);
                lobjPersonAccountDeferredComp.LoadPersonAccountProviderInfo();
                lobjPersonAccountDeferredComp.LoadPADeffCompHistory();
                lobjPersonAccountDeferredComp.LoadContributionSummary();
                lobjPersonAccountDeferredComp.LoadErrors();
                lobjPersonAccountDeferredComp.LoadPreviousHistory();
                lobjPersonAccountDeferredComp.LoadAllPersonEmploymentDetails();

                //UCS- 041 Load Transfer detials of this person account
                lobjPersonAccountDeferredComp.LoadDefCompTransferDetails();
            }
            lobjPersonAccountDeferredComp.icdoPersonAccount.history_change_date = DateTime.MinValue; // UAT PIR ID 997
            lobjPersonAccountDeferredComp.LoadPersonAccount();
            lobjPersonAccountDeferredComp.RefreshValues();
            lobjPersonAccountDeferredComp.Set457Limit();
            lobjPersonAccountDeferredComp.iblnIsEnrollmentValidationApplicable = true;
            return lobjPersonAccountDeferredComp;
        }
        public busPersonAccountDeferredCompProvider NewPersonAccountDeffCompProvider(int Aintpersonaccountid, int AintEmpDtlID)
        {
            busPersonAccountDeferredCompProvider lobjPersonAccountDeffCompProvider = new busPersonAccountDeferredCompProvider();
            lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider();
            lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.person_account_id = Aintpersonaccountid;
            lobjPersonAccountDeffCompProvider.LoadPersonAccountDeferredComp();
            lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadPerson();
            lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = AintEmpDtlID;
            lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadPersonEmploymentDetail();
            lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan();
            lobjPersonAccountDeffCompProvider.EvaluateInitialLoadRules();
            return lobjPersonAccountDeffCompProvider;
        }
        public busPersonAccountDeferredCompProvider FindPersonAccountDeffCompProvider(int Aintpersonaccountproviderid, int AintEmpDtlID)
        {
            busPersonAccountDeferredCompProvider lobjPersonAccountDeffCompProvider = new busPersonAccountDeferredCompProvider();
            if (lobjPersonAccountDeffCompProvider.FindPersonAccountDeferredCompProvider(Aintpersonaccountproviderid))
            {
                lobjPersonAccountDeffCompProvider.LoadPersonAccountDeferredComp();
                //UAT PIR 1916                
                lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadPersonAccount();
                lobjPersonAccountDeffCompProvider.LoadProviderOrgPlan();
                lobjPersonAccountDeffCompProvider.ibusProviderOrgPlan.LoadOrganization();
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode =
                    lobjPersonAccountDeffCompProvider.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_code;
                lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadPerson();

                lobjPersonAccountDeffCompProvider.LoadPersonEmployment();
                lobjPersonAccountDeffCompProvider.ibusPersonEmployment.LoadOrganization();
                lobjPersonAccountDeffCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan(
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.start_date,  //PROD PIR ID 4737
                lobjPersonAccountDeffCompProvider.ibusPersonEmployment.icdoPersonEmployment.org_id);

                lobjPersonAccountDeffCompProvider.LoadProviderAgentOrgContact();
                lobjPersonAccountDeffCompProvider.ibusProviderAgentOrgContact.LoadContact();

                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_person_employment_id =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.person_employment_id;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_provider_agent_contact_id =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.provider_agent_contact_id;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_per_pay_period_contribution_amt =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_istrProviderOrgCode =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.istrProviderOrgCode;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_mutual_fund_window_flag =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.mutual_fund_window_flag;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_start_date =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.start_date;
                lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.new_end_date =
                    lobjPersonAccountDeffCompProvider.icdoPersonAccountDeferredCompProvider.end_date;
            }
            return lobjPersonAccountDeffCompProvider;
        }
        public busPersonAccountDeferredCompHistory FindPersonAccountDeferredCompHistory(int Aintpersonaccountdeferredcomphistoryid)
        {
            busPersonAccountDeferredCompHistory lobjPersonAccountDeferredCompHistory = new busPersonAccountDeferredCompHistory();
            if (lobjPersonAccountDeferredCompHistory.FindPersonAccountDeferredCompHistory(Aintpersonaccountdeferredcomphistoryid))
            {
            }
            return lobjPersonAccountDeferredCompHistory;
        }

        public busPersonAccountGhdv NewPersonAccountGhdv(int AintPersonID, int AintEmploymentDetailID, int AintPlanid)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv();
            lobjPersonAccountGhdv.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountGhdv.icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
            lobjPersonAccountGhdv.ibusPaymentElection = new busPersonAccountPaymentElection();
            lobjPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            lobjPersonAccountGhdv.icdoPersonAccount.person_id = AintPersonID;
            lobjPersonAccountGhdv.icdoPersonAccount.plan_id = AintPlanid;
            lobjPersonAccountGhdv.LoadPerson();
            lobjPersonAccountGhdv.LoadPlan();

            //Initialize the Org Object to Avoid the NULL error
            lobjPersonAccountGhdv.InitializeObjects();

            if (lobjPersonAccountGhdv.IsHealthOrMedicare)
            {
                //Retiee Member - Defaulting the Health Insurance Type and Plan Option
                if (lobjPersonAccountGhdv.ibusPerson.IsRetiree())
                {
                    lobjPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value = string.Empty;

                    if (lobjPersonAccountGhdv.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
                    else
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.MedicarePartDInsuranceTypeRetiree;
                }
            }

            lobjPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id_from_screen = AintEmploymentDetailID;

            lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(DateTime.Now, true);

            if (lobjPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id > 0)
            {
                //Defaulting Health Insurance Type, Plan Option for Active Member / COBRA member, Dependent Coverage
                if (lobjPersonAccountGhdv.IsHealthOrMedicare)
                {
                    //Dependent COBRA must follow what member has
                    if (lobjPersonAccountGhdv.iblnIsDependentCobra)
                    {
                        if (lobjPersonAccountGhdv.ibusMemberGHDVForDependent != null)
                        {
                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value =
                                lobjPersonAccountGhdv.ibusMemberGHDVForDependent.icdoPersonAccountGhdv.plan_option_value;

                            lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value =
                                lobjPersonAccountGhdv.ibusMemberGHDVForDependent.icdoPersonAccountGhdv.health_insurance_type_value;
                        }
                    }
                    else
                    {
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.plan_option_value = lobjPersonAccountGhdv.ibusOrgPlan.icdoOrgPlan.plan_option_value;
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = lobjPersonAccountGhdv.ibusOrgPlan.HealthInsuranceType;
                    }
                }
            }

            //Load the Code Description
            lobjPersonAccountGhdv.LoadHealthPlanOptionDescription();
            lobjPersonAccountGhdv.EvaluateInitialLoadRules();
            lobjPersonAccountGhdv.LoadPersonAccountGHDVHistory();
            lobjPersonAccountGhdv.iblnIsFromInternal = true;//PIR 17962 
            lobjPersonAccountGhdv.iblnIsEnrollmentValidationApplicable = true;
            lobjPersonAccountGhdv.LoadCorObjects(); //PIR 19355
            lobjPersonAccountGhdv.LoadPersonAccountGHDVHsa();  //19997
            return lobjPersonAccountGhdv;
        }

        /*********************************
         * If You Make any changes here, 
         * you need to make changes in LoadInsuranceSummary Method in busPerson Object.
         * ******************************/
        public busPersonAccountGhdv FindPersonAccountGhdv(int AintPersonAccountID)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv();
            if (lobjPersonAccountGhdv.FindPersonAccount(AintPersonAccountID))
            {
                if (lobjPersonAccountGhdv.FindGHDVByPersonAccountID(AintPersonAccountID))
                {
                    lobjPersonAccountGhdv.LoadPerson();
                    lobjPersonAccountGhdv.LoadPlan();
                    lobjPersonAccountGhdv.LoadWorkersCompensation();
                    lobjPersonAccountGhdv.LoadOtherCoverageDetails();
                    lobjPersonAccountGhdv.LoadPaymentElection();
                    lobjPersonAccountGhdv.LoadBillingOrganization();
                    lobjPersonAccountGhdv.LoadPersonAccountGHDVHistory();
                    lobjPersonAccountGhdv.LoadInsuranceYTD();

                    //Initialize the Org Object to Avoid the NULL error
                    lobjPersonAccountGhdv.InitializeObjects();

                    //PIR 12986 -- If type is not retiree then load employer org plan and if type is retiree then load provider org plan.
                    if ((lobjPersonAccountGhdv.istrIsPlanHealth == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value != busConstant.HealthInsuranceTypeRetiree)
                        || (lobjPersonAccountGhdv.istrIsPlanDental == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeRetiree)
                        || (lobjPersonAccountGhdv.istrIsPlanVision == busConstant.Flag_Yes && lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeRetiree))
                    {
                        lobjPersonAccountGhdv.LoadPlanEffectiveDate();
                        lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGhdv.idtPlanEffectiveDate, false);
                    }
                    else
                    {
                        lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(DateTime.Now);
                    }

                    if (lobjPersonAccountGhdv.IsHealthOrMedicare)
                    {
                        lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGhdv.icdoPersonAccount.current_plan_start_date_no_null, false);//15752
                        if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                        {
                            lobjPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                        }
                        else
                        {
                            //Load the Health Plan Participation Date (based on effective Date)
                            lobjPersonAccountGhdv.LoadHealthParticipationDate();
                            //To Get the Rate Structure Code (Derived Field)
                            lobjPersonAccountGhdv.LoadRateStructure();
                        }
                        //Get the Coverage Ref ID
                        lobjPersonAccountGhdv.LoadCoverageRefID();

                        lobjPersonAccountGhdv.GetMonthlyPremiumAmountByRefID();
                    }
                    else
                    {
                        //PIR 17842 - Employment detail was not loaded for Dental and Vision
                        lobjPersonAccountGhdv.DetermineEnrollmentAndLoadObjects(lobjPersonAccountGhdv.icdoPersonAccount.current_plan_start_date_no_null, false);
                        lobjPersonAccountGhdv.GetMonthlyPremiumAmount();
                    }
                    lobjPersonAccountGhdv.LoadErrors();
                    lobjPersonAccountGhdv.LoadAllPersonEmploymentDetails();
                    lobjPersonAccountGhdv.iintPreviousEPOProviderOrgID = lobjPersonAccountGhdv.icdoPersonAccountGhdv.epo_org_id;
                    lobjPersonAccountGhdv.LoadPreviousHistory();
                    lobjPersonAccountGhdv.LoadPersonAccountAchDetail();
                    lobjPersonAccountGhdv.LoadPersonAccountInsuranceTransfer();
                    //PIR 12737 & 8565 -- added payment election history
                    lobjPersonAccountGhdv.LoadPaymentElectionHistory();
                }
                lobjPersonAccountGhdv.LoadEnrolledPlanAndPremiumForPersonAccountGHDV(); // PIR 6919
            }
            // UAT PIR ID 997 - Change effective date to be blank
            lobjPersonAccountGhdv.icdoPersonAccount.history_change_date = DateTime.MinValue;
            lobjPersonAccountGhdv.RefreshValues();
            lobjPersonAccountGhdv.LoadPersonAccount();
            lobjPersonAccountGhdv.iblnIsFromInternal = true;//PIR 17962 
            lobjPersonAccountGhdv.iblnIsEnrollmentValidationApplicable = true;
            lobjPersonAccountGhdv.LoadCorObjects(); //PIR 19355
            lobjPersonAccountGhdv.LoadPersonAccountGHDVHsa();  //19997
            lobjPersonAccountGhdv.LoadPaymentElectionAdjustment();
            return lobjPersonAccountGhdv;
        }

        public busPersonAccountOtherCoverageDetail NewOtherCoverageDetail(int AintPersonAccountID, int AintPersonID)
        {
            busPersonAccountOtherCoverageDetail lobjOtherCoverage = new busPersonAccountOtherCoverageDetail();
            lobjOtherCoverage.icdoPersonAccount = new cdoPersonAccount();
            lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail = new cdoPersonAccountOtherCoverageDetail();
            lobjOtherCoverage.icdoPersonAccount.person_id = AintPersonID;
            lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.person_account_id = AintPersonAccountID;
            lobjOtherCoverage.FindPersonAccount(AintPersonAccountID);
            lobjOtherCoverage.LoadPlan();
            lobjOtherCoverage.LoadPerson();
            return lobjOtherCoverage;
        }

        public busPersonAccountOtherCoverageDetail FindOtherCoverageDetail(int AintOtherCoverageDetailID)
        {
            busPersonAccountOtherCoverageDetail lobjOtherCoverage = new busPersonAccountOtherCoverageDetail();
            if (lobjOtherCoverage.FindPersonAccountOtherCoverageDetail(AintOtherCoverageDetailID))
            {
                lobjOtherCoverage.FindPersonAccount(lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.person_account_id);
                lobjOtherCoverage.LoadPlan();
                lobjOtherCoverage.LoadPerson();
                lobjOtherCoverage.LoadProviderOrgCode();
            }
            return lobjOtherCoverage;
        }

        public busPersonAccountWorkerCompensation NewWorkersCompensation(int AintPersonAccountID, int AintPersonID)
        {
            busPersonAccountWorkerCompensation lobjWorkersCompensation = new busPersonAccountWorkerCompensation();
            lobjWorkersCompensation.icdoPersonAccount = new cdoPersonAccount();
            lobjWorkersCompensation.icdoPersonAccountWorkerCompensation = new cdoPersonAccountWorkerCompensation();
            lobjWorkersCompensation.icdoPersonAccount.person_id = AintPersonID;
            lobjWorkersCompensation.icdoPersonAccountWorkerCompensation.person_account_id = AintPersonAccountID;
            lobjWorkersCompensation.FindPersonAccount(AintPersonAccountID);
            lobjWorkersCompensation.LoadPlan();
            lobjWorkersCompensation.LoadPerson();
            return lobjWorkersCompensation;
        }

        public busPersonAccountWorkerCompensation FindWorkersCompensation(int AintWorkerCompID)
        {
            busPersonAccountWorkerCompensation lobjWorkersCompensation = new busPersonAccountWorkerCompensation();
            if (lobjWorkersCompensation.FindPersonAccountWorkerCompensation(AintWorkerCompID))
            {
                lobjWorkersCompensation.FindPersonAccount(lobjWorkersCompensation.icdoPersonAccountWorkerCompensation.person_account_id);
                lobjWorkersCompensation.LoadPerson();
                lobjWorkersCompensation.LoadPlan();
                lobjWorkersCompensation.LoadProviderOrgCode();
            }
            return lobjWorkersCompensation;
        }

        public busPersonAccountLife NewGroupLife(int AintPersonID, int AintEmploymentDetailID)
        {
            busPersonAccountLife lobjGroupLife = new busPersonAccountLife();
            lobjGroupLife.icdoPersonAccount = new cdoPersonAccount();
            lobjGroupLife.icdoPersonAccountLife = new cdoPersonAccountLife();
            lobjGroupLife.icdoPersonAccount.person_id = AintPersonID;
            lobjGroupLife.icdoPersonAccount.plan_id = busConstant.PlanIdGroupLife;
            lobjGroupLife.ibusPaymentElection = new busPersonAccountPaymentElection();
            lobjGroupLife.ibusPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            lobjGroupLife.LoadPerson();
            lobjGroupLife.LoadPlan();
            lobjGroupLife.LoadLifeOption();
            lobjGroupLife.LoadMemberAge();
            lobjGroupLife.LoadHistory();
            if (AintEmploymentDetailID != 0)
            {
                lobjGroupLife.icdoPersonAccount.person_employment_dtl_id = AintEmploymentDetailID;
                lobjGroupLife.LoadPersonEmploymentDetail();
                lobjGroupLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjGroupLife.LoadOrgPlan();
                lobjGroupLife.LoadProviderOrgPlan();
            }
            else
            {
                lobjGroupLife.LoadActiveProviderOrgPlan(lobjGroupLife.icdoPersonAccount.current_plan_start_date_no_null);
            }
            lobjGroupLife.iblnIsEnrollmentValidationApplicable = true;
            return lobjGroupLife;
        }

        public busPersonAccountLife FindGroupLife(int AintPersonAccountID)
        {
            busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife();
            if (lobjPersonAccountLife.FindPersonAccount(AintPersonAccountID))
            {
                if (lobjPersonAccountLife.FindPersonAccountLife(AintPersonAccountID))
                {
                    lobjPersonAccountLife.LoadPerson();
                    lobjPersonAccountLife.LoadPlan();
                    lobjPersonAccountLife.LoadLifeOption();
                    lobjPersonAccountLife.LoadLifeOptionData();
                    lobjPersonAccountLife.LoadPlanEffectiveDate();
                    lobjPersonAccountLife.LoadMemberAge(lobjPersonAccountLife.idtPlanEffectiveDate);
                    lobjPersonAccountLife.LoadHistory();
                    lobjPersonAccountLife.LoadProviderName();
                    lobjPersonAccountLife.LoadPaymentElection();
                    lobjPersonAccountLife.LoadBillingOrganization();
                    lobjPersonAccountLife.LoadInsuranceYTD();
                    lobjPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccountLife.GetEmploymentDetailID();
                    if (lobjPersonAccountLife.icdoPersonAccount.person_employment_dtl_id != 0)
                    {
                        lobjPersonAccountLife.LoadPersonEmploymentDetail();
                        lobjPersonAccountLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                        //PIR 2052 : Load the Org Plan by Plan Effective Date (Transfer Employment Scenario)
                        lobjPersonAccountLife.LoadOrgPlan(lobjPersonAccountLife.idtPlanEffectiveDate);
                        lobjPersonAccountLife.LoadProviderOrgPlan(lobjPersonAccountLife.idtPlanEffectiveDate);
                    }
                    else
                    {
                        lobjPersonAccountLife.LoadActiveProviderOrgPlan(lobjPersonAccountLife.idtPlanEffectiveDate);
                    }
                    if (lobjPersonAccountLife.icdoPersonAccountLife.premium_waiver_flag != busConstant.Flag_Yes)
                    {
                        lobjPersonAccountLife.GetMonthlyPremiumAmount();
                    }
                    lobjPersonAccountLife.LoadErrors();
                    lobjPersonAccountLife.LoadPreviousHistory();
                    lobjPersonAccountLife.LoadAllPersonEmploymentDetails();
                    lobjPersonAccountLife.LoadPersonAccountAchDetail();
                    lobjPersonAccountLife.LoadPersonAccountInsuranceTransfer();
                    //PIR 12737 & 8565 -- added payment election history
                    lobjPersonAccountLife.LoadPaymentElectionHistory();
                }
            }
            // UAT PIR ID 997 - Change effective date to be blank
            lobjPersonAccountLife.icdoPersonAccount.history_change_date = DateTime.MinValue;
            lobjPersonAccountLife.RefreshValues();
            lobjPersonAccountLife.LoadPersonAccount();
            lobjPersonAccountLife.iblnIsEnrollmentValidationApplicable = true;
            return lobjPersonAccountLife;
        }

        public busPersonAccountEAP NewEAP(int AintEmploymentDetailID)
        {
            busPersonAccountEAP lobjEAP = new busPersonAccountEAP();
            lobjEAP.icdoPersonAccount = new cdoPersonAccount();
            lobjEAP.icdoPersonAccount.person_employment_dtl_id = AintEmploymentDetailID;
            lobjEAP.LoadPersonEmploymentDetail();
            lobjEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjEAP.icdoPersonAccount.person_id = lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lobjEAP.icdoPersonAccount.start_date = new DateTime(lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddMonths(1).Year,
                                                  lobjEAP.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date.AddMonths(1).Month, 1);
            lobjEAP.icdoPersonAccount.plan_id = busConstant.PlanIdEAP;
            lobjEAP.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjEAP.LoadPlan();
            lobjEAP.LoadPerson();
            lobjEAP.LoadOrgPlan();
            //UAT PIR: 971. Loading Providers For EAP in New mode.
            lobjEAP.LoadEapProvidersNewMode();
            lobjEAP.icdoPersonAccount.provider_org_id = lobjEAP.GetDefaultEAPProvider();
            lobjEAP.iblnIsEnrollmentValidationApplicable = true;
            return lobjEAP;
        }
        public busPersonAccountEAP FindEAP(int AintPersonAccountID)
        {
            busPersonAccountEAP lobjEAP = new busPersonAccountEAP();
            if (lobjEAP.FindPersonAccount(AintPersonAccountID))
            {
                lobjEAP.LoadPlanEffectiveDate();
                lobjEAP.icdoPersonAccount.person_employment_dtl_id = lobjEAP.GetEmploymentDetailID();
                lobjEAP.LoadPersonEmploymentDetail();
                lobjEAP.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjEAP.LoadPlan();
                lobjEAP.LoadPerson();
                lobjEAP.LoadOrgPlan(lobjEAP.idtPlanEffectiveDate);
                lobjEAP.LoadProviderOrgPlanByProviderOrgID(lobjEAP.icdoPersonAccount.provider_org_id, lobjEAP.idtPlanEffectiveDate);
                lobjEAP.GetMonthlyPremium();
                lobjEAP.LoadInsuranceYTD();
                lobjEAP.LoadEAPHistory();
                lobjEAP.LoadErrors();
                lobjEAP.LoadPreviousHistory();
                lobjEAP.LoadAllPersonEmploymentDetails();
                lobjEAP.LoadPersonAccountInsuranceTransfer();
                // UAT PIR ID 971
                if (!lobjEAP.IsProviderOrgIDValid())
                    lobjEAP.icdoPersonAccount.provider_org_id = 0;
                lobjEAP.iblnIsEnrollmentValidationApplicable = true;
            }
            // UAT PIR ID 997 - Change effective date to be blank
            lobjEAP.icdoPersonAccount.history_change_date = DateTime.MinValue;
            lobjEAP.RefreshValues();
            lobjEAP.LoadPersonAccountForEAP();
            return lobjEAP;
        }
        public busPersonAccountFlexComp NewPersonAccountFlexComp(int AintPersonID, int Aintemploymentdtlid)
        {
            busPersonAccountFlexComp lobjPersonAccountFlexComp = new busPersonAccountFlexComp();
            lobjPersonAccountFlexComp.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp();
            lobjPersonAccountFlexComp.icdoPersonAccount.person_id = AintPersonID;
            lobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = Aintemploymentdtlid;
            lobjPersonAccountFlexComp.icdoPersonAccount.plan_id = busConstant.PlanIdFlex;
            lobjPersonAccountFlexComp.LoadPlan();
            lobjPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;
            if (Aintemploymentdtlid != 0)
            {
                lobjPersonAccountFlexComp.LoadPersonEmploymentDetail();
                lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountFlexComp.icdoPersonAccount.person_id = lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
                lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                lobjPersonAccountFlexComp.LoadOrgPlan();
            }
            lobjPersonAccountFlexComp.LoadPerson();
            lobjPersonAccountFlexComp.LoadFlexCompOptionNew();
            lobjPersonAccountFlexComp.LoadFlexCompIndividualOptions();//pir 7705
            lobjPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();//pir 7705
            lobjPersonAccountFlexComp.LoadPersonAccountFlexCompConversion();//pir 7705
            lobjPersonAccountFlexComp.LoadPreTaxEnrollmentStatusGrid(); //PIR 26579
            lobjPersonAccountFlexComp.iblnIsEnrollmentValidationApplicable = true;
            lobjPersonAccountFlexComp.EvaluateInitialLoadRules();
            return lobjPersonAccountFlexComp;
        }
        public busPersonAccountFlexComp FindPersonAccountFlexComp(int AintPersonAccountID)
        {
            busPersonAccountFlexComp lobjPersonAccountFlexComp = new busPersonAccountFlexComp();
            if (lobjPersonAccountFlexComp.FindPersonAccountFlexComp(AintPersonAccountID))
            {
                lobjPersonAccountFlexComp.LoadPerson();
                lobjPersonAccountFlexComp.LoadPlan();
                lobjPersonAccountFlexComp.LoadPlanEffectiveDate();

                lobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccountFlexComp.GetEmploymentDetailID();
                if (lobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id != 0)
                {
                    lobjPersonAccountFlexComp.LoadPersonEmploymentDetail();
                    lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lobjPersonAccountFlexComp.LoadOrgPlan(lobjPersonAccountFlexComp.idtPlanEffectiveDate);
                }
                lobjPersonAccountFlexComp.LoadFlexCompOptionUpdate();
                lobjPersonAccountFlexComp.LoadFlexCompConversion();
                lobjPersonAccountFlexComp.LoadFlexCompHistory();
                lobjPersonAccountFlexComp.LoadErrors();
                lobjPersonAccountFlexComp.LoadPreviousHistory();
                lobjPersonAccountFlexComp.LoadAllPersonEmploymentDetails();
				lobjPersonAccountFlexComp.LoadPreTaxEnrollmentStatusGrid(); //PIR 26579
            }
            lobjPersonAccountFlexComp.RefreshValues();
            lobjPersonAccountFlexComp.LoadPersonAccount();
            lobjPersonAccountFlexComp.LoadFlexCompIndividualOptions();//pir 7705
            lobjPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();//pir 7705
            lobjPersonAccountFlexComp.LoadPersonAccountFlexCompConversion();//pir 7705
            lobjPersonAccountFlexComp.LoadModifiedFlexCompHistory();//pir 7705
            lobjPersonAccountFlexComp.iblnIsEnrollmentValidationApplicable = true;
            return lobjPersonAccountFlexComp;
        }

        public busPersonAccountFlexCompOption FindPersonAccountFlexCompOption(int Aintaccountflexcompoptionid)
        {
            busPersonAccountFlexCompOption lobjPersonAccountFlexCompOption = new busPersonAccountFlexCompOption();
            if (lobjPersonAccountFlexCompOption.FindPersonAccountFlexCompOption(Aintaccountflexcompoptionid))
            {

            }
            return lobjPersonAccountFlexCompOption;
        }

        public busPersonAccountLtcOption FindPersonAccountLtcOption(int Aintpersonaccountltcoptionid)
        {
            busPersonAccountLtcOption lobjPersonAccountLtcOption = new busPersonAccountLtcOption();
            if (lobjPersonAccountLtcOption.FindPersonAccountLtcOption(Aintpersonaccountltcoptionid))
            {
                lobjPersonAccountLtcOption.LoadPersonAccount();
            }
            return lobjPersonAccountLtcOption;
        }
        public busPersonAccountFlexcompConversion NewPersonAccountFlexcompConversion(int Aintpersonaccountid, int AintEmpDtlID)
        {
            busPersonAccountFlexcompConversion lobjPersonAccountFlexcompConversion = new busPersonAccountFlexcompConversion();
            lobjPersonAccountFlexcompConversion.icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion();
            lobjPersonAccountFlexcompConversion.icdoPersonAccountFlexcompConversion.person_account_id = Aintpersonaccountid;
            lobjPersonAccountFlexcompConversion.LoadPersonAccount();
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = AintEmpDtlID;
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPerson();
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPlan();
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPersonEmploymentDetail();
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadOrgPlan();
            return lobjPersonAccountFlexcompConversion;
        }

        public busPersonAccountFlexcompConversion FindPersonAccountFlexcompConversion(int Aintpersonaccountflexcompconversionid, int AintEmpDtlID)
        {
            busPersonAccountFlexcompConversion lobjPersonAccountFlexcompConversion = new busPersonAccountFlexcompConversion();
            if (lobjPersonAccountFlexcompConversion.FindPersonAccountFlexcompConversion(Aintpersonaccountflexcompconversionid))
            {
                lobjPersonAccountFlexcompConversion.LoadProvider();
                lobjPersonAccountFlexcompConversion.icdoPersonAccountFlexcompConversion.istrOrgCodeID
                    = lobjPersonAccountFlexcompConversion.ibusProvider.icdoOrganization.org_code;
                lobjPersonAccountFlexcompConversion.LoadPersonAccount();
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPerson();
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPlan();
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = AintEmpDtlID;
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadPersonEmploymentDetail();
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountFlexcompConversion.ibusPersonAccount.LoadOrgPlan();
            }
            return lobjPersonAccountFlexcompConversion;
        }

        public busPersonAccountLtcOptionHistory FindPersonAccountLtcOptionHistory(int Aintpersonaccountltcoptionhistoryid)
        {
            busPersonAccountLtcOptionHistory lobjPersonAccountLtcOptionHistory = new busPersonAccountLtcOptionHistory();
            if (lobjPersonAccountLtcOptionHistory.FindPersonAccountLtcOptionHistory(Aintpersonaccountltcoptionhistoryid))
            {

            }
            return lobjPersonAccountLtcOptionHistory;
        }
        public busPersonAccountDeferredCompContribution FindDefCompContributions(int AintPrimarydefId ,int AintPersonAccountID, string astrIsCYTD)
        {
            busPersonAccountDeferredCompContribution lobjDefCompContribution = new busPersonAccountDeferredCompContribution();
            lobjDefCompContribution.icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution();
            lobjDefCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id = AintPersonAccountID;
            lobjDefCompContribution.LoadPersonAccountDeferredComp();
            lobjDefCompContribution.ibusPADeferredComp.LoadPerson();
            lobjDefCompContribution.ibusPADeferredComp.LoadPlan();
            if (astrIsCYTD == busConstant.Flag_Yes)
            {
                lobjDefCompContribution.iblnIsCYTDFlag = true;
                lobjDefCompContribution.ibusPADeferredComp.LoadCYTDDetail();
            }
            else
            {
                lobjDefCompContribution.iblnIsCYTDFlag = false;
                lobjDefCompContribution.ibusPADeferredComp.LoadLTDDetail();
            }
            return lobjDefCompContribution;
        }
        public busPersonAccountRetirementContribution FindRetirementContributions(int AintPrimaryID ,int AintPersonAccountID, string astrIsFYTD)
        {
            busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = AintPersonAccountID;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.iintPrimaryId = AintPrimaryID;
            lobjRetirementContribution.LoadPersonAccountRetirement();
            lobjRetirementContribution.ibusPARetirement.LoadPerson();
            lobjRetirementContribution.ibusPARetirement.LoadPlan();
            if (astrIsFYTD == busConstant.Flag_Yes)
            {
                lobjRetirementContribution.iblnIsFYTDFlag = true;
                lobjRetirementContribution.ibusPARetirement.LoadFYTDDetail();
            }
            else
            {
                lobjRetirementContribution.iblnIsFYTDFlag = false;
                lobjRetirementContribution.ibusPARetirement.LoadLTDDetail();
            }
            return lobjRetirementContribution;
        }

        public busPersonAccountInsuranceContribution FindInsuranceContributions(int AintPersonAccountID)
        {
            busPersonAccountInsuranceContribution lobjInsuranceContribution = new busPersonAccountInsuranceContribution();
            lobjInsuranceContribution.icdoPersonAccountInsuranceContribution = new cdoPersonAccountInsuranceContribution();
            lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id = AintPersonAccountID;
            lobjInsuranceContribution.LoadPersonAccount();
            lobjInsuranceContribution.ibusPersonAccount.LoadInsuranceLTD();
            lobjInsuranceContribution.ibusPersonAccount.LoadPerson();
            lobjInsuranceContribution.ibusPersonAccount.LoadPlan();
            lobjInsuranceContribution.LoadPaymentElectionAdjustment();
            return lobjInsuranceContribution;
        }

        public busPersonAccountFlexCompHistory FindPersonAccountFlexCompHistory(int Aintpersonaccountflexcomphistoryid)
        {
            busPersonAccountFlexCompHistory lobjPersonAccountFlexCompHistory = new busPersonAccountFlexCompHistory();
            if (lobjPersonAccountFlexCompHistory.FindPersonAccountFlexCompHistory(Aintpersonaccountflexcomphistoryid))
            {
            }
            return lobjPersonAccountFlexCompHistory;
        }

        public busServiceCreditPlanFormulaRefLookup LoadServiceCreditPlanFormulaRefs(DataTable adtbSearchResult)
        {
            busServiceCreditPlanFormulaRefLookup lobjServiceCreditPlanFormulaRefLookup = new busServiceCreditPlanFormulaRefLookup();
            lobjServiceCreditPlanFormulaRefLookup.LoadServiceCreditPlanFormulaRefs(adtbSearchResult);
            return lobjServiceCreditPlanFormulaRefLookup;
        }

        public busServiceCreditPlanFormulaRef NewServiceCreditPlanFormulaRef(int AintServiceCreditPlanFormulaRefid)
        {
            busServiceCreditPlanFormulaRef lobjServiceCreditPlanFormulaRef = new busServiceCreditPlanFormulaRef();
            lobjServiceCreditPlanFormulaRef.icdoServiceCreditPlanFormulaRef = new cdoServiceCreditPlanFormulaRef();
            return lobjServiceCreditPlanFormulaRef;
        }



        public busPersonAccountEmploymentDetail FindPersonAccountEmploymentDetail(int Aintpersonaccountemploymentdtlid)
        {
            busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
            if (lobjPersonAccountEmploymentDetail.FindPersonAccountEmploymentDetail(Aintpersonaccountemploymentdtlid))
            {
            }

            return lobjPersonAccountEmploymentDetail;
        }

        public busPersonAccountRetirementDbDcTransferEstimate NewPersonDBDCTransferEstimate(int AintPersonAcctID)
        {
            busPersonAccountRetirementDbDcTransferEstimate lobjDBDCTransferEstimate = new busPersonAccountRetirementDbDcTransferEstimate();
            lobjDBDCTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate = new cdoPersonAccountRetirementDbDcTransferEstimate();
            lobjDBDCTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate.person_account_id = AintPersonAcctID;
            lobjDBDCTransferEstimate.LoadPersonAccount();
            lobjDBDCTransferEstimate.ibusPersonAccount.LoadPerson();
            lobjDBDCTransferEstimate.LoadPlan();
            busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();

            return lobjDBDCTransferEstimate;
        }

        public busPersonAccountRetirementDbDcTransferEstimate FindPersonAccountRetirementDbDcTransferEstimate(int Aintdbdctransferestimateid)
        {
            busPersonAccountRetirementDbDcTransferEstimate lobjPersonAccountRetirementDbDcTransferEstimate = new busPersonAccountRetirementDbDcTransferEstimate();
            if (lobjPersonAccountRetirementDbDcTransferEstimate.FindPersonAccountRetirementDbDcTransferEstimate(Aintdbdctransferestimateid))
            {
                lobjPersonAccountRetirementDbDcTransferEstimate.LoadErrors();
                lobjPersonAccountRetirementDbDcTransferEstimate.LoadPersonAccount();
                lobjPersonAccountRetirementDbDcTransferEstimate.ibusPersonAccount.LoadPerson();
                lobjPersonAccountRetirementDbDcTransferEstimate.LoadPlan();
                lobjPersonAccountRetirementDbDcTransferEstimate.idecProjectedAnnualSalary = lobjPersonAccountRetirementDbDcTransferEstimate.icdoPersonAccountRetirementDbDcTransferEstimate.proj_monthly_salary_amount * 12;
            }

            return lobjPersonAccountRetirementDbDcTransferEstimate;
        }
        // Person account Retirement DB/DB Transfer
        public busPersonAccountRetirementDbDbTransfer NewPersonDBDBTransferEstimate(int AintPersonAcctID)
        {
            busPersonAccountRetirementDbDbTransfer lobjDBDBTransfer = new busPersonAccountRetirementDbDbTransfer();
            lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer = new cdoPersonAccountRetirementDbDbTransfer();
            lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.from_person_account_id = AintPersonAcctID;
            lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.status_value = busConstant.TransferStatusValid;
            lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.status_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.TransferStatusCodeID, lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.status_value);
            lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.transfer_date = DateTime.Now;
            lobjDBDBTransfer.LoadFromPersonAccountRetirement();
            lobjDBDBTransfer.ibusFromPersonAccountRetirement.LoadPerson();
            lobjDBDBTransfer.ibusFromPersonAccountRetirement.LoadPlan();
            lobjDBDBTransfer.LoadRetirementDbDbTransferContribution();
            lobjDBDBTransfer.LoadPersonAccountContributionDetail();
            //UCS-041 
            if (String.IsNullOrEmpty(lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.transfer_type_value))
                lobjDBDBTransfer.icdoPersonAccountRetirementDbDbTransfer.transfer_type_value = busConstant.TransferTypeDBToDB;
            return lobjDBDBTransfer;
        }
        public busPersonAccountRetirementDbDbTransfer FindPersonAccountRetirementDbDbTransfer(int aintReferenceID)
        {
            busPersonAccountRetirementDbDbTransfer lobjPersonAccountRetirementDbDbTransfer = new busPersonAccountRetirementDbDbTransfer();
            if (lobjPersonAccountRetirementDbDbTransfer.FindPersonAccountRetirementDbDbTransfer(aintReferenceID))
            {
                lobjPersonAccountRetirementDbDbTransfer.LoadOtherDetails();
                lobjPersonAccountRetirementDbDbTransfer.LoadPersonAccountContributionDetail();

                //UCS- 041 if the status is posted then only show the records that are transfered
                lobjPersonAccountRetirementDbDbTransfer.LoadRetirementDbDbTransferContribution();

                lobjPersonAccountRetirementDbDbTransfer.SetSummaryDataForDisplay();
                lobjPersonAccountRetirementDbDbTransfer.LoadErrors();
                lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.iintTransferToPlanPersonAccountID = 0;
                lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.iintTransferToMemberPersonID = 0;
                if (lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.transfer_type_value == busConstant.TransferTypeMember)
                    lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.iintTransferToMemberPersonID = lobjPersonAccountRetirementDbDbTransfer.ibusToPersonAccountRetirement.ibusPerson.icdoPerson.person_id;
                else
                    lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.iintTransferToPlanPersonAccountID = lobjPersonAccountRetirementDbDbTransfer.icdoPersonAccountRetirementDbDbTransfer.to_person_account_id;
            }
            return lobjPersonAccountRetirementDbDbTransfer;
        }

        // Person account adjustment
        public busPersonAccountAdjustment NewPersonAccountAdjustment(int aintPersonAccountID)
        {
            busPersonAccountAdjustment lobjAdjustment = new busPersonAccountAdjustment();
            lobjAdjustment.icdoPersonAccountAdjustment = new cdoPersonAccountAdjustment();
            lobjAdjustment.icdoPersonAccountAdjustment.person_account_id = aintPersonAccountID;
            lobjAdjustment.icdoPersonAccountAdjustment.transaction_date = DateTime.Now;
            lobjAdjustment.icdoPersonAccountAdjustment.status_value = busConstant.AdjustmentStatusValid;
            lobjAdjustment.icdoPersonAccountAdjustment.status_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.AdjustmentStatusCodeID, lobjAdjustment.icdoPersonAccountAdjustment.status_value);
            lobjAdjustment.LoadPersonAccount();
            return lobjAdjustment;
        }
        public busPersonAccountAdjustment FindPersonAccountAdjustment(int aintAdjustmentID)
        {
            busPersonAccountAdjustment lobjAdjustment = new busPersonAccountAdjustment();
            if (lobjAdjustment.FindPersonAccountAdjustment(aintAdjustmentID))
            {
                lobjAdjustment.LoadPersonAccount();
                lobjAdjustment.LoadRetirementAdjustmentContribution();
            }
            return lobjAdjustment;
        }

        // Person account retirement adjustment contribution
        public busPersonAccountRetirementAdjustmentContribution NewRetirementAdjustmentContribution(int aintAdjustmentID)
        {
            busPersonAccountRetirementAdjustmentContribution lobjAdjustmentContribution = new busPersonAccountRetirementAdjustmentContribution();
            lobjAdjustmentContribution.icdoRetirementAdjustmentContribution = new cdoPersonAccountRetirementAdjustmentContribution();
            lobjAdjustmentContribution.icdoRetirementAdjustmentContribution.person_account_adjustment_id = aintAdjustmentID;
            lobjAdjustmentContribution.icdoRetirementAdjustmentContribution.transaction_date = DateTime.Now;
            lobjAdjustmentContribution.LoadPersonAccountAdjustment();
            return lobjAdjustmentContribution;
        }

        public busPersonAccountRetirementAdjustmentContribution FindRetirementAdjustmentContribution(int aintAdjustmentContributionID)
        {
            busPersonAccountRetirementAdjustmentContribution lobjAdjustmentContribution = new busPersonAccountRetirementAdjustmentContribution();
            if (lobjAdjustmentContribution.FindRetirementAdjustmentContribution(aintAdjustmentContributionID))
            {
                lobjAdjustmentContribution.LoadPersonAccountAdjustment();
            }
            return lobjAdjustmentContribution;
        }
        public busPersonAccountTFFRTIAA NewPersonAccountTFFRTIAA(int AintPersonEmploymentdtlid, int AintPlanid)
        {
            busPersonAccountTFFRTIAA lobjPersonAccountTFFRTIAA = new busPersonAccountTFFRTIAA();
            lobjPersonAccountTFFRTIAA.icdoPersonAccount = new cdoPersonAccount();
            lobjPersonAccountTFFRTIAA.icdoPersonAccount.plan_id = AintPlanid;
            lobjPersonAccountTFFRTIAA.icdoPersonAccount.person_employment_dtl_id = AintPersonEmploymentdtlid;
            lobjPersonAccountTFFRTIAA.LoadPersonEmploymentDetail();
            lobjPersonAccountTFFRTIAA.ibusPersonEmploymentDetail.LoadPersonEmployment();
            lobjPersonAccountTFFRTIAA.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
            lobjPersonAccountTFFRTIAA.icdoPersonAccount.person_id =
                lobjPersonAccountTFFRTIAA.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
            lobjPersonAccountTFFRTIAA.LoadPerson();
            lobjPersonAccountTFFRTIAA.LoadPlan();
            return lobjPersonAccountTFFRTIAA;
        }

        public busPersonAccountTFFRTIAA FindPersonAccountTFFRTIAA(int AintPersonAccountID)
        {
            busPersonAccountTFFRTIAA lobjPersonAccountTFFRTIAA = new busPersonAccountTFFRTIAA();
            if (lobjPersonAccountTFFRTIAA.FindPersonAccount(AintPersonAccountID))
            {
                lobjPersonAccountTFFRTIAA.LoadPlan();
                lobjPersonAccountTFFRTIAA.LoadPersonEmploymentDetail();
                lobjPersonAccountTFFRTIAA.ibusPersonEmploymentDetail.LoadPersonEmployment();
                lobjPersonAccountTFFRTIAA.LoadPerson();
                lobjPersonAccountTFFRTIAA.LoadTFFRTIAAHistory();
            }
            return lobjPersonAccountTFFRTIAA;
        }

        public busPersonAccountTFFRTIAAHistory FindPersonAccountTffrtiaaHistory(int Aintpersonaccounttffrtiaahistoryid)
        {
            busPersonAccountTFFRTIAAHistory lobjPersonAccountTffrtiaaHistory = new busPersonAccountTFFRTIAAHistory();
            if (lobjPersonAccountTffrtiaaHistory.FindPersonAccountTffrtiaaHistory(Aintpersonaccounttffrtiaahistoryid))
            {
            }
            return lobjPersonAccountTffrtiaaHistory;
        }
        public busPersonTffrTiaaService NewTffrTiaaService(int Aintpersonid)
        {
            busPersonTffrTiaaService lobjTffrTiaaService = new busPersonTffrTiaaService();
            lobjTffrTiaaService.icdoPersonTffrTiaaService = new cdoPersonTffrTiaaService();
            lobjTffrTiaaService.icdoPersonTffrTiaaService.person_id = Aintpersonid;
            lobjTffrTiaaService.LoadPerson();
            return lobjTffrTiaaService;
        }
        public busPersonTffrTiaaService FindTffrTiaaService(int AintTffrTIiaaServiceid)
        {
            busPersonTffrTiaaService lobjTffrTiaaService = new busPersonTffrTiaaService();
            if (lobjTffrTiaaService.FindPersonTffrTiaaService(AintTffrTIiaaServiceid))
            {
                //UAT PIR : 791 Rounding the Service Fields to 4 Digits
                //lobjTffrTiaaService.icdoPersonTffrTiaaService.tffr_service =
                //    Math.Round(lobjTffrTiaaService.icdoPersonTffrTiaaService.tffr_service, 4,
                //               MidpointRounding.AwayFromZero);

                //lobjTffrTiaaService.icdoPersonTffrTiaaService.tiaa_service =
                //    Math.Round(lobjTffrTiaaService.icdoPersonTffrTiaaService.tiaa_service, 4,
                //               MidpointRounding.AwayFromZero);

                lobjTffrTiaaService.LoadPerson();
            }
            return lobjTffrTiaaService;
        }

        public busPersonAccountAchDetail NewPersonAccountAchDetail(int Aintpersonaccountid)
        {
            busPersonAccountAchDetail lobjPersonAccountAchDetail = new busPersonAccountAchDetail();
            lobjPersonAccountAchDetail.icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail();
            lobjPersonAccountAchDetail.icdoPersonAccountAchDetail.person_account_id = Aintpersonaccountid;
            lobjPersonAccountAchDetail.icdoPersonAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
            lobjPersonAccountAchDetail.LoadPersonAccount();
            lobjPersonAccountAchDetail.ibusPersonAccount.LoadPerson(); // PIR 15408 - Internal finding while testing the fix for PIR 15408
            return lobjPersonAccountAchDetail;
        }
        public busPersonAccountAchDetail FindPersonAccountAchDetail(int Aintpersonaccountachdetailid)
        {
            busPersonAccountAchDetail lobjPersonAccountAchDetail = new busPersonAccountAchDetail();
            if (lobjPersonAccountAchDetail.FindPersonAccountAchDetail(Aintpersonaccountachdetailid))
            {
                lobjPersonAccountAchDetail.LoadPersonAccount();
                lobjPersonAccountAchDetail.LoadBankOrgByOrgID();
                //uat pir 1463
                lobjPersonAccountAchDetail.icdoPersonAccountAchDetail.org_code = lobjPersonAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
                lobjPersonAccountAchDetail.ibusPersonAccount.LoadPerson();
            }

            return lobjPersonAccountAchDetail;
        }

        # region UCS-041
        public busPersonAccountDeferredCompTransfer FindPersonAccountDeferredCompTransfer(int aintpersonaccountdeferredcomptransferid)
        {
            busPersonAccountDeferredCompTransfer lobjPersonAccountDeferredCompTransfer = new busPersonAccountDeferredCompTransfer();
            if (lobjPersonAccountDeferredCompTransfer.FindPersonAccountDeferredCompTransfer(aintpersonaccountdeferredcomptransferid))
            {
                lobjPersonAccountDeferredCompTransfer.LoadFromPersonAccountDefComp();
                lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.LoadPerson();
                lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.LoadPlan();
                lobjPersonAccountDeferredCompTransfer.LoadToPersonAccountDefComp();
                lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.LoadPerson();
                lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.LoadPlan();
                lobjPersonAccountDeferredCompTransfer.icdoPersonAccountDeferredCompTransfer.iintTransferToPersonId = lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.icdoPersonAccount.person_id;
                lobjPersonAccountDeferredCompTransfer.LoadErrors();
                lobjPersonAccountDeferredCompTransfer.LoadDeferredCompContribution();
                lobjPersonAccountDeferredCompTransfer.LoadDestinationContributionDetails();
                lobjPersonAccountDeferredCompTransfer.LoadTransferedAmount();
            }

            return lobjPersonAccountDeferredCompTransfer;
        }

        public busPersonAccountDeferredCompTransfer NewPersonAccountDeferredCompTransfer(int Aintpersonaccountid)
        {
            busPersonAccountDeferredCompTransfer lobjPersonAccountDeferredCompTransfer = new busPersonAccountDeferredCompTransfer
            {
                icdoPersonAccountDeferredCompTransfer = new cdoPersonAccountDeferredCompTransfer
                {
                    from_person_account_id = Aintpersonaccountid,
                    status_value = busConstant.TransferStatusValid,
                    transfer_date = DateTime.Now,
                },
            };
            lobjPersonAccountDeferredCompTransfer.icdoPersonAccountDeferredCompTransfer.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.TransferStatusCodeID,
                lobjPersonAccountDeferredCompTransfer.icdoPersonAccountDeferredCompTransfer.status_value);
            lobjPersonAccountDeferredCompTransfer.LoadFromPersonAccountDefComp();
            lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.LoadPerson();
            lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.LoadPlan();
            lobjPersonAccountDeferredCompTransfer.GetToPersonAccountID();
            lobjPersonAccountDeferredCompTransfer.LoadToPersonAccountDefComp();
            if (lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.icdoPersonAccount.person_account_id > 0)
            {
                lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.LoadPerson();
                lobjPersonAccountDeferredCompTransfer.ibusToPADefComp.LoadPlan();
            }
            if (lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.iclbDefCompContributionAll.IsNull())
                lobjPersonAccountDeferredCompTransfer.ibusFromPADefComp.LoadDefCompContributionAll();
            lobjPersonAccountDeferredCompTransfer.LoadDeferredCompContribution();
            lobjPersonAccountDeferredCompTransfer.LoadDestinationContributionDetails();

            return lobjPersonAccountDeferredCompTransfer;
        }

        public busPersonAccountInsuranceTransfer NewPersonAccountInsuranceTransfer(int Aintpersonaccountid)
        {
            busPersonAccountInsuranceTransfer lobjPersonAccountInsuranceTransfer = new busPersonAccountInsuranceTransfer
            {
                icdoPersonAccountInsuranceTransfer = new cdoPersonAccountInsuranceTransfer
                {
                    from_person_account_id = Aintpersonaccountid,
                    status_value = busConstant.TransferStatusValid,
                    transfer_date = DateTime.Now,
                },
            };
            lobjPersonAccountInsuranceTransfer.icdoPersonAccountInsuranceTransfer.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.TransferStatusCodeID,
                lobjPersonAccountInsuranceTransfer.icdoPersonAccountInsuranceTransfer.status_value);
            lobjPersonAccountInsuranceTransfer.LoadFromPersonAccount();
            lobjPersonAccountInsuranceTransfer.ibusFromPersonAccount.LoadPerson();
            lobjPersonAccountInsuranceTransfer.ibusFromPersonAccount.LoadPlan();
            lobjPersonAccountInsuranceTransfer.GetToPersonAccountID();
            lobjPersonAccountInsuranceTransfer.LoadToPersonAccount();
            if (lobjPersonAccountInsuranceTransfer.icdoPersonAccountInsuranceTransfer.to_person_account_id > 0)
            {
                lobjPersonAccountInsuranceTransfer.ibusToPersonAccount.LoadPerson();
                lobjPersonAccountInsuranceTransfer.ibusToPersonAccount.LoadPlan();
            }
            lobjPersonAccountInsuranceTransfer.LoadPersonAccountContributionDetail();
            lobjPersonAccountInsuranceTransfer.LoadInsuranceTransferContribution();
            lobjPersonAccountInsuranceTransfer.LoadTransferedAmount();

            return lobjPersonAccountInsuranceTransfer;
        }


        public busPersonAccountInsuranceTransfer FindPersonAccountInsuranceTransfer(int aintpersonaccountinsurancetransferid)
        {
            busPersonAccountInsuranceTransfer lobjPersonAccountInsuranceTransfer = new busPersonAccountInsuranceTransfer();
            if (lobjPersonAccountInsuranceTransfer.FindPersonAccountInsuranceTransfer(aintpersonaccountinsurancetransferid))
            {
                lobjPersonAccountInsuranceTransfer.LoadFromPersonAccount();
                lobjPersonAccountInsuranceTransfer.ibusFromPersonAccount.LoadPerson();
                lobjPersonAccountInsuranceTransfer.ibusFromPersonAccount.LoadPlan();
                lobjPersonAccountInsuranceTransfer.LoadToPersonAccount();
                lobjPersonAccountInsuranceTransfer.ibusToPersonAccount.LoadPerson();
                lobjPersonAccountInsuranceTransfer.ibusToPersonAccount.LoadPlan();
                lobjPersonAccountInsuranceTransfer.icdoPersonAccountInsuranceTransfer.iintTransferToPersonID = lobjPersonAccountInsuranceTransfer.ibusToPersonAccount.icdoPersonAccount.person_id;
                lobjPersonAccountInsuranceTransfer.LoadPersonAccountContributionDetail();
                lobjPersonAccountInsuranceTransfer.LoadInsuranceTransferContribution();
                lobjPersonAccountInsuranceTransfer.LoadTransferedAmount();
            }

            return lobjPersonAccountInsuranceTransfer;
        }
        # endregion

        #region UCS - 092
        public busActuaryFileHeader NewActuaryFileHeader()
        {
            busActuaryFileHeader lobjActuaryFileHeader = new busActuaryFileHeader { icdoActuaryFileHeader = new cdoActuaryFileHeader() };
            return lobjActuaryFileHeader;
        }

        public busActuaryFileHeader FindActuaryFileHeader(int aintactuaryfileheaderid)
        {
            busActuaryFileHeader lobjActuaryFileHeader = new busActuaryFileHeader();
            if (lobjActuaryFileHeader.FindActuaryFileHeader(aintactuaryfileheaderid))
            {
            }

            return lobjActuaryFileHeader;
        }

        public busActuaryFilePensionDetail FindActuaryFilePensionDetail(int aintactuaryfilepensiondetailid)
        {
            busActuaryFilePensionDetail lobjActuaryFilePensionDetail = new busActuaryFilePensionDetail();
            if (lobjActuaryFilePensionDetail.FindActuaryFilePensionDetail(aintactuaryfilepensiondetailid))
            {
            }

            return lobjActuaryFilePensionDetail;
        }

        public busActuaryFileRhicDetail FindActuaryFileRhicDetail(int aintactuaryfilerhicdetailid)
        {
            busActuaryFileRhicDetail lobjActuaryFileRhicDetail = new busActuaryFileRhicDetail();
            if (lobjActuaryFileRhicDetail.FindActuaryFileRhicDetail(aintactuaryfilerhicdetailid))
            {
            }

            return lobjActuaryFileRhicDetail;
        }
        #endregion

        public busCafrReportBatchRequest FindCafrReportBatchRequest(int aintcafrreportbatchrequestid)
        {
            busCafrReportBatchRequest lobjCafrReportBatchRequest = new busCafrReportBatchRequest();
            if (lobjCafrReportBatchRequest.FindCafrReportBatchRequest(aintcafrreportbatchrequestid))
            {
            }

            return lobjCafrReportBatchRequest;
        }

        public busCafrReportBatchRequest NewCAFRReportBatchRequest()
        {
            busCafrReportBatchRequest lobjCafrReportBatchRequest = new busCafrReportBatchRequest { icdoCafrReportBatchRequest = new cdoCafrReportBatchRequest() };
            return lobjCafrReportBatchRequest;
        }

        public busCAFRReportBatchRequestLookup LoadCAFRReportBatchRequests(DataTable adtbSearchResult)
        {
            busCAFRReportBatchRequestLookup lobjCAFRReportBatchRequestLookup = new busCAFRReportBatchRequestLookup();
            lobjCAFRReportBatchRequestLookup.LoadCafrReportBatchRequests(adtbSearchResult);
            return lobjCAFRReportBatchRequestLookup;
        }

        public busDuplicatePersonScreen GetDuplicatePersons(int aintPersonID, string astrLastName, DateTime adteDateofBirth, string astrGenderValue,
                                                                                int aintScreenIdentifier, int aintOrgID, string astrSSN)
        {
            busDuplicatePersonScreen lobjDuplicatePerson = new busDuplicatePersonScreen();
            lobjDuplicatePerson.LoadDuplicatePersons(aintPersonID, astrLastName, adteDateofBirth, astrGenderValue, aintScreenIdentifier, aintOrgID, astrSSN);
            return lobjDuplicatePerson;
        }


        public busPerson ESSFindPerson(int aintpersonid, int aintOrgId, int aintContactID)
        {
            SetWebParameters();
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(aintpersonid))
            {
                lobjPerson.iintOrgID = aintOrgId != 0 ? aintOrgId : iintOrgID;
                lobjPerson.OrgID = aintOrgId;
                lobjPerson.iblnIsFromESS = true;
                lobjPerson.iintContactID = aintContactID != 0 ? aintContactID : iintContactID;
                lobjPerson.icdoPerson.temp_ssn = lobjPerson.icdoPerson.ssn;
                lobjPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
                lobjPerson.ESSLoadPersonEmployment();
                lobjPerson.ibusESSPersonEmployment.LoadOrganization();
                lobjPerson.LoadEmploymentChangeRequestDetails();
                lobjPerson.ESSLoadPersonAccountForEnrolledPlans();
                lobjPerson.LoadESSEmploymentChangeRequestDetails();
                lobjPerson.LoadACAEligibilityCertifications(lobjPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id);
                lobjPerson.EvaluateInitialLoadRules();
            }
            return lobjPerson;
        }
        public busIbsCheckEntryHeader NewbsCheckEntryHeader()
        {
            busIbsCheckEntryHeader lobjIbsCheckEntryHeader = new busIbsCheckEntryHeader { icdoIbsCheckEntryHeader = new cdoIbsCheckEntryHeader() };
            lobjIbsCheckEntryHeader.icdoIbsCheckEntryHeader.deposit_method_value = busConstant.DepositTapeMethodCheck;
            lobjIbsCheckEntryHeader.icdoIbsCheckEntryHeader.deposit_method_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1501, lobjIbsCheckEntryHeader.icdoIbsCheckEntryHeader.deposit_method_value);
            lobjIbsCheckEntryHeader.icdoIbsCheckEntryHeader.deposit_date = DateTime.Today;
            lobjIbsCheckEntryHeader.icdoIbsCheckEntryHeader.status_value = busConstant.IBSCheckEntryHeaderStatusReview;
            return lobjIbsCheckEntryHeader;
        }
        public busIbsCheckEntryHeader FindIbsCheckEntryHeader(int aintibscheckentryheaderid)
        {
            busIbsCheckEntryHeader lobjIbsCheckEntryHeader = new busIbsCheckEntryHeader();
            if (lobjIbsCheckEntryHeader.FindIbsCheckEntryHeader(aintibscheckentryheaderid))
            {  //PIR 6617
                lobjIbsCheckEntryHeader.DisplayIbsCheckEntryDetail();
                //lobjIbsCheckEntryHeader.LoadIbsCheckEntryDetail();
                lobjIbsCheckEntryHeader.LoadTotalFromDetails();
            }
            return lobjIbsCheckEntryHeader;
        }
        public busIbsCheckEntryDetail NewIbsCheckEntryDetail(int aintibscheckentryheaderid)
        {
            busIbsCheckEntryDetail lobjIbsCheckEntryDetail = new busIbsCheckEntryDetail { icdoIbsCheckEntryDetail = new cdoIbsCheckEntryDetail() };
            lobjIbsCheckEntryDetail.icdoIbsCheckEntryDetail.ibs_check_entry_header_id = aintibscheckentryheaderid;
            lobjIbsCheckEntryDetail.icdoIbsCheckEntryDetail.payment_date = DateTime.Today;
            lobjIbsCheckEntryDetail.LoadIbsCheckEntryHeader();
            //lobjIbsCheckEntryDetail.ibusIbsCheckEntryHeader.LoadIbsCheckEntryDetail();
            //lobjIbsCheckEntryDetail.DisplayIbsCheckEntryDetail();

            return lobjIbsCheckEntryDetail;
        }
        public busIbsCheckEntryDetail FindIbsCheckEntryDetail(int aintibscheckentrydetailid)
        {
            busIbsCheckEntryDetail lobjIbsCheckEntryDetail = new busIbsCheckEntryDetail();
            if (lobjIbsCheckEntryDetail.FindIbsCheckEntryDetail(aintibscheckentrydetailid))
            {
                lobjIbsCheckEntryDetail.LoadIbsCheckEntryHeader();
                lobjIbsCheckEntryDetail.LoadPerson();
                //lobjIbsCheckEntryDetail.ibusIbsCheckEntryHeader.LoadIbsCheckEntryDetail();
                //lobjIbsCheckEntryDetail.DisplayIbsCheckEntryDetail();
                //lobjIbsCheckEntryDetail.icdoIbsCheckEntryDetail.due_amount = lobjIbsCheckEntryDetail.GetDueAmount();
            }
            return lobjIbsCheckEntryDetail;
        }
        public busIbsCheckEntryHeaderLookup LoadIbsCheckEntryHeaders(DataTable adtbSearchResult)
        {
            busIbsCheckEntryHeaderLookup lobjIbsCheckEntryHeaderLookup = new busIbsCheckEntryHeaderLookup();
            lobjIbsCheckEntryHeaderLookup.LoadIbsCheckEntryHeaders(adtbSearchResult);
            return lobjIbsCheckEntryHeaderLookup;
        }
        public busWssEmploymentChangeRequest FindWssEmploymentChangeRequest(int aintEmploymentChangeRequestID)
        {
            busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest = new busWssEmploymentChangeRequest
            {
                icdoWssEmploymentChangeRequest = new cdoWssEmploymentChangeRequest()
            };
            lbusWssEmploymentChangeRequest.ibusContact = new busContact() { icdoContact = new cdoContact() };  //PIR 7952
            lbusWssEmploymentChangeRequest.FindWssEmploymentChangeRequest(aintEmploymentChangeRequestID);
            if (!string.IsNullOrEmpty(lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.is_on_teaching_contract_value))
                lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.is_on_teaching_contract_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(311, lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.is_on_teaching_contract_value);
            lbusWssEmploymentChangeRequest.LoadPerson();
            lbusWssEmploymentChangeRequest.LoadOrganization();
            lbusWssEmploymentChangeRequest.ibusPerson.iintOrgID = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.org_id;
            lbusWssEmploymentChangeRequest.ibusPerson.ESSLoadPersonEmployment();
            //For PIR 7952
            lbusWssEmploymentChangeRequest.SetRequestedByDetails();
            if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service != DateTime.MinValue)
            {
                lbusWssEmploymentChangeRequest.GetDefaultDatesForTermination();
                lbusWssEmploymentChangeRequest.LoadCurrentlyEnrolledPlans();
                if (!string.IsNullOrEmpty(lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.posted_in_perslink_by) &&
                    lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.status_value != busConstant.EmploymentChangeRequestStatusProcessed &&
                    lbusWssEmploymentChangeRequest.ibusPerson.ibusESSPersonEmployment.iblnIsHireDateSameMonthAsTerminationDate)
                {
                    if (!lbusWssEmploymentChangeRequest.iblnAreAllPlansNotSuspended)
                    {
                        lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.status_value = busConstant.EmploymentChangeRequestStatusProcessed;
                        lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.Update();
                    }
                }
            }
            return lbusWssEmploymentChangeRequest;
        }

        public busWssMemberRecordRequest FindMemberRecordRequest(int aintMemberRecordRequestID)
        {
            busWssMemberRecordRequest lobjMemberRecordRequest = new busWssMemberRecordRequest();
            lobjMemberRecordRequest.FindWssMemberRecordRequest(aintMemberRecordRequestID);
            return lobjMemberRecordRequest;
        }

        public busHealthPremiumReportBatchRequest NewHealthPremiumReportBatchRequest()
        {
            busHealthPremiumReportBatchRequest lobjBatchRequest = new busHealthPremiumReportBatchRequest
            {
                icdoHealthPremiumReportBatchRequest =
                    new cdoHealthPremiumReportBatchRequest()
            };
            return lobjBatchRequest;
        }

        public busHealthPremiumReportBatchRequest FindHealthPremiumReportBatchRequest(int aintBatchRequestID)
        {
            busHealthPremiumReportBatchRequest lobjBatchRequest = new busHealthPremiumReportBatchRequest
            {
                icdoHealthPremiumReportBatchRequest =
                    new cdoHealthPremiumReportBatchRequest()
            };
            lobjBatchRequest.FindHealthPremiumReportBatchRequest(aintBatchRequestID);
            lobjBatchRequest.istrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lobjBatchRequest.icdoHealthPremiumReportBatchRequest.org_id);
            return lobjBatchRequest;
        }

        public busHealthPremiumReportBatchRequestLookup LoadHealthPremiumReportBatchRequests(DataTable adtbSearchResult)
        {
            busHealthPremiumReportBatchRequestLookup lobjHealthPremiumReportBatchRequestLookup = new busHealthPremiumReportBatchRequestLookup();
            lobjHealthPremiumReportBatchRequestLookup.LoadHealthPremiumReportBatchRequests(adtbSearchResult);
            return lobjHealthPremiumReportBatchRequestLookup;
        }

        //pir 8184
        public busPerson FindPersonContactAppointment(int AintPersonID)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(AintPersonID))
            {
                lobjPerson.LoadContactsAppointments();
            }
            return lobjPerson;
        }

        //pir 8184
        public busPerson FindPersonWorkFlowProcessHistory(int AintPersonID)
        {
            busPerson lobjPerson = new busPerson();
            if (lobjPerson.FindPerson(AintPersonID))
            {
                lobjPerson.LoadWorkflowProcessHistory();
            }
            return lobjPerson;
        }

        public busPersonAccount NewUpdateHistory()
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.icdoPersonAccount = new cdoPersonAccount();
            return lobjPersonAccount;
        }

        public busPersonAccountMedicarePartDHistory NewPersonAccountMedicarePartDDetail(int AintPersonID, int AintEmploymentDtlID)
        {
            busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount = new cdoPersonAccount();

            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.person_id = AintPersonID;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.plan_id = busConstant.PlanIdMedicarePartD;
            lobjPersonAccountMedicarePartDHistory.LoadPerson();
            lobjPersonAccountMedicarePartDHistory.LoadPlan();
            lobjPersonAccountMedicarePartDHistory.iintPersonMemberID = AintPersonID;
            lobjPersonAccountMedicarePartDHistory.LoadMedicarePartDMembers();
            lobjPersonAccountMedicarePartDHistory.LoadHistory();
            lobjPersonAccountMedicarePartDHistory.LoadPaymentElection();
            lobjPersonAccountMedicarePartDHistory.LoadActiveProviderOrgPlan(lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.current_plan_start_date_no_null);

            return lobjPersonAccountMedicarePartDHistory;
        }


        
        //FMUpgrade: Changes for method btnDownloadWelcomeLetter_Click in Default.aspx.cs
           
        protected virtual ArrayList DownLoadWelComeLetter_Click(int aintPersonId,int aintOrgId)
        {
            Hashtable lhstNavigationParam = new Hashtable();
            busPerson lbusPerson = new busPerson(); 
            lbusPerson.icdoPerson = new cdoPerson();
            string lstrGeneratedFileName = string.Empty;
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            ArrayList larrWelcomeLetter = new ArrayList();

            try
            {

                if (lbusPerson.ibusESSPersonEmployment.IsNull())
                {
                    lbusPerson.icdoPerson.person_id = aintPersonId;
                    lbusPerson.iintOrgID = aintOrgId;
                    lbusPerson.ESSLoadPersonEmployment();
                }

                if (lbusPerson.IsNotNull() || lbusPerson.ibusESSPersonEmployment.icdoPersonEmployment.IsNotNull())
                {
                    lstrGeneratedFileName = DownloadWelcomeLetter(aintPersonId, lbusPerson.ibusESSPersonEmployment.icdoPersonEmployment.person_employment_id);
                }
                if (!string.IsNullOrEmpty(lstrGeneratedFileName))
                {
                    if (lstrGeneratedFileName.Contains("Exception"))
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = lstrGeneratedFileName;
                        larrErrors.Add(lobjError);
                        return larrErrors;
                    }

                    byte[] larrFileContent = null;
                    FileInfo lfioFileInfo = new FileInfo(lstrGeneratedFileName);
                    if (lfioFileInfo.IsNotNull() && lfioFileInfo.Exists)
                    {
                        using (FileStream lobjFileStream = lfioFileInfo.OpenRead())
                        {
                            larrFileContent = new byte[lobjFileStream.Length];
                            lobjFileStream.Read(larrFileContent, 0, (int)lobjFileStream.Length);
                        }

                        larrWelcomeLetter.Add(Path.GetFileName(lstrGeneratedFileName));
                        larrWelcomeLetter.Add(larrFileContent);
                        larrWelcomeLetter.Add(busNeoSpinBase.DeriveMimeTypeFromFileName(lstrGeneratedFileName));
                        return larrWelcomeLetter;
                    }
                    
                }
                else
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Error : Welcome Letter Could Not Be Generated.";
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
            }
            catch (Exception ex)
            {
                // Trap the error, if any.
                //Response.Write("Error : " + ex.Message);
            }
            return larrWelcomeLetter;
        }

        // F/W Upgrade PIR 11081 - Added to retrieve person records of the specific organization with which user has logged in.  
        protected override void AddWhereClause(string astrFormName, Collection<utlWhereClause> acolWhereClause)
        {

            if (astrFormName == "wfmESSPersonLookup")
            {
                SetWebParameters();
                utlWhereClause lobjWhereClause = new utlWhereClause();
                lobjWhereClause.istrQueryId = "cdoPerson";
                lobjWhereClause.istrSubSelect = "select emp.*,org.* from sgt_person_employment emp inner join sgt_organization org on org.org_id = emp.org_id where emp.end_date is null and emp.person_id = P.person_id";
                lobjWhereClause.istrFieldName = "org.org_id";
                if (acolWhereClause.Count > 0)
                {
                    lobjWhereClause.istrCondition = "and";
                }
                lobjWhereClause.istrOperator = "=";
                lobjWhereClause.iobjValue1 = iintOrgID;
                acolWhereClause.Add(lobjWhereClause);
            }
            //PIR 24678 combine the phone numbers into one search box that searches home, cell, and work numbers
            if ((astrFormName == "wfmPersonLookup") && acolWhereClause.IsNotNull() && acolWhereClause.Count > 0)
            {
                string lstrPhoneNumber = "";
                foreach (utlWhereClause iobjutlWhereClause in acolWhereClause)
                {
                    if (iobjutlWhereClause.istrControlId == "txtCellPhoneNo1")
                    {
                        lstrPhoneNumber = Convert.ToString(iobjutlWhereClause.iobjValue1);
                        lstrPhoneNumber = lstrPhoneNumber.Contains('%') ? lstrPhoneNumber : '%' + lstrPhoneNumber + '%';

                        iobjutlWhereClause.istrQueryId = "cdoPersonPhone";
                        iobjutlWhereClause.istrSubSelect = "select SPPhone.* from sgt_person SPPhone where SPPhone.person_id = P.person_id and (cell_phone_no like '" + lstrPhoneNumber + "' or work_phone_no like '" + lstrPhoneNumber + "' or home_phone_no like '" + lstrPhoneNumber + "')";

                        //replace the other properties
                        iobjutlWhereClause.iblnSkipParameter = true;
                        iobjutlWhereClause.istrFieldName = "1";
                        iobjutlWhereClause.istrOperator = "=";
                        iobjutlWhereClause.iobjValue1 = "1";
                        //iobjutlWhereClause.istrCondition = "";
                    }
                }
            }

            base.AddWhereClause(astrFormName, acolWhereClause);
        }

        public busPersonAccountMedicarePartDHistory FindPersonAccountMedicarePartDDetail(int AintPersonAccountID)
        {
            busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
            if (lobjPersonAccountMedicarePartDHistory.FindPersonAccount(AintPersonAccountID))
            {
                if (lobjPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountID(AintPersonAccountID))
                {
                    lobjPersonAccountMedicarePartDHistory.LoadPerson();
                    lobjPersonAccountMedicarePartDHistory.LoadPlan();
                    lobjPersonAccountMedicarePartDHistory.LoadMedicarePartDMembers();//For the grid
                    lobjPersonAccountMedicarePartDHistory.LoadHistory();
                    lobjPersonAccountMedicarePartDHistory.LoadPersonAccountMedicarePartDHistory(lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_id);
                    if (lobjPersonAccountMedicarePartDHistory.FindByPrimaryKey(AintPersonAccountID))
                    {
                        lobjPersonAccountMedicarePartDHistory.GetTotalPremiumAmountForMedicareForPapit();
                    }
                    if (lobjPersonAccountMedicarePartDHistory.FindByPrimaryKey(AintPersonAccountID))
                    {
                        lobjPersonAccountMedicarePartDHistory.GetMonthlyPremiumAmountForMedicarePartD();
                    }
                    lobjPersonAccountMedicarePartDHistory.LoadPaymentElection();
                    lobjPersonAccountMedicarePartDHistory.LoadPersonAccountAchDetail();
                    //Internal finding : billing organization not showing on UI after refresh
                    lobjPersonAccountMedicarePartDHistory.LoadBillingOrganization();
                    lobjPersonAccountMedicarePartDHistory.LoadErrors(); //PIR 15408 - Loading soft errors
                    lobjPersonAccountMedicarePartDHistory.LoadInsuranceYTD();
                    lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.history_change_date = DateTime.MinValue;
                    lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.reason_value = string.Empty;
                    lobjPersonAccountMedicarePartDHistory.LoadActiveProviderOrgPlan(lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.current_plan_start_date_no_null);
                    //PIR 12737 & 8565 -- added payment election history
                    lobjPersonAccountMedicarePartDHistory.LoadPaymentElectionHistory();
                }
            }

            return lobjPersonAccountMedicarePartDHistory;
        }

        //Welcome Letter Generation on Button Click Instead of Popup

        public string DownloadWelcomeLetter(int aPersonId, int aPersonEmplId)
        {
            busPerson lbusPerson = new busPerson();
            if (lbusPerson.FindPerson(aPersonId))
            {
                busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPersonEmployment.FindPersonEmployment(aPersonEmplId);
                lbusPersonEmployment.LoadLatestPersonEmploymentDetail();
                lbusPerson.LoadEmployerWelcomeBenefitPlans(lbusPersonEmployment.ibusLatestEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id);
                return lbusPerson.WelcomeLetterCorrespondenceGeneration();
            }
            return string.Empty;
        }
        //19997
        public busPersonAccountGhdvHsa FindMSSGHDVHSA(int aintPersonAccountGhdvHsaId)
        {
            busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa() { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };
            lbusPersonAccountGhdvHsa.iblnIsFromMSS = false;
            if (lbusPersonAccountGhdvHsa.FindGHDVHsaByPersonAccountGhdvHsaID(aintPersonAccountGhdvHsaId))
            {
                if(lbusPersonAccountGhdvHsa.FindPersonAccountGHDV(lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_ghdv_id))
                {
                    if (lbusPersonAccountGhdvHsa.FindPersonAccount(lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_id))
                    {
                        lbusPersonAccountGhdvHsa.LoadGHDVHsaByPersonAccountGhdvID();
                        lbusPersonAccountGhdvHsa.LoadPersonAccount();
                        lbusPersonAccountGhdvHsa.LoadPlanEffectiveDate();
                        lbusPersonAccountGhdvHsa.DetermineEnrollmentAndLoadObjects(lbusPersonAccountGhdvHsa.idtPlanEffectiveDate, false);
                        lbusPersonAccountGhdvHsa.LoadPerson();
                        lbusPersonAccountGhdvHsa.LoadPersonAccountGHDV();
                        lbusPersonAccountGhdvHsa.LoadPersonAccountGHDVHistory();
                    }
                }
            }
            return lbusPersonAccountGhdvHsa;
        }
        public busPersonAccountGhdvHsa NewMSSGHDVHSA(int aintPersonAccountId, int aintPersonAccountGhdvId)
        {
            busPersonAccountGhdvHsa lbusPersonAccountGhdvHsa = new busPersonAccountGhdvHsa() { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };
            lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_ghdv_id = aintPersonAccountGhdvId;
            lbusPersonAccountGhdvHsa.icdoPersonAccountGhdvHsa.person_account_id = aintPersonAccountId;
            lbusPersonAccountGhdvHsa.iblnIsFromMSS = false;
            if (lbusPersonAccountGhdvHsa.FindPersonAccount(aintPersonAccountId))
            {
                if (lbusPersonAccountGhdvHsa.FindGHDVByPersonAccountID(aintPersonAccountId))
                {
                    lbusPersonAccountGhdvHsa.LoadGHDVHsaByPersonAccountGhdvID();
                    lbusPersonAccountGhdvHsa.LoadPersonAccount();
                    lbusPersonAccountGhdvHsa.LoadPlanEffectiveDate();
                    lbusPersonAccountGhdvHsa.DetermineEnrollmentAndLoadObjects(lbusPersonAccountGhdvHsa.idtPlanEffectiveDate, false);
                    lbusPersonAccountGhdvHsa.LoadPerson();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDV();
                    lbusPersonAccountGhdvHsa.LoadPersonAccountGHDVHistory();                  
                }
            }
            return lbusPersonAccountGhdvHsa;
        }
        public busWssEmploymentAcaCert FindACACertificationRequest(int aintWssEmploymentAcaCertId, int aintPersonId, int aintPersonEmpId, int aintPersonEmpDtlId)
        {
            SetWebParameters();
            busWssEmploymentAcaCert lbusWssEmploymentAcaCert = new busWssEmploymentAcaCert { icdoWssEmploymentAcaCert = new cdoWssEmploymentAcaCert() };
            lbusWssEmploymentAcaCert.iintOrgId = iintOrgID;
            lbusWssEmploymentAcaCert.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            lbusWssEmploymentAcaCert.ibusPerson.FindPerson(aintPersonId);
            lbusWssEmploymentAcaCert.ibusPerson.ibusESSPersonEmployment = new busPersonEmployment() { icdoPersonEmployment = new cdoPersonEmployment() };
            lbusWssEmploymentAcaCert.ibusPerson.ibusESSPersonEmployment.FindPersonEmployment(aintPersonEmpId);
            DateTime ldteSystBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            lbusWssEmploymentAcaCert.istrIsNewHire = ldteSystBatchDate < lbusWssEmploymentAcaCert.ibusPerson.ibusESSPersonEmployment.icdoPersonEmployment.start_date.AddDays(31) ? busConstant.Flag_Yes : busConstant.Flag_No;
            if (aintWssEmploymentAcaCertId > 0)
            {
                lbusWssEmploymentAcaCert.FindWssEmploymentAcaCert(aintWssEmploymentAcaCertId);
                if (lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.method.IsNotNullOrEmpty())
                    lbusWssEmploymentAcaCert.iblnIsCertifyClick = true;
                if (lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeAnnual)
                {
                    lbusWssEmploymentAcaCert.idtAnnualToDate = lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date;
                    lbusWssEmploymentAcaCert.idtAnnualFromDate = lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.from_date;
                }
            }
            else
            {
                lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.person_id = aintPersonId;
                lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.contact_id = iintContactID;
                lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.person_employment_id = aintPersonEmpId;
                lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.per_emp_dtl_id = aintPersonEmpDtlId;
                lbusWssEmploymentAcaCert.iblnIsCertifyClick = false;
                lbusWssEmploymentAcaCert.idtAnnualFromDate = new DateTime(ldteSystBatchDate.Year - 1, 11, 01);
                lbusWssEmploymentAcaCert.idtAnnualToDate = new DateTime(ldteSystBatchDate.Year, 10, 31);
            }
            lbusWssEmploymentAcaCert.EvaluateInitialLoadRules();
            return lbusWssEmploymentAcaCert;
        }
        public busPersonTffrHeader FindPersonTffrHeader(int aintUploadId)
        {
            busPersonTffrHeader lbusPersonTffrHeader = new busPersonTffrHeader { icdoPersonTffrHeader = new doPersonTffrHeader() };
            if(aintUploadId > 0)
            {
                lbusPersonTffrHeader.LoadPersonTffrHeaderFromUploadId(aintUploadId);
                //lbusPersonTffrHeader.istrNotesForTffrHeader = lbusPersonTffrHeader.icdoPersonTffrHeader.notes;
                lbusPersonTffrHeader.LoadPersonTffrDetailsFromUploadId(aintUploadId);
            }

            return lbusPersonTffrHeader;
        }
    }
}
