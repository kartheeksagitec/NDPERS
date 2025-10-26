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
using NeoSpin.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busContactTicket : busExtendBase
    {        
        //FWUpgrade PIR 12184-used to set primary key as 0 after refreshing the saved form
        public override long iintPrimaryKey
        {
            get
            {
                //F/W Upgrade Systest Issue - Tracing need to be on to reproduce this issue
                if (iobjPassInfo?.idictParams != null && iobjPassInfo?.istrSenderID == "btnSave" && iobjPassInfo?.istrFormName == "wfmMSSContactNDPERSMaintenance")
                {
                    return 0;
                }
                return base.iintPrimaryKey;
            }
        }
        #region properties
        /// <summary>
        /// Business rule - To Check the Visibility of Delete Button.
        /// </summary>
        private DateTime _ldtSeminarCompletionDate;
        public DateTime ldtSeminarCompletionDate
        {
            get
            {
                if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
                {
                    _ldtSeminarCompletionDate = _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.AddDays(15);
                }
                return _ldtSeminarCompletionDate;
            }
        }

        private busAppointmentSchedule _ibusAppointmentSchedule;
        public busAppointmentSchedule ibusAppointmentSchedule
        {
            get
            {
                return _ibusAppointmentSchedule;
            }

            set
            {
                _ibusAppointmentSchedule = value;
            }
        }
        private busBenefitEstimate _ibusBenefitEstimate;
        public busBenefitEstimate ibusBenefitEstimate
        {
            get
            {
                return _ibusBenefitEstimate;
            }

            set
            {
                _ibusBenefitEstimate = value;
            }
        }
        private busDeathNotice _ibusDeathNotice;
        public busDeathNotice ibusDeathNotice
        {
            get
            {
                return _ibusDeathNotice;
            }

            set
            {
                _ibusDeathNotice = value;
            }
        }
        private busNewGroup _ibusNewGroup;
        public busNewGroup ibusNewGroup
        {
            get
            {
                return _ibusNewGroup;
            }
            set
            {
                _ibusNewGroup = value;
            }
        }
        //PIR-75
        private string _ProcessDescription;
        public string ProcessDescription
        {
            get { return _ProcessDescription; }
            set { _ProcessDescription = value; }
        }

        private string _istrActivityDisplayName;
        public string istrActivityDisplayName
        {
            get { return _istrActivityDisplayName; }
            set { _istrActivityDisplayName = value; }
        }

        public int iintContactTicketActivityInstanceID { get; set; }

        private busSeminarSchedule _ibusSeminarSchedule;
        public busSeminarSchedule ibusSeminarSchedule
        {
            get
            {
                return _ibusSeminarSchedule;
            }
            set
            {
                _ibusSeminarSchedule = value;
            }
        }
        private busContactMgmtServicePurchase _ibusContactMgmtServicePurchase;
        public busContactMgmtServicePurchase ibusContactMgmtServicePurchase
        {
            get
            {
                return _ibusContactMgmtServicePurchase;
            }
            set
            {
                _ibusContactMgmtServicePurchase = value;
            }
        }

        //The following Property might be used to business rules.. will clean it up later..
        private busSerPurServiceType _ibusSerPurServiceType;

        public busSerPurServiceType ibusSerPurServiceType
        {
            get { return _ibusSerPurServiceType; }
            set { _ibusSerPurServiceType = value; }
        }
        private Collection<busSerPurServiceType> _iclbSerPurServiceType;

        public Collection<busSerPurServiceType> iclbSerPurServiceType
        {
            get { return _iclbSerPurServiceType; }
            set { _iclbSerPurServiceType = value; }
        }
        private busSerPurRolloverInfo _ibusSerPurRolloverInfo;

        public busSerPurRolloverInfo ibusSerPurRolloverInfo
        {
            get { return _ibusSerPurRolloverInfo; }
            set { _ibusSerPurRolloverInfo = value; }
        }
        private busNewGroupPlanType _ibusNewGroupPlanType;

        public busNewGroupPlanType ibusNewGroupPlanType
        {
            get { return _ibusNewGroupPlanType; }
            set { _ibusNewGroupPlanType = value; }
        }
        //end

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
        private busUser _ibusUser;
        public busUser ibusUser
        {
            get
            {
                return _ibusUser;
            }

            set
            {
                _ibusUser = value;
            }
        }
        private busUser _ibusCounselor;
        public busUser ibusCounselor
        {
            get
            {
                return _ibusCounselor;
            }

            set
            {
                _ibusCounselor = value;
            }
        }
        private busUser _ibusFacilitator;
        public busUser ibusFacilitator
        {
            get
            {
                return _ibusFacilitator;
            }

            set
            {
                _ibusFacilitator = value;
            }
        }
        private string _Organizer_Name;
        public string Organizer_Name
        {
            get
            {
                LoadSponsorName();
                return _Organizer_Name;
            }
        }

        //to display blank when PERSLink Id is not selected.  
        //PIR- 312
        public string istrPERLSinkId
        {
            get
            {
                string lstrPERLSinkID = String.Empty;
                if (_icdoContactTicket.person_id != 0)
                    lstrPERLSinkID = Convert.ToString(_icdoContactTicket.person_id);

                return lstrPERLSinkID;
            }

        }
        public DateTime appointmentStartDateTime
        {
            get
            {
                if (!String.IsNullOrEmpty(_ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_description))
                    return DateTime.ParseExact(_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date.ToString("MM/dd/yyyy") + " " +
                                    _ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_description, "MM/dd/yyyy h:mm tt", null);
                else
                    return Convert.ToDateTime(_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date.ToString("MM/dd/yyyy"));

            }
        }

        public DateTime appointmentEndDateTime
        {
            get
            {
                if (!String.IsNullOrEmpty(_ibusAppointmentSchedule.icdoAppointmentSchedule.end_time_description))
                    return DateTime.ParseExact(_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date.ToString("MM/dd/yyyy") + " " +
                                    _ibusAppointmentSchedule.icdoAppointmentSchedule.end_time_description, "MM/dd/yyyy h:mm tt", null);
                else
                    return Convert.ToDateTime(_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date.ToString("MM/dd/yyyy"));
            }
        }

        public DateTime SeminarStartDatetime
        {
            get
            {
                return DateTime.ParseExact(_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.ToString("MM/dd/yyyy") + " " +
                                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_start_time_description, "MM/dd/yyyy h:mm tt", null);
            }

        }

        public DateTime SeminarEndDatetime
        {
            get
            {
                return DateTime.ParseExact(_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.ToString("MM/dd/yyyy") + " " +
                                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_end_time_description, "MM/dd/yyyy h:mm tt", null);
            }

        }

        public DateTime SeminarStartDatetime2
        {
            get
            {
                return DateTime.ParseExact(_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2.ToString("MM/dd/yyyy") + " " +
                                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_start_time_description, "MM/dd/yyyy h:mm tt", null);
            }

        }

        public DateTime SeminarEndDatetime2
        {
            get
            {
                return DateTime.ParseExact(_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2.ToString("MM/dd/yyyy") + " " +
                                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_end_time_description, "MM/dd/yyyy h:mm tt", null);
            }

        }

        public bool iblnIsFromMSS { get; set; }//For MSS Layout change
        public bool iblnIsFromESS { get; set; }
        // PIR UAT36
        private int _iintLoggedinUser;

        public int iintLoggedinUser
        {
            get { return _iintLoggedinUser = iobjPassInfo.iintUserSerialID; }
        }

        public busContact ibusWebContact { get; set; }

        private string _istrSuppressWarning;
        public bool iblnIsFromInternal { get; set; } // PIR-13737

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }

        public string istrAmntMonths { get; set; }

        public string isElligibleOrRolloverPlan { get; set; }

        # endregion

        # region Load Methods
        public void LoadSponsorName()
        {
            _Organizer_Name = string.Empty;
            //if (_icdoContactTicket.person_id != 0)
            //{
            //    LoadPerson();
            //    _Organizer_Name = _ibusPerson.icdoPerson.FullName;
            //}
            if (_icdoContactTicket.org_id != 0)
            {
                LoadOrganization();
                _Organizer_Name = _ibusOrganization.icdoOrganization.org_name;
            }
            //if (_icdoContactTicket.caller_name != null)
            //{
            //    _Organizer_Name = _icdoContactTicket.caller_name;
            //}
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoContactTicket.person_id);
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoContactTicket.org_id);
        }

        public void LoadUser()
        {
            if (_ibusUser == null)
            {
                _ibusUser = new busUser();
            }
            _ibusUser.FindUser(_icdoContactTicket.assign_to_user_id);
        }

        public void LoadAppointmentCounselorName()
        {
            if (_ibusCounselor == null)
            {
                _ibusCounselor = new busUser();
            }
            _ibusCounselor.FindUser(_ibusAppointmentSchedule.icdoAppointmentSchedule.counselor_user_id);
        }

        public void LoadSeminarFacilitatorName()
        {
            if (_ibusFacilitator == null)
            {
                _ibusFacilitator = new busUser();
            }
            _ibusFacilitator.FindUser(_ibusSeminarSchedule.icdoSeminarSchedule.facilitator);
        }

        public bool FindContactTicketByDeathNotiWorkflowActivityInstanceID(int aintDeathNotiWorkflowActivityInstanceId)
        {
            if (_icdoContactTicket == null)
            {
                _icdoContactTicket = new cdoContactTicket();
            }
            DataTable ldtbContactTicket = Select<cdoContactTicket>(new string[1] { "DEATH_NOTI_WORKFLOW_ACTIVITY_INSTANCE_ID" },
                  new object[1] { aintDeathNotiWorkflowActivityInstanceId }, null, null);
            if (ldtbContactTicket.Rows.Count > 0)
            {
                _icdoContactTicket.LoadData(ldtbContactTicket.Rows[0]);
                return true;
            }
            return false;
        }

        public void LoadWebContact()
        {
            if (ibusWebContact == null)
            {
                ibusWebContact = new busContact();
            }
            ibusWebContact.FindContact(icdoContactTicket.web_contact_id);
        }

        # endregion

        # region Button Logic
        public ArrayList btnGetPersonAndOrgDetails_Click()
        {
            if (_icdoContactTicket.person_id != 0)
            {
                LoadPerson();

                icdoContactTicket.callback_phone = ibusPerson.icdoPerson.work_phone_no;
                icdoContactTicket.email = ibusPerson.icdoPerson.email_address;
            }
            else if (_icdoContactTicket.istrOrgCodeID != string.Empty)
            {
                _icdoContactTicket.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(_icdoContactTicket.istrOrgCodeID);
                LoadOrganization();
                icdoContactTicket.callback_phone = ibusOrganization.icdoOrganization.telephone;
            }
            ArrayList larrResult = new ArrayList();
            larrResult.Add(this);
            return larrResult;
        }

        public ArrayList ValidateAddressDeathNotice()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
            {
                cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
                _lobjcdoWebServiceAddress.addr_line_1 = _ibusDeathNotice.icdoDeathNotice.contact_address;
                _lobjcdoWebServiceAddress.addr_city = _ibusDeathNotice.icdoDeathNotice.contact_city;
                _lobjcdoWebServiceAddress.addr_state_value = _ibusDeathNotice.icdoDeathNotice.contact_state_value;
                _lobjcdoWebServiceAddress.addr_zip_code = _ibusDeathNotice.icdoDeathNotice.contact_zip;
                cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
                if (_lobjcdoWebServiceAddressResult.address_validate_flag == busConstant.Flag_No)
                {
                    //ERROR MESSAGE
                    //RETURN
                    //utlError lutlerror = null;
                    lobjError = AddError(127, "Address is Invalid, Please Validate the Address.");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
                else
                {
                    //ASSSIGN 
                    _ibusDeathNotice.icdoDeathNotice.contact_address = _lobjcdoWebServiceAddressResult.addr_line_1;
                    _ibusDeathNotice.icdoDeathNotice.contact_city = _lobjcdoWebServiceAddressResult.addr_city;
                    _ibusDeathNotice.icdoDeathNotice.contact_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                    _ibusDeathNotice.icdoDeathNotice.contact_zip = _lobjcdoWebServiceAddressResult.addr_zip_code;
                    larrErrors.Add(this);
                    return larrErrors;
                }
            }
            return larrErrors;
        }
        public ArrayList ValidateAddressNewGroup()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeNewGroup)
            {
                cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
                _lobjcdoWebServiceAddress.addr_line_1 = _ibusNewGroup.icdoNewGroup.address;
                _lobjcdoWebServiceAddress.addr_city = _ibusNewGroup.icdoNewGroup.city;
                _lobjcdoWebServiceAddress.addr_state_value = _ibusNewGroup.icdoNewGroup.state_value;
                _lobjcdoWebServiceAddress.addr_zip_code = _ibusNewGroup.icdoNewGroup.zip_code;
                cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
                if (_lobjcdoWebServiceAddressResult.address_validate_flag == busConstant.Flag_No)
                {
                    //ERROR MESSAGE
                    //RETURN
                    //utlError lutlerror = null;
                    lobjError = AddError(127, "Address is Invalid, Please Validate the Address.");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
                else
                {
                    //ASSSIGN 
                    _ibusNewGroup.icdoNewGroup.address = _lobjcdoWebServiceAddressResult.addr_line_1;
                    _ibusNewGroup.icdoNewGroup.city = _lobjcdoWebServiceAddressResult.addr_city;
                    _ibusNewGroup.icdoNewGroup.state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                    _ibusNewGroup.icdoNewGroup.zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                    larrErrors.Add(this);
                    return larrErrors;
                }
            }
            return larrErrors;
        }

        # endregion

        public bool iblnWSSIsHistoryInserted { get; set; }
        # region Before Persist Changes
        public override void BeforePersistChanges()
        {
            //Assigning the Current State into Variable
            ObjectState lenuCurrentState = _icdoContactTicket.ienuObjectState;
            string lstrOldStatusValue = String.Empty;
            LoadSponsorName();

            /** UID-011  History should be saved anytime changes in the fields occur; not necessarily every time the record is saved **/
            if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
            {
                lstrOldStatusValue = _icdoContactTicket.ihstOldValues["status_value"].ToString();
                _icdoContactTicket.NeedHistory = false;
                if ((Convert.ToString(_icdoContactTicket.ihstOldValues["notes"]) != _icdoContactTicket.notes) ||
                    (Convert.ToInt32(_icdoContactTicket.ihstOldValues["assign_to_user_id"]) != _icdoContactTicket.assign_to_user_id) ||
                    (Convert.ToString(_icdoContactTicket.ihstOldValues["status_value"]) != _icdoContactTicket.status_value) ||
                    (Convert.ToString(_icdoContactTicket.ihstOldValues["ticket_type_value"]) != _icdoContactTicket.ticket_type_value) ||
                    (Convert.ToString(_icdoContactTicket.ihstOldValues["modified_by"]) != _icdoContactTicket.modified_by))
                {
                    _icdoContactTicket.NeedHistory = true;
                }
                _icdoContactTicket.contact_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1001, _icdoContactTicket.contact_type_value);
            }

            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
            {
                if (icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    _icdoContactTicket.Insert();

                    if ((_icdoContactTicket.original_contact_ticket_id != 0) && (_icdoContactTicket.copy_status_flag == "N") && (!iblnIsFromPortal))
                    {
                        _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid = Guid.NewGuid().ToString("B");
                    }
                    _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id = icdoContactTicket.contact_ticket_id;
                    _ibusAppointmentSchedule.icdoAppointmentSchedule.Insert();

                    LoadAppointmentCounselorName();
                    if ((_icdoContactTicket.original_contact_ticket_id != 0) && (_icdoContactTicket.copy_status_flag == "N") && (!iblnIsFromPortal))
                    {
                        //Sending Outlook email to the Counselor on creation of new Appointment
                        busGlobalFunctions.SendMeetingRequest(appointmentStartDateTime, appointmentEndDateTime,
                             _ibusCounselor.icdoUser.user_id + ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id +
                            ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                            String.Empty, String.Empty, _Organizer_Name, _ibusCounselor.icdoUser.email_address, _ibusCounselor.icdoUser.email_address,
                            _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid);
                    }
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    LoadAppointmentCounselorName();
                    if (_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_cancel_flag == busConstant.Flag_Yes)
                    {
                        //UAT - PIr - 24
                        _icdoContactTicket.status_value = busConstant.ContactTicketStatusClosed;
                    }
                    _icdoContactTicket.Update();
                    if (_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_cancel_flag != busConstant.Flag_Yes)
                    {
                        //Time Change Only ( Sends the Updated Meeting Request)
                        if ((Convert.ToDateTime(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["appointment_date"]) != _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date) ||
                       (Convert.ToString(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["start_time_value"]) != _ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_value) ||
                       (Convert.ToString(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["end_time_value"]) != _ibusAppointmentSchedule.icdoAppointmentSchedule.end_time_value))
                        {

                            _ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_value));
                            _ibusAppointmentSchedule.icdoAppointmentSchedule.end_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusAppointmentSchedule.icdoAppointmentSchedule.end_time_value));

                            if (Convert.ToInt32(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["counselor_user_id"]) == _ibusAppointmentSchedule.icdoAppointmentSchedule.counselor_user_id)
                            {
                                // Sending Outlook Email to Appointment Counselor regarding change of time or date
                                busGlobalFunctions.SendMeetingRequest(appointmentStartDateTime, appointmentEndDateTime,
                                    "Updated: " + _ibusCounselor.icdoUser.user_id + ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id +
                            ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                                     string.Empty, String.Empty, _Organizer_Name, _ibusCounselor.icdoUser.email_address, _ibusCounselor.icdoUser.email_address,
                                     _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid);
                            }
                        }
                        if (Convert.ToInt32(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["counselor_user_id"]) != _ibusAppointmentSchedule.icdoAppointmentSchedule.counselor_user_id)
                        {
                            //sending mail to old counselor
                            if (Convert.ToInt32(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["counselor_user_id"]) != 0)
                            {
                                busUser _ibusOldCounselor = new busUser();
                                _ibusOldCounselor.FindUser(Convert.ToInt32(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["counselor_user_id"]));

                                busGlobalFunctions.CancelMeetingRequest(appointmentStartDateTime, appointmentEndDateTime,
                                                                        "{Cancelled} " +
                                                                        _ibusOldCounselor.icdoUser.user_id + ", " +
                                                                        _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id +
                                                                        ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                                                                        string.Empty, String.Empty, _Organizer_Name,
                                                                        _ibusOldCounselor.icdoUser.email_address,
                                                                        _ibusOldCounselor.icdoUser.email_address,
                                                                        Convert.ToString(_ibusAppointmentSchedule.icdoAppointmentSchedule.ihstOldValues["meeting_request_uid"]));
                            }

                            //sending mail to new counselor
                            _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid = Guid.NewGuid().ToString("B");
                            busGlobalFunctions.SendMeetingRequest(appointmentStartDateTime, appointmentEndDateTime,
                                                          _ibusCounselor.icdoUser.user_id + ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id +
                                                  ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                                                           string.Empty, String.Empty, _Organizer_Name, _ibusCounselor.icdoUser.email_address, _ibusCounselor.icdoUser.email_address,
                                                           _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid);

                            if ((!iblnIsFromPortal) && (icdoContactTicket.is_ticket_created_from_portal_flag == busConstant.Flag_Yes) && (icdoContactTicket.person_id > 0))
                            {
                                string lstrMessage = string.Format(@"Your appointment for {0} counseling has been tentatively scheduled 
                                                                   for {1} beginning {2} with {3}.
                                                                   You will be contacted by {3} if this appointment needs to be rescheduled",
                                                                   ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                                                                   ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_date.ToString("MM/dd/yyyy"),
                                                                   ibusAppointmentSchedule.icdoAppointmentSchedule.start_time_description,
                                                                   ibusCounselor.icdoUser.User_Name);
                                busWSSHelper.PublishMSSMessage(0, 0, lstrMessage, busConstant.WSS_MessageBoard_Priority_High, icdoContactTicket.person_id);
                            }

                        }
                    }
                    else if (_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_cancel_flag == busConstant.Flag_Yes)
                    {
                        // Sending Outlook Cancellation Email to Appointment Counselor to remove the meeting from Outlook Calender
                        busGlobalFunctions.CancelMeetingRequest(appointmentStartDateTime, appointmentEndDateTime,
                            "{Cancelled} " + _ibusCounselor.icdoUser.user_id + ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.contact_ticket_id +
                        ", " + _ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_type_description,
                             String.Empty, String.Empty, _Organizer_Name, _ibusCounselor.icdoUser.email_address, _ibusCounselor.icdoUser.email_address,
                             _ibusAppointmentSchedule.icdoAppointmentSchedule.meeting_request_uid);
                    }
                    _ibusAppointmentSchedule.icdoAppointmentSchedule.Update();
                }
            }
            else if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetBenefitEstimate)
            {
                if (icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    icdoContactTicket.Insert();
                    _ibusBenefitEstimate.icdoBenefitEstimate.contact_ticket_id = icdoContactTicket.contact_ticket_id;
                    _ibusBenefitEstimate.icdoBenefitEstimate.Insert();
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    _icdoContactTicket.Update();
                    _ibusBenefitEstimate.icdoBenefitEstimate.Update();
                }

                //Updating the Plan Types
                foreach (cdoBenefitEstimateRetirementType lcdoRetirementType in _ibusBenefitEstimate.iclcRetirementType)
                {
                    if (lcdoRetirementType.ienuObjectState == ObjectState.CheckListInsert || lcdoRetirementType.contact_ticket_retirement_type_id == 0)
                    {
                        lcdoRetirementType.benefit_estimate_id = _ibusBenefitEstimate.icdoBenefitEstimate.benefit_estimate_id;
                        lcdoRetirementType.Insert();
                    }
                    else if (lcdoRetirementType.ienuObjectState == ObjectState.CheckListDelete)
                    {
                        lcdoRetirementType.Delete();
                    }
                }
            }
            else if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
            {
                if (icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    icdoContactTicket.Insert();
                    if (ibusDeathNotice.icdoDeathNotice.reporting_Month.IsNotNullOrEmpty())
                    {
                        ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions =
                                                        Convert.ToDateTime(ibusDeathNotice.icdoDeathNotice.reporting_Month);
                    }
                    _ibusDeathNotice.icdoDeathNotice.contact_ticket_id = icdoContactTicket.contact_ticket_id;
                    if (_ibusDeathNotice.icdoDeathNotice.perslink_id < 0)
                        _ibusDeathNotice.icdoDeathNotice.perslink_id = 0;
                    if(iblnIsFromInternal && _ibusDeathNotice.icdoDeathNotice.perslink_id > 0)
                    {
                        busPerson lbusperson = new busPerson() { icdoPerson = new cdoPerson() };
                        lbusperson.FindPerson(_ibusDeathNotice.icdoDeathNotice.perslink_id);
                        _ibusDeathNotice.icdoDeathNotice.deceased_name = lbusperson.icdoPerson.FullName;
                    }
                    _ibusDeathNotice.icdoDeathNotice.Insert();
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    if (ibusDeathNotice.icdoDeathNotice.reporting_Month.IsNotNullOrEmpty())
                    {
                        ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions =
                                                      Convert.ToDateTime(ibusDeathNotice.icdoDeathNotice.reporting_Month);
                    }
                    _icdoContactTicket.Update();
                    _ibusDeathNotice.icdoDeathNotice.Update();
                }
            }
            else if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
            {
                _ibusSeminarSchedule.TotalAmount = _ibusSeminarSchedule.NoOfAttendees * _ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee;

                if (_icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    _icdoContactTicket.Insert();
                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid = Guid.NewGuid().ToString("B");

                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                    {
                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid = Guid.NewGuid().ToString("B");
                    }

                    _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id = _icdoContactTicket.contact_ticket_id;
                    _ibusSeminarSchedule.icdoSeminarSchedule.Insert();
                    LoadSeminarFacilitatorName();

                    // sending Email on creation of new Seminar
                    busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                        _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                            ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                       String.Empty,
                        _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                        _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);

                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                    {
                        // sending Email on creation of new Seminar
                        busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                            _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                           String.Empty,
                            _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                            _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                            _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                    }
                }
                else if (
                            (_icdoContactTicket.ienuObjectState == ObjectState.Update) &&
                            (_icdoContactTicket.ihstOldValues["contact_type_value"].ToString() == busConstant.ContactTicketTypeWorkflow)
                        )
                {
                    _icdoContactTicket.Update();
                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid = Guid.NewGuid().ToString("B");

                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                    {
                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid = Guid.NewGuid().ToString("B");
                    }
                    _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id = _icdoContactTicket.contact_ticket_id;
                    _ibusSeminarSchedule.icdoSeminarSchedule.Insert();
                    LoadSeminarFacilitatorName();

                    // sending Email on creation of new Seminar
                    busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                        _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                            ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                       String.Empty,
                        _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                        _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);

                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                    {
                        // sending Email on creation of new Seminar
                        busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                            _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                           String.Empty,
                            _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                            _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                            _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                    }

                }
                else if (
                            ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                            &&
                            (_icdoContactTicket.ihstOldValues["contact_type_value"].ToString() != busConstant.ContactTicketTypeWorkflow)
                        )
                {
                    LoadSeminarFacilitatorName();
                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                    {
                        //UAT PIR - 25
                        _icdoContactTicket.status_value = busConstant.ContactTicketStatusClosed;
                    }
                    _icdoContactTicket.Update();

                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag != busConstant.Flag_Yes)
                    {
                        if ((Convert.ToDateTime(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date1"]) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1) ||
                       ((_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date1_start_time_value"].ToString()) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_start_time_value) ||
                       ((_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date1_end_time_value"].ToString()) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_end_time_value))
                        {
                            _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_start_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_start_time_value));
                            _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_end_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_end_time_value));

                            //If the Facilitator not changed, Send the Updated Meeting Request
                            if (Convert.ToInt32(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["facilitator"]) == _ibusSeminarSchedule.icdoSeminarSchedule.facilitator)
                            {
                                // Sending Outlook Email to Seminar Facilitator regarding change of time or date
                                busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                                   "Updated: " + _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                   String.Empty,
                                   _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                   _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                                   _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);
                            }
                        }

                        if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                        {
                            if ((Convert.ToDateTime(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2"]) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2) ||
                              (Convert.ToString(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2_start_time_value"]) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_start_time_value) ||
                              (Convert.ToString(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2_end_time_value"]) != _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_end_time_value))
                            {
                                _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_start_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_start_time_value));
                                _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_end_time_description = Convert.ToString(iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1009, _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_end_time_value));

                                //If the Facilitator not changed, Send the Updated Meeting Request
                                if (Convert.ToInt32(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["facilitator"]) == _ibusSeminarSchedule.icdoSeminarSchedule.facilitator)
                                {
                                    if (Convert.ToDateTime(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2"]) == DateTime.MinValue)
                                    {
                                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid = Guid.NewGuid().ToString("B");
                                    }

                                    // sending Email to Seminar Facilitator regarding change of time or date
                                    busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                                        "Updated: " + _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                            ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                       String.Empty,
                                        _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                        _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                                }
                            }
                        }

                        if (Convert.ToInt32(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["facilitator"]) != _ibusSeminarSchedule.icdoSeminarSchedule.facilitator)
                        {
                            //sending mail to old counselor
                            if (Convert.ToInt32(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["facilitator"]) != 0)
                            {
                                busUser _ibusOldFacilitator = new busUser();
                                _ibusOldFacilitator.FindUser(Convert.ToInt32(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["facilitator"]));

                                busGlobalFunctions.CancelMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                                                                      "{Cancelled} " +
                                                                      _ibusOldFacilitator.icdoUser.user_id + ", " +
                                                                      _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                                                      ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                                                      String.Empty, _ibusSeminarSchedule.icdoSeminarSchedule.location_name ??
                                                                      String.Empty + ", "
                                                                      + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                                                       _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty,
                                                                      _Organizer_Name, _ibusOldFacilitator.icdoUser.email_address,
                                                                      _ibusOldFacilitator.icdoUser.email_address,
                                                                      _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);

                                if (Convert.ToDateTime(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2"]) != DateTime.MinValue)
                                {
                                    busGlobalFunctions.CancelMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                                                                          "{Cancelled} " +
                                                                          _ibusOldFacilitator.icdoUser.user_id + ", " +
                                                                          _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                                                          ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                                                          String.Empty, _ibusSeminarSchedule.icdoSeminarSchedule.location_name ??
                                                                          String.Empty + ", "
                                                                          + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                                                           _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty,
                                                                          _Organizer_Name, _ibusOldFacilitator.icdoUser.email_address,
                                                                          _ibusOldFacilitator.icdoUser.email_address,
                                                                          _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                                }
                            }

                            //sending mail to new counselor
                            _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid = Guid.NewGuid().ToString("B");

                            busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                                    _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                        ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                   String.Empty,
                                    _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                    _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                                    _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);

                            if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2 != DateTime.MinValue)
                            {
                                _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid = Guid.NewGuid().ToString("B");

                                busGlobalFunctions.SendMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                                        _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                            ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                                       String.Empty,
                                        _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                        _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                                        _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                            }
                        }
                    }
                    else if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                    {
                        // Sending Email on Cancellation of Seminar to Facilitator to remove the meeting from Outlook Calender
                        busGlobalFunctions.CancelMeetingRequest(SeminarStartDatetime, SeminarEndDatetime,
                            "{Cancelled} " + _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                            ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                           String.Empty,
                           _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                           _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                           _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1_meeting_request_uid);

                        //If seminar day 2 is not null send email Cancellation of Seminar to Facilitator to remove the meeting from Outlook Calender
                        if (Convert.ToDateTime(_ibusSeminarSchedule.icdoSeminarSchedule.ihstOldValues["seminar_date2"]) != DateTime.MinValue)
                        {
                            // sending Email on Cancellation of Seminar to Facilitator to remove the meeting from Outlook Calender
                            busGlobalFunctions.CancelMeetingRequest(SeminarStartDatetime2, SeminarEndDatetime2,
                                "{Cancelled} " + _ibusFacilitator.icdoUser.user_id + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id +
                                    ", " + _ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description,
                               String.Empty,
                                _ibusSeminarSchedule.icdoSeminarSchedule.location_name ?? String.Empty + ", " + _ibusSeminarSchedule.icdoSeminarSchedule.location_address ?? String.Empty + ", " +
                                _ibusSeminarSchedule.icdoSeminarSchedule.location_city ?? String.Empty, _Organizer_Name, _ibusFacilitator.icdoUser.email_address, _ibusFacilitator.icdoUser.email_address,
                                _ibusSeminarSchedule.icdoSeminarSchedule.seminar_date2_meeting_request_uid);
                        }
                    }
                    _ibusSeminarSchedule.icdoSeminarSchedule.Update();
                }

            }
            else if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeNewGroup)
            {
                if (icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    icdoContactTicket.Insert();
                    _ibusNewGroup.icdoNewGroup.contact_ticket_id = icdoContactTicket.contact_ticket_id;
                    _ibusNewGroup.icdoNewGroup.Insert();
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    _icdoContactTicket.Update();
                    _ibusNewGroup.icdoNewGroup.Update();
                }

                //Updating the Plan Types
                foreach (cdoNewGroupPlanType lobjNewGroupPlanType in _ibusNewGroup.iclcNewGroupPlanType)
                {
                    if (lobjNewGroupPlanType.ienuObjectState == ObjectState.CheckListInsert)
                    {
                        lobjNewGroupPlanType.new_group_id = _ibusNewGroup.icdoNewGroup.new_group_id;
                        lobjNewGroupPlanType.Insert();
                    }
                    else if (lobjNewGroupPlanType.ienuObjectState == ObjectState.CheckListDelete)
                    {
                        lobjNewGroupPlanType.Delete();
                    }
                }
            }
            else if (this.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetPurchases)
            {
                if (icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    _icdoContactTicket.Insert();
                    _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.contact_ticket_id = icdoContactTicket.contact_ticket_id;
                    _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.Insert();
                    foreach (busSerPurServiceType lobjserpur in _ibusContactMgmtServicePurchase.iclbSerPurServiceType)
                    {
                        lobjserpur.icdoSerPurServiceType.service_purchase_id = _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.service_purchase_id;
                        lobjserpur.icdoSerPurServiceType.Insert();
                    }
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    _icdoContactTicket.Update();
                    _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.Update();
                    foreach (busSerPurServiceType lobjserpur in _ibusContactMgmtServicePurchase.iclbSerPurServiceType)
                    {
                        lobjserpur.icdoSerPurServiceType.service_purchase_id = _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.service_purchase_id;
                        lobjserpur.icdoSerPurServiceType.Update();
                    }
                }

                //Updating the RollOver
                foreach (cdoSerPurRolloverInfo lobjSerPurRollInfo in _ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo)
                {
                    if (lobjSerPurRollInfo.ienuObjectState == ObjectState.CheckListInsert)
                    {
                        lobjSerPurRollInfo.service_purchase_id = _ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.service_purchase_id;
                        lobjSerPurRollInfo.Insert();
                    }
                    else if (lobjSerPurRollInfo.ienuObjectState == ObjectState.CheckListDelete)
                    {
                        lobjSerPurRollInfo.Delete();
                    }
                }
            }
            else
            {
                if (_icdoContactTicket.ienuObjectState == ObjectState.Insert)
                {
                    _icdoContactTicket.Insert();
                }
                else if ((_icdoContactTicket.ienuObjectState == ObjectState.Update) || (_icdoContactTicket.ienuObjectState == ObjectState.Select))
                {
                    _icdoContactTicket.Update();
                }
            }

            if (lenuCurrentState == ObjectState.Insert)
            {
                /*****************************************************************************************************************************
                 * Initializing the Transfer Call and Contact Ticket Workflow Whenever the Contact Ticket Created with the Reassigned Status                          
                 * ***************************************************************************************************************************/
                if (_icdoContactTicket.status_value == busConstant.ContactTicketStatusReassigned)
                {
                    InitializeTransferCallWorkflow(_icdoContactTicket);
                }

                /*****************************************************************************************************************************
                 * Initializing the Schedule Appointment Workflow Whenever the Contact Ticket Created 
                 * with the Apponitment Contact Type (Only at Copy & Close Time)
                 * and Death Notification
                 * ***************************************************************************************************************************/
                if (_icdoContactTicket.original_contact_ticket_id > 0)
                {
                    if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
                    {
                        InitializeWorkflow(_icdoContactTicket, busConstant.Map_Schedule_Appointment, _ibusAppointmentSchedule.icdoAppointmentSchedule.counselor_user_id);
                    }
                    else if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
                    {
                        //if (_icdoContactTicket.person_id > 0)
                        //{
                        InitializeWorkflowForDeath(_icdoContactTicket, busConstant.Map_Initialize_Process_Death_Notification_Workflow, 0);//PIR UAT - 2038 && systest - 2325
                        // }
                    }
                }

                //UCS 11 Addendum - Initialize Workflow
                if (iblnIsFromPortal)
                {
                    switch (icdoContactTicket.contact_type_value)
                    {
                        case busConstant.ContactTicketTypeAppointment:
                            if (icdoContactTicket.person_id > 0)
                                InitiateWSSWorkflow(busConstant.Map_MSS_Schedule_Appointment);
                            else
                                InitiateWSSWorkflow(busConstant.Map_ESS_Schedule_Appointment);
                            break;
                        case busConstant.ContactTicketTypeDeath:
                            InitializeWorkflowForDeath(_icdoContactTicket, busConstant.Map_Initialize_Process_Death_Notification_Workflow, 0);//PIR UAT - 2038 && systest - 2325
                            break;
                        case busConstant.ContactTicketTypeRetBenefitEstimate:
                            InitiateWSSWorkflow(busConstant.Map_Process_Online_Benefit_Estimate_Request);
                            break;
                        case busConstant.ContactTicketTypeRetPurchases:
                            InitiateWSSWorkflow(busConstant.Map_Process_Online_Service_Purchase_Request);
                            break;
                        case busConstant.ContactTicketTypeDefCompProblem:
                        case busConstant.ContactTicketTypeOtherProblem:
                        case busConstant.ContactTicketTypeInsuranceProblem:
                        case busConstant.ContactTicketTypeRetProblem:
                            //Do Nothing
                            break;
                        default:
                            InitiateWSSWorkflow(busConstant.Map_Process_Online_Contact_Ticket);
                            break;

                        //Seminar, we never initialize from Portal. It initialize from Scanning & Indexing
                    }
                }

                //UAT PIR 2179
                //initialize workflow based on problem type
                int lintWorkflowID = 0;
                if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDefCompProblem)
                    lintWorkflowID = 333;
                else if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeOtherProblem)
                    lintWorkflowID = 335;
                else if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeInsuranceProblem)
                    lintWorkflowID = 334;
                else if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetProblem)
                    lintWorkflowID = 336;

                if (lintWorkflowID != 0)
                    InitializeWorkflowForProblems(lintWorkflowID);
            }
            else if (lenuCurrentState == ObjectState.Update)
            {
                /*****************************************************************************************************************************
                * Initializing the Transfer Call and Contact Ticket Workflow 
                * Whenever the Contact Ticket is changed from OPEN Status to Reassigned Status                          
                * ***************************************************************************************************************************/
                if ((_icdoContactTicket.status_value == busConstant.ContactTicketStatusReassigned) &&
                    (lstrOldStatusValue == busConstant.ContactTicketStatusOpen || lstrOldStatusValue == busConstant.ContactTicketStatusClosed))
                {
                    InitializeTransferCallWorkflow(_icdoContactTicket);
                }
                //PROD PIR - 242
                //if the ticket is assigned to the third person,
                //activity must be checked out by the third person 
                if ((_icdoContactTicket.status_value == busConstant.ContactTicketStatusReassigned) &&
                  (lstrOldStatusValue == busConstant.ContactTicketStatusReassigned))
                {
                    if (ibusBaseActivityInstance != null)
                    {
                        if (ibusUser.IsNull())
                            LoadUser();
                        busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                        lbusActivityInstance.icdoBpmActivityInstance.checked_out_user = ibusUser.icdoUser.user_id;
                        lbusActivityInstance.icdoBpmActivityInstance.Update();
                    }
                }

                /*****************************************************************************************************************************
                 * Close the Workflow Instance if available (This calls only if the contact ticket status is closed)
                 * ***************************************************************************************************************************/
                if (_icdoContactTicket.status_value == busConstant.ContactTicketStatusClosed)
                {
                    if (ibusBaseActivityInstance != null)
                    {
                        busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;

                        if ((lbusActivityInstance.icdoBpmActivityInstance.status_value != busConstant.ActivityStatusProcessed) &&
                            (lbusActivityInstance.icdoBpmActivityInstance.status_value != busConstant.ActivityStatusSuspended))
                        {

                            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
                            {
                                if (_ibusSeminarSchedule != null)
                                {
                                    if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                                    {
                                        //Invoke the Cancel Event Activity
                                        //busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
                                    }
                                    else
                                    {
                                       // busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                                    }
                                }
                            }
                            else
                            {
                                //busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Next, busConstant.ActivityStatusProcessed, iobjPassInfo);
                            }
                        }
                    }
                }
            }

        }

        //PIR UAT - 2038 && systest - 2325
        private void InitializeWorkflowForDeath(cdoContactTicket aobjContactTicket, int aintWorkflowID, int aintUserID)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            ////Initializing the Workflow (Request Model)

            int person_id = aobjContactTicket.person_id;
            if (ibusDeathNotice.IsNotNull() && ibusDeathNotice.icdoDeathNotice.perslink_id != 0)
                person_id = ibusDeathNotice.icdoDeathNotice.perslink_id;
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = icdoContactTicket.contact_ticket_id;
            busWorkflowHelper.InitiateBpmRequest(aintWorkflowID, person_id, 0, 0, iobjPassInfo, adictInstanceParameters: ldctParams);
        }

        private void InitializeWorkflow(cdoContactTicket aobjContactTicket, int aintWorkflowID, int aintUserID)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            
            string lstrLoggedInUserId = utlPassInfo.iobjPassInfo.istrUserID;
            if (aobjContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
            {
                //Setting the Counsoler ID ass iobjPassInfo User ID
                utlPassInfo.iobjPassInfo.istrUserID = busGlobalFunctions.GetUserIdFromUserSerialId(aintUserID);
            }
            //lcdoWorkflowRequest.Insert();
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = aobjContactTicket.contact_ticket_id;
            if (!string.IsNullOrEmpty(ibusOrganization.icdoOrganization.org_code))
               ldctParams["org_code"] = ibusOrganization.icdoOrganization.org_code;
            busWorkflowHelper.InitiateBpmRequest(aintWorkflowID, aobjContactTicket.person_id, 0, aobjContactTicket.contact_ticket_id, iobjPassInfo, adictInstanceParameters: ldctParams);
            //Resetting the iObjPassInfo User ID into Logged In User ID
            utlPassInfo.iobjPassInfo.istrUserID = lstrLoggedInUserId;

            

        }

        private void InitializeTransferCallWorkflow(cdoContactTicket acdoContactTicket)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = acdoContactTicket.contact_ticket_id;
            if (!string.IsNullOrEmpty(ibusOrganization.icdoOrganization.org_code))
                ldctParams["org_code"] = ibusOrganization.icdoOrganization.org_code;
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Transfer_Call_And_Contact_Ticket, acdoContactTicket.person_id, 0, acdoContactTicket.contact_ticket_id, iobjPassInfo, adictInstanceParameters: ldctParams);

        }

        private void InitializeWorkflowForProblems(int aintWorkflowID)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            
            string lstrLoggedInUserId = utlPassInfo.iobjPassInfo.istrUserID;
            //lcdoWorkflowRequest.Insert();
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = icdoContactTicket.contact_ticket_id;
            if (!string.IsNullOrEmpty(ibusOrganization.icdoOrganization.org_code))
               ldctParams["org_code"] = ibusOrganization.icdoOrganization.org_code;
            busWorkflowHelper.InitiateBpmRequest(aintWorkflowID, icdoContactTicket.person_id, 0, icdoContactTicket.contact_ticket_id, iobjPassInfo, adictInstanceParameters: ldctParams);

            //Resetting the iObjPassInfo User ID into Logged In User ID
            utlPassInfo.iobjPassInfo.istrUserID = lstrLoggedInUserId;
        }

        # endregion


        /// <summary>
        /// on saving a contact ticket empty records get inserted into the child tables
        /// to avoid this PersistChanges Method have been overridden
        /// </summary>
        /// <returns></returns>
        public override int PersistChanges()
        {
            return 1;
        }
        public override void AfterPersistChanges()
        {
            //Portal : BR-011-97 - Posting Message to Message Board whenever internal user responds to the Ticket which is created by Web User
            if ((!iblnIsFromPortal) && (icdoContactTicket.is_ticket_created_from_portal_flag == busConstant.Flag_Yes))
            {
                if (icdoContactTicket.contact_type_value != busConstant.ContactTicketTypeAppointment &&
                    icdoContactTicket.contact_type_value != busConstant.ContactTicketTypeSeminarAndCounselingOutReach &&
                    icdoContactTicket.contact_type_value != busConstant.ContactTicketTypeDeath)
                {
                    if (icdoContactTicket.person_id > 0 && icdoContactTicket.istrPublishToWss == busConstant.Flag_Yes) //PIR-19351
                    {
                        //PIR-15743 Changed the responce message text //19351
                        busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10348, iobjPassInfo),
                            icdoContactTicket.contact_ticket_id), busConstant.WSS_MessageBoard_Priority_High, icdoContactTicket.person_id);
                    }
                    else if (icdoContactTicket.org_id > 0 && icdoContactTicket.web_contact_id > 0 && icdoContactTicket.istrPublishToWss == busConstant.Flag_Yes)
                    {
                        string lstrPrioityValue = string.Empty;
                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(15, iobjPassInfo, ref lstrPrioityValue),
                            icdoContactTicket.contact_ticket_id), busConstant.WSS_MessageBoard_Priority_High, aintOrgID: icdoContactTicket.org_id, aintContactID: icdoContactTicket.web_contact_id);
                    }
                }
            }

            this.icdoContactTicket.notes = string.Empty;
            LoadContactTicketHistory();

            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetPurchases)
            {
                _ibusContactMgmtServicePurchase.LoadSerPurServiceTypeAfterInserting();
                _ibusContactMgmtServicePurchase.LoadServiceRollInfo();
            }

            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeNewGroup)
            {
                _ibusNewGroup.LoadPlanTypes();
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetBenefitEstimate)
            {
                _ibusBenefitEstimate.LoadRetirementType();
            }

            if (_icdoContactTicket.person_id != 0)
            {
                LoadPerson();
            }
            else if (_icdoContactTicket.org_id != 0)
            {
                LoadOrganization();
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
            {
                _ibusSeminarSchedule.TotalAmount = _ibusSeminarSchedule.NoOfAttendees * _ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee;
            }
            if (iblnIsFromPortal && ibusDeathNotice.IsNotNull() && ibusDeathNotice.icdoDeathNotice.perslink_id > 0)
            {
                //ibusDeathNotice.icdoDeathNotice.deceased_name = GetPersonNameByPersonID(ibusDeathNotice.icdoDeathNotice.perslink_id).icdoPerson.FullName;
                ibusDeathNotice.icdoDeathNotice.istrFullNameWithPersonId = GetPersonNameByPersonID(ibusDeathNotice.icdoDeathNotice.perslink_id).icdoPerson.NameWithPersonID;
            }
            if (iblnIsFromInternal && ibusDeathNotice.IsNotNull() && ibusDeathNotice.icdoDeathNotice.IsNotNull() && ibusDeathNotice.icdoDeathNotice.perslink_id > 0)
                ibusDeathNotice.icdoDeathNotice.deceased_name = GetPersonNameByPersonID(ibusDeathNotice.icdoDeathNotice.perslink_id).icdoPerson.FullName;

            if (iblnIsFromPortal && iblnIsFromReportAProblem) LoadESSReportedProblems();
            if (iblnIsFromPortal && iblnIsFromESS)
            {
                LoadESSAppointments();
                LoadESSDeathNotices();
            }
            LoadContactTicketByPerson(); //PIR-19351 - For MSS
            LoadContactTicketDetailHistory(icdoContactTicket.contact_ticket_id); //PIR-19351 - For ESS
            base.AfterPersistChanges();
			iblnWSSIsHistoryInserted = true;  //19351
        }

        # region Mass Update of Seminar Attendee and Guests
        /// <summary>
        /// Mass Updating of attended_flag as yes for selected Seminar attendees
        /// </summary>
        /// <param name="aarrSelectedObjects"></param>
        /// <returns></returns>
        public ArrayList UpdateStatusAttended(ArrayList aarrSelectedObjects)
        {
            ArrayList larrlist = new ArrayList();
            utlError lerror = new utlError();
            if (aarrSelectedObjects != null)
            {
                if (aarrSelectedObjects.Count > 0)
                {
                    //PIR 268
                    if ((_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1 >= DateTime.Now) ||
                        (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.AddDays(15) <= DateTime.Now))
                    {
                        //selected attendees can not be marked as attended. Either of the below conditions fails
                        //Seminar date is greater than today's date
                        //Seminar date + 15 days is less than todays date                         
                        lerror = AddError(3049, String.Empty);
                        larrlist.Add(lerror);
                    }
                    else
                    {
                        foreach (busSeminarAttendeeDetail lbusSeminarDetail in aarrSelectedObjects)
                        {
                            lbusSeminarDetail.icdoSeminarAttendeeDetail.attended_flag = "Y";
                            lbusSeminarDetail.icdoSeminarAttendeeDetail.Update();
                        }
                        larrlist.Add(this);
                    }
                }
            }
            return larrlist;
        }
        /// <summary>
        /// Mass Updating of guest_flag as yes for selected Seminar attendees
        /// </summary>
        /// <param name="aarrSelectedObjects"></param>
        /// <returns></returns>
        public ArrayList UpdateStatusGuest(ArrayList aarrSelectedObjects)
        {
            ArrayList larrlist = new ArrayList();
            utlError lerror = new utlError();
            if (aarrSelectedObjects != null)
            {
                if (aarrSelectedObjects.Count > 0)
                {
                    //PIR 268
                    if ((_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1 >= DateTime.Now) ||
                       (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.AddDays(15) <= DateTime.Now))
                    {
                        //selected attendees can not be marked as Guests. Either of the below conditions fails
                        //Seminar date is greater than today's date
                        //Seminar date + 15 days is less than todays date 
                        lerror = AddError(3053, String.Empty);
                        larrlist.Add(lerror);
                        return larrlist;
                    }
                    else
                    {
                        foreach (busSeminarAttendeeDetail lbusSeminarDetail in aarrSelectedObjects)
                        {
                            lbusSeminarDetail.icdoSeminarAttendeeDetail.guest_flag = "Y";
                            lbusSeminarDetail.icdoSeminarAttendeeDetail.Update();
                        }
                        larrlist.Add(this);
                    }
                }
            }
            return larrlist;
        }
        # endregion

        # region Reports
        public DataSet rptCallCenterMetrics(int lintFromMonth, int lintFromYear, int lintToMonth, int lintToYear)
        {
            DataSet ldsCallCenterMetrics = new DataSet("Contact Ticket");
            DateTime ldtStartDate = new DateTime(lintFromYear, lintFromMonth, 01);
            DateTime ldtEndDate = new DateTime(lintToYear, lintToMonth, DateTime.DaysInMonth(lintToYear, lintToMonth));
            DataTable ldtCallCenterMetrics = Select("cdoContactTicket.RPT_CALLCENTER_METRICS", new object[2] { ldtStartDate, ldtEndDate });

            if (ldtCallCenterMetrics != null)
            {
                if (ldtCallCenterMetrics.Rows.Count > 0)
                {
                    //Fill up the Blank Month Year
                    DateTime ldtCurrentDate = ldtStartDate;
                    DateTime ldtDataTableDate;
                    while (ldtCurrentDate <= ldtEndDate)
                    {
                        bool lblnFound = false;
                        foreach (DataRow ldrData in ldtCallCenterMetrics.Rows)
                        {
                            ldtDataTableDate = Convert.ToDateTime(ldrData["DATE"]);
                            if ((ldtDataTableDate.Month == ldtCurrentDate.Month) && (ldtDataTableDate.Year == ldtCurrentDate.Year))
                            {
                                lblnFound = true;
                                break;
                            }
                        }

                        if (!lblnFound)
                        {
                            DataRow ldtNewRow = ldtCallCenterMetrics.NewRow();
                            ldtNewRow["GROUPNAME"] = ldtCallCenterMetrics.Rows[0]["GROUPNAME"];
                            ldtNewRow["DESCRIPTION"] = ldtCallCenterMetrics.Rows[0]["DESCRIPTION"];
                            ldtNewRow["DATE"] = ldtCurrentDate;
                            ldtNewRow["RECORDCOUNT"] = 0;
                            ldtCallCenterMetrics.Rows.Add(ldtNewRow);
                        }
                        ldtCurrentDate = ldtCurrentDate.AddMonths(1);
                    }
                }
            }

            ldtCallCenterMetrics.DataSet.Tables.RemoveAt(0);
            ldtCallCenterMetrics.TableName = busConstant.ReportTableName;
            ldsCallCenterMetrics.Tables.Add(ldtCallCenterMetrics);
            return ldsCallCenterMetrics;
        }

        public DataSet rptCallDistribution(int lintUserID, int lintFromMonth, int lintFromYear, int lintToMonth, int lintToYear)
        {
            DataSet ldsCallDistribution = new DataSet("Call Distribution");
            DateTime ldtStartDate = new DateTime(lintFromYear, lintFromMonth, 01);
            DateTime ldtEndDate = new DateTime(lintToYear, lintToMonth, DateTime.DaysInMonth(lintToYear, lintToMonth));
            DataTable ldtCallDistribution = Select("cdoContactTicket.RPT_CALL_DISTRIBUTION", new object[3] { ldtStartDate, ldtEndDate, lintUserID });
            ldtCallDistribution.DataSet.Tables.RemoveAt(0);
            ldtCallDistribution.TableName = busConstant.ReportTableName;
            ldsCallDistribution.Tables.Add(ldtCallDistribution);
            return ldsCallDistribution;
        }

        public DataSet rptDeferredComp(DateTime adtEffectiveDate)
        {
            //PIR - 115
            DataSet ldsDeferredComp = new DataSet("Deferred Comp");
            DataTable ldtDeferredComp = new DataTable();
            
            ldtDeferredComp = busNeoSpinBase.Select("cdoSeminarSchedule.ListofNewDefCompAgentsHavingNOTraining", new object[1] { adtEffectiveDate });
            ldtDeferredComp.DataSet.Tables.RemoveAt(0);
            ldtDeferredComp.TableName = busConstant.ReportTableName;
            ldtDeferredComp.Columns.Add("NAME", Type.GetType("System.String"));
            ldtDeferredComp.Columns.Add("CITY", Type.GetType("System.String"));
            ldtDeferredComp.Columns.Add("STATE", Type.GetType("System.String"));
            ldtDeferredComp.Columns.Add("LAST_DATE", Type.GetType("System.DateTime"));
            foreach (DataRow ldrRow in ldtDeferredComp.Rows)
            {
                busContact lobjContact = new busContact
                {
                    icdoContact = new cdoContact(),
                    ibusContactPrimaryAddress = new busOrgContactAddress { icdoOrgContactAddress = new cdoOrgContactAddress() }
                };
                lobjContact.FindContact(Convert.ToInt32(ldrRow["contact_id"]));
                lobjContact.LoadContactAddressForDeferredCompAgent(Convert.ToInt32(ldrRow["contact_id"]));
                ldrRow["NAME"] = lobjContact.full_name;
                ldrRow["CITY"] = lobjContact.ibusContactPrimaryAddress.icdoOrgContactAddress.city;
                ldrRow["STATE"] = lobjContact.ibusContactPrimaryAddress.icdoOrgContactAddress.state_description;
                if (ldrRow["SEMINAR_DATE1"] != DBNull.Value)
                    ldrRow["LAST_DATE"] = Convert.ToDateTime(ldrRow["SEMINAR_DATE1"]);
                else
                    ldrRow["LAST_DATE"] = DBNull.Value;
            }            
            ldsDeferredComp.Tables.Add(ldtDeferredComp);
            return ldsDeferredComp;
        }

        // SEMINAR DEMOGRAPHIC REPORT
        public DataSet rptSeminarDemogrhapics(string astrSeminarType, DateTime adtSeminarDateFrom, DateTime adtSeminarDateTo)
        {
            //PIR - 115
            DataSet ldsSeminarGroup = new DataSet("Seminar Demographics");
            DataTable ldtSeminarGroup = busNeoSpinBase.Select("cdoSeminarSchedule.rptSeminarDemographicReport",
                new object[3] { adtSeminarDateFrom, adtSeminarDateTo, astrSeminarType });
            ldtSeminarGroup.DataSet.Tables.RemoveAt(0);
            ldtSeminarGroup.TableName = busConstant.ReportTableName;
            // ldtSeminarGroup.Columns.Add("PersonID", Type.GetType("System.Int32"));
            //ldtSeminarGroup.Columns.Add("PersonName", Type.GetType("System.String"));
            ldtSeminarGroup.Columns.Add("Age", Type.GetType("System.Int32"));
            ldtSeminarGroup.Columns.Add("Plan", Type.GetType("System.String"));
            ldtSeminarGroup.Columns.Add("Gender", Type.GetType("System.String"));
            ldtSeminarGroup.Columns.Add("FAS", Type.GetType("System.Decimal"));
            ldtSeminarGroup.Columns.Add("TVSC", Type.GetType("System.Decimal"));
            ldtSeminarGroup.Columns.Add("NRD", Type.GetType("System.DateTime"));

            foreach (DataRow ldrRow in ldtSeminarGroup.Rows)
            {
                string lstrPersonName = ldrRow["PERSON_NAME"].ToString();
                DateTime ldtDateOfBirth = Convert.ToDateTime(ldrRow["DATEOFBIRTH"]);
                int lintPersonId = Convert.ToInt32(ldrRow["PERSONID"]);
                string lstrPlanName = ldrRow["PLANNAME"].ToString();
                DateTime ldtSeminarDate = Convert.ToDateTime(ldrRow["SEMINARDATE"]);

                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPerson.icdoPerson.LoadData(ldrRow);

                busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lobjPersonAccount.icdoPersonAccount.LoadData(ldrRow);

                busSeminarSchedule lobjSeminarSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lobjSeminarSchedule.icdoSeminarSchedule.LoadData(ldrRow);

                busPlan lobjPlan = new busPlan { icdoPlan = new cdoPlan() };
                lobjPlan.icdoPlan.LoadData(ldrRow);

                //***** calculate age
                int lintMemberAgeMonthPart = 0;
                int lintMemberAgeYearPart = 0;
                int lintMembersAgeInMonthsAsOnRetirementDate = 0;
                decimal ldecMemberAgeBasedOnRetirementDate = 0.00M;
                busPersonBase.CalculateAge(ldtDateOfBirth, ldtSeminarDate,
                   ref lintMembersAgeInMonthsAsOnRetirementDate, ref ldecMemberAgeBasedOnRetirementDate, 2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
                //******

                ldrRow["PersonID"] = lintPersonId;
                ldrRow["Person_Name"] = lstrPersonName;
                ldrRow["Age"] = ldecMemberAgeBasedOnRetirementDate;
                ldrRow["Plan"] = lstrPlanName;
                ldrRow["Gender"] = lobjPerson.icdoPerson.gender_description;

                //get TVSC as on seminar date              
                bool lblnIsPlanJobService;
                lblnIsPlanJobService = false;
                if (lobjPlan.icdoPlan.plan_id == busConstant.PlanIdJobService)
                    lblnIsPlanJobService = true;
                ldrRow["TVSC"] = lobjPerson.GetTotalVSCForPerson(lblnIsPlanJobService, lobjSeminarSchedule.icdoSeminarSchedule.seminar_date1);
                decimal ldecTVSC = Convert.ToDecimal(ldrRow["TVSC"]);

                //Get NRD as on seminar date 
                //get NRD for DB plans only
                if (lobjPlan.IsRetirementPlan())
                {
                    ldrRow["NRD"] = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(lobjPlan.icdoPlan.plan_id,
                        lobjPlan.icdoPlan.plan_code, lobjPlan.icdoPlan.benefit_provision_id, busConstant.BenefitAppealTypeRetirement,
                        lobjPerson.icdoPerson.date_of_birth, ldecTVSC, 0, iobjPassInfo, lobjSeminarSchedule.icdoSeminarSchedule.seminar_date1, lobjPersonAccount.icdoPersonAccount.person_account_id, true, 0.00M);
                }
                //Get FAS as on seminar date 
                busBenefitCalculation lobjBenefitCalculation = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
                lobjBenefitCalculation.ibusMember = lobjPerson;
                lobjBenefitCalculation.ibusPlan = lobjPlan;
                lobjBenefitCalculation.ibusPersonAccount = lobjPersonAccount;
                lobjBenefitCalculation.icdoBenefitCalculation.credited_vsc = ldecTVSC;
                lobjBenefitCalculation.icdoBenefitCalculation.retirement_date = lobjSeminarSchedule.icdoSeminarSchedule.seminar_date1;
                lobjBenefitCalculation.icdoBenefitCalculation.termination_date = lobjSeminarSchedule.icdoSeminarSchedule.seminar_date1;
                lobjBenefitCalculation.icdoBenefitCalculation.plan_id = lobjPlan.icdoPlan.plan_id;
                lobjBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                lobjBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
                lobjBenefitCalculation.CalculateFAS();
                ldrRow["FAS"] = lobjBenefitCalculation.icdoBenefitCalculation.calculation_final_average_salary;

            }
            ldsSeminarGroup.Tables.Add(ldtSeminarGroup);
            return ldsSeminarGroup;
        }
        # endregion

        # region Business Rules
        //Rule if Plan Type is not checked for New Group Contact Type
        public bool CheckNewPlantypeIsSelected()
        {
            Collection<cdoNewGroupPlanType> lcblNewGroupPlantype = new Collection<cdoNewGroupPlanType>(_ibusNewGroup.iclcNewGroupPlanType);
            if (lcblNewGroupPlantype.Count != 0)
            {
                return false;
            }
            return true;
        }


        //Rule if seminar Cancelled checkbox or Appointment cancel 
        //checkbox is checked or the completion date is less than todays date
        //save button should be invisible
        public bool DisableSaveButton()
        {
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
            {
                if (_ibusAppointmentSchedule == null)
                {
                    _ibusAppointmentSchedule = new busAppointmentSchedule();
                    _ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(_icdoContactTicket.contact_ticket_id);
                }
                if (_ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_cancel_flag == "Y")
                {
                    return true;
                }
            }
            else if ((_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach))
            {
                if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                {
                    return true;

                }
                //PROD PIR - 239
                //when contact ticket status is closed then only the deleted button should be disabled
                //else for closing the the contact ticket the Save button must be available
                //else if ((_ldtSeminarCompletionDate < DateTime.Now) && (_ldtSeminarCompletionDate != DateTime.MinValue))
                //{
                //    return true;
                //}
            }
            else if (IsTicketCreatedFromWorkflow())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This Method will return true if the contact ticket is created from workflow.
        /// The Purpose of this Method is to disable the SAVE button for such contact tickets.
        /// PROD PIR : 224
        /// Exception Case : for Appointment, Transfer Call Workflow, This Rule should not be applicable 
        /// because the workflow is driven thru that screen only. 
        /// (little bit of hard coding involved here.. later, this can be done in better way)
        /// </summary>
        /// <returns></returns>
        public bool IsTicketCreatedFromWorkflow()
        {
            bool lblnResult = false;

            if (ibusProcessInstance == null)
            {
                LoadProcessInstance();
            }
            if (ibusProcessInstance.icdoBpmCaseInstance.case_instance_id > 0)
            {
                //modify logic - venkat
                ibusProcessInstance.iclbBpmProcessInstance[0].LoadBpmProcess();
                if (!busGlobalFunctions.IsContactTicketRelatedWorkflow(ibusProcessInstance.iclbBpmProcessInstance[0].ibusBpmProcess.icdoBpmProcess.name))
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        public bool IsRetirementTypeIsNotOtherTypeAndEffectiveDateEntered()
        {
            bool lblnResult = false;
            if ((ibusBenefitEstimate != null) && (ibusBenefitEstimate.iclcRetirementType != null) &&
                ibusBenefitEstimate.icdoBenefitEstimate.retirement_effective_date != DateTime.MinValue)
            {
                bool lblnOtherSelected = false;
                foreach (cdoBenefitEstimateRetirementType lcdoBERType in ibusBenefitEstimate.iclcRetirementType)
                {
                    if ((lcdoBERType.ienuObjectState != ObjectState.CheckListDelete) && (lcdoBERType.retirement_type_value == busConstant.ContactTicket_BenEstimate_RetirementTypeOther))
                    {
                        lblnOtherSelected = true;
                        break;
                    }
                }
                if (!lblnOtherSelected)
                    lblnResult = true;
            }
            return lblnResult;
        }

        // PIR 9713
        public bool IsAnyRetirementTypeSelected()
        {
            if (ibusBenefitEstimate != null && ibusBenefitEstimate.iclcRetirementType != null)
                return ibusBenefitEstimate.iclcRetirementType.Where(iobj => iobj.ienuObjectState != ObjectState.CheckListDelete).Any();
            return false;
        }

        public bool IsRetirementTypeOtherSelected()
        {
            if (ibusBenefitEstimate != null && ibusBenefitEstimate.iclcRetirementType != null)
            {
                foreach (cdoBenefitEstimateRetirementType lcdoBERType in ibusBenefitEstimate.iclcRetirementType)
                {
                    if (lcdoBERType.ienuObjectState != ObjectState.CheckListDelete && 
                        lcdoBERType.retirement_type_value == busConstant.ContactTicket_BenEstimate_RetirementTypeOther)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsRetirementEffectiveDateFirstOfMonth()
        {
            bool lblnResult = false;
            if ((ibusBenefitEstimate != null) && (ibusBenefitEstimate.icdoBenefitEstimate.retirement_effective_date != DateTime.MinValue))
            {
                if (ibusBenefitEstimate.icdoBenefitEstimate.retirement_effective_date.Day == 1)
                    lblnResult = true;
            }
            return lblnResult;
        }


        // PIR 13381 -- adding registration expiry check
        public Boolean IsSeminarRegistrationDateExpired()
        {
           
            if (iblnIsFromPortal)
            {
                if (iobjPassInfo != null)
                {
                    //Check for new Seminar entry
                    DateTime dt = new DateTime(0001, 01, 01);

                    int intSeminarValidityDays = -1 * Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypePayrollConferenceDays, iobjPassInfo));

                    if (ibusSeminarSchedule != null)
                    {
                        if (ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1 > dt)
                        {

                            if (ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.AddDays(intSeminarValidityDays) < DateTime.Today)
                                return false;
                        }

                    }
                }
            }
            return true;
        }

        # endregion

        # region Override Delete

        public override int Delete()
        {
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
            {
                _ibusAppointmentSchedule = new busAppointmentSchedule();
                if (_ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusAppointmentSchedule.Delete();
                }

            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
            {
                _ibusDeathNotice = new busDeathNotice();
                if (_ibusDeathNotice.FindDeathNoticeByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusDeathNotice.Delete();
                }
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeNewGroup)
            {
                _ibusNewGroup = new busNewGroup();
                if (_ibusNewGroup.FindNewGroupByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusNewGroup.Delete();
                }
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetBenefitEstimate)
            {
                _ibusBenefitEstimate = new busBenefitEstimate();
                if (_ibusBenefitEstimate.FindBenefitEstimateByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusBenefitEstimate.Delete();
                }
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetPurchases)
            {
                _ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase();
                if (_ibusContactMgmtServicePurchase.FindServicePurchaseByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusContactMgmtServicePurchase.Delete();
                }
            }
            if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
            {
                _ibusSeminarSchedule = new busSeminarSchedule();
                if (_ibusSeminarSchedule.FindSeminarScheduleByContactTicket(_icdoContactTicket.contact_ticket_id))
                {
                    _ibusSeminarSchedule.Delete();
                }

            }
            return base.Delete();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!String.IsNullOrEmpty(icdoContactTicket.istrOrgCodeID))
            {
                ibusOrganization = new busOrganization();
                ibusOrganization.FindOrganizationByOrgCode(icdoContactTicket.istrOrgCodeID);
                icdoContactTicket.org_id = ibusOrganization.icdoOrganization.org_id;
            }
            else
            {
                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                icdoContactTicket.org_id = 0;
            }

            if (!iblnIsFromPortal) // PIR 9506
            {
                if (icdoContactTicket.person_id > 0)
                {
                    LoadPerson();
                }
                else
                {
                    ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                }
            }
            if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeNewGroup)
            {
                if (icdoContactTicket.org_id != 0)
                {
                    //PROD PIR - 79
                    SetDepartmentName();
                }
                else
                {
                    ibusNewGroup.icdoNewGroup.department_name = string.Empty;
                }
            }
            //UAT PIR - 2038 and systest PIR -2325
            if (iblnIsFromPortal)
            {
                if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
                { 
                    if (ibusDeathNotice == null)
                    {
                        ibusDeathNotice = new busDeathNotice();
                        ibusDeathNotice.FindDeathNoticeByContactTicket(icdoContactTicket.contact_ticket_id);
                    }
                    if (ibusDeathNotice.icdoDeathNotice.deceased_member_flag == busConstant.Flag_Yes
                        && ibusDeathNotice.icdoDeathNotice.perslink_id != 0)
                        LoadDeceasedPerson();
                }
                if (_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetPurchases)
                {
                    // PIR 9766
                    if (ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.unused_sick_leave_hours > 0)
                        ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.conver_unused_sick_leave_flag = busConstant.Flag_Yes;
                    else
                        ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.conver_unused_sick_leave_flag = busConstant.Flag_No;
                }
                if (ibusDeathNotice.IsNotNull())
                {
                    busPerson lbusPerson = GetPersonNameByPersonID(ibusDeathNotice.icdoDeathNotice.perslink_id);
                    //if (iblnIsFromPortal && lbusPerson.icdoPerson.person_id > 0)
                    //      ibusDeathNotice.icdoDeathNotice.deceased_name = lbusPerson.icdoPerson.FullName;
                    ibusDeathNotice.icdoDeathNotice.iblnIsPersonEnrolledInRetirementPlan = lbusPerson.icdoPerson.iblnIsPersonEnrolledInRetirementPlan;
                }
                if (ibusDeathNotice.IsNotNull() && iblnIsFromESS == true)
                {
                    VerifyAddressUsingUSPS();
                }
            }
            base.BeforeValidate(aenmPageMode);
        }
        # endregion

        # region Correspondence
        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }
        public override busBase GetCorOrganization()
        {
            if (ibusOrganization == null)
                LoadOrganization();
            return _ibusOrganization;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadSponsorName();
            LoadAdditionalDetails();
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            icdoContactTicket.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoContactTicket.istrOrgCodeID);
            LoadPerson();
            LoadOrganization();
            larrList.Add(this);
            return larrList;
        }

        //this method is used to load other details related to correspondence
        public void LoadAdditionalDetails()
        {
            //Create Dummy Seminar Schedule object to correpondence error
            if (_ibusSeminarSchedule == null)
            {
                _ibusSeminarSchedule = new busSeminarSchedule();
            }
            if (_ibusSeminarSchedule.icdoSeminarSchedule == null)
            {
                _ibusSeminarSchedule.icdoSeminarSchedule = new cdoSeminarSchedule();
            }
        }
        # endregion

        #region UCS - 011 add
        public bool iblnIsFromPortal { get; set; }

        //BR - 11 - If Seminar is cancelled , display the status as cancelled.
        public string SeminarTicketStatus { get; set; }
        public void LoadSeminarTicketStatus()
        {
            SeminarTicketStatus = icdoContactTicket.status_description;
            if (ibusSeminarSchedule != null)
            {
                if (ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                    SeminarTicketStatus = "Cancelled";
            }
        }

        public ArrayList btnCreateIDBRemittance_Click()
        {
            ArrayList larrList = new ArrayList();
            if (ibusSeminarSchedule.iclbSeminarAttendeeDetail == null)
                ibusSeminarSchedule.LoadSeminarAttendeeDetail();

            var lenuDistinctOrgs = ibusSeminarSchedule.iclbSeminarAttendeeDetail
                                            .Where(i => i.icdoSeminarAttendeeDetail.payment_method_value == busConstant.PaymentMethodIDB &&
                                                        i.icdoSeminarAttendeeDetail.org_to_bill_id != 0) // PROD PIR ID 5749
                                            .Select(i => i.icdoSeminarAttendeeDetail.org_to_bill_id).Distinct();

            foreach (int lintOrgToBillId in lenuDistinctOrgs)
            {
                //Create Deposit                
                decimal ldecTotalDeposit = ibusSeminarSchedule.iclbSeminarAttendeeDetail
                                            .Where(i => i.icdoSeminarAttendeeDetail.payment_method_value == busConstant.PaymentMethodIDB &&
                                                    i.icdoSeminarAttendeeDetail.org_to_bill_id == lintOrgToBillId).Sum(i => ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee);
                //Creating Deposit
                cdoDeposit lcdoDeposit = new cdoDeposit();
                CreateDeposit(lcdoDeposit, lintOrgToBillId, ldecTotalDeposit);

                //Create Remittance for Each Member
                var lenuFilteredList = ibusSeminarSchedule.iclbSeminarAttendeeDetail.Where(i => i.icdoSeminarAttendeeDetail.org_to_bill_id == lintOrgToBillId
                                                                                        && i.icdoSeminarAttendeeDetail.payment_method_value == busConstant.PaymentMethodIDB);  //PIR-18059
                foreach (busSeminarAttendeeDetail lbusSeminatAttendeeDetail in lenuFilteredList)
                {
                    CreateRemittance(lbusSeminatAttendeeDetail.icdoSeminarAttendeeDetail.person_id, lcdoDeposit.deposit_id, ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee);

                    //BR -11 - Addendum - 119 - Publish message to message board            
                    busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your payment has been received and your seminar registration for {0} has been confirmed.",
                                                        ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.ToString("MM/dd/yyyy")),
                                                        busConstant.WSS_MessageBoard_Priority_High,
                                                        lbusSeminatAttendeeDetail.icdoSeminarAttendeeDetail.person_id);
                }
            }

            //Update the IDB Remittance Flag
            icdoContactTicket.idb_remittance_created_flag = busConstant.Flag_Yes;
            icdoContactTicket.Update();

            //Initialize the IDB Workflow
            InitiateWSSWorkflow(busConstant.Map_Process_Seminar_IDB);

            EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

        private void CreateDeposit(cdoDeposit acdoDeposit, int aintOrgID, decimal ldecTotalDeposit)
        {
            acdoDeposit.org_id = aintOrgID;
            acdoDeposit.reference_no = "Contact Ticket : " + icdoContactTicket.contact_ticket_id;
            acdoDeposit.deposit_amount = ldecTotalDeposit;
            acdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
            acdoDeposit.deposit_source_value = busConstant.DepositSourceIDBPayment;
            acdoDeposit.contact_ticket_id = icdoContactTicket.contact_ticket_id;
            acdoDeposit.deposit_date = DateTime.Today;
            acdoDeposit.Insert();
        }

        private void CreateRemittance(int aintPersonID, int aintDepositID, decimal adecFeeAmount)
        {
            cdoRemittance lcdoRemittance = new cdoRemittance();
            lcdoRemittance.person_id = aintPersonID;
            lcdoRemittance.deposit_id = aintDepositID;
            lcdoRemittance.remittance_type_value = busConstant.RemittanceTypeSeminar;
            //systest pir 2410
            lcdoRemittance.plan_id = busConstant.PlanIdMain; //PIR 20232 ?code
            lcdoRemittance.remittance_amount = adecFeeAmount;
            lcdoRemittance.applied_date = DateTime.Today;
            lcdoRemittance.Insert();
        }

        private void InitiateWSSWorkflow(int aintProcessID)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            ////Initializing the Workflow (Request Model)
            
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = icdoContactTicket.contact_ticket_id;
            if (!string.IsNullOrEmpty(ibusOrganization.icdoOrganization.org_code))
               ldctParams["org_code"] = ibusOrganization.icdoOrganization.org_code;
            //PIR 25255 ‘Process Online Service Purchase Request’ need to be always initiated no matter even there are already existing in progress instances
            if (aintProcessID == busConstant.Map_Process_Online_Service_Purchase_Request)
                ldctParams["check_for_existing_inst"] = busConstant.Flag_No;
            //PIR 26149 ‘Process Online Contact Ticket’ BPM's not creating when there are multiple MSS online tickets submitted the same day
            if (aintProcessID == busConstant.Map_Process_Online_Contact_Ticket)
                ldctParams["check_for_existing_inst"] = busConstant.Flag_No;
            busWorkflowHelper.InitiateBpmRequest(aintProcessID, icdoContactTicket.person_id, 0, 0, iobjPassInfo, adictInstanceParameters: ldctParams);
        }
        //PIR UAT - 2038 && systest - 2325
        private void InitiateWSSWorkflowForDeath(int aintProcessID)
        {
            ////Initializing the Workflow (Request Model)
            

            int person_id = icdoContactTicket.person_id;
            if (iblnIsFromPortal &&
              ibusDeathNotice.icdoDeathNotice.deceased_member_flag == busConstant.Flag_Yes
              && ibusDeathNotice.icdoDeathNotice.perslink_id != 0)
                person_id = ibusDeathNotice.icdoDeathNotice.perslink_id;
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["contact_ticket_id"] = icdoContactTicket.contact_ticket_id;
            busWorkflowHelper.InitiateBpmRequest(aintProcessID, person_id, 0, 0, iobjPassInfo, adictInstanceParameters: ldctParams);
        }

        public DataSet rptSeminarAttendanceReport(int aintTicketNumber)
        {
            DataSet ldsReport = new DataSet("Seminar Attendance Report");
            DataTable ldtTable = CreateSeminarAttendanceTable();
            busContactTicket lbusContactTicket = new busContactTicket();
            if (lbusContactTicket.FindContactTicket(aintTicketNumber))
            {
                lbusContactTicket.ibusSeminarSchedule = new busSeminarSchedule();
                if (lbusContactTicket.ibusSeminarSchedule.FindSeminarScheduleByContactTicket(aintTicketNumber))
                {
                    lbusContactTicket.ibusSeminarSchedule.LoadSeminarAttendeeDetail();
                    lbusContactTicket.LoadOrganization();
                    foreach (busSeminarAttendeeDetail lbusSemAttDetail in lbusContactTicket.ibusSeminarSchedule.iclbSeminarAttendeeDetail)
                    {
                        lbusSemAttDetail.LoadRetirementType();

                        DataRow ldrRow = ldtTable.NewRow();
                        ldrRow["PERSLinkID"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.person_id;
                        ldrRow["Name"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.istrAttendeeDisplayName;
                        ldrRow["Normal"] = lbusSemAttDetail.iclcRetirementType.Any(i => i.retirement_type_value == "NORE") ? "Y" : "";
                        ldrRow["Early"] = lbusSemAttDetail.iclcRetirementType.Any(i => i.retirement_type_value == "EART") ? "Y" : "";
                        ldrRow["Disability"] = lbusSemAttDetail.iclcRetirementType.Any(i => i.retirement_type_value == "DIRT") ? "Y" : "";
                        if (lbusSemAttDetail.icdoSeminarAttendeeDetail.retirement_effective_date != DateTime.MinValue)
                            ldrRow["RetirementDate"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.retirement_effective_date;
                        ldrRow["UnusedSickLeaveHours"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.unused_sick_leave_hours;
                        ldrRow["SponsorOrgName"] = lbusContactTicket.ibusOrganization.icdoOrganization.org_name;
                        ldrRow["SeminarDate1"] = lbusContactTicket.ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1;
                        ldtTable.Rows.Add(ldrRow);
                    }
                }
            }
            ldsReport.Tables.Add(ldtTable);
            return ldsReport;
        }

        private DataTable CreateSeminarAttendanceTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("Normal", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("Early", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("Disability", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("RetirementDate", Type.GetType("System.DateTime"));
            DataColumn dc7 = new DataColumn("UnusedSickLeaveHours", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("SponsorOrgName", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("SeminarDate1", Type.GetType("System.DateTime"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.Columns.Add(dc8);
            ldtbReportTable.Columns.Add(dc9);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public DataSet rptUnpaidSeminarReport()
        {
            DataSet ldsReport = new DataSet("Unpaid Seminar Report");
            DataTable ldtTable = CreateUnpaidSeminarTable();

            DataTable ldtbResult = Select("cdoContactTicket.rptUnpaidSeminar", new object[0] { });
            foreach (DataRow ldrQueryRow in ldtbResult.Rows)
            {
                busSeminarAttendeeDetail lbusSemAttDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
                lbusSemAttDetail.icdoSeminarAttendeeDetail.LoadData(ldrQueryRow);
                lbusSemAttDetail.LoadSeminarSchedule();
                lbusSemAttDetail.ibusSeminarSchedule.LoadContactTicket();
                lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket.LoadOrganization();
                lbusSemAttDetail.LoadDisplayAttendeeName();
                lbusSemAttDetail.LoadTotalPaidFeeAmount();

                DataRow ldrRow = ldtTable.NewRow();
                ldrRow["PERSLinkID"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.person_id;
                ldrRow["Name"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.istrAttendeeDisplayName;
                ldrRow["SeminarType"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description;
                ldrRow["AmountDue"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee - lbusSemAttDetail.TotalPaidFeeAmount;
                ldrRow["ContactTicketNo"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id;
                ldrRow["SponsorOrgName"] = lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket.ibusOrganization.icdoOrganization.org_name;
                ldrRow["SeminarDate1"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1;
                ldtTable.Rows.Add(ldrRow);
            }
            ldsReport.Tables.Add(ldtTable);
            return ldsReport;
        }

        private DataTable CreateUnpaidSeminarTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("SeminarType", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("AmountDue", Type.GetType("System.Decimal"));
            DataColumn dc5 = new DataColumn("ContactTicketNo", Type.GetType("System.Int32"));
            DataColumn dc6 = new DataColumn("SponsorOrgName", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("SeminarDate1", Type.GetType("System.DateTime"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }
        public DataSet rptSeminarIDBReport(int aintTicketNumber, int aintsortby) //PIR 8885
        {
            DataSet ldsReport = new DataSet("Seminar IDB Report");
            DataTable ldtTable = CreateSeminarIDBTable();

            DataTable ldtbResult = Select("cdoContactTicket.rptSeminarIDB", new object[2] { aintTicketNumber, aintsortby }); //PIR 8885
            foreach (DataRow ldrQueryRow in ldtbResult.Rows)
            {
                busSeminarAttendeeDetail lbusSemAttDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
                lbusSemAttDetail.icdoSeminarAttendeeDetail.LoadData(ldrQueryRow);
                lbusSemAttDetail.ibusSeminarSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.LoadData(ldrQueryRow);
                lbusSemAttDetail.LoadDisplayAttendeeName();
                lbusSemAttDetail.LoadTotalPaidFeeAmount();
                lbusSemAttDetail.LoadOrgToBillOrganization();
                lbusSemAttDetail.ibusOrgToBillOrganization.LoadOrgPrimaryAddress();

                DataRow ldrRow = ldtTable.NewRow();
                ldrRow["OrgName"] = (lbusSemAttDetail.ibusOrgToBillOrganization.icdoOrganization.org_name ?? string.Empty).ToUpper();
                ldrRow["OrgCode"] = lbusSemAttDetail.ibusOrgToBillOrganization.icdoOrganization.org_code;
                ldrRow["Address1"] = (lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1 ?? string.Empty).ToUpper();
                ldrRow["Address2"] = (lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2 ?? string.Empty).ToUpper();
                ldrRow["City"] = (lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city ?? string.Empty).ToUpper();
                ldrRow["State"] = (lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value ?? string.Empty).ToUpper();
                string lstrZipCode = lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code ?? String.Empty;
                if ((lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code != null) &&
                    (lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code.Trim() != ""))
                {
                    lstrZipCode += "-" + lbusSemAttDetail.ibusOrgToBillOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code ?? String.Empty;
                }
                ldrRow["Zip"] = lstrZipCode.ToUpper();
                ldrRow["SeminarDate"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1;
                ldrRow["SeminarType"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_description;
                ldrRow["ContactTicketNo"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.contact_ticket_id;
                ldrRow["PERSLinkID"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.person_id;
                ldrRow["Name"] = lbusSemAttDetail.icdoSeminarAttendeeDetail.istrAttendeeDisplayName;
                ldrRow["DueAmount"] = lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.attendee_fee; //PIR 8885
                ldtTable.Rows.Add(ldrRow);
            }
            ldsReport.Tables.Add(ldtTable);
            return ldsReport;
        }

        private DataTable CreateSeminarIDBTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("Address1", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("Address2", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("City", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("State", Type.GetType("System.String"));
            DataColumn dc7 = new DataColumn("Zip", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("OrgCode", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("SeminarDate", Type.GetType("System.DateTime"));
            DataColumn dc10 = new DataColumn("SeminarType", Type.GetType("System.String"));
            DataColumn dc11 = new DataColumn("ContactTicketNo", Type.GetType("System.Int32"));
            DataColumn dc12 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            DataColumn dc13 = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn dc14 = new DataColumn("DueAmount", Type.GetType("System.Decimal"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.Columns.Add(dc8);
            ldtbReportTable.Columns.Add(dc9);
            ldtbReportTable.Columns.Add(dc10);
            ldtbReportTable.Columns.Add(dc11);
            ldtbReportTable.Columns.Add(dc12);
            ldtbReportTable.Columns.Add(dc13);
            ldtbReportTable.Columns.Add(dc14);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }


        #endregion

        //PROD PIR - 239
        //checkbox is checked or the completion date is less than todays date
        //also till contact status is changed to close delete button should be available
        //delete button should be invisible
        public bool DisableDeleteButton()
        {
            if ((_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach))
            {
                if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                {
                    return true;

                }
                //else if ((_ldtSeminarCompletionDate < DateTime.Now) 
                //    && (_ldtSeminarCompletionDate != DateTime.MinValue) 
                //    && 
                if (icdoContactTicket.status_value == busConstant.ContactTicketStatusClosed)
                {
                    return true;
                }
            }
            else if (IsTicketCreatedFromWorkflow())
            {
                return true;
            }
            return false;
        }

        //PROD PIR - 79
        //if New group populate the department name with Org Name if org entered
        private void SetDepartmentName()
        {
            if (ibusOrganization.IsNull())
                LoadOrganization();

            ibusNewGroup.icdoNewGroup.department_name = ibusOrganization.icdoOrganization.org_name;
        }

        //PROD PIR - 239
        //visible rule for New Guest and Attendee button
        //if the seminar date is not past date and seminar is not cancelled then display the New button
        public bool VisibleRuleForNewGuestAttendeeButton()
        {
            if ((_icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach))
            {
                if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == busConstant.Flag_Yes)
                {
                    return true;
                }
                else if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1 < DateTime.Now)
                    return true;
            }
            return false;
        }

        //UAT PIR - 2038 and systest PIR -2325
        public busPerson ibusDeceasedPerson { get; set; }
        public void LoadDeceasedPerson()
        {
            if (ibusDeceasedPerson.IsNull())
                ibusDeceasedPerson = new busPerson();

            ibusDeceasedPerson.FindPerson(ibusDeathNotice.icdoDeathNotice.perslink_id);
        }

        public bool IsPerslinkNotEnteredForDetPurchaseEstimate()
        {
            if (!iblnIsFromPortal)
            {
                if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetBenefitEstimate
                    || icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetPurchases)
                {
                    if (icdoContactTicket.person_id == 0)
                        return true;
                }
                else if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
                {
                    if (icdoContactTicket.is_ticket_created_from_portal_flag != busConstant.Flag_Yes)
                    {
                        if (icdoContactTicket.person_id == 0)
                            return true;
                    }
                }
            }
            return false;

        }

        //this is used to check if the contact ticket is created from in the portal        
        public bool iblnIsFromReportAProblem { get; set; }
        public bool iblnIsFromSeminarSchedule { get; set; }
        //Backlog PIR 12832      
        public bool iblnIsFromMssContactNDPERS { get; set; }


        //prod pir 5190 : validation to check whether person id is working in organization for whom death is reported
        public bool IsEmployeeNotWorkingInSameOrg()
        {
            bool lblnResult = true;

            if (icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath && icdoContactTicket.org_id > 0
                && ibusDeathNotice.icdoDeathNotice.perslink_id > 0)
            {
                DataTable ldtPersonEmployment = Select<cdoPersonEmployment>(new string[2] { "person_id", "org_id" },
                                                                            new object[2] { ibusDeathNotice.icdoDeathNotice.perslink_id, icdoContactTicket.org_id },
                                                                            null, "start_date desc, isnull(end_date,'99991231') desc", true);
                foreach (DataRow ldr in ldtPersonEmployment.Rows)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(DateTime.Today, Convert.ToDateTime(ldr["start_date"]), (ldr["end_date"] == DBNull.Value ?
                        DateTime.Today.AddMonths(1) : Convert.ToDateTime(ldr["start_date"]))))
                    {
                        lblnResult = false;
                        break;
                    }
                }
            }
            else
                lblnResult = false;

            return lblnResult;
        }
        //For MSS Layout change
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            iarrErrors = new ArrayList();
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromMSS || iblnIsFromPortal)
            {
                foreach (utlError lobjErr in iarrErrors)
                {
                    if (lobjErr.istrErrorID == "3019")
                        lobjErr.istrErrorMessage = "Date of Death is Mandatory";
                    else if(lobjErr.istrErrorID == "3020")
                        lobjErr.istrErrorMessage = "Contact Name is Mandatory";
                    else if (lobjErr.istrErrorID == "3021")
                        lobjErr.istrErrorMessage = "Contact Address is Mandatory";
                    else if (lobjErr.istrErrorID == "3022")
                        lobjErr.istrErrorMessage = "Contact City is Mandatory";
                    else if (lobjErr.istrErrorID == "3023")
                        lobjErr.istrErrorMessage = "Contact Zip is Mandatory";
                    else if (lobjErr.istrErrorID == "3025")
                        lobjErr.istrErrorMessage = "Contact Phone is Mandatory";
                    else if (lobjErr.istrErrorID == "3039")
                        lobjErr.istrErrorMessage = "Deceased must be selected";
                    else if (lobjErr.istrErrorID == "3014")
                        lobjErr.istrErrorMessage = "Appointment Type is Mandatory";
                    //Backlog PIR  12832
                    else if (lobjErr.istrErrorID == "10276")
                        lobjErr.istrErrorMessage = "Callback Phone Number is required.";
                    lobjErr.istrErrorID = string.Empty;
                }
            }
        }

        // PROD PIR 9491
        public string IsAdditionalGenericServiceNOTEntered()
        {
            if (iblnIsFromPortal)
            {
                if (ibusContactMgmtServicePurchase.IsNotNull())
                {
                    if (ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.addi_generic_flag == busConstant.Flag_Yes)
                    {
                        if (ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.addi_generic_amount == 0M &&
                            ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase.addi_generic_month == 0M)
                            return busConstant.Flag_No;
                    }
                }
            }
            return busConstant.Flag_Yes;
        }
        public busPerson GetPersonNameByPersonID(int AintPersonID)
        {
            busPerson lbusperson = new busPerson();
            busPerson lbusPersonESS = new busPerson { icdoPerson = new cdoPerson() };

            lbusperson.FindPerson(AintPersonID);
            lbusPersonESS.icdoPerson.person_id = lbusperson.icdoPerson.person_id;
            lbusPersonESS.icdoPerson.first_name = lbusperson.icdoPerson.first_name;
            lbusPersonESS.icdoPerson.middle_name = lbusperson.icdoPerson.middle_name;
            lbusPersonESS.icdoPerson.last_name = lbusperson.icdoPerson.last_name;

            if (iblnIsFromESS)  //PIR-17314
            {
              lbusPersonESS.LoadRetirementAccount();
              lbusPersonESS.icdoPerson.iblnIsPersonEnrolledInRetirementPlan = lbusPersonESS.iclbRetirementAccount.Any(pa => pa.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled);
            }
            return lbusPersonESS;
        }
        public Collection<busContactTicket> iclbESSReportedProblems { get; set; }
        public void LoadESSReportedProblems()
        {
            DataTable ldtbESSReportedProblems = busNeoSpinBase.Select("cdoContactTicket.LoadESSReportedProblems", new object[1] { _icdoContactTicket.org_id });
            iclbESSReportedProblems = GetCollection<busContactTicket>(ldtbESSReportedProblems, "icdoContactTicket");
        
        }
        public Collection<busContactTicket> iclbESSAppointments { get; set; }
        public void LoadESSAppointments()
        {
            iclbESSAppointments = new Collection<busContactTicket>();
            DataTable ldtbESSAppointments = busNeoSpinBase.Select("cdoContactTicket.LoadESSAppointments", new object[1] { _icdoContactTicket.org_id });
            foreach (DataRow dr in ldtbESSAppointments.Rows)
            {
                busContactTicket lobjbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lobjbusContactTicket.icdoContactTicket.LoadData(dr);

                lobjbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
                lobjbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.LoadData(dr);

                iclbESSAppointments.Add(lobjbusContactTicket);
            }
        }
        public Collection<busContactTicket> iclbESSDeathNotices { get; set; }
        public void LoadESSDeathNotices()
        {
            iclbESSDeathNotices = new Collection<busContactTicket>();
            DataTable ldtbESSDeathNotices = busNeoSpinBase.Select("cdoContactTicket.LoadESSDeathNotices", new object[1] { _icdoContactTicket.org_id });
            foreach (DataRow dr in ldtbESSDeathNotices.Rows)
            {
                busContactTicket lobjbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lobjbusContactTicket.icdoContactTicket.LoadData(dr);

                lobjbusContactTicket.ibusDeathNotice = new busDeathNotice { icdoDeathNotice = new cdoDeathNotice() };
                lobjbusContactTicket.ibusDeathNotice.icdoDeathNotice.LoadData(dr);
                iclbESSDeathNotices.Add(lobjbusContactTicket);
            }
        }
        public void VerifyAddressUsingUSPS()
        {
            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                ibusDeathNotice.icdoDeathNotice.address_validate_flag = busConstant.Flag_Yes;
                return;
            }     
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = ibusDeathNotice.icdoDeathNotice.contact_address;            
            _lobjcdoWebServiceAddress.addr_city = ibusDeathNotice.icdoDeathNotice.contact_city;
            _lobjcdoWebServiceAddress.addr_state_value = ibusDeathNotice.icdoDeathNotice.contact_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = ibusDeathNotice.icdoDeathNotice.contact_zip;
            _lobjcdoWebServiceAddress.addr_zip_4_code = ibusDeathNotice.icdoDeathNotice.contact_zip_4_code;
            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                ibusDeathNotice.icdoDeathNotice.contact_address = _lobjcdoWebServiceAddressResult.addr_line_1;
                ibusDeathNotice.icdoDeathNotice.contact_city = _lobjcdoWebServiceAddressResult.addr_city;
                ibusDeathNotice.icdoDeathNotice.contact_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                ibusDeathNotice.icdoDeathNotice.contact_zip = _lobjcdoWebServiceAddressResult.addr_zip_code;
                ibusDeathNotice.icdoDeathNotice.contact_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;

            }
            ibusDeathNotice.icdoDeathNotice.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;
            ibusDeathNotice.icdoDeathNotice.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
        }

        //PIR-18492
        public bool ValidateEmailPattern() 
        {
            if (!String.IsNullOrEmpty(icdoContactTicket.email))
            {
                return busGlobalFunctions.IsEmailValid(icdoContactTicket.email);
            }
            return true;
        }

        //PIR 13190
        public Boolean  VerifyContactPhoneNumber()
        {
            Boolean bIsPhoneValid = true;

            //Validation only for ESS link only
            if (iblnIsFromPortal  && iblnIsFromESS)
            {
                bIsPhoneValid = false;
                if (ibusDeathNotice != null)
                {
                    if (ibusDeathNotice.icdoDeathNotice.contact_phone != null)
                    {
                        if (ibusDeathNotice.icdoDeathNotice.contact_phone.Length == 10)
                        {
                            bIsPhoneValid = true;
                        }
                    }
                }

                else if (icdoContactTicket.callback_phone != null)
                {
                    if (icdoContactTicket.callback_phone.Length == 10)
                    {
                        bIsPhoneValid = true;
                    }                  
                }
            }
            return bIsPhoneValid;
        
        }
        // PIR-13737  Date Validation
        public int CheckDate()
        {
            if (_ibusContactMgmtServicePurchase.IsNotNull() && _ibusContactMgmtServicePurchase.iclbSerPurServiceType.IsNotNull())
            {

                foreach (busSerPurServiceType lobjSerPur in _ibusContactMgmtServicePurchase.iclbSerPurServiceType)
                {

                    if (lobjSerPur.icdoSerPurServiceType.service_type_from_date != DateTime.MinValue || lobjSerPur.icdoSerPurServiceType.service_type_to_date != DateTime.MinValue)
                    {
                        if (lobjSerPur.icdoSerPurServiceType.service_type_from_date == DateTime.MinValue)
                            return 1;
                        else if (lobjSerPur.icdoSerPurServiceType.service_type_to_date == DateTime.MinValue)
                            return 2;
                        else if (lobjSerPur.icdoSerPurServiceType.service_type_from_date > lobjSerPur.icdoSerPurServiceType.service_type_to_date)
                            return 3;
                    }
                }
            }
            return 0;
        }

         //PIR-17314
        public bool isReportingMonthValid = true;
        public bool IsLastReportingMonthForRetrContributionValid()
        {
            if (!String.IsNullOrEmpty(this.ibusDeathNotice.icdoDeathNotice.reporting_Month))
            {
                String strReportingMonth = this.ibusDeathNotice.icdoDeathNotice.reporting_Month;
                string[] strdates = strReportingMonth.Split("/");
                int month = 0;
                int year = 0;
                // For some cases the Reporting month field is read with the remaining underscores ("_")
                //so addidng below check ex "01/111_"

                if (strdates[0].IsNotNullOrEmpty() && !strdates[0].Contains("_"))
                {
                    month = Convert.ToInt32(strdates[0]);
                }
                if (strdates[1].IsNotNullOrEmpty() && !strdates[1].Contains("_"))
                {
                    year = Convert.ToInt32(strdates[1]);
                }
                if ((month >= 1 && month <= 12) && (year >= 1901 && year <= 2100))
                {
                    isReportingMonthValid = true;
                    return true;
                }
            }
            isReportingMonthValid = false;
            return false;
      }

        //PIR-19351
        public Collection<busContactTicket> iclbContactTicket { get; set; }        
        public void LoadContactTicketByPerson() => iclbContactTicket = GetCollection<busContactTicket>(busNeoSpinBase.Select("cdoContactTicket.LoadContactTicketByPerson", new object[1] { _icdoContactTicket.person_id }), "icdoContactTicket");
        public Collection<busContactTicketHistory> iclbContactTicketDetailHistory { get; set; }
        public void LoadContactTicketDetailHistory(int aintContactTicketId) => iclbContactTicketDetailHistory = GetCollection<busContactTicketHistory>(busNeoSpinBase.Select("cdoContactTicket.LoadContactTicketDetailHistory", new object[1] { aintContactTicketId }), "icdoContactTicketHistory");
        Collection<cdoPerson> iclbDeceasedContactNDependentName = new Collection<cdoPerson>();
        public Collection<cdoPerson> LoadDeceasedContactandDependentNames()
        {
            iclbDeceasedContactNDependentName = new Collection<cdoPerson>();
            if (icdoContactTicket.person_id > 0)
            {
                DataTable ldtDeceasedContactNDependentId = busNeoSpinBase.Select("entContactTicket.LoadDeceasedContactAndDependent", new object[1] { icdoContactTicket.person_id });
                busPerson lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lbusPerson.icdoPerson.MemberPersonName = "";
                lbusPerson.icdoPerson.person_id = Convert.ToInt32(0);
                iclbDeceasedContactNDependentName.Add(lbusPerson.icdoPerson);
                foreach (DataRow ldtRow in ldtDeceasedContactNDependentId.Rows)
                {
                    if (!(iblnIsFromMSS && Convert.ToInt32(ldtRow["PERSON_ID"]) == icdoContactTicket.person_id))
                    {
                        busPerson lobjPerson = new busPerson() { icdoPerson = new cdoPerson() };
                        lobjPerson.icdoPerson.MemberPersonName = ldtRow["FULL_NAME"].ToString();
                        lobjPerson.icdoPerson.person_id = Convert.ToInt32(ldtRow["PERSON_ID"]);
                        iclbDeceasedContactNDependentName.Add(lobjPerson.icdoPerson);
                    }
                }
                lbusPerson = new busPerson() { icdoPerson = new cdoPerson() };
                lbusPerson.icdoPerson.MemberPersonName = "Other";
                lbusPerson.icdoPerson.person_id = iblnIsFromMSS ? icdoContactTicket.person_id : Convert.ToInt32(-1);
                iclbDeceasedContactNDependentName.Add(lbusPerson.icdoPerson);
            }
            return iclbDeceasedContactNDependentName;
        }
        public ArrayList btnRetriveDeceasedData_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            if (icdoContactTicket.person_id <= 0)
            {
                lobjError = AddError(176, busGlobalFunctions.GetMessageTextByMessageID(176, iobjPassInfo));
                larrList.Add(lobjError);
                return larrList;
            }
            LoadDeceasedContactandDependentNames();
            larrList.Add(this);
            return larrList;
        }
    }
}

