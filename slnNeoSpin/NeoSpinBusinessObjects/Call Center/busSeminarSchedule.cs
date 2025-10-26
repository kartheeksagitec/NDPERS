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
    public partial class busSeminarSchedule : busExtendBase
    {
        //Custom Property Attendees plus Guests used for screen
        private int _NoOfAttendees;
        public int NoOfAttendees
        {
            get { return _NoOfAttendees; }
            set { _NoOfAttendees = value; }
        }        
        public int NoOfWebcastAttendees
        {
            get {                
                    if (iclbSeminarAttendeeDetail != null)
                    {
                        if (iclbSeminarAttendeeDetail == null)
                            LoadSeminarAttendeeDetail();
                        return iclbSeminarAttendeeDetail.Sum(i => i.icdoSeminarAttendeeDetail.no_of_webcast_attendees);
                    }
                return 0;
            }
        }
        //PIR-14230 
        public int NoOfOnsiteAttendees
        {
            get
            {
                if (iclbSeminarAttendeeDetail != null)
                {
                    return 
                        iclbSeminarAttendeeDetail.Count(i => i.icdoSeminarAttendeeDetail.person_id != 0);  // PIR 14230 - including guest count in Onsite Attendee count
                }
                return 0;
            }
        }
        //PIR-14230
        public int NoOfOnsiteGuest
        {
            get
            {
                if (iclbSeminarAttendeeDetail != null)
                {
                    return iclbSeminarAttendeeDetail.Sum(i => i.icdoSeminarAttendeeDetail.no_of_guest_attending) +
                         iclbSeminarAttendeeDetail.Count(i => i.icdoSeminarAttendeeDetail.guest_flag == busConstant.Flag_Yes);
                }
                return 0;
            }
        }

        // used for correspondence
        public int NoOfGuests
        {
            get
            {
                if (_icdoSeminarSchedule != null) 
                {
                    if (iclbSeminarAttendeeDetail == null)
                        LoadSeminarAttendeeDetail();
                    return iclbSeminarAttendeeDetail.Sum(i => i.icdoSeminarAttendeeDetail.no_of_guest_attending) +
                            iclbSeminarAttendeeDetail.Count(i => i.icdoSeminarAttendeeDetail.guest_flag == busConstant.Flag_Yes);
                }
                return 0;
            }
        }

        //PIR 9154
        public int NoOfGuestsUI
        {
            get 
            {
                if (iclbSeminarAttendeeDetail != null)
                {
                    return iclbSeminarAttendeeDetail.Sum(i => i.icdoSeminarAttendeeDetail.no_of_guest_attending) +
                            iclbSeminarAttendeeDetail.Count(i => i.icdoSeminarAttendeeDetail.guest_flag == busConstant.Flag_Yes);
                }
                return 0;
            }
        }

        // used for correspondence
        public int TotalNoOfAttendees
        {
            get
            {
                return NoOfOnlyAttendees + NoOfGuests;
            }
        }

        // used for correspondence
        public int NoOfOnlyAttendees
        {
            get
            {
                if (_icdoSeminarSchedule != null)
                {
                    if (iclbSeminarAttendeeDetail == null)
                        LoadSeminarAttendeeDetail();
                    return _iclbSeminarAttendeeDetail.Count;
                }
                return 0;
            }
        }

        private decimal _TotalAmount;
        public decimal TotalAmount
        {
            get { return _TotalAmount; }
            set { _TotalAmount = value; }
        }

        //Custom property for display in MSSHome
        public string istrSeminarDate
        {
            get
            {
                string sem_time = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(icdoSeminarSchedule.seminar_date1_start_time_id, icdoSeminarSchedule.seminar_date1_start_time_value);
                string format = "MMMM d yyyy";    // Use this format
                string sem_date = icdoSeminarSchedule.seminar_date1.ToString(format);
                string lstrSeminarDate = "(" + sem_date + " , " + sem_time + ") ";
                return lstrSeminarDate;
            }
        }
        //Custom Property for Correpondence
        public DateTime SeminarStartDateDay1Minus30Days
        {
            get
            {
                if (_icdoSeminarSchedule != null)
                {
                    if (_icdoSeminarSchedule.seminar_date1 != DateTime.MinValue)
                    {
                        return _icdoSeminarSchedule.seminar_date1.AddDays(-30);
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        //Custom Property for Correpondence
        public DateTime SeminarStartDateDay1Minus15Days
        {
            get
            {
                if (_icdoSeminarSchedule != null)
                {
                    if (_icdoSeminarSchedule.seminar_date1 != DateTime.MinValue)
                    {
                        return _icdoSeminarSchedule.seminar_date1.AddDays(-15);
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
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

        public bool FindSeminarScheduleByContactTicket(int Aintcontactticketid)
        {
            if (_icdoSeminarSchedule == null)
            {
                _icdoSeminarSchedule = new cdoSeminarSchedule();
            }
            DataTable ldtb = Select<cdoSeminarSchedule>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoSeminarSchedule.LoadData(ldtb.Rows[0]);
                return true;
            }
            return false;

        }
        public void LoadSeminarAttendeeDetail()
        {
            _TotalAmount = 0.00M;
            // sorting order done for PIR - 287
            DataTable ldtbList = Select<cdoSeminarAttendeeDetail>(
                new string[1] { "seminar_schedule_id" },
                new object[1] { icdoSeminarSchedule.seminar_schedule_id }, null, "attendee_name asc");
            _iclbSeminarAttendeeDetail = GetCollection<busSeminarAttendeeDetail>(ldtbList, "icdoSeminarAttendeeDetail");

            //Filter the Guest Speaker
            _iclbSeminarAttendeeDetail = _iclbSeminarAttendeeDetail.Where(i => i.icdoSeminarAttendeeDetail.guest_speaker_flag == busConstant.Flag_No ||
                                                                               i.icdoSeminarAttendeeDetail.guest_speaker_flag == null).ToList().ToCollection();

            //Set Count of attendees and amount
            _NoOfAttendees = _iclbSeminarAttendeeDetail.Count;
            _TotalAmount = _NoOfAttendees * _icdoSeminarSchedule.attendee_fee;
            foreach (busSeminarAttendeeDetail lbusSemAttDtl in _iclbSeminarAttendeeDetail)
            {
                lbusSemAttDtl.LoadPerson();
                lbusSemAttDtl.LoadOrganizationSeminar(); //PIR 9542
                lbusSemAttDtl.LoadDisplayAttendeeName();
                lbusSemAttDtl.LoadSeminarAttendeePaymentAllocation();
                lbusSemAttDtl.LoadTotalPaidFeeAmount();
            }

            //Sorting of names

            utlSortComparer lobjComparer = new utlSortComparer();
            lobjComparer.istrSortExpression = "icdoSeminarAttendeeDetail.istrAttendeeDisplayName";
            ArrayList larrBase = new ArrayList(_iclbSeminarAttendeeDetail);
            larrBase.Sort(lobjComparer);
            _iclbSeminarAttendeeDetail.Clear();
            foreach (object lobjTemp in larrBase)
            {
                _iclbSeminarAttendeeDetail.Add((busSeminarAttendeeDetail)lobjTemp);
            }

        }

        //Systest PIR 2345: Filter other Members (Not to show in MSS Attendee Summary)
        public void LoadMSSSeminarAttendeeDetail(int aintPersonID)
        {
            _TotalAmount = 0.00M;
            // sorting order done for PIR - 287
            DataTable ldtbList = Select<cdoSeminarAttendeeDetail>(
                new string[1] { "seminar_schedule_id" },
                new object[1] { icdoSeminarSchedule.seminar_schedule_id }, null, "attendee_name asc");
            _iclbSeminarAttendeeDetail = GetCollection<busSeminarAttendeeDetail>(ldtbList, "icdoSeminarAttendeeDetail");

            //Filter the Guest Speaker
            _iclbSeminarAttendeeDetail = _iclbSeminarAttendeeDetail.Where(i => i.icdoSeminarAttendeeDetail.guest_speaker_flag == busConstant.Flag_No ||
                                                                               i.icdoSeminarAttendeeDetail.guest_speaker_flag == null).ToList().ToCollection();

            //Set Count of attendees and amount
            _NoOfAttendees = _iclbSeminarAttendeeDetail.Count;
            _TotalAmount = _NoOfAttendees * _icdoSeminarSchedule.attendee_fee;

            //Systest PIR 2345: Filter other Members (Not to show in MSS Attendee Summary)
            _iclbSeminarAttendeeDetail = _iclbSeminarAttendeeDetail.Where(i => i.icdoSeminarAttendeeDetail.person_id == aintPersonID).ToList().ToCollection();
            foreach (busSeminarAttendeeDetail lbusSemAttDtl in _iclbSeminarAttendeeDetail)
            {
                lbusSemAttDtl.LoadPerson();
                lbusSemAttDtl.LoadOrganization();
                lbusSemAttDtl.LoadDisplayAttendeeName();
                lbusSemAttDtl.LoadSeminarAttendeePaymentAllocation();
                lbusSemAttDtl.LoadTotalPaidFeeAmount();
            }
        }

        public void LoadESSSeminarAttendeeDetail()
        {
            _TotalAmount = 0.00M;
            // sorting order done for PIR - 287
            DataTable ldtbList = Select<cdoSeminarAttendeeDetail>(
                new string[1] { "seminar_schedule_id" },
                new object[1] { icdoSeminarSchedule.seminar_schedule_id }, null, "attendee_name asc");
            Collection<busSeminarAttendeeDetail> lclbSeminarAttendee = new Collection<busSeminarAttendeeDetail>();
            lclbSeminarAttendee = GetCollection<busSeminarAttendeeDetail>(ldtbList, "icdoSeminarAttendeeDetail");
            //Filter the Guest Speaker
            lclbSeminarAttendee = lclbSeminarAttendee.Where(i => i.icdoSeminarAttendeeDetail.guest_speaker_flag == busConstant.Flag_No ||
                                                                               i.icdoSeminarAttendeeDetail.guest_speaker_flag == null).ToList().ToCollection();
            //prod pir 7844
            busOrganization lobjOrganization = new busOrganization();
            lobjOrganization.FindOrganization(icdoSeminarSchedule.org_id);
            lobjOrganization.LoadOrgContact();

            DataTable ldtEmployees = Select("cdoPersonEmployment.LoadOpenEmploymentForOrgFromESS", new object[1] { icdoSeminarSchedule.org_id });

            _iclbSeminarAttendeeDetail = new Collection<busSeminarAttendeeDetail>();
            foreach (busSeminarAttendeeDetail lbusAttendee in lclbSeminarAttendee)
            {
                if ((lbusAttendee.icdoSeminarAttendeeDetail.contact_id > 0 &&
                    lobjOrganization.iclbOrgContact.Where(o => o.icdoOrgContact.contact_id == lbusAttendee.icdoSeminarAttendeeDetail.contact_id).Any()) ||
                    (lbusAttendee.icdoSeminarAttendeeDetail.person_id > 0 &&
                    ldtEmployees.AsEnumerable().Where(o => o.Field<int>("person_id") == lbusAttendee.icdoSeminarAttendeeDetail.person_id).Any()))
                {
                    _iclbSeminarAttendeeDetail.Add(lbusAttendee);
                }
            }

            //Set Count of attendees and amount
            _NoOfAttendees = _iclbSeminarAttendeeDetail.Count;
            _TotalAmount = _NoOfAttendees * _icdoSeminarSchedule.attendee_fee;
            foreach (busSeminarAttendeeDetail lbusSemAttDtl in _iclbSeminarAttendeeDetail)
            {
                lbusSemAttDtl.LoadPerson();
                lbusSemAttDtl.LoadOrganization();
                lbusSemAttDtl.LoadDisplayAttendeeName();
                lbusSemAttDtl.LoadSeminarAttendeePaymentAllocation();
                lbusSemAttDtl.LoadTotalPaidFeeAmount();
            }

            //Sorting of names

            utlSortComparer lobjComparer = new utlSortComparer();
            lobjComparer.istrSortExpression = "icdoSeminarAttendeeDetail.istrAttendeeDisplayName";
            ArrayList larrBase = new ArrayList(_iclbSeminarAttendeeDetail);
            larrBase.Sort(lobjComparer);
            _iclbSeminarAttendeeDetail.Clear();
            foreach (object lobjTemp in larrBase)
            {
                _iclbSeminarAttendeeDetail.Add((busSeminarAttendeeDetail)lobjTemp);
            }

        }

        public void LoadGuestSpeakers()
        {
            DataTable ldbt = busNeoSpinBase.Select("cdoSeminarAttendeeDetail.GetGuestSpeakers", new object[1] { _icdoSeminarSchedule.seminar_schedule_id });

            _iclbGuestSpeakers = GetCollection<busSeminarAttendeeDetail>(ldbt, "icdoSeminarAttendeeDetail");
            foreach (busSeminarAttendeeDetail lobjseminarAttendeeDetail in _iclbGuestSpeakers)
            {
                lobjseminarAttendeeDetail.LoadDisplayAttendeeName();
            }
        }

        public busContactTicket ibusContactTicket { get; set; }
        public void LoadContactTicket()
        {
            if (ibusContactTicket == null)
                ibusContactTicket = new busContactTicket();

            ibusContactTicket.FindContactTicket(icdoSeminarSchedule.contact_ticket_id);
        }

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            if (ahstParam["aint_seminar_id"] != null)
            {
                DataTable ldtbList = busNeoSpinBase.Select<cdoSeminarSchedule>(new string[1] { "seminar_schedule_id" }
                                            , new object[1] { Convert.ToInt32(ahstParam["aint_seminar_id"]) }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    int lintSeminarValidityDays = 0;
                    string lstrSeminarType = ldtbList.Rows[0]["seminar_type_value"].ToString();
                    DateTime ldtSeminarDate = Convert.ToDateTime(ldtbList.Rows[0]["seminar_date1"]);
                    if (lstrSeminarType == busConstant.SeminarTypeOnSiteCounseling || lstrSeminarType == busConstant.SeminarTypePrepEmployer
                        || lstrSeminarType == busConstant.SeminarTypePrepNDPERS)
                    {
                        if (!string.IsNullOrEmpty(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypeCommonDays, iobjPassInfo)))
                            lintSeminarValidityDays = -1 * Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypePayrollConferenceDays, iobjPassInfo));
                        if (ldtSeminarDate.AddDays(lintSeminarValidityDays).Date < DateTime.Now.Date)
                        {
                            utlError lobjError = AddError(3078, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    else if (lstrSeminarType == busConstant.SeminarTypeWellnessForum) 
                    {
                        if (!string.IsNullOrEmpty(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypeWellnessForumDays, iobjPassInfo)))
                            lintSeminarValidityDays = -1 * Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypePayrollConferenceDays, iobjPassInfo));
                        if (ldtSeminarDate.AddDays(lintSeminarValidityDays).Date < DateTime.Now.Date)
                        {
                            utlError lobjError = AddError(3080, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                    else if (lstrSeminarType == busConstant.SeminarTypePayrollConference) //PROD PIR 7881 : added code value for days to be subtracted
                    {
                        if (!string.IsNullOrEmpty(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypePayrollConferenceDays, iobjPassInfo)))
                            lintSeminarValidityDays = -1 * Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SeminarTypePayrollConferenceDays, iobjPassInfo));
                        if (ldtSeminarDate.AddDays(lintSeminarValidityDays).Date < DateTime.Now.Date)
                        {
                            utlError lobjError = AddError(3080, "");
                            larrErrors.Add(lobjError);
                        }
                    }
                }
            }
            return larrErrors;
        }
        // PIR 6927
        public busPerson ibusPerson { get; set; }
        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoSeminarSchedule.attendee_person_id);
            return ibusPerson;
        }
    }
}
