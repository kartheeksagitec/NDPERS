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

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busOrgContactAddress : busExtendBase
    {
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get
            {
                return _ibusOrganization;
            }

            set
            {
                _ibusOrganization = value;
            }
        }
        private string _address_validate_flag;
        public string address_validate_flag
        {
            get
            {
                return _address_validate_flag;
            }

            set
            {
                _address_validate_flag = value;
            }
        }
        private busContact _ibusContact;
        public busContact ibusContact
        {
            get
            {
                return _ibusContact;
            }

            set
            {
                _ibusContact = value;
            }
        }

        private string _istrSuppressWarning;

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        private Collection<busOrgContactAddress> _iclbOtherOrgAddress;
        public Collection<busOrgContactAddress> iclbOtherOrgAddress
        {
            get
            {
                return _iclbOtherOrgAddress;
            }

            set
            {
                _iclbOtherOrgAddress = value;
            }
        }

        private Collection<busOrgContactAddress> _iclbOtherContactAddress;
        public Collection<busOrgContactAddress> iclbOtherContactAddress
        {
            get
            {
                return _iclbOtherContactAddress;
            }

            set
            {
                _iclbOtherContactAddress = value;
            }
        }

        public string primary_address_flag { get; set; }

        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = "";
                if (_icdoOrgContactAddress.addr_line_1 != null)
                {
                    _addr_description += _icdoOrgContactAddress.addr_line_1 + ", ";
                }
                if (_icdoOrgContactAddress.addr_line_2 != null)
                {
                    _addr_description += _icdoOrgContactAddress.addr_line_2 + ", ";
                }
                if (_icdoOrgContactAddress.city != null)
                {
                    _addr_description += _icdoOrgContactAddress.city + ", ";
                }

                if (_icdoOrgContactAddress.country_value == busConstant.US_Code_ID)
                {
                    //PROD PIR : 170
                    //PIR - 13171 Update
                    if (_icdoOrgContactAddress.state_value != null)
                    {
                        _addr_description += _icdoOrgContactAddress.state_value + " ";
                    }

                    if (_icdoOrgContactAddress.zip_code != null)
                    {
                        _addr_description += _icdoOrgContactAddress.zip_code;
                    }
                    if (_icdoOrgContactAddress.zip_4_code != null)
                    {
                        _addr_description += "-" + _icdoOrgContactAddress.zip_4_code;
                    }
                }
                else
                {
                    if (_icdoOrgContactAddress.state_description != null)
                    {
                        _addr_description += _icdoOrgContactAddress.state_value + ", ";
                    }

                    if (!String.IsNullOrEmpty(_icdoOrgContactAddress.foreign_province))
                    {
                        _addr_description += _icdoOrgContactAddress.foreign_province + ", ";
                    }

                    if (!String.IsNullOrEmpty(_icdoOrgContactAddress.foreign_postal_code))
                    {
                        _addr_description += _icdoOrgContactAddress.foreign_postal_code + ", ";
                    }

                    if (_icdoOrgContactAddress.country_description != null)
                    {
                        _addr_description += _icdoOrgContactAddress.country_description;
                    }

                    //PIR - 13171 Update
                   if (_icdoOrgContactAddress.zip_code != null)
                    {
                        _addr_description += _icdoOrgContactAddress.zip_code;
                    }
                    if (_icdoOrgContactAddress.zip_4_code != null)
                    {
                        _addr_description += "-" + _icdoOrgContactAddress.zip_4_code;
                    }

                }
                return _addr_description;
            }
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoOrgContactAddress.org_id);
        }

        public void LoadContact()
        {
            if (_ibusContact == null)
            {
                _ibusContact = new busContact();
            }
            _ibusContact.FindContact(_icdoOrgContactAddress.contact_id);
        }

        public void LoadOtherOrgAddress()
        {
            if (_iclbOtherOrgAddress == null)
            {
                _iclbOtherOrgAddress = new Collection<busOrgContactAddress>();
            }
            DataTable ldtbOrgAddress = busNeoSpinBase.Select("cdoOrgContactAddress.LoadOtherOrgAddresses", new object[2] { _icdoOrgContactAddress.org_id, _icdoOrgContactAddress.contact_org_address_id });
            _iclbOtherOrgAddress = GetCollection<busOrgContactAddress>(ldtbOrgAddress, "icdoOrgContactAddress");
            foreach (busOrgContactAddress lobjTempOrgAddress in _iclbOtherOrgAddress)
            {
                if (lobjTempOrgAddress.icdoOrgContactAddress.contact_org_address_id == _ibusOrganization.icdoOrganization.primary_address_id)
                {
                    //Load the Org Property in Org Contact Address to fill the Primary Address Flag
                    lobjTempOrgAddress.LoadOrganization();

                    lobjTempOrgAddress.primary_address_flag = "Y";
                }
                else
                {
                    lobjTempOrgAddress.primary_address_flag = "N";
                }
            }
        }

        public void LoadOtherContactAddress()
        {
            if (_iclbOtherContactAddress == null)
            {
                _iclbOtherContactAddress = new Collection<busOrgContactAddress>();
            }

            DataTable ldtbContactAddress = busNeoSpinBase.Select("cdoOrgContactAddress.LoadOtherContactAddresses", new object[2] { _icdoOrgContactAddress.contact_id, _icdoOrgContactAddress.contact_org_address_id });
            _iclbOtherContactAddress = GetCollection<busOrgContactAddress>(ldtbContactAddress, "icdoOrgContactAddress");

            foreach (busOrgContactAddress lobjTempContactAddress in _iclbOtherContactAddress)
            {
                if (lobjTempContactAddress.icdoOrgContactAddress.contact_org_address_id == _ibusContact.icdoContact.primary_address_id)
                {
                    //Load the Contact Property in Org Contact Address to fill the Primary Address Flag
                    lobjTempContactAddress.LoadContact();

                    lobjTempContactAddress.primary_address_flag = "Y";
                }
                else
                {
                    lobjTempContactAddress.primary_address_flag = "N";
                }
            }
        }

        private string _istrCounty;
        public string istrCounty
        {
            get { return _istrCounty; }
            set { _istrCounty = value; }
        }

        public void LoadCounty()
        {
            _istrCounty = Convert.ToString(DBFunction.DBExecuteScalar("cdoCountyRef.LOOKUP", new object[1] { _icdoOrgContactAddress.city == null ? string.Empty : _icdoOrgContactAddress.city }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadCounty();
            if (_icdoOrgContactAddress.org_id != 0)
            {
                if ((primary_address_flag == "Yes") && (_icdoOrgContactAddress.status_value == "ACTV"))
                {
                    _ibusOrganization.icdoOrganization.primary_address_id = _icdoOrgContactAddress.contact_org_address_id;
                }
                else
                {
                    _ibusOrganization.icdoOrganization.primary_address_id = 0;
                }
                _ibusOrganization.icdoOrganization.Update();
            }
            if (_icdoOrgContactAddress.contact_id != 0)
            {
                if ((primary_address_flag == "Yes") && (_icdoOrgContactAddress.status_value == "ACTV"))
                {
                    _ibusContact.icdoContact.primary_address_id = _icdoOrgContactAddress.contact_org_address_id;
                }
                else
                {
                    _ibusContact.icdoContact.primary_address_id = 0;
                }
                _ibusContact.icdoContact.Update();
            }
        }

        // If Primary flag is set to Yes, cannot change the status of primary address.
        public bool CheckPrimaryflagAndStatus()
        {
            if ((primary_address_flag == "Yes") && (_icdoOrgContactAddress.status_value == "INAC"))
            {
                return true;
            }
            return false;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            VerifyAddressUsingUSPS();
            base.BeforeValidate(aenmPageMode);
        }
        //PIR - 294
        /// <summary>
        /// Verify Address using Web Service and save after validation.
        /// </summary>
        /// <returns></returns>
        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                address_validate_flag = busConstant.Flag_Yes;
                return;
            }

            //ArrayList larrErrors = new ArrayList();
            //utlError iobjerror = new utlError();
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = _icdoOrgContactAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = _icdoOrgContactAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = _icdoOrgContactAddress.city;
            _lobjcdoWebServiceAddress.addr_state_value = _icdoOrgContactAddress.state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = _icdoOrgContactAddress.zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = _icdoOrgContactAddress.zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                _icdoOrgContactAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                _icdoOrgContactAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                _icdoOrgContactAddress.city = _lobjcdoWebServiceAddressResult.addr_city;
                _icdoOrgContactAddress.state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                _icdoOrgContactAddress.zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                _icdoOrgContactAddress.zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            //return larrErrors.Add(this);
        }
    }
}
