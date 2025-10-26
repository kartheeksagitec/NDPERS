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
#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busSeminarAttendeeDetail : busExtendBase
    {

        private int iintOrgToBillId;
        private busSeminarSchedule _ibusSeminarSchedule;
        public busSeminarSchedule ibusSeminarSchedule
        {
            get { return _ibusSeminarSchedule; }
            set { _ibusSeminarSchedule = value; }
        }

        private Collection<busSeminarAttendeeDetail> _iclbSeminarAttendeeDetail;
        public Collection<busSeminarAttendeeDetail> iclbSeminarAttendeeDetail
        {
            get
            {
                return _iclbSeminarAttendeeDetail;
            }

            set
            {
                _iclbSeminarAttendeeDetail = value;
            }
        }
        private Collection<busSeminarAttendeeDetail> _iclbGuestSpeakers;
        public Collection<busSeminarAttendeeDetail> iclbGuestSpeakers
        {
            get
            {
                return _iclbGuestSpeakers;
            }

            set
            {
                _iclbGuestSpeakers = value;
            }
        }
        private busOrgContact _ibusOrgContact;
        public busOrgContact ibusOrgContact
        {
            get
            {
                return _ibusOrgContact;
            }

            set
            {
                _ibusOrgContact = value;
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
        private busPerson _ibusOrganizerPerson;
        public busPerson ibusOrganizerPerson
        {
            get
            {
                return _ibusOrganizerPerson;
            }

            set
            {
                _ibusOrganizerPerson = value;
            }
        }
        private busOrganization _ibusOrganizerOrganization;
        public busOrganization ibusOrganizerOrganization
        {
            get
            {
                return _ibusOrganizerOrganization;
            }

            set
            {
                _ibusOrganizerOrganization = value;
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
        ////PIR - 40
        public string istrattendedUnChecked
        {
            get
            {
                if ((_icdoSeminarAttendeeDetail.attended_flag == "N") || (_icdoSeminarAttendeeDetail.attended_flag.IsNullOrEmpty()))
                {
                    return String.Empty;
                }
                return "Y";
            }
        }
        ////PIR - 40
        public string istrGuestFlagUnChecked
        {
            get
            {
                if ((_icdoSeminarAttendeeDetail.guest_flag == "N") || (_icdoSeminarAttendeeDetail.guest_flag.IsNullOrEmpty()))
                {
                    return String.Empty;
                }
                return "Y";
            }
        }
        ////PIR - 40
        public string istrGuestSpeakerFlagUnChecked
        {
            get
            {
                if ((_icdoSeminarAttendeeDetail.guest_speaker_flag == "N") || (_icdoSeminarAttendeeDetail.guest_speaker_flag.IsNullOrEmpty()))
                {
                    return String.Empty;
                }
                return "Y";
            }
        }
        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoSeminarAttendeeDetail.person_id);
        }
        // To Load Person details if organizer is Person
        public void LoadOrganizerPerson()
        {
            if (_ibusOrganizerPerson == null)
            {
                _ibusOrganizerPerson = new busPerson();
            }

            if (ibusSeminarSchedule.ibusContactTicket == null)
                ibusSeminarSchedule.LoadContactTicket();

            _ibusOrganizerPerson.FindPerson(ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.person_id);
        }

        //Load the Latest Associated Employer. If multiple employment, match with sponsor org and get it. If it is not matching, get the employer by earliest eff.date
        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            }
            if (_icdoSeminarAttendeeDetail.person_id != 0)
            {
                if (_ibusPerson == null)
                    LoadPerson();

                if (ibusPerson.icolPersonEmployment == null)
                    ibusPerson.LoadPersonEmployment();

                var lenuActiveEmployments = ibusPerson.icolPersonEmployment.Where(i => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                                                        i.icdoPersonEmployment.start_date, i.icdoPersonEmployment.end_date));
                if (lenuActiveEmployments.Count() > 0)
                {
                    busPersonEmployment lbusPersonEmployment = null;
                    if (lenuActiveEmployments.Count() == 1)
                    {
                        lbusPersonEmployment = lenuActiveEmployments.First();
                    }
                    else if (lenuActiveEmployments.Count() > 1)
                    {
                        if (ibusOrganizerOrganization == null)
                            LoadOrganizerOrganization();
                        //PIR 14229 , repurposed under PIR-11258                           
                        if (!string.IsNullOrEmpty(icdoSeminarAttendeeDetail.org_to_bill_org_code))
                        {
                            busOrganization lbusOrganization = new busOrganization();
                            if(lbusOrganization.FindOrganizationByOrgCode(icdoSeminarAttendeeDetail.org_to_bill_org_code))
                            {
                                lbusPersonEmployment = lenuActiveEmployments.FirstOrDefault(i => i.icdoPersonEmployment.org_id == lbusOrganization.icdoOrganization.org_id);
                            }
                            else
                            {
                                lbusPersonEmployment = lenuActiveEmployments.FirstOrDefault(i => i.icdoPersonEmployment.org_id == ibusOrganizerOrganization.icdoOrganization.org_id);
                            }
                        }
                        else
                            lbusPersonEmployment = lenuActiveEmployments.FirstOrDefault(i => i.icdoPersonEmployment.org_id == ibusOrganizerOrganization.icdoOrganization.org_id);
                    }
                    if (lbusPersonEmployment == null)
                    {
                        //Earliest Date
                        lbusPersonEmployment = lenuActiveEmployments.OrderByDescending(i => i.icdoPersonEmployment.start_date).FirstOrDefault(); //PIR 9542
                    }

                    if (lbusPersonEmployment != null)
                    {
                        if (lbusPersonEmployment.ibusOrganization == null)
                            lbusPersonEmployment.LoadOrganization();
                        ibusOrganization = lbusPersonEmployment.ibusOrganization;
                        iintOrgToBillId = ibusOrganization.icdoOrganization.org_id;
                    }
                }
            }
        }

        //PIR 9542
        public void LoadOrganizationSeminar()
        {
            if (icdoSeminarAttendeeDetail.org_to_bill_id != 0)
            {
                icdoSeminarAttendeeDetail.org_name = busGlobalFunctions.GetOrgNameByOrgID(icdoSeminarAttendeeDetail.org_to_bill_id);
                icdoSeminarAttendeeDetail.org_to_bill_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoSeminarAttendeeDetail.org_to_bill_id);
            }
        }

        // To Load organization details if organizer is organization
        public void LoadOrganizerOrganization()
        {
            if (_ibusOrganizerOrganization.IsNull()) _ibusOrganizerOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (ibusSeminarSchedule.IsNull()) LoadSeminarSchedule(); // PROD PIR ID 6465 -- Null check handled.
            if (ibusSeminarSchedule.ibusContactTicket.IsNull()) ibusSeminarSchedule.LoadContactTicket();

            _ibusOrganizerOrganization.FindOrganization(ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.org_id);
        }

        //Changed PIR 279-283
        public void LoadContact()
        {
            if (_ibusContact == null)
            {
                _ibusContact = new busContact();
            }
            _ibusContact.FindContact(_icdoSeminarAttendeeDetail.contact_id);

        }
        public void LoadSeminarSchedule()
        {
            if (_ibusSeminarSchedule == null)
            {
                _ibusSeminarSchedule = new busSeminarSchedule();
            }
            _ibusSeminarSchedule.FindSeminarSchedule(icdoSeminarAttendeeDetail.seminar_schedule_id);
        }

        public void LoadSeminarFacilitatorName()
        {
            if (_ibusFacilitator == null)
            {
                _ibusFacilitator = new busUser();
            }
            _ibusFacilitator.FindUser(_ibusSeminarSchedule.icdoSeminarSchedule.facilitator);
        }

        private string _seminar_sponsor;
        public string seminar_sponsor
        {
            get
            {
                return _seminar_sponsor;
            }
        }

        public void LoadSponsorName()
        {
            //PIR - 279 - 283            
            if (ibusOrganizerOrganization == null)
                LoadOrganizerOrganization();
            _seminar_sponsor = _ibusOrganizerOrganization.icdoOrganization.org_name;
        }

        //PIR 279-283
        public void LoadSeminarAttendeeDetail()
        {
            DataTable ldtbAttendeeList = Select<cdoSeminarAttendeeDetail>(
                new string[2] { "seminar_schedule_id", "guest_speaker_flag" },
                new object[2] { _icdoSeminarAttendeeDetail.seminar_schedule_id, "N" }, null, null);
            _iclbSeminarAttendeeDetail = GetCollection<busSeminarAttendeeDetail>(ldtbAttendeeList, "icdoSeminarAttendeeDetail");
            foreach (busSeminarAttendeeDetail lobjSeminarAttendeeDetail in _iclbSeminarAttendeeDetail)
            {
                lobjSeminarAttendeeDetail.LoadContact();
                lobjSeminarAttendeeDetail.LoadPerson();
                lobjSeminarAttendeeDetail.LoadDisplayAttendeeName();
                lobjSeminarAttendeeDetail.LoadOrganizationSeminar(); //PIR 9542
            }
        }
        public void LoadSeminarAttendeeDetailForESS()
        {
            DataTable ldtbAttendeeList = Select<cdoSeminarAttendeeDetail>(
                           new string[1] { "seminar_schedule_id"},
                           new object[1] { _icdoSeminarAttendeeDetail.seminar_schedule_id }, null, null);
            _iclbSeminarAttendeeDetail = GetCollection<busSeminarAttendeeDetail>(ldtbAttendeeList, "icdoSeminarAttendeeDetail");
            foreach (busSeminarAttendeeDetail lobjSeminarAttendeeDetail in _iclbSeminarAttendeeDetail)
            {
                lobjSeminarAttendeeDetail.LoadContact();
                lobjSeminarAttendeeDetail.LoadPerson();
                lobjSeminarAttendeeDetail.LoadDisplayAttendeeName();
                lobjSeminarAttendeeDetail.LoadOrganizationSeminar(); //PIR 9542
            }
        }
        public bool iblnIsFromESS { get; set; }

        public void LoadGuestSpeakers()
        {
            DataTable ldbtGuestSpeakersList = busNeoSpinBase.Select("cdoSeminarAttendeeDetail.GetGuestSpeakers", new object[1] { _icdoSeminarAttendeeDetail.seminar_schedule_id });

            _iclbGuestSpeakers = GetCollection<busSeminarAttendeeDetail>(ldbtGuestSpeakersList, "icdoSeminarAttendeeDetail");
            foreach (busSeminarAttendeeDetail lobjSeminarAttendeeDetail in _iclbGuestSpeakers)
            {
                lobjSeminarAttendeeDetail.LoadContact();
                lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.istrAttendeeDisplayName = lobjSeminarAttendeeDetail.ibusContact.full_name;
            }
        }


        public override void AfterPersistChanges()
        {
            //Post Message to Portal Message Board
            if (iblnIsFromPortal)
            {
               if ((ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypePayrollConference ||
                    ibusSeminarSchedule.icdoSeminarSchedule.seminar_type_value == busConstant.SeminarTypeWellnessForum)
                   && iintLoginOrgId > 0 && icdoSeminarAttendeeDetail.contact_id > 0)
                {
                    //PIR-14667, repurposed under PIR-11258
                    string lstrPrioityValue = string.Empty;
                    busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(18, iobjPassInfo, ref lstrPrioityValue), ibusSeminarSchedule.icdoSeminarSchedule.seminar_name,
                        ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.ToString("MM/dd/yyyy")), lstrPrioityValue,
                        aintOrgID: iintLoginOrgId, aintContactID: icdoSeminarAttendeeDetail.contact_id);
                }

            }
            base.AfterPersistChanges();
            LoadSeminarAttendeeDetail();
            LoadGuestSpeakers();
            //LoadOrganization();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (icdoSeminarAttendeeDetail.org_to_bill_org_code.IsNotNullOrEmpty())
            {
                ibusOrgToBillOrganization = new busOrganization();
                ibusOrgToBillOrganization.FindOrganizationByOrgCode(icdoSeminarAttendeeDetail.org_to_bill_org_code);
                icdoSeminarAttendeeDetail.org_to_bill_id = ibusOrgToBillOrganization.icdoOrganization.org_id;
            }
            //else//PIR 7495: If Bill to Org is empty/removed , Default to zero .
            //{
            //    icdoSeminarAttendeeDetail.org_to_bill_id = 0;
            //}

            //Reload the Organization (Validation (paym ent method IDB) 

            if (icdoSeminarAttendeeDetail.person_id > 0)
            {
                if (icdoSeminarAttendeeDetail.seminar_attendee_detail_id == 0)
                {
                    if (IsMemberhavingTwoOpenEmployments())
                    {
                        LoadOrganization();
                    }
                    else
                    {
                        LoadOrganization();
                        icdoSeminarAttendeeDetail.org_to_bill_id = iintOrgToBillId;
                        LoadOrganizationSeminar();
                    }
                }
                else
                {               //PIR-14229  -- Repurposed under PIR-11258
                    //if (icdoSeminarAttendeeDetail.org_to_bill_org_code.IsNullOrEmpty())
                    //{
                        if (IsMemberhavingTwoOpenEmployments())
                        {
                            LoadOrganization();
                        }
                        else
                        {
                            LoadOrganization();
                            icdoSeminarAttendeeDetail.org_to_bill_id = iintOrgToBillId;
                            LoadOrganizationSeminar();
                        }
					//}                 
                }
            }
            if (icdoSeminarAttendeeDetail.guest_speaker_flag == busConstant.Flag_Yes) // PROD PIR ID 6785 - Assigned in Before Validate
                icdoSeminarAttendeeDetail.contact_id = icdoSeminarAttendeeDetail.guest_speaker_contact_id;
            base.BeforeValidate(aenmPageMode);
        }
        
        public override void BeforePersistChanges()
        {
            LoadDisplayAttendeeName();
            _icdoSeminarAttendeeDetail.seminar_schedule_id = ibusSeminarSchedule.icdoSeminarSchedule.seminar_schedule_id;
            base.BeforePersistChanges();
        }

        //Rule if seminar Cancelled checkbox cancel 
        //checkbox is checked or the completion date is less than todays date
        //save button should be invisible
        public bool DisableSaveButton()
        {
            if (ibusSeminarSchedule == null)
                LoadSeminarSchedule();
            if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_cancel_flag == "Y")
            {
                return true;
            }
            // PIR 9452 -- As discussed with Maik, no such visibility check needed
            //else if (_ibusSeminarSchedule.icdoSeminarSchedule.seminar_date1.AddDays(15) < DateTime.Now) 
            //{
            //    return true;
            //}
            return false;
        }

        //Rule to check if the logged in user perslink id 
        //and seminar attendee perslink id are same
        //throw an error
        public bool CheckLoggedPerslinkId()
        {
            if (_icdoSeminarAttendeeDetail.person_id != 0)
            {
                if (LoadLoggedinUserIdPerslinkID == _icdoSeminarAttendeeDetail.person_id)
                {
                    return true;
                }
            }
            return false;
        }

        public int LoadLoggedinUserIdPerslinkID
        {
            get
            {
                int LoggedInUserPERSLinkID = 0;
                DataTable ldtbPersLinkId = Select<cdoUser>(new string[1] { "user_serial_id" },
                                                            new object[1] { iobjPassInfo.iintUserSerialID }, null, null);
                if (ldtbPersLinkId.Rows.Count > 0)
                {
                    if (!(String.IsNullOrEmpty((ldtbPersLinkId.Rows[0]["person_id"]).ToString())))
                    {
                        LoggedInUserPERSLinkID = Convert.ToInt32(ldtbPersLinkId.Rows[0]["person_id"]);
                    }
                }
                return LoggedInUserPERSLinkID;
            }
        }

        # region Correspondence
        public override busBase GetCorPerson()
        {
            return _ibusPerson;
        }

        # endregion

        //        PIR 279-283
        public void LoadDisplayAttendeeName()
        {
            if (_icdoSeminarAttendeeDetail.contact_id != 0)
            {
                if (ibusContact == null)
                    LoadContact();
                _icdoSeminarAttendeeDetail.istrAttendeeDisplayName = _ibusContact.icdoContact.ContactName;

            }
            else if (_icdoSeminarAttendeeDetail.person_id != 0)
            {
                if (ibusPerson == null)
                    LoadPerson();
                _icdoSeminarAttendeeDetail.istrAttendeeDisplayName = _ibusPerson.icdoPerson.PersonName;
            }
            else if (_icdoSeminarAttendeeDetail.attendee_name != null)
            {
                _icdoSeminarAttendeeDetail.istrAttendeeDisplayName = _icdoSeminarAttendeeDetail.attendee_name;
            }
        }

        public busOrganization ibusOrgToBillOrganization { get; set; }
        public void LoadOrgToBillOrganization()
        {
            if (ibusOrgToBillOrganization == null)
            {
                ibusOrgToBillOrganization = new busOrganization();
            }
            ibusOrgToBillOrganization.FindOrganization(icdoSeminarAttendeeDetail.org_to_bill_id);
        }

        public utlCollection<cdoSeminarAttendeeDetailRetirementType> iclcRetirementType { get; set; }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoSeminarAttendeeDetailRetirementType)
            {
                cdoSeminarAttendeeDetailRetirementType lcdoRetirementType = (cdoSeminarAttendeeDetailRetirementType)aobjBase;
                lcdoRetirementType.seminar_attendee_detail_id = icdoSeminarAttendeeDetail.seminar_attendee_detail_id;
            }
        }

        public void LoadRetirementType()
        {
            iclcRetirementType = new utlCollection<cdoSeminarAttendeeDetailRetirementType>();
            iclcRetirementType = GetCollection<cdoSeminarAttendeeDetailRetirementType>(
                new string[1] { "seminar_attendee_detail_id" }, new object[1] { icdoSeminarAttendeeDetail.seminar_attendee_detail_id }, null, null);
        }

        //UCS 11 - MSS Seminar Attendee Detail Screen - Org To Bill (Droplist of Active Employers)
        public Collection<cdoOrganization> LoadActiveEmployers()
        {
            Collection<cdoOrganization> lclbActiveEmployers = new Collection<cdoOrganization>();

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.iclbActivePersonEmployment == null)
                ibusPerson.LoadActivePersonEmployment();
            foreach (busPersonEmployment lbusPersonEmployment in ibusPerson.iclbActivePersonEmployment)
            {
                lbusPersonEmployment.LoadOrganization();
                lclbActiveEmployers.Add(lbusPersonEmployment.ibusOrganization.icdoOrganization);
            }

            return lclbActiveEmployers;
        }

        public bool IsCentralPayrollEmploymentExistsForIDB()
        {
            if (ibusOrganization == null)
                LoadOrganization();

            if (ibusOrganization.icdoOrganization.org_id > 0 && ibusOrganization.icdoOrganization.central_payroll_flag == busConstant.Flag_Yes)
                return true;
            return false;
        }

        //PIR 9542 
        public bool GetOrgCountByOrgCode()
        {
            if (ibusOrganization == null)
                LoadOrganization();

            if (icdoSeminarAttendeeDetail.org_to_bill_org_code.IsNotNull())
            {
                int lintOrgCount = 0;
                lintOrgCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoOrganization.GetOrgCountByOrgCode", new object[1] { icdoSeminarAttendeeDetail.org_to_bill_org_code },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintOrgCount > 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        //PIR 9542
        public bool IsMemberhavingTwoOpenEmployments()
        {
            if (_ibusPerson == null)
                LoadPerson();
            DataTable ldtPersonEmplooymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                                                     new object[1] { this.ibusPerson.icdoPerson.person_id }
                                                     , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtPersonEmplooymentCount.Rows.Count > 1)
                return true;
            else
                return false;
        }

        #region UCS 11 Addendum
        public bool iblnIsFromPortal { get; set; }

        public Collection<busSeminarAttendeePaymentAllocation> iclbSeminarAttendeePaymentAllocation { get; set; }
        public void LoadSeminarAttendeePaymentAllocation()
        {
            if (iclbSeminarAttendeePaymentAllocation == null)
                iclbSeminarAttendeePaymentAllocation = new Collection<busSeminarAttendeePaymentAllocation>();

            DataTable ldtbList = Select<cdoSeminarAttendeePaymentAllocation>(
              new string[1] { "SEMINAR_ATTENDEE_DETAIL_ID" },
              new object[1] { icdoSeminarAttendeeDetail.seminar_attendee_detail_id }, null, null);
            iclbSeminarAttendeePaymentAllocation = GetCollection<busSeminarAttendeePaymentAllocation>(ldtbList, "icdoSeminarAttendeePaymentAllocation");
        }

        public decimal TotalPaidFeeAmount { get; set; }
        public void LoadTotalPaidFeeAmount()
        {
            if (iclbSeminarAttendeePaymentAllocation == null)
                LoadSeminarAttendeePaymentAllocation();

            TotalPaidFeeAmount = iclbSeminarAttendeePaymentAllocation.Sum(i => i.icdoSeminarAttendeePaymentAllocation.applied_amount);
        }

        #endregion

        public override busBase GetCorOrganization()
        {
            if(_ibusContact == null)
                LoadContact();
            if (ibusOrganization == null)
                LoadOrganization();
            ibusOrganization.ibusContact = _ibusContact;
            _ibusContact.LoadContactPrimaryAddress();
            ibusOrganization.ibusOrgContactPrimaryAddress = _ibusContact.ibusContactPrimaryAddress;
            return ibusOrganization;
        }
        public bool IsPaymentMethodSelected()
        {
            if (icdoSeminarAttendeeDetail.person_id != 0)
            {
                return icdoSeminarAttendeeDetail.payment_method_value.IsNull() ? true: false;
            }
            if (icdoSeminarAttendeeDetail.attendee_name.IsNotNull())
            {
                return icdoSeminarAttendeeDetail.payment_method_value.IsNull() ? true: false;
            }
            return false;
        }
        public int iintLoginOrgId { get; set; }   //PIR-14667, repurposed under PIR-11258
    }
    
}

