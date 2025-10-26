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
    [Serializable]
    public class busFilePersonInbound : busFileBase
    {

        public busFilePersonInbound()
        {

        }

        private bool iblnErrorFound;

        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get
            {
                return _ibusPerson;
            }

            set
            {
                _ibusPerson = value;
            }
        }

        private busPerson _ibusUpdatePerson;
        public busPerson ibusUpdatePerson
        {
            get
            {
                return _ibusUpdatePerson;
            }

            set
            {
                _ibusUpdatePerson = value;
            }
        }

        public override busBase NewDetail()
        {
            _ibusPerson = new busPerson();
            _ibusPerson.icdoPerson = new cdoPerson();
            _ibusPerson.icdoFilePersonAddress = new cdoPersonAddress();
            return _ibusPerson;
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            string lstrObjectField = astrFieldName.IndexOf(".") > -1 ? astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1) : astrFieldName;

            if (lstrObjectField == "marital_status_value")
            {
                switch (astrFieldValue)
                {
                    case busConstant.PersonMaritalStatusMarriedDesc:
                        lstrReturnValue = busConstant.PersonMaritalStatusMarried;
                        break;
                    case busConstant.PersonMaritalStatusSingleDesc:
                        lstrReturnValue = busConstant.PersonMaritalStatusSingle;
                        break;
                    case busConstant.PersonMaritalStatusDivorcedDesc:
                        lstrReturnValue = busConstant.PersonMaritalStatusDivorced;
                        break;
                    case busConstant.PersonMaritalStatusWidowDesc:
                        lstrReturnValue = busConstant.PersonMaritalStatusWidow;
                        break;
                    case busConstant.PersonMaritalStatusMarried:
                    case busConstant.PersonMaritalStatusSingle:
                    case busConstant.PersonMaritalStatusDivorced:
                    case busConstant.PersonMaritalStatusWidow:
                        lstrReturnValue = astrFieldValue;
                        break;
                    default:
                        lstrReturnValue = string.Empty;
                        break;
                }
            }

            if (lstrObjectField == "email_address")
            {
                if ((String.IsNullOrEmpty(astrFieldValue)) || (astrFieldValue == "null"))
                {
                    lstrReturnValue = String.Empty;
                }
            }

            return lstrReturnValue;
        }

        /// <summary>
        /// 1) Validating the Required Fields
        /// 2) Loading the Person Object from the Database by Given SSN
        /// 3) Assign the PeopleSoft ID, Phone, Email Address Values from the File to Person Object
        /// 4) Loading the Latest Permanent Address Object
        /// 5) If exists, Compare with File Address Object.
        /// 6) If Not Matches, Validate the File Address with USPS Web Service
        /// 7) If not Valid, Throw an Error. Else, End the Last Permanent Address, and Create New Address
        /// 8) Validate the Person Contact Address
        /// 9) If Valid, Inactive the Previoid Active Primary Contat and Create New Contact Address
        /// 10) If not Valid, Throw an error.
        /// 11)Update the Person Object if there is no errors in Person Contact / Address.
        /// </summary>
        public override void ProcessDetail()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;

            //Validate the Primary Fields
            larrList = ValidatePrimaryFileFields();

            //If Primary File Layout Error Exists, Return
            if (larrList.Count > 0)
            {
                _ibusPerson.iarrErrors = larrList;
                return;
            }

            ibusUpdatePerson = new busPerson();
            ibusUpdatePerson.icdoPerson = new cdoPerson();
            bool lblnPersonExists = LoadPersonBySSNAndUpdateFileData();
            if (!lblnPersonExists)
            {
                //SSN not exists
                lobjError = new utlError();
                lobjError.istrErrorID = "1074";
                lobjError.istrErrorMessage = "SSN does not exists!";
                larrList.Add(lobjError);
            }

            //If SSN not Exists, Return
            if (larrList.Count > 0)
            {
                _ibusPerson.iarrErrors = larrList;
                return;
            }
            bool lblnPersonAddressValid = ValidatePersonAddress();
            if (!lblnPersonAddressValid)
            {
                //If Address is not Valid Throw an Error
                lobjError = new utlError();
                lobjError.istrErrorID = "130";
                lobjError.istrErrorMessage = "Person Address is invalid : " + _ibusPerson.icdoFilePersonAddress.address_validate_error;
                larrList.Add(lobjError);
            }
            else
            {
                //Updating the Person Address
                //Get the Latest Primary Address
                busPersonAddress lobjPersonAddress = new busPersonAddress();
                lobjPersonAddress.icdoPersonAddress = new cdoPersonAddress();
                bool lblnAddressExists = LoadLatestPrimaryAddress(lobjPersonAddress);
                if (lblnAddressExists)
                {
                    //If Address Match, we do not need to update, Skip the Record
                    //else, Validate the Address with USPS
                    if (!CheckIfPersonAddressMatch(lobjPersonAddress))
                    {

                        //If Address Valid, Check if it is Future Date
                        if (CheckIfAddressStartDateIsFutureDate(lobjPersonAddress))
                        {
                            lobjError = new utlError();
                            lobjError.istrErrorID = "133";
                            lobjError.istrErrorMessage = "Future Date Person Address already exists! Address can not be updated!";
                            larrList.Add(lobjError);
                        }
                        else
                        {
                            //Update the Previous Address End Date and Create New Address
                            //If End Date is Future Date or Null, Update it with Current Date
                            //Else, leave the End Date as past Date
                            if ((lobjPersonAddress.icdoPersonAddress.end_date > DateTime.Now) ||
                                (lobjPersonAddress.icdoPersonAddress.end_date == DateTime.MinValue))
                            {
                                lobjPersonAddress.icdoPersonAddress.end_date = DateTime.Now;
                            }
                            lobjPersonAddress.icdoPersonAddress.Update();

                            //Create the New Person Address
                            CreateNewPersonAddress();
                        }

                    }
                }
                else
                {
                    //If Address Not Exists Create New Person Address
                    CreateNewPersonAddress();
                }
            }
            //If Address level Exception Occurs, dont update the Person Object. Return the Error List
            if (larrList.Count > 0)
            {
                _ibusPerson.iarrErrors = larrList;
                return;
            }

            //If Contact level Exception Occurs, dont update the Person Object. Return the Error List
            if (larrList.Count > 0)
            {
                _ibusPerson.iarrErrors = larrList;
                return;
            }

            //Update the Person Object
            _ibusUpdatePerson.icdoPerson.Update();
        }

        /// <summary>
        /// Check if the Address Start Date is Future Date
        /// </summary>
        /// <param name="lobjPersonAddress"></param>
        /// <returns></returns>
        private bool CheckIfAddressStartDateIsFutureDate(busPersonAddress lobjPersonAddress)
        {
            bool lblnFutureDate = false;
            if (lobjPersonAddress.icdoPersonAddress.start_date > DateTime.Now)
            {
                lblnFutureDate = true;
            }
            return lblnFutureDate;
        }

        /// <summary>
        /// Check If Latest Person Permanent Address Match with File Address
        /// </summary>
        /// <param name="lobjPersonAddress"></param>
        /// <returns></returns>
        private bool CheckIfPersonAddressMatch(busPersonAddress lobjPersonAddress)
        {
            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_line_1))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_line_1))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_line_1.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_line_1.ToLower())
            {
                return false;
            }

            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_line_2))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_line_2))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_line_2.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_line_2.ToLower())
            {
                return false;
            }

            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_city))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_city))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_city.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_city.ToLower())
            {
                return false;
            }

            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_state_value))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_state_value))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_state_value.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_state_value.ToLower())
            {
                return false;
            }
            //prod pir 2470 
            //if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_country_value))
            //{
            //    if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_country_value))
            //    {
            //        return false;
            //    }
            //}
            //else if (_ibusPerson.icdoFilePersonAddress.addr_country_value.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_country_value.ToLower())
            //{
            //    return false;
            //}

            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_zip_code))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_zip_code))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_zip_code.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_zip_code.ToLower())
            {
                return false;
            }

            if (String.IsNullOrEmpty(_ibusPerson.icdoFilePersonAddress.addr_zip_4_code))
            {
                if (!String.IsNullOrEmpty(lobjPersonAddress.icdoPersonAddress.addr_zip_4_code))
                {
                    return false;
                }
            }
            else if (_ibusPerson.icdoFilePersonAddress.addr_zip_4_code.ToLower() != lobjPersonAddress.icdoPersonAddress.addr_zip_4_code.ToLower())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Load the Latest Primary Address 
        /// </summary>
        /// <param name="lobjPersonAddress"></param>
        /// <returns></returns>
        private bool LoadLatestPrimaryAddress(busPersonAddress lobjPersonAddress)
        {
            bool lblnAddressExists = false;
            DataTable ldtbList = busBase.Select<cdoPersonAddress>(new string[2] { "person_id", "address_type_value" },
                                                                  new object[2]
                                                                      {
                                                                          _ibusUpdatePerson.icdoPerson.person_id,
                                                                          busConstant.AddressTypePermanent
                                                                      }, null,
                                                                  "start_date desc");
            if (ldtbList.Rows.Count > 0)
            {
                lobjPersonAddress.icdoPersonAddress.LoadData(ldtbList.Rows[0]);
                lblnAddressExists = true;
            }
            return lblnAddressExists;
        }

        /// <summary>
        /// Updating only the following data from File for a Person Object
        /// 1) Peopole Soft ID
        /// 2) Home Phone
        /// 3) Cell Phone
        /// 4) Work Phone
        /// 5) Email Address
        /// </summary>        
        /// <returns></returns>
        private bool LoadPersonBySSNAndUpdateFileData()
        {
            bool lblnPersonExists = false;
            DataTable ldtbPerson = busBase.Select<cdoPerson>(new string[1] { "ssn" }, new object[1] { _ibusPerson.icdoPerson.ssn }, null, null);
            if (ldtbPerson.Rows.Count > 0)
            {
                ibusUpdatePerson.icdoPerson.LoadData(ldtbPerson.Rows[0]);
                if (!String.IsNullOrEmpty(_ibusPerson.icdoPerson.marital_status_value))
                {
                    ibusUpdatePerson.icdoPerson.marital_status_value = _ibusPerson.icdoPerson.marital_status_value;
                }
                ibusUpdatePerson.icdoPerson.peoplesoft_id = _ibusPerson.icdoPerson.peoplesoft_id;
                ibusUpdatePerson.icdoPerson.home_phone_no = _ibusPerson.icdoPerson.home_phone_no;
                ibusUpdatePerson.icdoPerson.cell_phone_no = _ibusPerson.icdoPerson.cell_phone_no;
                ibusUpdatePerson.icdoPerson.work_phone_no = _ibusPerson.icdoPerson.work_phone_no;
                ibusUpdatePerson.icdoPerson.email_address = _ibusPerson.icdoPerson.email_address;
                ibusUpdatePerson.icdoPerson.ms_change_date = _ibusPerson.icdoPerson.ms_change_date;
                lblnPersonExists = true;
            }
            return lblnPersonExists;
        }

        /// <summary>
        /// Function to Validate the Primary Field Fields
        /// </summary>
        /// <returns>Boolean</returns>
        private ArrayList ValidatePrimaryFileFields()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;

            string lstrSSN = _ibusPerson.icdoPerson.ssn;
            if (String.IsNullOrEmpty(lstrSSN))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "152";
                lobjError.istrErrorMessage = "SSN must be entered.";
                larrList.Add(lobjError);
            }

            if (lstrSSN.Trim().Length != 9)
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1055";
                lobjError.istrErrorMessage = "SSN should be 9 digits.";
                larrList.Add(lobjError);
            }

            //Peoplesoft id ahould be numeric
            if (!(HelperFunction.IsNumeric(_ibusPerson.icdoPerson.peoplesoft_id)))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1081";
                lobjError.istrErrorMessage = "PeopleSoft ID should be numeric.";
                larrList.Add(lobjError);
            }

            ////Check marital Status
            //if (String.IsNullOrEmpty(_ibusPerson.icdoPerson.marital_status_value))
            //{
            //    lobjError = new utlError();
            //    lobjError.istrErrorID = "1080";
            //    lobjError.istrErrorMessage = "Marital Status is not valid.";
            //    larrList.Add(lobjError);
            //}

            //first name should be maximum 40 characters
            if (!String.IsNullOrEmpty(_ibusPerson.icdoPerson.first_name))
            {
                if (_ibusPerson.icdoPerson.first_name.Trim().Length > 40)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "1078";
                    lobjError.istrErrorMessage = "Person First Name should be maximum of 40 characters.";
                    larrList.Add(lobjError);
                }
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "150";
                lobjError.istrErrorMessage = "First Name is Required!";
                larrList.Add(lobjError);
            }

            //last name should be maximum 40 characters
            if (!String.IsNullOrEmpty(_ibusPerson.icdoPerson.last_name))
            {
                if (_ibusPerson.icdoPerson.last_name.Trim().Length > 40)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "1079";
                    lobjError.istrErrorMessage = "Person Last Name should be maximum of 40 characters.";
                    larrList.Add(lobjError);
                }
            }
            else
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "151";
                lobjError.istrErrorMessage = "Last Name is Required!";
                larrList.Add(lobjError);
            }

            //middle name should be maximum 40 characters
            if (!String.IsNullOrEmpty(_ibusPerson.icdoPerson.middle_name))
            {
                if (_ibusPerson.icdoPerson.middle_name.Trim().Length > 40)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "1080";
                    lobjError.istrErrorMessage = "Person Middle Name should be maximum of 40 characters.";
                    larrList.Add(lobjError);
                }
            }

            //home phone number should be numeric
            if (!(HelperFunction.IsNumeric(_ibusPerson.icdoPerson.home_phone_no)))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1040";
                lobjError.istrErrorMessage = "Home phone must be numeric.";
                larrList.Add(lobjError);
            }

            //cell phone number should be numeric
            if (!(HelperFunction.IsNumeric(_ibusPerson.icdoPerson.cell_phone_no)))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1039";
                lobjError.istrErrorMessage = "Cell phone must be numeric.";
                larrList.Add(lobjError);
            }

            //work phone number should be numeric
            if (!(HelperFunction.IsNumeric(_ibusPerson.icdoPerson.work_phone_no)))
            {
                lobjError = new utlError();
                lobjError.istrErrorID = "1038";
                lobjError.istrErrorMessage = "Work phone must be numeric.";
                larrList.Add(lobjError);
            }

            return larrList;
        }

        /// <summary>
        /// Function to Create New Person Permanent Address
        /// </summary>
        public void CreateNewPersonAddress()
        {
            cdoPersonAddress lobjPersonAddress = _ibusPerson.icdoFilePersonAddress;
            lobjPersonAddress.person_id = _ibusUpdatePerson.icdoPerson.person_id;
            lobjPersonAddress.address_type_value = busConstant.AddressTypePermanent;
            lobjPersonAddress.start_date = DateTime.Now.AddDays(1);
            lobjPersonAddress.peoplesoft_flag = busConstant.Flag_Yes;
            if (!string.IsNullOrEmpty(lobjPersonAddress.addr_state_value))
                lobjPersonAddress.addr_country_value = busConstant.US_Code_ID;
            lobjPersonAddress.Insert();
        }

        /// <summary>
        /// Validate the File Person Address with USPS
        /// </summary>
        /// <returns>Boolean</returns>
        public bool ValidatePersonAddress()
        {
            bool lblnPersonAddressValid = false;

            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = _ibusPerson.icdoFilePersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = _ibusPerson.icdoFilePersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = _ibusPerson.icdoFilePersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = _ibusPerson.icdoFilePersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = _ibusPerson.icdoFilePersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = _ibusPerson.icdoFilePersonAddress.addr_zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult1 = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult1.address_validate_flag == busConstant.Flag_Yes)
            {
                _ibusPerson.icdoFilePersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult1.addr_line_1;
                _ibusPerson.icdoFilePersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult1.addr_line_2;
                _ibusPerson.icdoFilePersonAddress.addr_city = _lobjcdoWebServiceAddressResult1.addr_city;
                _ibusPerson.icdoFilePersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult1.addr_state_value;
                _ibusPerson.icdoFilePersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult1.addr_zip_code;
                _ibusPerson.icdoFilePersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult1.addr_zip_4_code;
                lblnPersonAddressValid = true;
            }
            _ibusPerson.icdoFilePersonAddress.address_validate_error = _lobjcdoWebServiceAddressResult1.address_validate_error;
            _ibusPerson.icdoFilePersonAddress.address_validate_flag = _lobjcdoWebServiceAddressResult1.address_validate_flag;
            return lblnPersonAddressValid;
        }
    }
}
