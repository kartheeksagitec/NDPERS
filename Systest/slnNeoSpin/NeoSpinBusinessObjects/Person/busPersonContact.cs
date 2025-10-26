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

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busPersonContact : busExtendBase
    {
        public bool iblnIsFromMSS { get; set; }

        /** To Load the other Contacts in Person Contact Maintenance **/
        private Collection<busPersonContact> _iclbOtherContacts;
        public Collection<busPersonContact> iclbOtherContacts
        {
            get { return _iclbOtherContacts; }
            set { _iclbOtherContacts = value; }
        }

        private string _istrSuppressWarning;
        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        public void LoadOtherContacts()
        {
            if (_iclbOtherContacts == null)
            {
                _iclbOtherContacts = new Collection<busPersonContact>();
            }
            DataTable ldtbContacts = busNeoSpinBase.Select("cdoPersonContact.GetOtherContacts", new object[2] { icdoPersonContact.person_id, icdoPersonContact.person_contact_id });
            _iclbOtherContacts = GetCollection<busPersonContact>(ldtbContacts, "icdoPersonContact");
            foreach (busPersonContact lobjPersonContact in _iclbOtherContacts)
            {
                lobjPersonContact.LoadPerson();
                lobjPersonContact.LoadOrganization();
                lobjPersonContact.LoadContactName();
            }
        }

        /** To get Parent Person Information **/

        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoPersonContact.person_id);

        }

        //to display blank when PERSLink Id is not selected.  
        //PIR- 312
        public string istrPERLSinkId
        {
            get
            {
                string lstrPERLSinkID = String.Empty;
                if (_icdoPersonContact.contact_person_id != 0)
                    lstrPERLSinkID = Convert.ToString(_icdoPersonContact.contact_person_id);

                return lstrPERLSinkID;
            }
        }

        // To display same as member addess as Yes Or No
        public string istrSameAsMemberAddressFlag
        {
            get
            {
                string lstrMemberAddressFlag = busConstant.Flag_No_Value;
                if (_icdoPersonContact.same_as_member_address == "Y")
                {
                    lstrMemberAddressFlag = busConstant.Flag_Yes_Value;
                }
                return lstrMemberAddressFlag;
            }
        }

        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                _icdoPersonContact.address_validate_flag = busConstant.Flag_Yes;
                return;
            }

            // ArrayList larrErrors = new ArrayList();            
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = _icdoPersonContact.address_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = _icdoPersonContact.address_line_2;
            _lobjcdoWebServiceAddress.addr_city = _icdoPersonContact.address_city;
            _lobjcdoWebServiceAddress.addr_state_value = _icdoPersonContact.address_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = _icdoPersonContact.address_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = _icdoPersonContact.address_zip_4_code;
            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                _icdoPersonContact.address_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                _icdoPersonContact.address_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                _icdoPersonContact.address_city = _lobjcdoWebServiceAddressResult.addr_city;
                _icdoPersonContact.address_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                _icdoPersonContact.address_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                _icdoPersonContact.address_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            _icdoPersonContact.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;
            _icdoPersonContact.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            //return larrErrors.Add(this);
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadContactName();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusPerson.IsNull())
                LoadPerson();
            // pir 7232 Trim the leading and trailing spaces before saving
            //PIR 11071
            if (!icdoPersonContact.contact_name.IsNullOrEmpty())
                icdoPersonContact.contact_name = icdoPersonContact.contact_name.Trim();

            if (_icdoPersonContact.same_as_member_address != "N")
            {
                _icdoPersonContact.address_state_value = null;
                _icdoPersonContact.address_country_value = null;
            }
            VerifyAddressUsingUSPS();
            _AintValidateSpouse = IsValidSpouse();
            _icdoPersonContact.contact_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_icdoPersonContact.istrOrgCodeID);
            base.BeforeValidate(aenmPageMode);
        }

        /** To Load Contact Person Information **/

        private busPerson _ibusContactPerson;
        public busPerson ibusContactPerson
        {
            get { return _ibusContactPerson; }
            set { _ibusContactPerson = value; }
        }

        public void LoadContactPerson()
        {
            if (_ibusContactPerson == null)
            {
                _ibusContactPerson = new busPerson();
            }
            _ibusContactPerson.FindPerson(_icdoPersonContact.contact_person_id);
        }

        /** To Load Contact Organization Information**/

        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoPersonContact.contact_org_id);
        }

        /** To Load Contact Name **/

        public void LoadContactName()
        {
            if (_icdoPersonContact.contact_person_id != 0)
            {
                LoadContactPerson();
                _icdoPersonContact.lstrContactName = _ibusContactPerson.icdoPerson.FullName;
            }
            else if (_icdoPersonContact.contact_org_id != 0)
            {
                LoadOrganization();
                _icdoPersonContact.lstrContactName = _ibusOrganization.icdoOrganization.org_name;
            }
            //PIR - 441
            else
            {
                _icdoPersonContact.lstrContactName = _icdoPersonContact.contact_name;
            }
        }

        /** To Validate Email **/
        public bool ValidateEmail()
        {
            if (_icdoPersonContact.email_address != null)
            {
                return busGlobalFunctions.IsEmailValid(_icdoPersonContact.email_address); //18492
            }
            return true;
        }

        public override busBase GetCorPerson()
        {
            if (_ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }

        private int _AintValidateSpouse;
        public int AintValidateSpouse
        {
            get { return _AintValidateSpouse; }
            set { _AintValidateSpouse = value; }
        }

        public int IsValidSpouse()
        {
            if (_icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
            {
                DataTable ldtbList = new DataTable();
                if (icdoPersonContact.ienuObjectState == ObjectState.Insert)
                {
                    ldtbList = Select<cdoPersonContact>(new string[3] { "person_id", "relationship_value", "status_value" },
                                        new object[3] { icdoPersonContact.person_id, busConstant.PersonContactTypeSpouse, busConstant.PersonContactStatusActive }, null, null);
                }
                else
                {
                    ldtbList = SelectWithOperator<cdoPersonContact>(new string[4] { "person_id", "relationship_value", "person_contact_id", "status_value" },
                                            new string[4] { "=", "=", "<>", "=" }, new object[4] { icdoPersonContact.person_id, 
                                            busConstant.PersonContactTypeSpouse,icdoPersonContact.person_contact_id,busConstant.PersonContactStatusActive }, null);
                }
                if (ldtbList.Rows.Count > 0)
                    return 1;

                if (_icdoPersonContact.contact_person_id != 0)
                {
                    busPerson lobjContactPerson = new busPerson();
                    lobjContactPerson.icdoPerson = new cdoPerson();
                    lobjContactPerson.FindPerson(_icdoPersonContact.contact_person_id);
                    if (ibusPerson.icdoPerson.gender_value == lobjContactPerson.icdoPerson.gender_value)
                        return 2;

                    if ((!iblnIsFromMSS) && (ibusPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried ||
                       lobjContactPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried))
                        return 3;
                }
            }
            return 0;
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            LoadContactName();
            larrList.Add(this);
            return larrList;
        }

        #region UCS 24
        public string istrMSSContactName { get; set; }
        public void LoadMSSContactName()
        {
            if (icdoPersonContact.contact_person_id != 0)
            {
                LoadContactPerson();
                icdoPersonContact.istrMSSContactName = ibusContactPerson.icdoPerson.FullName;
                if (iblnReadOnly)
                {
                    icdoPersonContact.istrMSSNamePrefix = ibusContactPerson.icdoPerson.name_prefix_description;
                    icdoPersonContact.istrMSSNameSuffix = ibusContactPerson.icdoPerson.name_suffix_description;
                }
                else
                {
                    icdoPersonContact.istrMSSNamePrefix = ibusContactPerson.icdoPerson.name_prefix_value;
                    icdoPersonContact.istrMSSNameSuffix = ibusContactPerson.icdoPerson.name_suffix_value;
                }
            }
            else if (icdoPersonContact.contact_org_id != 0)
            {
                if (ibusOrganization.IsNull())
                    LoadOrganization();
                icdoPersonContact.istrMSSContactName = ibusOrganization.icdoOrganization.org_name;
            }
            else
                icdoPersonContact.istrMSSContactName = icdoPersonContact.contact_name;
        }

        public string istrPeopleSoftUrl
        {
            get
            {
                String lstr =iobjPassInfo.isrvDBCache.GetCodeDescriptionString(52, "PURL");
                return lstr;
            }
        }

        public string istrPeopleSoftInfoBubble
        {
            get
            {
                String prestr, poststr;   
                prestr = "<a href= \" https://" + istrPeopleSoftUrl + " \" target= \" _blank \" >";
                poststr = "</a>";
                String lstr = string.Format(busGlobalFunctions.GetMessageTextByMessageID(8567, iobjPassInfo),prestr,poststr);
                return lstr;
            }
        }
        #endregion


        public string contact_name_Mixed_Case
        {
            get
            {
                return string.IsNullOrEmpty(icdoPersonContact.lstrContactName) ? string.Empty : busGlobalFunctions.ToTitleCase(icdoPersonContact.lstrContactName);
            }
        }
        //Prod PIR:1901 Contact name in the letter to appear in Mixed Case whereas in Address to appear in Upper case.
        public string contact_name_CAPS
        {
            get
            {
                return string.IsNullOrEmpty(icdoPersonContact.lstrContactName) ? string.Empty : icdoPersonContact.lstrContactName.ToUpper();
            }
        }
        public bool iblnIsCodeId304Data2Null
        {
            get
            {
                return (busGlobalFunctions.GetData2ByCodeValue(304, icdoPersonContact.relationship_value, iobjPassInfo).IsNullOrEmpty()) ? true : false;
            }

        }
    }
}


