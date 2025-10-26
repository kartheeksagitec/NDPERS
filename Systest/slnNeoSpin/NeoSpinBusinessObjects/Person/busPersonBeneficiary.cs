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
using System.Web;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonBeneficiary : busPersonBeneficiaryGen
    {
        public bool iblnIsNewMode { get; set; }

        private busPerson _ibusPerson;
        public bool iblnIsRecordSelectToGenerateCor { get; set; }
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }
        private busPerson _ibusBeneficiaryPerson;
        public busPerson ibusBeneficiaryPerson
        {
            get { return _ibusBeneficiaryPerson; }
            set { _ibusBeneficiaryPerson = value; }
        }

        // this is used in death notification screen
        private busOrganization _ibusBeneficiaryOrganization;
        public busOrganization ibusBeneficiaryOrganization
        {
            get { return _ibusBeneficiaryOrganization; }
            set { _ibusBeneficiaryOrganization = value; }
        }

        //Property to contain DRO Application ID
        private int _iintDROApplicationID;
        public int iintDROApplicationID
        {
            get { return _iintDROApplicationID; }
            set { _iintDROApplicationID = value; }
        }
        public bool iblnIsFromDeathNotification { get; set; } = false; // BPM Death Automation


        private busBenefitApplicationBeneficiary _ibusBenefitApplicationBeneficiary;
        public busBenefitApplicationBeneficiary ibusBenefitApplicationBeneficiary
        {
            get { return _ibusBenefitApplicationBeneficiary; }
            set { _ibusBenefitApplicationBeneficiary = value; }
        }

        //Property to store the Parent Page Details
        private string _istrParentPage;
        public string istrParentPage
        {
            get { return _istrParentPage; }
            set { _istrParentPage = value; }
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
                _ibusPerson = new busPerson();
            _ibusPerson.FindPerson(icdoPersonBeneficiary.person_id);
        }

        /// <summary>
        /// Function to check for the parent page, whether its Person Maintenance or DRO Application Maintenance
        /// </summary>
        /// <returns>True if Person Maintenance / False if DRO Application Maintenance</returns>
        public bool IsParentPagePersonMaintenance()
        {
            if (istrParentPage == busConstant.PersonMaintenance)
                return true;
            else if (istrParentPage == busConstant.DROApplicationMaintenance)
                return false;
            return true;
        }

        private Collection<busPersonBeneficiary> _iclbOtherBeneficiaries;
        public Collection<busPersonBeneficiary> iclbOtherBeneficiaries
        {
            get { return _iclbOtherBeneficiaries; }
            set { _iclbOtherBeneficiaries = value; }
        }

        //pir 8506
        public string istrMSSPlanName { get; set; }
        public void LoadOtherBeneficiaries()
        {
            DataTable ldtbOtherBeneficiaries = Select("cdoPersonAccountBeneficiary.LoadOtherBeneficiaries", new object[2] {
                                                          icdoPersonBeneficiary.beneficiary_id,icdoPersonBeneficiary.person_id });
            _iclbOtherBeneficiaries = new Collection<busPersonBeneficiary>();
            foreach (DataRow dr in ldtbOtherBeneficiaries.Rows)
            {
                busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary.LoadData(dr);
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(dr);
                lobjPersonBeneficiary.LoadBeneficiaryInfo();
                _iclbOtherBeneficiaries.Add(lobjPersonBeneficiary);
            }
            _iclbOtherBeneficiaries = busGlobalFunctions.Sort<busPersonBeneficiary>(
                "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                _iclbOtherBeneficiaries);
        }
        

        private Collection<busPersonAccountBeneficiary> _iclbPersonAccountBeneficiary;
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary
        {
            get { return _iclbPersonAccountBeneficiary; }
            set { _iclbPersonAccountBeneficiary = value; }
        }

        //this method is used exclusively in beneficiary screen where by withdrawn and cancelled plan accounts are not taken into account.
        //Only show Life when plan is Enrolled. Only show pension plans when they are Enrolled, Suspended or Retired.
        //Systest PIR - 2595
        public void LoadPersonAccountBeneficiary()
        {
            /// Load Grid objects in new mode.
            DataTable ldtbPersonAccount = Select("cdoPersonBeneficiary.LoadPlanEnrolledByPerson", new object[1] { icdoPersonBeneficiary.person_id });
            _iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbPersonAccount, "icdoPersonAccountBeneficiary");
        }


        public void LoadPersonAccountBeneficiaryData()
        {
            if (_iclbPersonAccountBeneficiary == null)
                LoadPersonAccountBeneficiary();

            /// Load Grid objects with data in update mode.
            foreach (busPersonAccountBeneficiary lobjPersonAccount in _iclbPersonAccountBeneficiary)
                if (lobjPersonAccount.FindPersonAccountBeneficiary(icdoPersonBeneficiary.beneficiary_id, lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id))
                    lobjPersonAccount.icdoPersonAccountBeneficiary.IsEnteredInNewMode = true;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoPersonBeneficiary.beneficiary_org_code != null)
                icdoPersonBeneficiary.benificiary_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonBeneficiary.beneficiary_org_code);
            //if (istrParentPage == busConstant.PersonMaintenance)
            //{
                foreach (busPersonAccountBeneficiary lobjPersonAccount in _iclbPersonAccountBeneficiary)
                {
                    if ((lobjPersonAccount.icdoPersonAccountBeneficiary.start_date != DateTime.MinValue) ||
                        (lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value != null))
                        lobjPersonAccount.icdoPersonAccountBeneficiary.lblnValueEntered = true;
                    else
                    {
                        lobjPersonAccount.icdoPersonAccountBeneficiary.lblnValueEntered = false;
                    }   
                    //set lblnValueEntered is not cleared, after make changes in drop down and blank again hence do it forcefully in else block
                }

                foreach (busBenefitApplicationBeneficiary lobjBAB in iclbBenefitApplicationBeneficiary)
                {
                    if ((lobjBAB.icdoBenefitApplicationBeneficiary.start_date != DateTime.MinValue) ||
                        (lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_type_value != null))
                    {
                        lobjBAB.icdoBenefitApplicationBeneficiary.lblnValueEntered = true;
                        break;
                    }
                }          
            if (icdoPersonBeneficiary.beneficiary_person_id != 0)
            {
                _ibusBeneficiaryPerson = new busPerson();
                _ibusBeneficiaryPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
                _ibusBeneficiaryPerson.LoadPersonCurrentAddress();
            }

            //validate address using USPS
            //if beneficiary perslink id is entered and address fields not 
            //entered and same as member address is not checked and beneficiary person doesnt have address.
            if (((icdoPersonBeneficiary.beneficiary_person_id == 0) &&
               (icdoPersonBeneficiary.same_as_member_address != busConstant.Flag_Yes))
                ||
                ((icdoPersonBeneficiary.beneficiary_person_id != 0)
                && (icdoPersonBeneficiary.same_as_member_address != busConstant.Flag_Yes)
                && (_ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id == 0)))
                VerifyAddressUsingUSPS();

            _AintValidatedAddress = ValidateAddress();
            if (istrParentPage == busConstant.PersonMaintenance)
            {
                _AintValidateBeneficiary = ValidatePersonAccountBeneficiary();
            }
            IsValidDataEnteredForApplications();
            //if (!IsApplicationSelected)
            //{
            //    if (iclbBenefitApplicationBeneficiary.Count > 0)
            //        IsApplicationSelected = true;
            //}

            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            // pir 7232 Trim the leading and trailing spaces before saving
            //PIR 11071
            if (!icdoPersonBeneficiary.first_name.IsNullOrEmpty())
                icdoPersonBeneficiary.first_name = icdoPersonBeneficiary.first_name.Trim();
            if (!icdoPersonBeneficiary.last_name.IsNullOrEmpty())
                icdoPersonBeneficiary.last_name = icdoPersonBeneficiary.last_name.Trim();
            if (!icdoPersonBeneficiary.middle_name.IsNullOrEmpty())
                icdoPersonBeneficiary.middle_name = icdoPersonBeneficiary.middle_name.Trim();

            if (icdoPersonBeneficiary.ienuObjectState == ObjectState.Insert)
            {
                icdoPersonBeneficiary.Insert();
                //Identifying the parent page and triggering the appropriate Insert for CDO
                //if (istrParentPage == busConstant.PersonMaintenance)
                //{
                foreach (busPersonAccountBeneficiary lobjPersonAccount in _iclbPersonAccountBeneficiary)
                {
                    if (lobjPersonAccount.icdoPersonAccountBeneficiary.lblnValueEntered)
                    {
                        lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_id = icdoPersonBeneficiary.beneficiary_id;
                        lobjPersonAccount.icdoPersonAccountBeneficiary.Insert();
                    }
                }
                foreach (busBenefitApplicationBeneficiary lobjBAB in iclbBenefitApplicationBeneficiary)
                {
                    if (lobjBAB.icdoBenefitApplicationBeneficiary.lblnValueEntered)
                    {
                        lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_id = icdoPersonBeneficiary.beneficiary_id;
                        lobjBAB.icdoBenefitApplicationBeneficiary.Insert();
                    }
                }
            }
            else
            {
                icdoPersonBeneficiary.Update();
                //Identifying the parent page and triggering the appropriate Update for CDO
                //if (istrParentPage == busConstant.PersonMaintenance)
                //{
                foreach (busPersonAccountBeneficiary lobjPersonAccount in _iclbPersonAccountBeneficiary)
                {
                    if (lobjPersonAccount.icdoPersonAccountBeneficiary.lblnValueEntered)
                    {
                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.IsEnteredInNewMode)
                            lobjPersonAccount.icdoPersonAccountBeneficiary.Update();
                        else
                        {
                            lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_id = icdoPersonBeneficiary.beneficiary_id;
                            lobjPersonAccount.icdoPersonAccountBeneficiary.Insert();
                        }
                    }
                }

                foreach (busBenefitApplicationBeneficiary lobjBAB in iclbBenefitApplicationBeneficiary)
                {

                    if (lobjBAB.icdoBenefitApplicationBeneficiary.lblnValueEntered)
                    {
                        if (lobjBAB.icdoBenefitApplicationBeneficiary.application_beneficiary_id > 0)
                            lobjBAB.icdoBenefitApplicationBeneficiary.Update();
                        else
                        {
                            lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_id = icdoPersonBeneficiary.beneficiary_id;
                            lobjBAB.icdoBenefitApplicationBeneficiary.Insert();
                        }
                    }
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadBeneficiaryInfo();
            LoadBenefitApplicationForBene();
            //refreshes the beneficiary plan collection UAT PIR - 2201
            //to avoid duplication of person account creation for same beneficiary
            LoadPersonAccountBeneficiaryData();
        }

        public void LoadBeneficiaryInfo(busPerson aobjBenePerson = null, busOrganization aobjBeneOrganization = null)
        {
            if (icdoPersonBeneficiary.beneficiary_person_id != 0)
            {
                busPerson lobjBeneficiaryPerson = new busPerson();
                if (aobjBenePerson.IsNotNull())
                    lobjBeneficiaryPerson = aobjBenePerson;
                else
                    lobjBeneficiaryPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
                icdoPersonBeneficiary.beneficiary_name = lobjBeneficiaryPerson.icdoPerson.FullName;
                icdoPersonBeneficiary.beneficiary_DOB = lobjBeneficiaryPerson.icdoPerson.date_of_birth;
                icdoPersonBeneficiary.beneficiary_Contact_No = lobjBeneficiaryPerson.icdoPerson.home_phone_no;
                icdoPersonBeneficiary.beneficiary_Email = lobjBeneficiaryPerson.icdoPerson.email_address;
                icdoPersonBeneficiary.beneficiary_gender = lobjBeneficiaryPerson.icdoPerson.gender_description;
                icdoPersonBeneficiary.beneficiary_Marital_Status = lobjBeneficiaryPerson.icdoPerson.marital_status_value;
                icdoPersonBeneficiary.beneficiary_first_name = lobjBeneficiaryPerson.icdoPerson.first_name;
                icdoPersonBeneficiary.beneficiary_last_name = lobjBeneficiaryPerson.icdoPerson.last_name;
            }
            else if (icdoPersonBeneficiary.benificiary_org_id != 0)
            {
                busOrganization lobjBeneficiaryOrganization = new busOrganization();
                if (aobjBeneOrganization.IsNotNull())
                    lobjBeneficiaryOrganization = aobjBeneOrganization;
                else
                    lobjBeneficiaryOrganization.FindOrganization(icdoPersonBeneficiary.benificiary_org_id);
                icdoPersonBeneficiary.beneficiary_name = lobjBeneficiaryOrganization.icdoOrganization.org_name;
                icdoPersonBeneficiary.beneficiary_first_name = lobjBeneficiaryOrganization.icdoOrganization.org_name;
                icdoPersonBeneficiary.beneficiary_Contact_No = lobjBeneficiaryOrganization.icdoOrganization.telephone;
                icdoPersonBeneficiary.beneficiary_Email = lobjBeneficiaryOrganization.icdoOrganization.email;
                icdoPersonBeneficiary.beneficiary_org_code = lobjBeneficiaryOrganization.icdoOrganization.org_code;
                _ibusBeneficiaryOrganization = lobjBeneficiaryOrganization;
            }
            else
            {
                icdoPersonBeneficiary.beneficiary_name = icdoPersonBeneficiary.last_name + "," +
                                    icdoPersonBeneficiary.first_name + " " + icdoPersonBeneficiary.middle_name; //PROD PIR ID 6885
                icdoPersonBeneficiary.beneficiary_first_name = icdoPersonBeneficiary.first_name;
                icdoPersonBeneficiary.beneficiary_last_name = icdoPersonBeneficiary.last_name;
                icdoPersonBeneficiary.beneficiary_DOB = icdoPersonBeneficiary.date_of_birth;
                icdoPersonBeneficiary.beneficiary_Contact_No = icdoPersonBeneficiary.contact_phone_no;
                icdoPersonBeneficiary.beneficiary_Email = icdoPersonBeneficiary.email_address;
                icdoPersonBeneficiary.beneficiary_Marital_Status = icdoPersonBeneficiary.marital_status_value;
            }
            if (icdoPersonBeneficiary.beneficiary_name.IsNotNullOrEmpty())
                icdoPersonBeneficiary.beneficiary_name_caps = icdoPersonBeneficiary.beneficiary_name.ToUpper();            
        }

        public void LoadBeneficiaryAddress(bool ablnIsCorrespondence = false)
        {
            if (icdoPersonBeneficiary.same_as_member_address == busConstant.Flag_Yes)
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPerson.ibusPersonCurrentAddress.IsNull()) ibusPerson.LoadPersonCurrentAddress(); ;
                if (icdoPersonBeneficiary.beneficiary_person_id != 0)                
                    icdoPersonBeneficiary.bene_id = icdoPersonBeneficiary.beneficiary_person_id.ToString();
                else if (icdoPersonBeneficiary.benificiary_org_id != 0)
                    icdoPersonBeneficiary.bene_id = icdoPersonBeneficiary.beneficiary_org_code;
                icdoPersonBeneficiary.beneficiary_address_line_1 = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
                icdoPersonBeneficiary.beneficiary_address_line_2 = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
                icdoPersonBeneficiary.beneficiary_city = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
                icdoPersonBeneficiary.beneficiary_state = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_description;
                icdoPersonBeneficiary.beneficiary_state_abbr = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                icdoPersonBeneficiary.beneficiary_zip = ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code + " " +
                                                        ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;
            }
            else
            {
                if (icdoPersonBeneficiary.beneficiary_person_id != 0)
                {
                    busPerson lobjBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjBeneficiaryPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
                    lobjBeneficiaryPerson.LoadPersonCurrentAddress();

                    icdoPersonBeneficiary.bene_id = icdoPersonBeneficiary.beneficiary_person_id.ToString();
                    icdoPersonBeneficiary.beneficiary_address_line_1 = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
                    icdoPersonBeneficiary.beneficiary_address_line_2 = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
                    icdoPersonBeneficiary.beneficiary_city = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
                    icdoPersonBeneficiary.beneficiary_state = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_description;
                    icdoPersonBeneficiary.beneficiary_state_abbr = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
                    icdoPersonBeneficiary.beneficiary_zip = lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code + " " +
                                                            lobjBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;
                }
                else if (icdoPersonBeneficiary.benificiary_org_id != 0)
                {
                    busOrganization lobjBeneficiaryOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lobjBeneficiaryOrganization.FindOrganization(icdoPersonBeneficiary.benificiary_org_id);
                    lobjBeneficiaryOrganization.LoadOrgPrimaryAddress();
                    icdoPersonBeneficiary.bene_id = icdoPersonBeneficiary.beneficiary_org_code;
                    icdoPersonBeneficiary.beneficiary_address_line_1 = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1;
                    icdoPersonBeneficiary.beneficiary_address_line_2 = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2;
                    icdoPersonBeneficiary.beneficiary_city = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city;
                    icdoPersonBeneficiary.beneficiary_state = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_description;
                    icdoPersonBeneficiary.beneficiary_state_abbr = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value;
                    icdoPersonBeneficiary.beneficiary_zip = lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code + " " +
                                                            lobjBeneficiaryOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code;
                }
                else
                {
                    icdoPersonBeneficiary.beneficiary_address_line_1 = icdoPersonBeneficiary.address_line_1;
                    icdoPersonBeneficiary.beneficiary_address_line_2 = icdoPersonBeneficiary.address_line_2;
                    icdoPersonBeneficiary.beneficiary_city = icdoPersonBeneficiary.address_city;
                    icdoPersonBeneficiary.beneficiary_state = icdoPersonBeneficiary.address_state_description;
                    icdoPersonBeneficiary.beneficiary_state_abbr = icdoPersonBeneficiary.address_state_value;
                    icdoPersonBeneficiary.beneficiary_zip = icdoPersonBeneficiary.address_zip_code + " " + icdoPersonBeneficiary.address_zip_4_code;
                }
            }

            if (ablnIsCorrespondence)
            {
                if (icdoPersonBeneficiary.beneficiary_address_line_1.IsNotNullOrEmpty()) 
                    icdoPersonBeneficiary.beneficiary_address_line_1 = icdoPersonBeneficiary.beneficiary_address_line_1.ToUpper();
                if (icdoPersonBeneficiary.beneficiary_address_line_2.IsNotNullOrEmpty())
                    icdoPersonBeneficiary.beneficiary_address_line_2 = icdoPersonBeneficiary.beneficiary_address_line_2.ToUpper();
                if (icdoPersonBeneficiary.beneficiary_city.IsNotNullOrEmpty())
                    icdoPersonBeneficiary.beneficiary_city = icdoPersonBeneficiary.beneficiary_city.ToUpper();
                if (icdoPersonBeneficiary.beneficiary_state.IsNotNullOrEmpty())
                    icdoPersonBeneficiary.beneficiary_state = icdoPersonBeneficiary.beneficiary_state.ToUpper();
            }
        }

        private int _AintValidatedAddress;
        public int AintValidatedAddress
        {
            get { return _AintValidatedAddress; }
            set { _AintValidatedAddress = value; }
        }

        public int ValidateAddress()
        {
            /// Is Member has Valid Address
            bool IsMemberHasValidAddress = false;
            if (ibusPerson == null)
                LoadPerson();
            ibusPerson.LoadPersonCurrentAddress();
            if (ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id != 0)
                IsMemberHasValidAddress = true;

            if ((icdoPersonBeneficiary.beneficiary_person_id == 0) &&
                (icdoPersonBeneficiary.benificiary_org_id == 0))
            {
                if ((icdoPersonBeneficiary.date_of_birth == DateTime.MinValue) ||
                    (icdoPersonBeneficiary.first_name == null) ||
                    (icdoPersonBeneficiary.last_name == null))
                    return 2;   /// Enter Person ID or Org ID or Bene Info

                if (!IsMemberHasValidAddress)
                {
                    if (icdoPersonBeneficiary.same_as_member_address != busConstant.Flag_Yes)
                    {
                        if ((icdoPersonBeneficiary.address_line_1 == null) ||
                            (icdoPersonBeneficiary.address_city == null) ||
                            (icdoPersonBeneficiary.address_state_value == null) ||
                            (icdoPersonBeneficiary.address_country_value == null) ||
                            (icdoPersonBeneficiary.address_zip_code == null))
                            return 4; /// Address is info is mandatory.
                    }
                    else
                    {
                        return 5; // Member doesnt have valid address
                    }
                }
                else if (icdoPersonBeneficiary.address_country_value != null)
                {
                    if (icdoPersonBeneficiary.address_country_value != busConstant.US_Code_ID)
                        if ((icdoPersonBeneficiary.foreign_postal_code == null) || (icdoPersonBeneficiary.foreign_province == null))
                            return 3;   /// Foreign Postal code is mandatory
                }
            }
            else if (icdoPersonBeneficiary.beneficiary_person_id != 0)
            {
                if (!IsMemberHasValidAddress)
                    return 5;

                if ((icdoPersonBeneficiary.first_name != null) ||
                    (icdoPersonBeneficiary.last_name != null) ||
                    (icdoPersonBeneficiary.date_of_birth != DateTime.MinValue))
                    return 6;

                if (icdoPersonBeneficiary.same_as_member_address == busConstant.Flag_Yes)
                {
                    if ((icdoPersonBeneficiary.address_line_1 != null) ||
                        (icdoPersonBeneficiary.address_city != null) ||
                        (icdoPersonBeneficiary.address_state_value != null) ||
                        (icdoPersonBeneficiary.address_zip_4_code != null) ||
                        (icdoPersonBeneficiary.address_zip_code != null))
                        return 1;   /// Contact info should not be entered.
                }

                if (icdoPersonBeneficiary.same_as_member_address != busConstant.Flag_Yes)
                {
                    if (((icdoPersonBeneficiary.address_line_1 == null) ||
                        (icdoPersonBeneficiary.address_city == null) ||
                        (icdoPersonBeneficiary.address_state_value == null) ||
                        (icdoPersonBeneficiary.address_country_value == null) ||
                        (icdoPersonBeneficiary.address_zip_code == null))
                        &&
                        (_ibusBeneficiaryPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id == 0))
                        return 4; /// Address is info is mandatory.
                }
            }
            return 0;
        }

        private int _AintValidateBeneficiary;
        public int AintValidateBeneficiary
        {
            get { return _AintValidateBeneficiary; }
            set { _AintValidateBeneficiary = value; }
        }

        bool iblnIsPlanEnteredForBeneficiary = false;
        public int ValidatePersonAccountBeneficiary()
        {
            if (_iclbPersonAccountBeneficiary.IsNotNull() && _iclbPersonAccountBeneficiary.Count > 0)
            {
                //PIR 19865 Make Benefit Type is mandatory, if no plans selected system throws the error. No Plans entered for the Beneficiary 
                if (_iclbPersonAccountBeneficiary.All(beneficiary => string.IsNullOrEmpty(beneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value) &&
                                                                     beneficiary.icdoPersonAccountBeneficiary.start_date == DateTime.MinValue))
                    return 1;
            }
            if (_iclbPersonAccountBeneficiary.IsNotNull() && _iclbPersonAccountBeneficiary.Count > 0)
            {
                foreach (busPersonAccountBeneficiary lobjPersonAccount in _iclbPersonAccountBeneficiary)
                {
                    if (lobjPersonAccount.icdoPersonAccountBeneficiary.lblnValueEntered)
                    {
                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value == null)
                        {
                            // Benefit Type is mandatory.
                            return 1;
                        }
                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.start_date == DateTime.MinValue)
                        {
                            // Start Date is mandatory.
                            return 3;
                        }
                        else
                        {
                            if (!IsDateOverlapping(lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id,
                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.start_date))
                            {
                                // Start Date cannot be earlier than Plan Participation Start Date
                                return 6;
                            }
                        }

                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.start_date != DateTime.MinValue &&
                            lobjPersonAccount.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue &&
                            lobjPersonAccount.icdoPersonAccountBeneficiary.start_date > lobjPersonAccount.icdoPersonAccountBeneficiary.end_date)
                            return 15;

                        /// Contingent Beneficiary cannot be added before adding Primary Beneficiary.
                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent)
                        {
                            int lintCount = 0;
                            if (icdoPersonBeneficiary.ienuObjectState == ObjectState.Insert)
                            {
                                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonBeneficiary.CheckPrimaryBeneficiaryExistsNew",
                                                                new object[3] { lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id
                                                            ,lobjPersonAccount.icdoPersonAccountBeneficiary.start_date,
                                                            lobjPersonAccount.icdoPersonAccountBeneficiary.end_date_no_null},
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                            }
                            else
                            {
                                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonBeneficiary.CheckPrimaryBeneficiaryExistsUpdate",
                                                                new object[4] { lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_beneficiary_id,
                                                            lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id,
                                                            lobjPersonAccount.icdoPersonAccountBeneficiary.start_date,
                                                            lobjPersonAccount.icdoPersonAccountBeneficiary.end_date_no_null},
                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                            }
                            if (lintCount == 0)
                            {
                                return 4;
                            }
                        }

                        if ((lobjPersonAccount.icdoPersonAccountBeneficiary.dist_percent <= 0.0M) ||
                            (lobjPersonAccount.icdoPersonAccountBeneficiary.dist_percent > 100))
                        {
                            // Invalid Percentage.
                            return 2;
                        }
                        else
                        {
                            /// Validate the Percentage sum.
                            decimal idclTotalPercentage = 0.0M;
                            //PIR 7601 START
                            DataTable ldtPerAccBen;
                            Collection<busPersonAccountBeneficiary> templclbPerBen;
                            if (icdoPersonBeneficiary.ienuObjectState == ObjectState.Insert)
                            {
                                ldtPerAccBen = Select("cdoPersonBeneficiary.GetBeneficiariesByPersonAccountNew",
                                                        new object[4] { lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.end_date_no_null,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.start_date
                                                                      });
                            }
                            else
                            {
                                ldtPerAccBen = Select("cdoPersonBeneficiary.GetBeneficiariesByPersonAccountUpdate",
                                                        new object[5] { lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_beneficiary_id,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.end_date_no_null,
                                                                    lobjPersonAccount.icdoPersonAccountBeneficiary.start_date
                                                                      });
                            }
                            if (ldtPerAccBen.Rows.Count > 0)
                            {
                                //PIR 26611 - Skip validation 7010 if end date is modified.
                                if (lobjPersonAccount.icdoPersonAccountBeneficiary.ihstOldValues.Count > 0 && Convert.ToDateTime(lobjPersonAccount.icdoPersonAccountBeneficiary.ihstOldValues["end_date"]) == lobjPersonAccount.icdoPersonAccountBeneficiary.end_date)
                                {
                                    templclbPerBen = GetCollection<busPersonAccountBeneficiary>(ldtPerAccBen, "icdoPersonAccountBeneficiary");
                                    templclbPerBen.OrderBy(i => i.icdoPersonAccountBeneficiary.end_date_no_null);
                                    DateTime ldtPrevEndDate = DateTime.MinValue;
                                    //Checks across different start and end date regions
                                    foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in templclbPerBen)
                                    {
                                        idclTotalPercentage = 0;
                                        if (ldtPrevEndDate == DateTime.MinValue)
                                            ldtPrevEndDate = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date;

                                        var lenumlist = templclbPerBen.Where(i => i.icdoPersonAccountBeneficiary.end_date_no_null >= ldtPrevEndDate
                                            && i.icdoPersonAccountBeneficiary.start_date <= lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date_no_null);

                                        lenumlist.ForEach(i => idclTotalPercentage += i.icdoPersonAccountBeneficiary.dist_percent);

                                        if (idclTotalPercentage + lobjPersonAccount.icdoPersonAccountBeneficiary.dist_percent > 100)
                                        {
                                            return 5;
                                        }
                                        if (lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date_no_null != DateTime.MaxValue)
                                            ldtPrevEndDate = lbusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date.AddDays(1);
                                    }
                                }
                            }
                            //End
                        }
                        /// PIR ID 1998 - No longer the validation refers to the Plan participation status.
                        //if (lobjPersonAccount.icdoPersonAccountBeneficiary.end_date != DateTime.MinValue)
                        //{
                        //    if (!IsDateOverlapping(lobjPersonAccount.icdoPersonAccountBeneficiary.person_account_id,
                        //                             lobjPersonAccount.icdoPersonAccountBeneficiary.end_date))
                        //    {
                        //        // End Date should be between Plan Participation Start Date and End Date
                        //        return 7;
                        //    }
                        //}
                        if (lobjPersonAccount.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary)
                        {
                            if (lobjPersonAccount.ibusPersonAccount == null)
                                lobjPersonAccount.LoadPersonAccount();

                            if (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
                            {
                                if (ibusPerson.iclbBenefitApplication == null)
                                    ibusPerson.LoadBenefitApplication();
                                //PIR 21620 get the payee account and active one status 
                                if (ibusPerson.iclbPayeeAccount == null)
                                    ibusPerson.LoadPayeeAccount(true);

                                //BR-051-49  & 50
                                foreach (busBenefitApplication lobjRetirementApplication in ibusPerson.iclbBenefitApplication)
                                {
                                    //PIR 21620 get the payee account by application id and status (Payment complete and cancelled)
                                    IEnumerable<busPayeeAccount> linuBusPayeeAccounts = ibusPerson.iclbPayeeAccount.Where(
                                            i => i.icdoPayeeAccount.application_id == lobjRetirementApplication.icdoBenefitApplication.benefit_application_id);
                                    foreach (busPayeeAccount lobjPayeeAccountPerApplication in linuBusPayeeAccounts)
                                    {
                                        if (lobjPayeeAccountPerApplication?.ibusPayeeAccountActiveStatus?.icdoPayeeAccountStatus?.status_value_data1 != busConstant.PayeeAccountStatusRetirementPaymentCompleted &&
                                            lobjPayeeAccountPerApplication?.ibusPayeeAccountActiveStatus?.icdoPayeeAccountStatus?.status_value_data1 != busConstant.PayeeAccountStatusRetirmentCancelled
                                            )
                                        {
                                            //PIR 21620 if got the payee account status as Payment complete and cancelled skip the validation for that benefit application                                
                                            if ((lobjRetirementApplication.icdoBenefitApplication.plan_id == lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id)
                                                && (lobjRetirementApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)
                                                && (lobjRetirementApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement))
                                            {
                                                if (lobjRetirementApplication.IsJSBenefitOption)
                                                {
                                                    if (lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                                                    {
                                                        if (this.icdoPersonBeneficiary.beneficiary_person_id != lobjRetirementApplication.icdoBenefitApplication.joint_annuitant_perslink_id)
                                                        {
                                                            return 12;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.icdoPersonBeneficiary.beneficiary_person_id != lobjRetirementApplication.icdoBenefitApplication.joint_annuitant_perslink_id)
                                                        {
                                                            return 13;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //BR-051-51
                                if (ibusPerson.iclbDROApplication == null)
                                    ibusPerson.LoadDROApplications();

                                var query = from p in ibusPerson.iclbDROApplication
                                            where (p.icdoBenefitDroApplication.status_value == busConstant.StatusValid
                                            && lobjPersonAccount.ibusPersonAccount.icdoPersonAccount.plan_id == p.icdoBenefitDroApplication.plan_id)
                                            && ((p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
                                            || (p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                                            || (p.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved))
                                            select p;

                                if (query.Count<busBenefitDroApplication>() > 0)
                                {
                                    return 14;
                                }
                            }
                        }
                        iblnIsPlanEnteredForBeneficiary = true;
                    }
                }
            }
            // No Plans entered for the Beneficiary 
            //PIR - 1863 - checking for beneficiary plan commented since there can be beneficiary who are not enrolled in any plan but still can get benefits
            //if (!iblnIsPlanEnteredForBeneficiary)
            //    return 8;

            return CheckSpouseRules();
        }

        public bool IsDateOverlapping(int AintPersonAccountID, DateTime AdtGivenDate)
        {
            bool lblnFlag = false;
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (lobjPersonAccount.FindPersonAccount(AintPersonAccountID))
            {
                //PIR - 2119
                //for status retired it should check for overlapping dates
                if (!lobjPersonAccount.icdoPersonAccount.plan_participation_status_value.Equals(busConstant.PlanParticipationStatusRetirementRetired))
                {
                    lblnFlag = busGlobalFunctions.CheckDateOverlapping(AdtGivenDate,
                                            lobjPersonAccount.icdoPersonAccount.start_date,
                                            lobjPersonAccount.icdoPersonAccount.end_date);
                }
                else
                {
                    lblnFlag = true;
                }
            }
            return lblnFlag;
        }

        private busPersonAccountBeneficiary _ibusPersonAccountBeneficiary;
        public busPersonAccountBeneficiary ibusPersonAccountBeneficiary
        {
            get { return _ibusPersonAccountBeneficiary; }
            set { _ibusPersonAccountBeneficiary = value; }
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }

        /// <summary>
        /// Verify Address using Web Service and save after validation.
        /// </summary>
        /// <returns></returns>
        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if ((icdoPersonBeneficiary.suppress_warning == busConstant.Flag_Yes) ||
                (icdoPersonBeneficiary.same_as_member_address == busConstant.Flag_Yes))
            {
                icdoPersonBeneficiary.address_validate_flag = busConstant.Flag_Yes;
                return;
            }
            else
            {
                cdoWebServiceAddress lcdoWebServiceAddress = new cdoWebServiceAddress();
                lcdoWebServiceAddress.addr_line_1 = icdoPersonBeneficiary.address_line_1;
                lcdoWebServiceAddress.addr_line_2 = icdoPersonBeneficiary.address_line_2;
                lcdoWebServiceAddress.addr_city = icdoPersonBeneficiary.address_city;
                lcdoWebServiceAddress.addr_state_value = icdoPersonBeneficiary.address_state_value;
                lcdoWebServiceAddress.addr_zip_code = icdoPersonBeneficiary.address_zip_code;
                lcdoWebServiceAddress.addr_zip_4_code = icdoPersonBeneficiary.address_zip_4_code;

                cdoWebServiceAddress lcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(lcdoWebServiceAddress);
                if (lcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
                {
                    icdoPersonBeneficiary.address_line_1 = lcdoWebServiceAddressResult.addr_line_1;
                    icdoPersonBeneficiary.address_line_2 = lcdoWebServiceAddressResult.addr_line_2;
                    icdoPersonBeneficiary.address_city = lcdoWebServiceAddressResult.addr_city;
                    icdoPersonBeneficiary.address_state_value = lcdoWebServiceAddressResult.addr_state_value;
                    icdoPersonBeneficiary.address_zip_code = lcdoWebServiceAddressResult.addr_zip_code;
                    icdoPersonBeneficiary.address_zip_4_code = lcdoWebServiceAddressResult.addr_zip_4_code;
                }
                icdoPersonBeneficiary.address_validate_error = lcdoWebServiceAddressResult.address_validate_error;
                icdoPersonBeneficiary.address_validate_flag = lcdoWebServiceAddressResult.address_validate_flag;
            }
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (!String.IsNullOrEmpty(icdoPersonBeneficiary.beneficiary_org_code))
                icdoPersonBeneficiary.benificiary_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonBeneficiary.beneficiary_org_code);
            LoadBeneficiaryInfo();
            larrList.Add(this);
            return larrList;
        }

        //Set this flag as yes if the beneficiary is having the active address
        //PIR - 1480
        public void SetAddressStatusFlag()
        {
            if (ibusBeneficiaryPerson == null)
            {
                _ibusBeneficiaryPerson = new busPerson();
                _ibusBeneficiaryPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
            }
            if (ibusBeneficiaryPerson.icolPersonAddress == null)
                ibusBeneficiaryPerson.LoadAddresses();

            foreach (busPersonAddress lobjPersonAddress in ibusBeneficiaryPerson.icolPersonAddress)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjPersonAddress.icdoPersonAddress.start_date, lobjPersonAddress.icdoPersonAddress.end_date))
                {
                    icdoPersonBeneficiary.istrIsAddressActive = busConstant.Flag_Yes;
                }
            }

        }

        #region Rules for DRO Beneficiary

        //Function to check the rules if beneficiary is wife
        public int CheckSpouseRules()
        {
            if (icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
            {
                DataTable ldtbLists = new DataTable();
                if (icdoPersonBeneficiary.ienuObjectState == ObjectState.Insert)
                {
                    ldtbLists = Select<cdoPersonBeneficiary>(new string[2] { "person_id", "relationship_value" },
                                    new object[2] { icdoPersonBeneficiary.person_id, busConstant.PersonBeneficiaryRelationshipSpouse }, null, null);
                }
                else
                {
                    ldtbLists = SelectWithOperator<cdoPersonBeneficiary>(new string[3] { "person_id", "relationship_value", "beneficiary_id" },
                                    new string[3] { "=", "=", "<>" }, new object[3] { icdoPersonBeneficiary.person_id, 
                                        busConstant.PersonBeneficiaryRelationshipSpouse,icdoPersonBeneficiary.beneficiary_id }, null);
                }
                // Beneficiary of Relationship Spouse already exists
                if (ldtbLists.Rows.Count > 0)
                    return 9;

                if (icdoPersonBeneficiary.beneficiary_person_id != 0)
                {
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
                    if ((lobjPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried) ||
                        (ibusPerson?.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried))
                    {
                        // Maritial status should be married for Member and Beneficiary.
                        return 10;
                    }
                    if (((lobjPerson.icdoPerson.gender_value == busConstant.GenderTypeFemale) &&
                        (ibusPerson?.icdoPerson.gender_value == busConstant.GenderTypeFemale)) ||
                        ((lobjPerson.icdoPerson.gender_value == busConstant.GenderTypeMale) &&
                        (ibusPerson?.icdoPerson.gender_value == busConstant.GenderTypeMale)))
                    {
                        // Beneficiary Person Gender should not be same.
                        return 11;
                    }
                }
            }
            return 0;
        }
        #endregion

        // PIR ID 1432
        public bool IsSpouseRelationshipValid()
        {
            if ((icdoPersonBeneficiary.relationship_value == busConstant.FamilyRelationshipSpouse) &&
                (icdoPersonBeneficiary.beneficiary_person_id != 0))
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusPerson.icolPersonContact == null)
                    ibusPerson.LoadContacts();
                foreach (busPersonContact lobjContact in ibusPerson.icolPersonContact)
                {
                    if ((icdoPersonBeneficiary.beneficiary_person_id == lobjContact.icdoPersonContact.contact_person_id) &&
                        (lobjContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive) &&
                        (lobjContact.icdoPersonContact.relationship_value != busConstant.FamilyRelationshipSpouse))
                        return false;
                }
            }
            return true;
        }

        public void LoadBeneficiaryPerson()
        {
            if (ibusBeneficiaryPerson.IsNull())
                ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
            ibusBeneficiaryPerson.FindPerson(icdoPersonBeneficiary.beneficiary_person_id);
        }

        public bool IsDROBeneficiaryStartDateNotEntered()
        {
            //Reload always
            if (iintDROApplicationID > 0)
            {
                if (iclbBenefitApplicationBeneficiary != null && iclbBenefitApplicationBeneficiary.Count > 0 &&
                    iclbBenefitApplicationBeneficiary.Where(o => o.icdoBenefitApplicationBeneficiary.beneficiary_type_value != null && o.icdoBenefitApplicationBeneficiary.start_date == DateTime.MinValue).Count() > 0)
                    return true;
                if (iclbPersonAccountBeneficiary != null && iclbPersonAccountBeneficiary.Count > 0 &&
                    iclbPersonAccountBeneficiary.Where(o => o.icdoPersonAccountBeneficiary.beneficiary_type_value != null
                        && o.icdoPersonAccountBeneficiary.start_date == DateTime.MinValue).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        # region PIR 1863

        //public Collection<busBenefitApplication> iclbBenefitApplicationForCurrentBene { get; set; }
        public Collection<busBenefitApplication> iclbBenefitApplicationForOtherBene { get; set; }
        public Collection<busBenefitApplicationBeneficiary> iclbBenefitApplicationBeneficiary { get; set; }
        public void LoadBenefitApplicationForBene()
        {
            //iclbBenefitApplicationForCurrentBene = new Collection<busBenefitApplication>();
            iclbBenefitApplicationForOtherBene = new Collection<busBenefitApplication>();

            if (ibusPerson.IsNull())
                LoadPerson();

            //Load beneficiary application from db
            if (iclbBenefitApplicationBeneficiary.IsNull())
                LoadBenefitApplicationsBeneficiary();


            if (ibusPerson.iclbApplicantsBenefitApplications.IsNull())
            {
                ibusPerson.LoadApplicantsbenefitApplication();
                ibusPerson.LoadApplicantsBenefitApplicationWithDRO();
            }

            var lenumCurrentBeneApplcation = ibusPerson.iclbApplicantsBenefitApplications
                .Where(lobjBA => lobjBA.icdoBenefitApplication.recipient_person_id == icdoPersonBeneficiary.person_id);

            foreach (busBenefitApplication lobjBA in lenumCurrentBeneApplcation)
            {
                busBenefitDroApplication lobjDROApplication = new busBenefitDroApplication();
                busBenefitApplicationBeneficiary lobjBeneApplBeneficiary = new busBenefitApplicationBeneficiary() { icdoBenefitApplicationBeneficiary = new cdoBenefitApplicationBeneficiary() };


                if (lobjBA.icdoBenefitApplication.iintDROApplicationId == 0)
                    lobjBeneApplBeneficiary.iintApplicationId = lobjBA.icdoBenefitApplication.benefit_application_id;
                else
                {
                    lobjBeneApplBeneficiary.iintApplicationId = lobjBA.icdoBenefitApplication.iintDROApplicationId;
                    lobjDROApplication.FindBenefitDroApplication(lobjBA.icdoBenefitApplication.iintDROApplicationId);
                    lobjDROApplication.LoadDROCalculation();
                }

                if (lobjBA.ibusPlan.IsNull())
                    lobjBA.LoadPlan();
                var lBAB = iclbBenefitApplicationBeneficiary.Where(lobjBAB => ((lobjBAB.icdoBenefitApplicationBeneficiary.benefit_application_id
                                                        == lobjBA.icdoBenefitApplication.benefit_application_id
                                                        && (lobjBAB.icdoBenefitApplicationBeneficiary.benefit_application_id > 0
                                                        && lobjBA.icdoBenefitApplication.benefit_application_id> 0))
                                                        || (lobjBAB.icdoBenefitApplicationBeneficiary.dro_application_id == lobjBA.icdoBenefitApplication.iintDROApplicationId
                                                        && ((lobjBAB.icdoBenefitApplicationBeneficiary.dro_application_id > 0) && (lobjBA.icdoBenefitApplication.iintDROApplicationId > 0)))
                                                        && (lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_id == 0 
                                                        || lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_id == icdoPersonBeneficiary.beneficiary_id)));
                if (lBAB.Count() == 0)
                {
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_id = icdoPersonBeneficiary.beneficiary_id;
                    if (lobjBA.icdoBenefitApplication.iintDROApplicationId == 0)
                    {
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitOption = lobjBA.icdoBenefitApplication.benefit_option_description;
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountType = lobjBA.icdoBenefitApplication.benefit_account_type_description;
                        if (lobjBA.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                        {
                            if (lobjBA.ibusPerson.IsNull())
                                lobjBA.LoadPerson();
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.idtRetirementDate = lobjBA.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                        }
                        else
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.idtRetirementDate = lobjBA.icdoBenefitApplication.retirement_date;
                    }
                    else
                    {
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitOption = lobjDROApplication.icdoBenefitDroApplication.benefit_duration_option_description;
                        if (!String.IsNullOrEmpty(lobjDROApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value))
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjDROApplication.GetBenefitType());
                        else
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, busConstant.ApplicationBenefitTypeRetirement);

                        //lobjBeneApplBeneficiary.idtRetirementDate = lobjDROApplication.icdoBenefitDroApplication.received_date;
                    }
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id = lobjBA.icdoBenefitApplication.benefit_application_id;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id = lobjBA.icdoBenefitApplication.iintDROApplicationId;

                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrAccountRelationship = lobjBA.icdoBenefitApplication.account_relationship_description;

                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrPlanName = lobjBA.ibusPlan.icdoPlan.plan_name;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountTypeValue = lobjBA.icdoBenefitApplication.benefit_account_type_value;

                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.IsEnteredInNewMode = true;
                    iclbBenefitApplicationBeneficiary.Add(lobjBeneApplBeneficiary);
                }
                else
                {
                    lobjBeneApplBeneficiary = lBAB.First();
                    //assign fields description
                    if (lobjBA.icdoBenefitApplication.iintDROApplicationId == 0)
                    {
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitOption = lobjBA.icdoBenefitApplication.benefit_option_description;
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountType = lobjBA.icdoBenefitApplication.benefit_account_type_description;
                        if (lobjBA.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                        {
                            if (lobjBA.ibusPerson.IsNull())
                                lobjBA.LoadPerson();
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.idtRetirementDate = lobjBA.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                        }
                        else
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.idtRetirementDate = lobjBA.icdoBenefitApplication.retirement_date;
                    }
                    else
                    {
                        lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitOption = lobjDROApplication.icdoBenefitDroApplication.benefit_duration_option_description;
                        if (!String.IsNullOrEmpty(lobjDROApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value))
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjDROApplication.GetBenefitType());
                        else
                            lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary. istrBenefitAccountType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, busConstant.ApplicationBenefitTypeRetirement);
                        //lobjBeneApplBeneficiary.idtRetirementDate = lobjDROApplication.icdoBenefitDroApplication.received_date;
                    }
                    if (lobjBA.icdoBenefitApplication.benefit_application_id > 0)
                        lobjBeneApplBeneficiary.iintApplicationId = lobjBA.icdoBenefitApplication.benefit_application_id;
                    else
                        lobjBeneApplBeneficiary.iintApplicationId = lobjBA.icdoBenefitApplication.iintDROApplicationId;

                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrAccountRelationship = lobjBA.icdoBenefitApplication.account_relationship_description;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitOption = lobjBA.icdoBenefitApplication.benefit_option_description;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrPlanName = lobjBA.ibusPlan.icdoPlan.plan_name;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.istrBenefitAccountTypeValue = lobjBA.icdoBenefitApplication.benefit_account_type_value;
                    lobjBeneApplBeneficiary.icdoBenefitApplicationBeneficiary.IsEnteredInNewMode = true;
                }
            }
            //PIR 1391 - satya asked to load other beneficiaries new mode
            if (icdoPersonBeneficiary.beneficiary_person_id != 0 || iblnIsNewMode)
            {
                if (ibusPerson.iclbBeneficiaryApplication.IsNull())
                    ibusPerson.LoadBeneficiaryApplication(true);
                //this is to load the Other bene applications
                var lenumOtherBeneApplcation = 
                    ibusPerson.iclbBeneficiaryApplication.Where(lobjBA => lobjBA.iintBeneficiaryid != icdoPersonBeneficiary.beneficiary_id)
                    .GroupBy(i => i.iintBeneficiaryid).Select(o => o.First());

                foreach (busBenefitApplication lobjBA in lenumOtherBeneApplcation)
                    iclbBenefitApplicationForOtherBene.Add(lobjBA);
            }
        }

        public void LoadBenefitApplicationsBeneficiary()
        {
            busBase lbusbase = new busBase();
            if (iclbBenefitApplicationBeneficiary.IsNull())
                iclbBenefitApplicationBeneficiary = new Collection<busBenefitApplicationBeneficiary>();
            DataTable ldtbList = Select<cdoBenefitApplicationBeneficiary>(new string[1] { "BENEFICIARY_ID" },
                                                            new object[1] { icdoPersonBeneficiary.beneficiary_id }, null, null);
            iclbBenefitApplicationBeneficiary = lbusbase.GetCollection<busBenefitApplicationBeneficiary>(ldtbList);
        }

        public bool IsDroBeneficiaryAllowed()
        {
            if (iclbBenefitApplicationBeneficiary == null)
                LoadBenefitApplicationsBeneficiary();
            if (IsParentPagePersonMaintenance())
            {
                foreach (busBenefitApplicationBeneficiary lcdoBenefitApplicationBeneficiary in iclbBenefitApplicationBeneficiary)
                {
                    if (lcdoBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id > 0 && lcdoBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.start_date != DateTime.MinValue &&
                        lcdoBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value != null && lcdoBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent > 0)
                    {
                        busBenefitDroApplication lbusBenefitDroApplication = new busBenefitDroApplication { icdoBenefitDroApplication = new cdoBenefitDroApplication() };
                        lbusBenefitDroApplication.FindBenefitDroApplication(lcdoBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id);
                        if (!lbusBenefitDroApplication.IsBeneficiaryTabVisible())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        //validations
        //1 .percent for the application sould not exceed 100
        //UAT PIR: 1672.The Existing Application Beneficiary should be excluded to check for double counting.
        public bool IsPercentageGreaterThan100()
        {
            int lintBeneficiaryId = 0;
            lintBeneficiaryId = icdoPersonBeneficiary.beneficiary_id > 0 ? icdoPersonBeneficiary.beneficiary_id : -999;
            /*if (iintDROApplicationID > 0)
            {
                // iintDROApplicationID will be set only in the DRO Application Maintenance.
                IEnumerable<int> lenumApplicationId;
                DataTable ldtbApplicationList = new DataTable();
                lenumApplicationId = iclbBenefitApplicationBeneficiary.Select(lobjBAB => lobjBAB.dro_application_id).Distinct();
                
                ldtbApplicationList = SelectWithOperator<cdoBenefitApplicationBeneficiary>(new string[2] { "dro_application_id", "beneficiary_id" },
                new string[2] { "=", "!=" }, new object[2] { iintDROApplicationID, lintBeneficiaryId }, null);

                if (ldtbApplicationList.AsEnumerable().Where(lobjBAB =>
                    lobjBAB["dist_percent"] != DBNull.Value).Sum(lobjBAB => lobjBAB.Field<decimal>("dist_percent")) +
                    iclbBenefitApplicationBeneficiary.Where(i => i.dro_application_id == iintDROApplicationID).Sum(o => o.dist_percent) > 100M)
                    return true;
            }
            else
            {*/
            if (iintDROApplicationID == 0)
            {
                // SYS PIR ID 2277
                IEnumerable<int> lenumApplicationId;
                // Executed from the Person Beneficiary Maintenance.
                lenumApplicationId = iclbBenefitApplicationBeneficiary.Select(lobjBAB => lobjBAB.icdoBenefitApplicationBeneficiary.benefit_application_id).Distinct();
                foreach (int lintAppId in lenumApplicationId)
                {
                    if (lintAppId > 0)
                    {
                        DataTable ldtbApplicationList = SelectWithOperator<cdoBenefitApplicationBeneficiary>(new string[2] { "benefit_application_id", "beneficiary_id" }
                            , new string[2] { "=", "!=" }, new object[2] { lintAppId, lintBeneficiaryId }, null);
                        if (ldtbApplicationList.Rows.Count > 0)
                        {
                            decimal ldecPrimaryPercent = 0.00M;
                            decimal ldecContingentPercent = 0.00M;
                            foreach (DataRow dr in ldtbApplicationList.Rows)
                            {
                                if (dr["beneficiary_type_value"].ToString() == busConstant.BeneficiaryMemberTypePrimary)
                                {
                                    ldecPrimaryPercent += Convert.ToDecimal(dr["dist_percent"]);

                                    if (ldecPrimaryPercent +
                                        iclbBenefitApplicationBeneficiary.Where(i => i.icdoBenefitApplicationBeneficiary.benefit_application_id == lintAppId
                                        &&
                                        i.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary
                                        ).Sum(o => o.icdoBenefitApplicationBeneficiary.dist_percent) > 100M)
                                        return true;
                                }
                                if (dr["beneficiary_type_value"].ToString() == busConstant.BeneficiaryMemberTypeContingent)
                                {
                                    ldecContingentPercent += Convert.ToDecimal(dr["dist_percent"]);

                                    if (ldecContingentPercent +
                                        iclbBenefitApplicationBeneficiary.Where(i => i.icdoBenefitApplicationBeneficiary.benefit_application_id == lintAppId
                                        &&
                                        i.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent
                                        ).Sum(o => o.icdoBenefitApplicationBeneficiary.dist_percent) > 100M)
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
                IEnumerable<int> lenumDROApplicationId;
                lenumDROApplicationId = iclbBenefitApplicationBeneficiary.Select(lobjBAB => lobjBAB.icdoBenefitApplicationBeneficiary.dro_application_id).Distinct();
                foreach (int lintDROAppId in lenumDROApplicationId)
                {
                    if (lintDROAppId > 0)
                    {
                        DataTable ldtbApplicationList = SelectWithOperator<cdoBenefitApplicationBeneficiary>(new string[2] { "dro_application_id", "beneficiary_id" },
                            new string[2] { "=", "!=" }, new object[2] { lintDROAppId, lintBeneficiaryId }, null);
                        if (ldtbApplicationList.Rows.Count > 0)
                        {
                             decimal ldecPrimaryPercent = 0.00M;
                             decimal ldecContingentPercent = 0.00M;
                             foreach (DataRow dr in ldtbApplicationList.Rows)
                             {
                                 if (dr["beneficiary_type_value"].ToString() == busConstant.BeneficiaryMemberTypePrimary)
                                 {
                                     ldecPrimaryPercent += Convert.ToDecimal(dr["dist_percent"]);

                                     if (ldecPrimaryPercent +
                                         iclbBenefitApplicationBeneficiary.Where(i => i.icdoBenefitApplicationBeneficiary.dro_application_id == lintDROAppId
                                         &&
                                         i.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary
                                         ).Sum(o => o.icdoBenefitApplicationBeneficiary.dist_percent) > 100M)
                                         return true;
                                 }
                                 if (dr["beneficiary_type_value"].ToString() == busConstant.BeneficiaryMemberTypeContingent)
                                 {
                                     ldecContingentPercent += Convert.ToDecimal(dr["dist_percent"]);

                                     if (ldecContingentPercent +
                                         iclbBenefitApplicationBeneficiary.Where(i => i.icdoBenefitApplicationBeneficiary.dro_application_id == lintDROAppId
                                         &&
                                         i.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent
                                         ).Sum(o => o.icdoBenefitApplicationBeneficiary.dist_percent) > 100M)
                                         return true;
                                 }
                             }
                        }
                        else if (iclbBenefitApplicationBeneficiary.Where(i => i.icdoBenefitApplicationBeneficiary.dro_application_id == lintDROAppId).Sum(o => o.icdoBenefitApplicationBeneficiary.dist_percent) > 100M)
                            return true;
                    }
                }
            //}
            return false;
        }

        public void IsValidDataEnteredForApplications()
        {
            IsStartDateNotEnteredForApplication = false;
            IsApplicationStartDateIsEarlierThanRetDAte = false;
            IsStartDateEarlierThanEndDate = false;
            IsApplicationBeneficiaryTypeNotEntered = false;
            IsContigentBeneNotAllowed = false;
            IsInValidPercentageEnteredForApplication = false;

            foreach (busBenefitApplicationBeneficiary lobjBAB in iclbBenefitApplicationBeneficiary)
            {
                if (lobjBAB.icdoBenefitApplicationBeneficiary.lblnValueEntered)
                {
                    if (lobjBAB.icdoBenefitApplicationBeneficiary.start_date == DateTime.MinValue)
                        IsStartDateNotEnteredForApplication = true;
                    else
                    {
                        if (lobjBAB.icdoBenefitApplicationBeneficiary.start_date < lobjBAB.icdoBenefitApplicationBeneficiary.idtRetirementDate)
                            IsApplicationStartDateIsEarlierThanRetDAte = true;

                        if (lobjBAB.icdoBenefitApplicationBeneficiary.end_date != DateTime.MinValue)
                        {
                            if (lobjBAB.icdoBenefitApplicationBeneficiary.end_date < lobjBAB.icdoBenefitApplicationBeneficiary.start_date)
                                IsStartDateEarlierThanEndDate = true;
                        }
                    }
                    if (String.IsNullOrEmpty(lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_type_value))
                        IsApplicationBeneficiaryTypeNotEntered = true;
                    else
                    {
                        if (lobjBAB.icdoBenefitApplicationBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypeContingent)
                        {
                            //new mode
                            DataTable ldtbBeneByApplicationId = new DataTable();
                            if (iintDROApplicationID > 0)
                            {
                               ldtbBeneByApplicationId = Select<cdoBenefitApplicationBeneficiary>(new string[2] { "benefit_application_id", "BENEFICIARY_TYPE_VALUE" },
                                                         new object[2] { lobjBAB.icdoBenefitApplicationBeneficiary.benefit_application_id, busConstant.BeneficiaryMemberTypePrimary }, null, null);
                            }
                            else
                            {
                                ldtbBeneByApplicationId = Select<cdoBenefitApplicationBeneficiary>(new string[2] { "dro_application_id", "BENEFICIARY_TYPE_VALUE" },
                                                         new object[2] { iintDROApplicationID, busConstant.BeneficiaryMemberTypePrimary }, null, null);
                            }
                            if (ldtbBeneByApplicationId.Rows.Count > 0)
                            {
                                if (lobjBAB.icdoBenefitApplicationBeneficiary.application_beneficiary_id > 0)
                                {
                                    var lcount = ldtbBeneByApplicationId.AsEnumerable().Where(ldr => ldr.Field<int>("beneficiary_id") != icdoPersonBeneficiary.beneficiary_id).Count();
                                    if (lcount == 0)
                                        IsContigentBeneNotAllowed = true;
                                }
                            }
                            else
                                IsContigentBeneNotAllowed = true;
                        }
                    }

                    if (lobjBAB.icdoBenefitApplicationBeneficiary.dist_percent <= 0
                        || lobjBAB.icdoBenefitApplicationBeneficiary.dist_percent > 100)
                        IsInValidPercentageEnteredForApplication = true;

                    IsApplicationSelected = true;
                    if (IsStartDateNotEnteredForApplication
                        || IsApplicationStartDateIsEarlierThanRetDAte
                        || IsApplicationBeneficiaryTypeNotEntered
                        || IsInValidPercentageEnteredForApplication
                        || IsContigentBeneNotAllowed
                        || IsStartDateEarlierThanEndDate)
                        break;
                }
            }
            if (!IsApplicationSelected && iclbBenefitApplicationBeneficiary.Count >0)
            {
                if (iblnIsPlanEnteredForBeneficiary)
                    IsApplicationSelected = true;
            }
        }

        public int iintApplicationCount
        {
            get
            {
                return iclbBenefitApplicationBeneficiary.Count;
            }
        }

        //get first payment date
        public DateTime idtFirstPaymentDate
        {
            get
            {
                DateTime ldtFirstPaymentDate = DateTime.MinValue;
                if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    ldtFirstPaymentDate = ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                    ldtFirstPaymentDate = new DateTime(ldtFirstPaymentDate.Year, ldtFirstPaymentDate.Month, 1);
                }
                return ldtFirstPaymentDate;
            }
        }

        public string istrFirstPaymentDate
        {
            get
            {
                return idtFirstPaymentDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public bool IsStartDateNotEnteredForApplication { get; set; }
        public bool IsApplicationStartDateIsEarlierThanRetDAte { get; set; }
        public bool IsApplicationBeneficiaryTypeNotEntered { get; set; }
        public bool IsInValidPercentageEnteredForApplication { get; set; }
        public bool IsContigentBeneNotAllowed { get; set; }
        public bool IsStartDateEarlierThanEndDate { get; set; }
        public bool IsApplicationSelected { get; set; }

        public string istrPlanName { get; set; }
        public string istrBeneficiaryToName { get; set; }
        public DateTime idtInitialPlanStartDate { get; set; }
        public string istrPlanParticipationStatus { get; set; }
        public int iintBeneficiaryToPersonId { get; set; }
        # endregion

        // PROD PIR ID 5660
        public override int Delete()
        {
            // Load all PA Beneficiary
            Collection<busPersonAccountBeneficiary> lclbPABeneficiary = new Collection<busPersonAccountBeneficiary>();
            DataTable ldtbPABene = Select<cdoPersonAccountBeneficiary>(new string[1] { "BENEFICIARY_ID" },
                                    new object[1] { icdoPersonBeneficiary.beneficiary_id }, null, null);
            lclbPABeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbPABene, "icdoPersonAccountBeneficiary");
            // Delete Beneficiary
            foreach (busPersonAccountBeneficiary lobjPABene in lclbPABeneficiary)
                lobjPABene.icdoPersonAccountBeneficiary.Delete();

            DataTable ldtbBene = Select<cdoPersonBeneficiary>(new string[1] { "BENEFICIARY_ID" },
                                    new object[1] { icdoPersonBeneficiary.beneficiary_id }, null, null);
            if (ldtbBene.Rows.Count > 0)
            {
                busPersonBeneficiary lobjBene = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                lobjBene.icdoPersonBeneficiary.LoadData(ldtbBene.Rows[0]);
                lobjBene.icdoPersonBeneficiary.Delete();
            }
            return 1;
        }
        public void LoadBeneficiaryOrganization()
        {
            if (ibusBeneficiaryOrganization.IsNull())
                ibusBeneficiaryOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                ibusBeneficiaryOrganization.FindOrganization(icdoPersonBeneficiary.benificiary_org_id);
        }

        public bool ValidateEmailPattern()  //PIR-18492
        {
            if (!string.IsNullOrEmpty(icdoPersonBeneficiary.email_address))
            {
                return busGlobalFunctions.IsEmailValid(icdoPersonBeneficiary.email_address);
            }
            return true;
        }
        public void LoadPersonAccountBeneficiaryByID()
        {
            DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
                new string[1] { "BENEFICIARY_ID" },
                new object[1] { icdoPersonBeneficiary.beneficiary_id }, null, null);
            _iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");
        }
        public bool IsPersonAddressExistOnBeneficiary()
        {
            if (ibusBeneficiaryPerson.IsNull())
                LoadBeneficiaryPerson();

            busPersonAddress lbusPersonCureentAddress = new busPersonAddress();
            if (ibusPerson.icolPersonAddress.IsNull())
                ibusPerson.LoadAddresses();

            if (ibusPerson.icolPersonAddress.Count > 0)
            {
                lbusPersonCureentAddress = ibusPerson.icolPersonAddress.Where(i => i.icdoPersonAddress.end_date == DateTime.MinValue && i.icdoPersonAddress.undeliverable_address != busConstant.Flag_Yes).FirstOrDefault();
            }

            if (icdoPersonBeneficiary.same_as_member_address == busConstant.Flag_Yes)
            {
                return true;
            }
            else if (icdoPersonBeneficiary.address_line_1.IsNotNullOrEmpty() && icdoPersonBeneficiary.address_city.IsNotNullOrEmpty() &&
                icdoPersonBeneficiary.address_state_value.IsNotNullOrEmpty() && icdoPersonBeneficiary.address_country_value.IsNotNullOrEmpty() &&
                icdoPersonBeneficiary.address_zip_code.IsNotNullOrEmpty())
            {
                return true;
            }
            else if (lbusPersonCureentAddress.IsNotNull() && lbusPersonCureentAddress.icdoPersonAddress.IsNotNull() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_city.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_state_value.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_country_value.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.addr_zip_code.IsNotNullOrEmpty() &&
                lbusPersonCureentAddress.icdoPersonAddress.end_date == DateTime.MinValue &&
                lbusPersonCureentAddress.icdoPersonAddress.undeliverable_address != busConstant.Flag_Yes)
            {
                return true;
            }
            return false;
        }
    }
}