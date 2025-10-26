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
using System.Net;
using System.Xml;
using System.IO;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMSSPersonContact : busPersonContact
    {
        public bool iblnIsReadOnly { get; set; } //pir 8623
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (utlError lobjError in iarrErrors)
                lobjError.istrErrorID = string.Empty;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            istrSuppressWarning = icdoPersonContact.istrMSSSuppressWarning;            
            GetPersonId();
            iblnIsFromMSS = true; // PIR 9752
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (icdoPersonContact.ienuObjectState == ObjectState.Insert)
            {
                bool lblnPersonFound = false;
                // PIR 9770 Person Needs to be created eventhough the relationship is not Spouse.
                if (//icdoPersonContact.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse &&
                    icdoPersonContact.status_value == busConstant.PersonContactStatusActive)
                {
                    //if last name, dob, ssn entered, then find the person id
                    if ((!String.IsNullOrEmpty(icdoPersonContact.istrMSSLastName))
                        && (!String.IsNullOrEmpty(icdoPersonContact.istrMSSFirstName))
                        && (icdoPersonContact.idtMSSContactDOB != DateTime.MinValue)
                        && (!String.IsNullOrEmpty(icdoPersonContact.istrMSSGender)))
                    {
                        if (!String.IsNullOrEmpty(icdoPersonContact.istrMSSSSN))
                        {
                            lblnPersonFound = true;
                            GetPersonId();
                            //only if the contact person id is not found then only create new person
                            if (icdoPersonContact.contact_person_id == 0)
                            {
                                CreatePerson();
                            }
                        }
                    }
                }
                //if no person id found then store only contact name
                if (!lblnPersonFound)
                {
                    icdoPersonContact.contact_name = icdoPersonContact.istrMSSNamePrefix + " " + icdoPersonContact.istrMSSLastName + ", " +
                        icdoPersonContact.istrMSSFirstName + " " + icdoPersonContact.istrMSSMiddleName + " " + icdoPersonContact.istrMSSNameSuffix;
                }
            }
            else
            {
                if (icdoPersonContact.contact_person_id > 0 && ibusContactPerson.IsNotNull())
                {
                    ibusContactPerson.icdoPerson.name_prefix_value = icdoPersonContact.istrMSSNamePrefix;
                    ibusContactPerson.icdoPerson.name_suffix_value = icdoPersonContact.istrMSSNameSuffix;
                    ibusContactPerson.icdoPerson.Update();
                }
            }
            //PIR-18492 :To Post message to message board           
            if(iblnExternalUser)
                busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10319, iobjPassInfo), "Spouse/Contact"), busConstant.WSS_MessageBoard_Priority_High,
                icdoPersonContact.person_id);
        }

        private void CreatePerson()
        {
            cdoPerson lcdoPerson = new cdoPerson();
            lcdoPerson.first_name = icdoPersonContact.istrMSSFirstName;
            lcdoPerson.last_name = icdoPersonContact.istrMSSLastName;
            lcdoPerson.middle_name = icdoPersonContact.istrMSSMiddleName;
            lcdoPerson.name_prefix_value = icdoPersonContact.istrMSSNamePrefix;
            lcdoPerson.name_suffix_value = icdoPersonContact.istrMSSNameSuffix;
            lcdoPerson.ssn = icdoPersonContact.istrMSSSSN;
            lcdoPerson.gender_value = icdoPersonContact.istrMSSGender;
            lcdoPerson.date_of_birth = icdoPersonContact.idtMSSContactDOB;
            lcdoPerson.marital_status_value = busConstant.PersonMaritalStatusMarried; // PIR 9752
            lcdoPerson.Insert();
            icdoPersonContact.contact_person_id = lcdoPerson.person_id;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadMSSContactName();
        }

        public void GetPersonId()
        {
            if ((!String.IsNullOrEmpty(icdoPersonContact.istrMSSLastName))
                   && (icdoPersonContact.idtMSSContactDOB != DateTime.MinValue))
            {
                if (!String.IsNullOrEmpty(icdoPersonContact.istrMSSSSN))
                {
                    //PIR 12794
                    DataTable ldtbList = Select<cdoPerson>(new string[1] { "SSN" }, 
                                                            new object[1] { icdoPersonContact.istrMSSSSN }, null, null);

                    if (ldtbList.Rows.Count > 0)
                    {
                        cdoPerson lcdoPerson = new cdoPerson();
                        lcdoPerson.LoadData(ldtbList.Rows[0]);
                        icdoPersonContact.contact_person_id = lcdoPerson.person_id;
                    }
                }
            }
        }

        public bool IsValidGender()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icdoPerson.gender_value == icdoPersonContact.istrMSSGender)
                return false;
            return true;
        }

        //check is same perslink added twice and with same relationship
        public bool IsContactPersonidDuplicated()
        {
            if (icdoPersonContact.person_contact_id > 0)
            {
                if (iclbOtherContacts.IsNull())
                    LoadOtherContacts();

                var llist = iclbOtherContacts.Where(lobj => lobj.icdoPersonContact.contact_person_id == icdoPersonContact.contact_person_id
                    && (lobj.icdoPersonContact.relationship_value == icdoPersonContact.relationship_value)
                    && lobj.icdoPersonContact.status_value.Trim() == icdoPersonContact.status_value);

                if (llist.Count() > 0)
                    return true;
            }
            return false;
        }

        // PIR 9770
        public void LoadContactAddress()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (icdoPersonContact.same_as_member_address == busConstant.Flag_Yes)
            {
                if (ibusPerson.ibusPersonCurrentAddress.IsNull()) ibusPerson.LoadPersonCurrentAddress();
            }
            else
            {
                if (ibusPerson.ibusPersonCurrentAddress.IsNull())
                    ibusPerson.ibusPersonCurrentAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress(), icdoPersonCurrentAddress = new cdoPersonAddress() };

                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1 = icdoPersonContact.istrAddressLine1_CAPS;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2 = icdoPersonContact.istrAddressLine2_CAPS;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city = icdoPersonContact.istrAddressCity_CAPS;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value = icdoPersonContact.address_state_value;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_description =
                    busGlobalFunctions.GetDescriptionByCodeValue(150, icdoPersonContact.address_state_value, iobjPassInfo);
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value = icdoPersonContact.address_country_value;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_description =
                    busGlobalFunctions.GetDescriptionByCodeValue(151, icdoPersonContact.address_country_value, iobjPassInfo);
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code = icdoPersonContact.address_zip_code;
                ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code = icdoPersonContact.address_zip_4_code;
            }
        }
        //PIR 12794
        public bool IsSSNDuplicated()
        {
            if (!String.IsNullOrEmpty(icdoPersonContact.istrMSSSSN) && icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
            {
                int lintPersonId = busGlobalFunctions.GetPersonIDBySSN(icdoPersonContact.istrMSSSSN);
                if (lintPersonId > 0)
                {
                    DataTable ldtbContact = Select<cdoPersonContact>(new string[3] { "CONTACT_PERSON_ID", "RELATIONSHIP_VALUE", "STATUS_VALUE" }, 
                                                                    new object[3] { lintPersonId, busConstant.PersonContactTypeSpouse, busConstant.PersonContactStatusActive }, null, null);
                    if (ldtbContact.Rows.Count > 0)
                        return true;
                }
            }
            return false;
        }
		
        // PIR - 17572
        public bool IsDuplicatePerson()
        {
            busPerson lbusDuplicatePerson = new busPerson();
            lbusDuplicatePerson = lbusDuplicatePerson.LoadPersonBySsn(icdoPersonContact.istrMSSSSN);
            return (lbusDuplicatePerson.IsNotNull()
			        && (lbusDuplicatePerson.icdoPerson.date_of_birth != icdoPersonContact.idtMSSContactDOB
                    || lbusDuplicatePerson.icdoPerson.gender_value != icdoPersonContact.istrMSSGender));
        }
		
        public bool IsInvalidSSN()
        {
            busPerson lobjInvalidSSN = new busPerson();
            return (lobjInvalidSSN.LoadInvalidSSN(icdoPersonContact.istrMSSSSN));
        }

        /** To Validate Email **/
        public bool ValidateEmailPattern()
        {
            if (icdoPersonContact.email_address != null)
            {
                return busGlobalFunctions.IsEmailValid(icdoPersonContact.email_address); //18492
            }
            return true;
        }

    }
}


