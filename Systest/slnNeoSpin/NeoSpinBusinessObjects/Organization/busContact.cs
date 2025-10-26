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
using System.Linq.Expressions;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busContact : busExtendBase
    {
        #region DefComp Batch

        public string istrIsNewDefCompAgent { get; set; }

        //this property will be used for deferred comp batch process //PIR 233
        private DateTime _idtSeminarDueDate;
        public DateTime idtSeminarDueDate
        {
            get
            {
                if (iclbDeferredCompSeminarSchedule == null)
                    LoadDeferredCompSeminarShcedule();
                _idtSeminarDueDate = DateTime.MinValue;
                foreach (busSeminarSchedule lobjseminarSchedule in iclbDeferredCompSeminarSchedule)
                {
                    if ((lobjseminarSchedule.icdoSeminarSchedule.seminar_date1 != null)
                        && (lobjseminarSchedule.icdoSeminarSchedule.seminar_date1 != DateTime.MinValue))
                    {
                        if ((_idtSeminarDueDate == DateTime.MinValue) || (lobjseminarSchedule.icdoSeminarSchedule.seminar_date1 < _idtSeminarDueDate))
                            _idtSeminarDueDate = lobjseminarSchedule.icdoSeminarSchedule.seminar_date1;
                    }
                }
                if (_idtSeminarDueDate != DateTime.MinValue)
                    _idtSeminarDueDate = _idtSeminarDueDate.AddDays(-15);
                return _idtSeminarDueDate;
            }
        }

        public Collection<busSeminarSchedule> iclbDeferredCompSeminarSchedule { get; set; }
        
        //Load Seminar Schedule Details //PIR 233
        public Collection<busSeminarSchedule> LoadDeferredCompSeminarShcedule()
        {
            DataTable ldtbDefCompSeminarScheduleList =
                busNeoSpinBase.Select("cdoSeminarSchedule.LISTOFSEMINARSCHDULEDETAILS", new object[1] { istrIsNewDefCompAgent });
            iclbDeferredCompSeminarSchedule = GetCollection<busSeminarSchedule>(ldtbDefCompSeminarScheduleList, "icdoSeminarSchedule");
            return iclbDeferredCompSeminarSchedule;
        }

        #endregion

        private busOrgContactAddress _ibusContactPrimaryAddress;
        public busOrgContactAddress ibusContactPrimaryAddress
        {
            get
            {
                return _ibusContactPrimaryAddress;
            }

            set
            {
                _ibusContactPrimaryAddress = value;
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
        private Collection<busSeminarAttendeeDetail> _icolSeminarAttendeeDetail;
        public Collection<busSeminarAttendeeDetail> icolSeminarAttendeeDetail
        {
            get { return _icolSeminarAttendeeDetail; }
            set { _icolSeminarAttendeeDetail = value; }
        }

        public Collection<busScreenNotes> iclbScreenNotes { get; set; }
        //PROD PIR 185
        private Collection<busOrgContact> _iclbOrgContact;

        public Collection<busOrgContact> iclbOrgContact
        {
            get { return _iclbOrgContact; }
            set { _iclbOrgContact = value; }
        }

        public Collection<busOrganization> iclbAffliatedOrgs { get; set; }


        //PIR 235
        public string full_name
        {
            get
            {
                string lstrName = String.Empty;
                if (!String.IsNullOrEmpty(_icdoContact.first_name))
                {
                    lstrName = icdoContact.first_name;
                }
                if (!String.IsNullOrEmpty(_icdoContact.middle_name))
                {
                    lstrName += " " + icdoContact.middle_name;
                }
                if (!String.IsNullOrEmpty(_icdoContact.last_name))
                {
                    lstrName += " " + icdoContact.last_name;
                }
                return lstrName;
            }
        }

        public string lstrContactNameAdr
        {
            get
            {
                if (!(String.IsNullOrEmpty(full_name)))
                    return full_name.ToUpper();
                return full_name;
            }
        }

        private busOrgContactAddress _ibusOrgContactAddress;
        public busOrgContactAddress ibusOrgContactAddress
        {
            get { return _ibusOrgContactAddress; }
            set { _ibusOrgContactAddress = value; }
        }

        public ArrayList SetContactPrimaryAddress(int aintContactAddressId)
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = new utlError();
            if (_ibusOrgContactAddress == null)
            {
                _ibusOrgContactAddress = new busOrgContactAddress();
            }
            _ibusOrgContactAddress.FindOrgContactAddress(aintContactAddressId);
            if (_ibusOrgContactAddress.icdoOrgContactAddress.status_value == busConstant.OrganizationStatusActive)
            {
                _icdoContact.primary_address_id = aintContactAddressId;
                _icdoContact.modified_by = iobjPassInfo.istrUserID;
                _icdoContact.Update();
            }
            else
            {
                lobjError = AddError(busConstant.MessageIdPrimaryAddress, string.Empty);
                larrError.Add(lobjError);
            }
            return larrError;
        }

        public void LoadContactPrimaryAddress()
        {
            if (_ibusContactPrimaryAddress == null)
            {
                _ibusContactPrimaryAddress = new busOrgContactAddress();
            }
            _ibusContactPrimaryAddress.FindOrgContactAddress(_icdoContact.primary_address_id);
        }

        //Check Email is Valid.
        public bool ValidateEmail()
        {
            if (_icdoContact.email_address != null)
            {
                return busGlobalFunctions.IsEmailValid(_icdoContact.email_address); //PIR-18492
            }
            return true;
        }

        // Load Seminar Details for this Contact
        public void LoadSeminarAttendance()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoContact.LOAD_SEMINAR_ATTENDANCE", new object[1] { _icdoContact.contact_id });

            _icolSeminarAttendeeDetail = new Collection<busSeminarAttendeeDetail>();
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busSeminarAttendeeDetail lbusSemAttDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
                lbusSemAttDetail.icdoSeminarAttendeeDetail.LoadData(ldrRow);

                lbusSemAttDetail.ibusSeminarSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.LoadData(ldrRow);

                lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.LoadData(ldrRow);

                lbusSemAttDetail.ibusFacilitator = new busUser { icdoUser = new cdoUser() };
                lbusSemAttDetail.ibusFacilitator.icdoUser.LoadData(ldrRow);
                _icolSeminarAttendeeDetail.Add(lbusSemAttDetail);
            }
        }

        //Load Contact Address for deferred comp training batch //PIR 233
        public void LoadContactAddressForDeferredCompAgent(int aintContactId)
        {
            if (_ibusContactPrimaryAddress == null)
            {
                _ibusContactPrimaryAddress = new busOrgContactAddress();
            }
            if (_icdoContact == null)
            {
                FindContact(aintContactId);
            }
            if (_icdoContact.primary_address_id != 0)
            {
                _ibusContactPrimaryAddress.FindOrgContactAddress(_icdoContact.primary_address_id);
            }
            else
            {
                DataTable ldtbContactAddresslist = Select<cdoOrgContactAddress>(new string[2] { "CONTACT_ID", "status_value" },
                 new object[2] { _icdoContact.contact_id, "ACTV" }, null, "created_date asc");
                if ((ldtbContactAddresslist.Rows.Count > 0) &&
                          (ldtbContactAddresslist != null))
                {
                    _ibusContactPrimaryAddress.FindOrgContactAddress(Convert.ToInt32(ldtbContactAddresslist.Rows[0]["contact_org_address_id"]));
                }
            }
        }
        /// <summary>
        /// Load screen notes based upon screen value contact maintenance CONM PIR 13155
        /// </summary>
        public void LoadContactScreenNotes()
        {
            busScreenNotes lbusScreenNotes = new busScreenNotes();
            iclbScreenNotes = lbusScreenNotes.LoadScreenNotes(busConstant.ContactMaintenanceScreenDifferentiator, icdoContact.contact_id);
        }

        public void LoadOrgContact()
        {
            DataTable ldtbAffiliatedOrgList = Select<cdoOrgContact>(new string[1] { "CONTACT_ID" }, new object[1] { _icdoContact.contact_id }, null, null);
            _iclbOrgContact = GetCollection<busOrgContact>(ldtbAffiliatedOrgList, "icdoOrgContact");
        }

        public void LoadAffiliatedOrgs()
        {
            if (iclbAffliatedOrgs == null)
                iclbAffliatedOrgs = new Collection<busOrganization>();
            if (iclbOrgContact == null)
                LoadOrgContact();

            var lenuDistinctOrg = iclbOrgContact.Select(i => i.icdoOrgContact.org_id).Distinct();

            foreach (int lintOrgID in lenuDistinctOrg)
            {
                busOrganization lbusOrganization = new busOrganization();
                lbusOrganization.FindOrganization(lintOrgID);
                lbusOrganization.istrOrgContactStatus = iclbOrgContact.Any(fetchStatus => fetchStatus.icdoOrgContact.org_id == lintOrgID && fetchStatus.icdoOrgContact.status_value == busConstant.OrgContactStatusActive)
                                        ? busGlobalFunctions.GetDescriptionByCodeValue(215, busConstant.OrgContactStatusActive, iobjPassInfo) 
                                        : busGlobalFunctions.GetDescriptionByCodeValue(215, busConstant.OrgContactStatusInActive, iobjPassInfo);
                iclbAffliatedOrgs.Add(lbusOrganization);
            }
        }

        public DataTable LoadOrgContactDetails()
        {
            return Select("cdoOrgContact.LoadOrgDetails", new object[1] { icdoContact.contact_id });
        }

        public void LoadPrimaryOrgContactAddress()
        {
            if (iclbContactAddress == null)
                LoadContactAddress();
            ibusOrgContactAddress = iclbContactAddress.Where(o => o.primary_address_flag == busConstant.Flag_Yes).FirstOrDefault();
            if (ibusOrgContactAddress.IsNull())
                ibusOrgContactAddress = new busOrgContactAddress { icdoOrgContactAddress = new cdoOrgContactAddress() };
        }

        public ArrayList btnRemoveWSSAccess_Click()
        {
            ArrayList larrList = new ArrayList();
            icdoContact.previous_ndpers_login_id = icdoContact.ndpers_login_id;
            icdoContact.ndpers_login_id = null;
            icdoContact.Update();

            //TODO: Remove the user from PERSLink Group (IBM Secureway)

            EvaluateInitialLoadRules(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
        public override void BeforePersistChanges()
        {
            // pir 7232 Trim the leading and trailing spaces before saving
            //PIR 11071
            if (!icdoContact.first_name.IsNullOrEmpty())
                icdoContact.first_name = icdoContact.first_name.Trim();
            if (!icdoContact.last_name.IsNullOrEmpty())
                icdoContact.last_name = icdoContact.last_name.Trim();
            if (!icdoContact.middle_name.IsNullOrEmpty())
                icdoContact.middle_name = icdoContact.middle_name.Trim();
            base.BeforePersistChanges();
        }
        // PIR 26305
        public bool IsOrgContactActive()
        {
            if (iclbOrgContact == null)
                LoadOrgContact();
            if (icdoContact.status_value == busConstant.OrgContactStatusInActive)
            {
                if (iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusActive).Any())
                    return true;
            }
            return false;
        }
        public bool iblnIsOrgContactActive { get; set; } // PIR 26305
        public bool iblnIsContactAffiliatedWithNoOrg { get; set; }
        public string istrOrgCodeId { get; set; }

        public int btnRemoveOrg_Click(string aintOrgCodeId, int aintContactId)
        {
            if (aintOrgCodeId.IsNull())
                return 0;
            int lintOrgId = Convert.ToInt32(aintOrgCodeId);
            ArrayList larrList = new ArrayList();
            return DBFunction.DBNonQuery("entContact.UpdateOrgContactStatusToInactive", new object[2] { lintOrgId, aintContactId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public ArrayList btnSaveNewOrg_Click(string aintOrgCodeId, int aintContactId)
        {
            ArrayList larrList = new ArrayList();
            
            int lintOrgId = busGlobalFunctions.GetOrgIdFromOrgCode(aintOrgCodeId);
            if (lintOrgId == 0)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(8214, "");
                larrList.Add(lobjError);
                return larrList;
            }
            else
            {
                busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
                if(!lbusOrganization.FindOrganization(lintOrgId))
                {
                    utlError lobjError1 = new utlError();
                    lobjError1 = AddError(8214, "");
                    larrList.Add(lobjError1);
                    return larrList;
                }
                else
                {
                    if(lbusOrganization.icdoOrganization.status_value != busConstant.OrganizationStatusActive)
                    {
                        utlError lobjError2 = new utlError();
                        lobjError2 = AddError(4144, "");
                        larrList.Add(lobjError2);
                        return larrList;
                    }
                }
            }
            busOrgContact lbusOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact() };
            lbusOrgContact = GetCollection<busOrgContact>(Select<cdoOrgContact>(new string[2] { "contact_id", "org_id" }, new object[2] { aintContactId, lintOrgId }, null, null), "icdoOrgContact").FirstOrDefault();
            if (lbusOrgContact.IsNotNull())
            {
                if (lbusOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusInActive)
                {
                    lbusOrgContact.icdoOrgContact.status_value = busConstant.OrgContactStatusActive;
                    lbusOrgContact.icdoOrgContact.Update();
                }
            }
            else
            {
                cdoOrgContact lcdoOrgContact = new cdoOrgContact();
                lcdoOrgContact.org_id = lintOrgId;
                lcdoOrgContact.contact_id = aintContactId;
                lcdoOrgContact.status_id = 215;
                lcdoOrgContact.status_value = busConstant.OrgContactStatusActive;
                lcdoOrgContact.Insert();
            }
            // PIR 26305
            if (icdoContact.status_value == busConstant.OrgContactStatusInActive)
            {
                utlError lobjError2 = new utlError();
                lobjError2 = AddError(10508, "");
                larrList.Add(lobjError2);
                return larrList;
            }
            EvaluateInitialLoadRules(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
        public ArrayList btnInactiveContact(int aintContactId)
        {
            ArrayList larrList = new ArrayList();

            if (icdoContact.contact_id == aintContactId)
            {
                icdoContact.status_value = busConstant.OrgContactStatusInActive;
                icdoContact.Update();
            }

            EvaluateInitialLoadRules(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
    }
}