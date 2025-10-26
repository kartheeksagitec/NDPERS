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
    public partial class busPersonAddress : busExtendBase
    {
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

        //PIR 14848
        private busPersonAccountMedicarePartDHistory _ibusPersonAccountMedicare;
        public busPersonAccountMedicarePartDHistory ibusPersonAccountMedicare
        {
            get
            {
                return _ibusPersonAccountMedicare;
            }

            set
            {
                _ibusPersonAccountMedicare = value;
            }
        }

        private string _istrSuppressWarning;
        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        private cdoPersonAddress _icdoPersonCurrentAddress;
        public cdoPersonAddress icdoPersonCurrentAddress
        {
            get
            {
                return _icdoPersonCurrentAddress;
            }

            set
            {
                _icdoPersonCurrentAddress = value;
            }
        }

        private Collection<busPersonAddress> _iclbOtherAddresses;
        public Collection<busPersonAddress> iclbOtherAddresses
        {
            get { return _iclbOtherAddresses; }
            set { _iclbOtherAddresses = value; }
        }

        private string _zip_description;
        public string zip_description
        {
            get
            {
                _zip_description = String.Empty;
                if (_icdoPersonAddress.addr_zip_code != null)
                {
                    if (_icdoPersonAddress.addr_country_value == busConstant.US_Code_ID)
                    {
                        if (_icdoPersonAddress.addr_zip_4_code != null)
                        {
                            _zip_description = _icdoPersonAddress.addr_zip_code + "-" + _icdoPersonAddress.addr_zip_4_code;
                        }
                        else
                        {
                            _zip_description = _icdoPersonAddress.addr_zip_code;
                        }
                    }
                    else
                    {
                        _zip_description = _icdoPersonAddress.addr_zip_code;
                    }
                }
                return _zip_description;
            }
        }

        private string _zip_description_without_dash;
        public string zip_description_without_dash
        {
            get
            {
                _zip_description_without_dash = String.Empty;
                if (_icdoPersonAddress.addr_zip_code != null)
                {
                    if (_icdoPersonAddress.addr_country_value == busConstant.US_Code_ID)
                    {
                        if (_icdoPersonAddress.addr_zip_4_code != null)
                        {
                            _zip_description_without_dash = _icdoPersonAddress.addr_zip_code + _icdoPersonAddress.addr_zip_4_code;
                        }
                        else
                        {
                            _zip_description_without_dash = _icdoPersonAddress.addr_zip_code;
                        }
                    }
                    else
                    {
                        _zip_description_without_dash = _icdoPersonAddress.addr_zip_code;
                    }
                }
                return _zip_description_without_dash;
            }
        }

        #region TIAACREF DC/457 Enrollment
        public string istrZipCodeForFile
        {
            get
            {
                if (zip_description.IsNotNullOrEmpty())
                    return zip_description.ReplaceWith("[-]", "");
                else
                    return String.Empty;
            }
        }
        public string istrAddressLine1ForFile
        {
            get
            {
                if (icdoPersonAddress.addr_line_1.IsNotNullOrEmpty())
                {
                    if (icdoPersonAddress.addr_line_1.Length <= 40)
                        return icdoPersonAddress.addr_line_1.ToUpper();//PIR 9122
                    else
                        return icdoPersonAddress.addr_line_1.Substring(0, 40).ToUpper();
                }
                else
                    return String.Empty;
            }
        }

        //PIR 9122
        public string istrAddressLine2ForFile
        {
            get
            {
                if (icdoPersonAddress.addr_line_2.IsNotNullOrEmpty())
                {
                    if (icdoPersonAddress.addr_line_2.Length <= 40)
                        return icdoPersonAddress.addr_line_2.ToUpper();
                    else
                        return icdoPersonAddress.addr_line_2.Substring(0, 40).ToUpper();
                }
                else
                    return String.Empty;
            }
        }
       
        #endregion
        private string _addr_description;
        public string addr_description
        {
            get
            {
                _addr_description = string.Empty;
                if (_icdoPersonAddress.addr_line_1 != null)
                {
                    _addr_description += _icdoPersonAddress.addr_line_1 + ", ";
                }
                if (_icdoPersonAddress.addr_line_2 != null)
                {
                    _addr_description += _icdoPersonAddress.addr_line_2 + ", ";
                }
                if (_icdoPersonAddress.addr_country_value == busConstant.US_Code_ID)
                {
                    if (_icdoPersonAddress.addr_city != null)
                    {
                        _addr_description += _icdoPersonAddress.addr_city + ", ";
                    }
                    if (_icdoPersonAddress.addr_state_value != null)
                    {
                        _addr_description += _icdoPersonAddress.addr_state_value + " ";
                    }
                    if (_icdoPersonAddress.addr_zip_code != null)
                    {
                        _addr_description += _icdoPersonAddress.addr_zip_code;
                    }
                    if (_icdoPersonAddress.addr_zip_4_code != null)
                    {
                        _addr_description += "-" + _icdoPersonAddress.addr_zip_4_code;
                    }
                }
                else
                {
                    if (_icdoPersonAddress.addr_state_description != null)
                    {
                        _addr_description += _icdoPersonAddress.addr_state_description + ", ";
                    }

                    if (!String.IsNullOrEmpty(_icdoPersonAddress.foreign_province))
                    {
                        _addr_description += _icdoPersonAddress.foreign_province + ", ";
                    }

                    if (!String.IsNullOrEmpty(_icdoPersonAddress.foreign_postal_code))
                    {
                        _addr_description += _icdoPersonAddress.foreign_postal_code + " ";
                    }

                    if (_icdoPersonAddress.addr_country_description != null)
                    {
                        _addr_description += _icdoPersonAddress.addr_country_description;
                    }
                }
                return _addr_description.ToUpper(); // PIR 8664- Client wants address to be displayed only in Upper Case
            }
        }
        public string istrAddressDescriptionWithCareOf      
        {
            //PIR 26139
            get
            {
                if (_icdoPersonAddress.care_of.IsNotNullOrEmpty())
                {
                    return ("C/O " + _icdoPersonAddress.care_of + ", " + addr_description);
                }
                else
                {
                    return addr_description;
                }
            }
        }
        //this resource property is used for giving full access 
        //to user under BIA role.

        public bool IsBIAResource
        {
            get
            {
                bool lblnHasBIARole = false;
                // check if user got BIA role

                DataTable ldtbCount = busNeoSpinBase.Select("cdoPersonAddress.CheckResourceForLoggedInUser", new object[1] { iobjPassInfo.iintUserSerialID });
                if (ldtbCount.Rows.Count > 0)
                {
                    lblnHasBIARole = true;
                }
                return lblnHasBIARole;
            }

        }

        public string istrPeopleSoftUrl
        {
            get
            {
                String lstr = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(52, "PURL");
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
                String lstr = string.Format(busGlobalFunctions.GetMessageTextByMessageID(8567, iobjPassInfo), prestr, poststr);
                return lstr;
            }
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoPersonAddress.person_id);
        }

        public void LoadOtherAddressses()
        {
            if (_iclbOtherAddresses == null)
            {
                _iclbOtherAddresses = new Collection<busPersonAddress>();
            }
            DataTable ldtOtherAddresses = busNeoSpinBase.Select("cdoPersonAddress.LoadAddresses", new object[2] { _icdoPersonAddress.person_id, _icdoPersonAddress.person_address_id });
            _iclbOtherAddresses = GetCollection<busPersonAddress>(ldtOtherAddresses, "icdoPersonAddress");
        }

        /// <summary>
        /// Verify Address using Web Service and save after validation.
        /// </summary>
        /// <returns></returns>
        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                _icdoPersonAddress.address_validate_flag = busConstant.Flag_Yes;
                return;
            }

            //ArrayList larrErrors = new ArrayList();            
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = _icdoPersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = _icdoPersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = _icdoPersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = _icdoPersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = _icdoPersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = _icdoPersonAddress.addr_zip_4_code;
            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                _icdoPersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                _icdoPersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                _icdoPersonAddress.addr_city = _lobjcdoWebServiceAddressResult.addr_city;
                _icdoPersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                _icdoPersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                _icdoPersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;

            }
            _icdoPersonAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;
            _icdoPersonAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
        }

        /// <summary>
        /// Check if the given Start and End date is overlapping with the existing records.
        /// </summary>
        /// <returns>bool</returns>

        public bool iblnIsFromMSS { get; set; }//For MSS Layout change
        public bool IsDatesOverlapping()
        {
            bool lblnRecordMatch = false;
            LoadOtherAddressses(); // PIR 23744 
            foreach (busPersonAddress lobjPersonAddress in _iclbOtherAddresses)
            {
                //For MSS Layout change
                if ((_icdoPersonAddress.address_type_value == lobjPersonAddress.icdoPersonAddress.address_type_value) && !iblnIsFromMSS &&
                    (busGlobalFunctions.CheckDateOverlapping(
                    _icdoPersonAddress.start_date,
                    lobjPersonAddress._icdoPersonAddress.start_date,
                    lobjPersonAddress._icdoPersonAddress.end_date) ||
                    (busGlobalFunctions.CheckDateOverlapping(
                    _icdoPersonAddress.end_date,
                    lobjPersonAddress._icdoPersonAddress.start_date,
                    lobjPersonAddress._icdoPersonAddress.end_date)
                    )))
                {
                    lblnRecordMatch = true;
                    break;
                }
            }
            return lblnRecordMatch;
        }

        private String _IsPOAExists;
        public String IsPOAExists
        {
            get
            {
                _IsPOAExists = CheckPOAExists();
                return _IsPOAExists;
            }
        }

        /// <summary>
        /// Check if POA exists for the Person.
        /// </summary>
        /// <returns>string</returns>
        private String CheckPOAExists()
        {
            int lintCount = 0;
            lintCount = (int)DBFunction.DBExecuteScalar("cdoPersonAddress.IS_POA_EXISTS",
                    new object[2] { _icdoPersonAddress.person_id, busConstant.POA_RelationshipValue }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintCount > 0)
                return busConstant.Flag_Yes;
            else
                return busConstant.Flag_No;
        }

        private string _istrCounty;
        public string istrCounty
        {
            get { return _istrCounty; }
            set { _istrCounty = value; }
        }

        public void LoadCounty()
        {
            _istrCounty = Convert.ToString(DBFunction.DBExecuteScalar("cdoCountyRef.LOOKUP", new object[1] { _icdoPersonAddress.addr_city == null ? string.Empty : _icdoPersonAddress.addr_city }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            VerifyAddressUsingUSPS();

            if (icdoPersonAddress.ienuObjectState == ObjectState.Insert)
                LoadPreviousAddressOfCurrentType();

            base.BeforeValidate(aenmPageMode);
            //pir 8622
            if(iblnIsFromMSS)
                base.EvaluateInitialLoadRules();
        }
        public override void BeforePersistChanges()
        {
            if (icdoPersonAddress.ienuObjectState == ObjectState.Insert)
            {
                UpdatePreviousAddressOfCurrentType();
                if (iblnExternalUser)  //PIR-18492 : To Post message to message board                   
                    busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10319, iobjPassInfo), "Mailing Address"), busConstant.WSS_MessageBoard_Priority_High,
                    icdoPersonAddress.person_id);
            }   
        
            base.BeforePersistChanges();
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadCounty();
            //pir 8622
            if(iblnIsFromMSS)
                base.EvaluateInitialLoadRules();
        }

        public bool IsValidStartDate()
        {
            int lintCount = 0;
            if (_icdoPersonAddress.ihstOldValues.Count > 0)
            {
                if (_icdoPersonAddress.start_date < DateTime.Now)
                {
                    lintCount = _icdoPersonAddress.start_date.CompareTo(_icdoPersonAddress.ihstOldValues["start_date"]);
                }
                else if (_icdoPersonAddress.start_date > DateTime.Now)
                {
                    lintCount = 1;
                }

            }
            bool lblnValidDate = true;
            if (lintCount < 0)
            {
                lblnValidDate = false;
            }
            else if (lintCount >= 0)
            {
                lblnValidDate = true;
            }
            return lblnValidDate;
        }

        public override int PersistChanges()
        {
            if (icdoPersonAddress.ienuObjectState == ObjectState.Insert)
                UpdateMedicarePartDFlagsForNewAddress();
            else
                UpdateMedicarePartDFlagsForAddress();


            //BtnVerifyAndSaveClicked();
            base.PersistChanges();
            return 1;
        }

        public override busBase GetCorPerson()
        {
            if (_ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }

        # region UAT PIR 67
        public busPersonAddress ibusPreviousAddressOfCurrentType { get; set; }
        /// <summary>
        /// UAT PIR - 67
        /// check if there exists ny address with same type and start date less than the entered date then
        /// end the old address with the date 1 day less than new start date
        /// check this for new mode only
        /// </summary>
        public void LoadPreviousAddressOfCurrentType()
        {
            if (_iclbOtherAddresses.IsNull())
                LoadOtherAddressses();

            ibusPreviousAddressOfCurrentType = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
            var lGetAddressListOfCurrentType = _iclbOtherAddresses.Where(lobjAdd => lobjAdd.icdoPersonAddress.address_type_value == icdoPersonAddress.address_type_value
                && icdoPersonAddress.end_date == DateTime.MinValue)
                .OrderByDescending(lobjAdd => lobjAdd.icdoPersonAddress.start_date);

            if (lGetAddressListOfCurrentType.Count() > 0)
            {
                //1 get the latest address
                ibusPreviousAddressOfCurrentType = lGetAddressListOfCurrentType.FirstOrDefault();
            }
        }

        //check if the start date of the address of current type is greater than the current address date
        //check only in the New mode
        public bool IsStartDateInValidForAddressType()
        {
            if (ibusPreviousAddressOfCurrentType.IsNull())
                LoadPreviousAddressOfCurrentType();

            if ((ibusPreviousAddressOfCurrentType.icdoPersonAddress.start_date != DateTime.MinValue)
                && (icdoPersonAddress.start_date != DateTime.MinValue))
            {
                if (ibusPreviousAddressOfCurrentType.icdoPersonAddress.start_date >= icdoPersonAddress.start_date)
                    return true;
            }
            return false;
        }

        //update the end date of previous address of current type
        private void UpdatePreviousAddressOfCurrentType()
        {
            if (ibusPreviousAddressOfCurrentType.IsNull())
                LoadPreviousAddressOfCurrentType();

            if (ibusPreviousAddressOfCurrentType.icdoPersonAddress.person_address_id > 0)
            {
                if ((ibusPreviousAddressOfCurrentType.icdoPersonAddress.end_date == DateTime.MinValue)
                    && (icdoPersonAddress.start_date != DateTime.MinValue))
                {
                    if (ibusPreviousAddressOfCurrentType.icdoPersonAddress.start_date.Date == icdoPersonAddress.start_date.Date)
                        ibusPreviousAddressOfCurrentType.icdoPersonAddress.end_date = icdoPersonAddress.start_date;
                    else
                        ibusPreviousAddressOfCurrentType.icdoPersonAddress.end_date = icdoPersonAddress.start_date.AddDays(-1);
                    ibusPreviousAddressOfCurrentType.icdoPersonAddress.addr_change_letter_status_flag = (iblnIsFromMSS) ? busConstant.AddrChangeLetterNotSent : null; //PIR 20868
                    ibusPreviousAddressOfCurrentType.icdoPersonAddress.Update();
                }
            }
        }

        //PIR-15430
        //update the end date of previous address of current type For ESS
        public void UpdatePreviousAddressOfCurrentTypeForESS()
        {
            UpdatePreviousAddressOfCurrentType();
        } 
		
        # endregion

        # region UCS 24
        //public string istrMSSAddressDescription 
        //{
        //    get
        //    {
        //        string lstrAddressDescription = string.Empty;
        //        if (!String.IsNullOrEmpty(addr_description))
        //            lstrAddressDescription = addr_description;
        //        return lstrAddressDescription;
        //    }
        //}
        public string istrAddressWithPeopleSoftID { get; set; }
        public string istrAddressWithoutPeopleSoftID { get; set; }


        //public string istrPeopleSoftUrl
        //{
        //    get
        //    {
        //        return busConstant.PeopleSoftURl;
        //    }
        //}
        #endregion

        //For MSS Layout change
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromMSS)
            {
                foreach (utlError lobjError in iarrErrors)
                {
                    if (lobjError.istrErrorID == "127")
                        lobjError.istrErrorMessage = "Warning: Address is Invalid. Please enter a valid address";
                    lobjError.istrErrorID = string.Empty;
                }
                EvaluateInitialLoadRules();
            }
        }

        public bool IsIgnoreWarningButtonVisible()
        {
            if (iblnIsFromMSS)
            {
                if (iarrErrors.IsNotNull())
                {
                    foreach (utlError lobjError in iarrErrors)
                    {
                        if (!string.IsNullOrEmpty(lobjError.istrErrorMessage))
                        {
                            if (lobjError.istrErrorMessage.ToLower().Contains("warning"))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        // PIR 9204
        // address exists in the address table already it should not throw an error.
        public bool IsAddressAlreadyExists()
        {
            if (iblnIsFromMSS && icdoPersonAddress.person_address_id == 0)
            {
                DataTable ldtbResult = Select("cdoWssPersonAddress.IsAddressAlredyExists", new object[9]{
                                                icdoPersonAddress.addr_line_1 ?? string.Empty,icdoPersonAddress.addr_line_2 ?? string.Empty,
                                                icdoPersonAddress.addr_city ?? string.Empty,icdoPersonAddress.addr_state_value ?? string.Empty,
                                                icdoPersonAddress.addr_country_value ?? string.Empty,icdoPersonAddress.addr_zip_code ?? string.Empty,
                                                icdoPersonAddress.addr_zip_4_code ?? string.Empty,icdoPersonAddress.foreign_province ?? string.Empty,
                                                icdoPersonAddress.foreign_postal_code ?? string.Empty});
                if (ldtbResult.Rows.Count > 0)
                    return true;
                return false;
            }
            return true;
        }

        //PIR 14848 - Medicare Part D changes
        public void UpdateMedicarePartDFlagsForAddress()
        {
            if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary 
                || (icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent && icdoPersonAddress.end_date == DateTime.MinValue))
            {
                if (icdoPersonAddress.ihstOldValues.Count > 0)
                {
                    if (Convert.ToString(icdoPersonAddress.ihstOldValues["addr_line_1"]) != icdoPersonAddress.addr_line_1 ||
                        Convert.ToString(icdoPersonAddress.ihstOldValues["addr_line_2"]) != icdoPersonAddress.addr_line_2 ||
                        Convert.ToString(icdoPersonAddress.ihstOldValues["addr_city"]) != icdoPersonAddress.addr_city ||
                        Convert.ToString(icdoPersonAddress.ihstOldValues["addr_state_value"]) != icdoPersonAddress.addr_state_value ||
                        Convert.ToString(icdoPersonAddress.ihstOldValues["addr_zip_code"]) != icdoPersonAddress.addr_zip_code ||
                        Convert.ToString(icdoPersonAddress.ihstOldValues["addr_zip_4_code"]) != icdoPersonAddress.addr_zip_4_code ||
                        (Convert.ToDateTime(icdoPersonAddress.ihstOldValues["end_date"]) != icdoPersonAddress.end_date)
                        )
                    {
                        ibusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory();
                        ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

                        if (ibusPersonAccountMedicare.FindByPersonID(icdoPersonAddress.person_id))
                        {
                            ibusPersonAccountMedicare.LoadMedicareByPersonIDForAddress(icdoPersonAddress.person_id);
                            ibusPersonAccountMedicare.FindPersonAccount(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id);

                            if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory == null)
                                ibusPersonAccountMedicare.LoadPersonAccountMedicarePartDHistory(icdoPersonAddress.person_id);

                            foreach (busPersonAccountMedicarePartDHistory lobj in ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDMembers)
                            {
                                if (icdoPersonAddress.person_id == lobj.icdoPersonAccountMedicarePartDHistory.member_person_id)
                                {
                                    //Insert a new history record if any change in person maintenance is made.
                                    cdoPersonAccountMedicarePartDHistory lobjHistory = new cdoPersonAccountMedicarePartDHistory();
                                    lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                                    lobjHistory.person_account_id = lobj.icdoPersonAccountMedicarePartDHistory.person_account_id;
                                    lobjHistory.person_id = lobj.icdoPersonAccountMedicarePartDHistory.person_id;
                                    lobjHistory.start_date = lobj.icdoPersonAccountMedicarePartDHistory.start_date;
                                    lobjHistory.plan_participation_status_value = lobj.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value;
                                    lobjHistory.status_value = lobj.icdoPersonAccountMedicarePartDHistory.status_value;

                                    lobjHistory.reason_value = busConstant.ChangeReasonAddress;
                                    lobjHistory.suppress_warnings_flag = lobj.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
                                    lobjHistory.medicare_claim_no = lobj.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
                                    lobjHistory.medicare_part_a_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
                                    lobjHistory.medicare_part_b_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
                                    lobjHistory.low_income_credit = lobj.icdoPersonAccountMedicarePartDHistory.low_income_credit;
                                    lobjHistory.late_enrollment_penalty = lobj.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                                    lobjHistory.member_person_id = lobj.icdoPersonAccountMedicarePartDHistory.member_person_id;

                                    lobjHistory.record_type_flag = "C";
                                    lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                    lobjHistory.send_after = icdoPersonAddress.start_date;

                                    if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date
                                        && i.icdoPersonAccountMedicarePartDHistory.start_date > i.icdoPersonAccountMedicarePartDHistory.end_date &&
                                        i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() == 0)
                                        lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.start_date;
                                    else
                                        lobjHistory.initial_enroll_date = lobj.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

                                    //PIR 24309
                                    ibusPersonAccountMedicare.LoadActiveProviderOrgPlan(lobjHistory.start_date);
                                    lobjHistory.provider_org_id = ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;
                                    if (icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent)
                                    {
                                        lobjHistory.send_after = icdoPersonAddress.start_date;
                                        lobjHistory.Insert();
                                    }
                                    else if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary)
                                    {
                                        lobjHistory.send_after = icdoPersonAddress.start_date;
                                        lobjHistory.Insert();
                                        if (icdoPersonAddress.end_date != DateTime.MinValue)
                                        {
                                            cdoPersonAccountMedicarePartDHistory lobjHistory1 = new cdoPersonAccountMedicarePartDHistory();
                                            lobjHistory1 = lobjHistory;

                                            //End the previous history record
                                            lobjHistory.end_date = lobjHistory.start_date;
                                            lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                            lobjHistory.Update();

                                            lobjHistory1.end_date = DateTime.MinValue;
                                            lobjHistory1.enrollment_file_sent_flag = busConstant.Flag_No;
                                            lobjHistory1.send_after = icdoPersonAddress.end_date;
                                            lobjHistory1.Insert();
                                        }
                                    }

                                    //Ending the previous history
                                    if (lobj.icdoPersonAccountMedicarePartDHistory.start_date == lobj.icdoPersonAccount.history_change_date)
                                        lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date;
                                    else
                                        lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date.AddDays(-1);

                                    lobj.icdoPersonAccountMedicarePartDHistory.Update();
                                }
                            }
                        }
                        else
                        {
                            if (ibusPersonAccountMedicare.FindByMemberPersonID(icdoPersonAddress.person_id))
                            {
                                ibusPersonAccountMedicare.LoadMedicareByPersonIDForAddress(icdoPersonAddress.person_id);
                                ibusPersonAccountMedicare.FindPersonAccount(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id);

                                if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory == null)
                                    ibusPersonAccountMedicare.LoadPersonAccountMedicarePartDHistory(icdoPersonAddress.person_id);

                                foreach (busPersonAccountMedicarePartDHistory lobj in ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDMembers)
                                {
                                    if (icdoPersonAddress.person_id == lobj.icdoPersonAccountMedicarePartDHistory.member_person_id)
                                    {
                                        //Insert a new history record if any change in person maintenance is made.
                                        cdoPersonAccountMedicarePartDHistory lobjHistory = new cdoPersonAccountMedicarePartDHistory();
                                        lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                                        lobjHistory.person_account_id = lobj.icdoPersonAccountMedicarePartDHistory.person_account_id;
                                        lobjHistory.person_id = lobj.icdoPersonAccountMedicarePartDHistory.person_id;
                                        lobjHistory.start_date = lobj.icdoPersonAccountMedicarePartDHistory.start_date;
                                        lobjHistory.plan_participation_status_value = lobj.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value;
                                        lobjHistory.status_value = lobj.icdoPersonAccountMedicarePartDHistory.status_value;

                                        lobjHistory.reason_value = busConstant.ChangeReasonAddress;
                                        lobjHistory.suppress_warnings_flag = lobj.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
                                        lobjHistory.medicare_claim_no = lobj.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
                                        lobjHistory.medicare_part_a_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
                                        lobjHistory.medicare_part_b_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
                                        lobjHistory.low_income_credit = lobj.icdoPersonAccountMedicarePartDHistory.low_income_credit;
                                        lobjHistory.late_enrollment_penalty = lobj.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                                        lobjHistory.member_person_id = lobj.icdoPersonAccountMedicarePartDHistory.member_person_id;

                                        lobjHistory.record_type_flag = "C";
                                        lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                        lobjHistory.send_after = icdoPersonAddress.start_date;
                                        //PIR 24309
                                        ibusPersonAccountMedicare.LoadActiveProviderOrgPlan(lobjHistory.start_date);
                                        lobjHistory.provider_org_id = ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;

                                        if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date
                                        && i.icdoPersonAccountMedicarePartDHistory.start_date > i.icdoPersonAccountMedicarePartDHistory.end_date &&
                                        i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() == 0)
                                            lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.start_date;
                                        else
                                            lobjHistory.initial_enroll_date = lobj.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

                                        if (icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent)
                                        {
                                            lobjHistory.send_after = icdoPersonAddress.start_date;
                                            lobjHistory.Insert();
                                        }
                                        else if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary)
                                        {
                                            lobjHistory.send_after = icdoPersonAddress.start_date;
                                            lobjHistory.Insert();
                                            if (icdoPersonAddress.end_date != DateTime.MinValue)
                                            {
                                                cdoPersonAccountMedicarePartDHistory lobjHistory1 = new cdoPersonAccountMedicarePartDHistory();
                                                lobjHistory1 = lobjHistory;

                                                //End the previous history record
                                                lobjHistory.end_date = lobjHistory.start_date;
                                                lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                                lobjHistory.Update();

                                                lobjHistory1.end_date = DateTime.MinValue;
                                                lobjHistory1.enrollment_file_sent_flag = busConstant.Flag_No;
                                                lobjHistory1.send_after = icdoPersonAddress.end_date;
                                                lobjHistory1.Insert();
                                            }
                                        }

                                        //Ending the previous history
                                        if (lobj.icdoPersonAccountMedicarePartDHistory.start_date == lobj.icdoPersonAccount.history_change_date)
                                            lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date;
                                        else
                                            lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date.AddDays(-1);

                                        lobj.icdoPersonAccountMedicarePartDHistory.Update();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //New PERM or TEMP address inserted
        public void UpdateMedicarePartDFlagsForNewAddress()
        {
            ibusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory();
            ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

            if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary || icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent)
            {

                if (ibusPersonAccountMedicare.FindByPersonID(icdoPersonAddress.person_id))
                {
                    ibusPersonAccountMedicare.LoadMedicareByPersonIDForAddress(icdoPersonAddress.person_id);
                    ibusPersonAccountMedicare.FindPersonAccount(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id);
                    foreach (busPersonAccountMedicarePartDHistory lobj in ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDMembers)
                    {
                        //History entry will be created only if members address is modified/added.
                        if (icdoPersonAddress.person_id == lobj.icdoPersonAccountMedicarePartDHistory.member_person_id)
                        {
                        //Insert a new history record if any change in person maintenance is made.
                        cdoPersonAccountMedicarePartDHistory lobjHistory = new cdoPersonAccountMedicarePartDHistory();
                        lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                        lobjHistory.person_account_id = lobj.icdoPersonAccountMedicarePartDHistory.person_account_id;
                        lobjHistory.person_id = lobj.icdoPersonAccountMedicarePartDHistory.person_id;
                        lobjHistory.start_date = lobj.icdoPersonAccountMedicarePartDHistory.start_date;
                        lobjHistory.plan_participation_status_value = lobj.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value;
                        lobjHistory.status_value = lobj.icdoPersonAccountMedicarePartDHistory.status_value;

                        lobjHistory.reason_value = busConstant.ChangeReasonAddress;
                        lobjHistory.suppress_warnings_flag = lobj.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
                        lobjHistory.medicare_claim_no = lobj.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
                        lobjHistory.medicare_part_a_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
                        lobjHistory.medicare_part_b_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
                        lobjHistory.low_income_credit = lobj.icdoPersonAccountMedicarePartDHistory.low_income_credit;
                        lobjHistory.late_enrollment_penalty = lobj.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                        lobjHistory.member_person_id = lobj.icdoPersonAccountMedicarePartDHistory.member_person_id;
                        lobjHistory.record_type_flag = "C";
                        lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
						//PIR 24309
                        ibusPersonAccountMedicare.LoadActiveProviderOrgPlan(lobjHistory.start_date);
                        lobjHistory.provider_org_id = ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;

                        lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

                        if (icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent)
                        {
                            lobjHistory.send_after = icdoPersonAddress.start_date;
                            lobjHistory.Insert();
                        }
                        else if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary)
                        {
                            lobjHistory.send_after = icdoPersonAddress.start_date;
                            lobjHistory.Insert();
                            if (icdoPersonAddress.end_date != DateTime.MinValue)
                            {
                                cdoPersonAccountMedicarePartDHistory lobjHistory1 = new cdoPersonAccountMedicarePartDHistory();
                                lobjHistory1 = lobjHistory;

                                //End the previous history record
                                lobjHistory.end_date = lobjHistory.start_date;
                                lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                lobjHistory.Update();

                                lobjHistory1.end_date = DateTime.MinValue;
                                lobjHistory1.enrollment_file_sent_flag = busConstant.Flag_No;
                                lobjHistory1.send_after = icdoPersonAddress.end_date;
                                lobjHistory1.Insert();
                            }
                        }

                        //Ending the previous history
                        if (lobj.icdoPersonAccountMedicarePartDHistory.start_date == lobj.icdoPersonAccount.history_change_date)
                            lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date;
                        else
                            lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date.AddDays(-1);

                        lobj.icdoPersonAccountMedicarePartDHistory.Update();
                    }
                    }
                }
                else
                {
                    if (ibusPersonAccountMedicare.FindByMemberPersonID(icdoPersonAddress.person_id))
                    {
                        ibusPersonAccountMedicare.LoadMedicareByPersonIDForAddress(icdoPersonAddress.person_id);
                        ibusPersonAccountMedicare.FindPersonAccount(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id);
                        foreach (busPersonAccountMedicarePartDHistory lobj in ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDMembers)
                        {
                            if (icdoPersonAddress.person_id == lobj.icdoPersonAccountMedicarePartDHistory.member_person_id)
                            {
                                //Insert a new history record if any change in person maintenance is made.
                                cdoPersonAccountMedicarePartDHistory lobjHistory = new cdoPersonAccountMedicarePartDHistory();
                                lobj.FindPersonAccount(lobj.icdoPersonAccountMedicarePartDHistory.person_account_id);
                                lobjHistory.person_account_id = lobj.icdoPersonAccountMedicarePartDHistory.person_account_id;
                                lobjHistory.person_id = lobj.icdoPersonAccountMedicarePartDHistory.person_id;
                                lobjHistory.start_date = lobj.icdoPersonAccountMedicarePartDHistory.start_date;
                                lobjHistory.plan_participation_status_value = lobj.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value;
                                lobjHistory.status_value = lobj.icdoPersonAccountMedicarePartDHistory.status_value;

                                lobjHistory.reason_value = busConstant.ChangeReasonAddress;
                                lobjHistory.suppress_warnings_flag = lobj.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
                                lobjHistory.medicare_claim_no = lobj.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
                                lobjHistory.medicare_part_a_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
                                lobjHistory.medicare_part_b_effective_date = lobj.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
                                lobjHistory.low_income_credit = lobj.icdoPersonAccountMedicarePartDHistory.low_income_credit;
                                lobjHistory.late_enrollment_penalty = lobj.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                                lobjHistory.member_person_id = lobj.icdoPersonAccountMedicarePartDHistory.member_person_id;
                                lobjHistory.record_type_flag = "C";
                                lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;

                                //PIR 24309
                                ibusPersonAccountMedicare.LoadActiveProviderOrgPlan(lobjHistory.start_date);
                                lobjHistory.provider_org_id = ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;

                                lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

                                if (icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent)
                                {
                                    lobjHistory.send_after = icdoPersonAddress.start_date;
                                    lobjHistory.Insert();
                                }
                                else if (icdoPersonAddress.address_type_value == busConstant.AddressTypeTemporary)
                                {
                                    lobjHistory.send_after = icdoPersonAddress.start_date;
                                    lobjHistory.Insert();
                                    if (icdoPersonAddress.end_date != DateTime.MinValue)
                                    {
                                        cdoPersonAccountMedicarePartDHistory lobjHistory1 = new cdoPersonAccountMedicarePartDHistory();
                                        lobjHistory1 = lobjHistory;

                                        //End the previous history record
                                        lobjHistory.end_date = lobjHistory.start_date;
                                        lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                                        lobjHistory.Update();

                                        lobjHistory1.end_date = DateTime.MinValue;
                                        lobjHistory1.enrollment_file_sent_flag = busConstant.Flag_No;
                                        lobjHistory1.send_after = icdoPersonAddress.end_date;
                                        lobjHistory1.Insert();
                                    }
                                }

                                //Ending the previous history
                                if (lobj.icdoPersonAccountMedicarePartDHistory.start_date == lobj.icdoPersonAccount.history_change_date)
                                    lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date;
                                else
                                    lobj.icdoPersonAccountMedicarePartDHistory.end_date = lobj.icdoPersonAccount.history_change_date.AddDays(-1);

                                lobj.icdoPersonAccountMedicarePartDHistory.Update();
                            }
                        }
                    }
                }
            }
        }
    }
}

