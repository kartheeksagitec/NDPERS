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
using Sagitec.DataObjects;
using System.Collections.Generic;
using Sagitec.CustomDataObjects;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busOrganization : busExtendBase
    {
        private bool lblnIsOrgCodeExceedRange;
        //This property used in corORG-1100.xml
        private DateTime _PlanParticipationDateforDeffComp;
        public DateTime PlanParticipationDateforDeffComp
        {
            get
            {
                int lintDeffCompPlanID = busConstant.PlanIdDeferredCompensation;
                DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.GetOrgPlanParticipationDate", new object[2] { _icdoOrganization.org_id, lintDeffCompPlanID });
                if (ldtbList.Rows.Count > 0)
                {
                    _PlanParticipationDateforDeffComp = Convert.ToDateTime(ldtbList.Rows[0]["PARTICIPATION_START_DATE"]);
                }
                return _PlanParticipationDateforDeffComp;
            }
        }
        //This property used in corORG-1104.xml
        private DateTime _PlanParticipationDateforGroupHealth;
        public DateTime PlanParticipationDateforGroupHealth
        {
            get
            {
                int lintPlanIdGroupHealth = busConstant.PlanIdGroupHealth;
                DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.GetOrgPlanParticipationDate", new object[2] { _icdoOrganization.org_id, lintPlanIdGroupHealth });
                if (ldtbList.Rows.Count > 0)
                {
                    _PlanParticipationDateforGroupHealth = Convert.ToDateTime(ldtbList.Rows[0]["PARTICIPATION_START_DATE"]);
                }
                return _PlanParticipationDateforGroupHealth;
            }
        }
        //PIR - 346
        private Collection<busNotes> _iclbNotes;
        public Collection<busNotes> iclbNotes
        {
            get
            {
                return _iclbNotes;
            }

            set
            {
                _iclbNotes = value;
            }
        }
        private Collection<busPersonEmployment> _iclbPersonEmployment;

        public Collection<busPersonEmployment> iclbPersonEmployment
        {
            get { return _iclbPersonEmployment; }
            set { _iclbPersonEmployment = value; }
        }

        public string  iProviderName { get; set; }
        public string istrOrgContactStatus { get; set; }
        private Collection<busOrgContact> _iclbOrgContact;
        public Collection<busOrgContact> iclbOrgContact
        {
            get { return _iclbOrgContact; }
            set { _iclbOrgContact = value; }
        }
        private busOrgContact _ibusOrgContact;
        public busOrgContact ibusOrgContact
        {
            get { return _ibusOrgContact; }
            set { _ibusOrgContact = value; }
        }
        private busContact _ibusContact;
        public busContact ibusContact
        {
            get { return _ibusContact; }
            set { _ibusContact = value; }
        }
        private Collection<busOrgBank> _iclbOrgBank;
        public Collection<busOrgBank> iclbOrgBank
        {
            get { return _iclbOrgBank; }
            set { _iclbOrgBank = value; }
        }

        public Collection<busSeminarSchedule> icolSeminarSchedule { get; set; }

        private Collection<busOrgContactAddress> _iclbOrgAddress;
        public Collection<busOrgContactAddress> iclbOrgAddress
        {
            get
            {
                return _iclbOrgAddress;
            }

            set
            {
                _iclbOrgAddress = value;
            }
        }

        private busOrgContactAddress _ibusOrgContactPrimaryAddress;
        public busOrgContactAddress ibusOrgContactPrimaryAddress
        {
            get
            {
                return _ibusOrgContactPrimaryAddress;
            }

            set
            {
                _ibusOrgContactPrimaryAddress = value;
            }
        }
        private busOrgPlan _ibusOrgPlan;
        public busOrgPlan ibusOrgPlan
        {
            get
            {
                return _ibusOrgPlan;
            }
            set
            {
                _ibusOrgPlan = value;
            }
        }

        private busOrgContactAddress _ibusOrgPrimaryAddress;
        public busOrgContactAddress ibusOrgPrimaryAddress
        {
            get
            {
                return _ibusOrgPrimaryAddress;
            }

            set
            {
                _ibusOrgPrimaryAddress = value;
            }
        }

        private Collection<busContactTicket> _icolOrganizationContactTicket;
        public Collection<busContactTicket> icolOrganizationContactTicket
        {
            get { return _icolOrganizationContactTicket; }
            set { _icolOrganizationContactTicket = value; }
        }
        private string _istrSuppressWarning;

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
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
        //prod pir 5574 : new collection for ESS
        public Collection<busOrgContact> iclbESSOrgContact { get; set; }

        public bool FindOrganizationByOrgCode(string AintOrgCodeId)
        {
            if (_icdoOrganization == null)
            {
                _icdoOrganization = new cdoOrganization();
            }
            DataTable ldtbOrganization = Select<cdoOrganization>(new string[1] { "org_code" },
                  new object[1] { AintOrgCodeId }, null, null);
            if (ldtbOrganization.Rows.Count > 0)
            {
                _icdoOrganization.LoadData(ldtbOrganization.Rows[0]);
                return true;
            }
            return false;
        }

        //Loading the ORG Contact by Contact ID
        public void LoadOrgContactPrimaryAddressByContact(int aintContactID)
        {
            DataTable ldtbOrgContact = Select<cdoOrgContact>(new string[2] { "ORG_ID", "CONTACT_ID" },
                     new object[2] { _icdoOrganization.org_id, aintContactID }, null, null);

            GetOrgContactPrimaryAddress(ldtbOrgContact);
        }

        //Loading the ORG Contact by Role
        public void LoadOrgContactPrimaryAddressByRole(string astrRoleValue)
        {
            DataTable ldtbOrgContactByRole = busNeoSpinBase.Select("cdoOrgContactRole.GetOrgContactByRole", new object[2] { _icdoOrganization.org_id, astrRoleValue });

            GetOrgContactPrimaryAddress(ldtbOrgContactByRole);
        }

        /// <summary>
        /// Here is the hierarchy for addressing on correspondence:
        /// 1) Contact Role for Plan (Plan specific) 
        /// 2) Authorized Agent for Plan (Plan specific) 
        /// 3) Primary Authorized Agent 
        /// 4) Organization Primary Address - Blank Contact Name. 
        /// 5) <unknown> 
        /// </summary>
        /// <param name="astrRole"></param>
        /// <param name="aintPlanID"></param>
        public void LoadOrgContactByRoleAndPlan(string astrRole, int aintPlanID)
        {
            busOrgContact lobjReturnOC = new busOrgContact();
            lobjReturnOC.icdoOrgContact = new cdoOrgContact();

            bool lblnRecordFound = false;

            if (_iclbOrgContact == null)
                LoadOrgContact();

            foreach (busOrgContact lobjOrgContact in _iclbOrgContact)
            {
                if (lobjOrgContact.icdoOrgContact.status_value == busConstant.StatusActive)
                {
                    if (lobjOrgContact.iclbOrgContactRole == null)
                        lobjOrgContact.LoadContactTypes();
                    foreach (cdoOrgContactRole lobjOrgContactRole in lobjOrgContact.iclbOrgContactRole)
                    {
                        if (lobjOrgContactRole.contact_role_value == astrRole)
                        {
                            //As of now I am commenting this, because if the plan id not matches, don't pick that org contact
                            if (aintPlanID == 0)
                            {
                                lobjReturnOC = lobjOrgContact;
                                lblnRecordFound = true;
                                break;
                            }

                            if (lobjOrgContact.icdoOrgContact.plan_id == aintPlanID)
                            {
                                lobjReturnOC = lobjOrgContact;
                                lblnRecordFound = true;
                                break;
                            }
                        }
                    }
                    if (lblnRecordFound)
                        break;
                }
            }

            if (!lblnRecordFound)
            {
                //If there is no specific plan based role found, get the Primary Authorized Agent
                foreach (busOrgContact lobjOrgContact in _iclbOrgContact)
                {
                    if (lobjOrgContact.icdoOrgContact.status_value == busConstant.StatusActive)
                    {
                        if (lobjOrgContact.iclbOrgContactRole == null)
                            lobjOrgContact.LoadContactTypes();
                        foreach (cdoOrgContactRole lobjOrgContactRole in lobjOrgContact.iclbOrgContactRole)
                        {
                            if ((lobjOrgContactRole.contact_role_value == busConstant.OrgContactRoleAuthorizedAgent) &&
                                (aintPlanID > 0) &&
                                (lobjOrgContact.icdoOrgContact.plan_id == aintPlanID))
                            {
                                lobjReturnOC = lobjOrgContact;
                                lblnRecordFound = true;
                                break;
                            }

                            if ((lobjOrgContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent) && (aintPlanID == 0))
                            {
                                lobjReturnOC = lobjOrgContact;
                                lblnRecordFound = true;
                                break;
                            }
                        }
                        if (lblnRecordFound)
                            break;
                    }
                }
            }

            if (_ibusOrgContactPrimaryAddress == null)
            {
                _ibusOrgContactPrimaryAddress = new busOrgContactAddress();
                _ibusOrgContactPrimaryAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            }

            //If the Record Found, get the Primary Address of the Org Contact
            if (lblnRecordFound)
            {
                _ibusOrgContactPrimaryAddress.FindOrgContactAddress(lobjReturnOC.icdoOrgContact.primary_address_id);

                //Load the Contact Object to Print the Contact Name in Correspondence
                if (lobjReturnOC.ibusContact == null)
                    lobjReturnOC.LoadContact();

                _ibusContact = lobjReturnOC.ibusContact;
            }

            if (_ibusOrgContactPrimaryAddress.icdoOrgContactAddress.contact_org_address_id == 0)
            {
                //If No Primary Address set for Org Contact, Load the Org Primary Address
                if (_ibusOrgPrimaryAddress == null)
                    LoadOrgPrimaryAddress();

                //Assign the Org Primary Address to Org Contact Primary Address
                _ibusOrgContactPrimaryAddress = ibusOrgPrimaryAddress;
            }
        }

        private void GetOrgContactPrimaryAddress(DataTable adtbOrgContact)
        {
            if (_ibusOrgContactPrimaryAddress == null)
            {
                _ibusOrgContactPrimaryAddress = new busOrgContactAddress();
                _ibusOrgContactPrimaryAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            }

            if (_ibusContact == null)
            {
                _ibusContact = new busContact();
                _ibusContact.icdoContact = new cdoContact();
            }

            if (adtbOrgContact.Rows.Count > 0)
            {
                bool lblnOrgContactExists = false;
                foreach (DataRow ldrRow in adtbOrgContact.Rows)
                {
                    busOrgContact lbusOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact() };
                    lbusOrgContact.icdoOrgContact.LoadData(ldrRow);
                    if (lbusOrgContact.icdoOrgContact.primary_address_id > 0)
                    {
                        lbusOrgContact.LoadOrgAndContactPrimaryAddress();
                        ibusOrgContactPrimaryAddress = lbusOrgContact.ibusOrgAndContactPrimaryAddress;
                        lblnOrgContactExists = true;
                        break;
                    }
                }

                if (!lblnOrgContactExists)
                {
                    //If the Contact is not assigned to Org OR if the Primary Address is not Set for the ORG Contact, 
                    //Get the Primary Address ORG.
                    //If ORG Primary Address also not Set, then take the Contact Primary Address
                    if (_icdoOrganization.primary_address_id != 0)
                    {
                        _ibusOrgContactPrimaryAddress.FindOrgContactAddress(_icdoOrganization.primary_address_id);
                    }
                    else
                    {
                        busOrgContact lbusOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact() };
                        lbusOrgContact.icdoOrgContact.LoadData(adtbOrgContact.Rows[0]);
                        lbusOrgContact.LoadContact();

                        if (lbusOrgContact.ibusContact.icdoContact.primary_address_id != 0)
                        {
                            _ibusOrgContactPrimaryAddress.FindOrgContactAddress(lbusOrgContact.ibusContact.icdoContact.primary_address_id);
                        }
                    }
                }
            }
        }

        //UAT PIR No:897-  Validation on Changing routing number
        public bool IsRoutingNoChanged()
        {
            if (icdoOrganization.ihstOldValues.Count > 0 && icdoOrganization.ihstOldValues["routing_no"].ToString() != icdoOrganization.routing_no)
            {
                return true;
            }
            return false;
        }
        public void LoadOrgContact()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.LoadOrgContact", new object[1] { _icdoOrganization.org_id });
            _iclbOrgContact = new Collection<busOrgContact>();
            foreach (DataRow ldtrRow in ldtbList.Rows)
            {
                busOrgContact lobjOrgContact = new busOrgContact { icdoOrgContact = new cdoOrgContact(), icdoContactRole = new cdoOrgContactRole() };
                lobjOrgContact.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjOrgContact.ibusOrgAndContactPrimaryAddress = new busOrgContactAddress { icdoOrgContactAddress = new cdoOrgContactAddress() };
                lobjOrgContact.ibusContact = new busContact { icdoContact = new cdoContact() };
                lobjOrgContact.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lobjOrgContact.icdoOrgContact.LoadData(ldtrRow);
                lobjOrgContact.icdoContactRole.LoadData(ldtrRow);
                lobjOrgContact.ibusOrganization.icdoOrganization.LoadData(ldtrRow);
                lobjOrgContact.ibusOrgAndContactPrimaryAddress.icdoOrgContactAddress.LoadData(ldtrRow);
                lobjOrgContact.ibusContact.icdoContact.LoadData(ldtrRow);
                lobjOrgContact.ibusContact.icdoContact.primary_address_id = ldtrRow["cnt_primary_address_id"] == DBNull.Value ?
                    0 : Convert.ToInt32(ldtrRow["cnt_primary_address_id"]);
                lobjOrgContact.ibusPlan.icdoPlan.LoadData(ldtrRow);
                _iclbOrgContact.Add(lobjOrgContact);
            }
        }
        /// <summary>
        /// This is to load all the members of Employer
        /// </summary>
        public void LoadPersonEmployment()
        {
            DataTable ldtbList = Select<cdoPersonEmployment>(
               new string[1] { "org_id" },
               new object[1] { icdoOrganization.org_id }, null, null);
            iclbPersonEmployment = GetCollection<busPersonEmployment>(ldtbList, "icdoPersonEmployment");
        }
        //To Check whether the selected Address is Active before setting Primary Address.
        private busOrgContactAddress _ibusOrgcontactAddress;
        public busOrgContactAddress ibusOrgContactAddress
        {
            get { return _ibusOrgcontactAddress; }
            set { _ibusOrgcontactAddress = value; }
        }

        public ArrayList SetOrgPrimaryAddress(int aintOrgAddressId)
        {
            ArrayList larrError = new ArrayList();
            utlError lobjError = new utlError();
            if (_ibusOrgcontactAddress == null)
            {
                _ibusOrgcontactAddress = new busOrgContactAddress();
            }
            _ibusOrgcontactAddress.FindOrgContactAddress(aintOrgAddressId);
            if (_ibusOrgcontactAddress.icdoOrgContactAddress.status_value == busConstant.OrganizationStatusActive)
            {
                _icdoOrganization.primary_address_id = aintOrgAddressId;
                _icdoOrganization.modified_by = iobjPassInfo.istrUserID;
                _icdoOrganization.Update();
            }
            else
            {
                lobjError = AddError(busConstant.MessageIdPrimaryAddress, string.Empty);
                larrError.Add(lobjError);
            }
            return larrError;
        }

        public void LoadOrgPrimaryAddress()
        {
            if (_ibusOrgPrimaryAddress == null)
            {
                _ibusOrgPrimaryAddress = new busOrgContactAddress();
            }
            _ibusOrgPrimaryAddress.FindOrgContactAddress(_icdoOrganization.primary_address_id);
        }

        public void LoadOrgAddresses()
        {
            DataTable ldtbList = Select<cdoOrgContactAddress>(
                new string[1] { "org_id" },
                new object[1] { icdoOrganization.org_id }, null, null);
            _iclbOrgAddress = GetCollection<busOrgContactAddress>(ldtbList, "icdoOrgContactAddress");
            foreach (busOrgContactAddress lobjTempOrgAddress in _iclbOrgAddress)
            {
                if (lobjTempOrgAddress.icdoOrgContactAddress.contact_org_address_id == _icdoOrganization.primary_address_id)
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
        public void LoadOrgBank()
        {
            DataTable ldtbList = Select<cdoOrgBank>(
                new string[1] { "org_id" },
                new object[1] { icdoOrganization.org_id }, null, null);
            _iclbOrgBank = GetCollection<busOrgBank>(ldtbList, "icdoOrgBank");
            foreach (busOrgBank lobjOrgBank in _iclbOrgBank)
            {
                //PIR 195
                lobjOrgBank.icdoOrgBank.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjOrgBank.icdoOrgBank.bank_org_id);
                lobjOrgBank.LoadBankOrg();
            }
        }
        // Load Contact/Appointment Details for this Organization
        public void LoadContactsAppointments()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.LOAD_CONTACT_TICKET", new object[1] { _icdoOrganization.org_id });
            _icolOrganizationContactTicket = GetCollection<busContactTicket>(ldtbList, "icdoContactTicket");
            foreach (busContactTicket lobjBusContactTicket in _icolOrganizationContactTicket)
            {
                if (lobjBusContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
                {
                    lobjBusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();
                    lobjBusContactTicket.ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(lobjBusContactTicket.icdoContactTicket.contact_ticket_id);
                    lobjBusContactTicket.LoadAppointmentCounselorName();
                }
                lobjBusContactTicket.LoadUser();
            }
        }

        // Load Seminar Details for this Organization
        public void LoadSeminarAttendance()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoOrganization.LOAD_SEMINAR_ATTENDANCE", new object[1] { _icdoOrganization.org_id });
            icolSeminarSchedule = new Collection<busSeminarSchedule>();
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busSeminarSchedule lbusSemSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lbusSemSchedule.icdoSeminarSchedule.LoadData(ldrRow);

                lbusSemSchedule.ibusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lbusSemSchedule.ibusContactTicket.icdoContactTicket.LoadData(ldrRow);

                lbusSemSchedule.ibusContactTicket.ibusFacilitator = new busUser { icdoUser = new cdoUser() };
                lbusSemSchedule.ibusContactTicket.ibusFacilitator.icdoUser.LoadData(ldrRow);
                icolSeminarSchedule.Add(lbusSemSchedule);
            }
        }

        public override int PersistChanges()
        {
            if (_icdoOrganization.ienuObjectState == ObjectState.Insert)
            {
                if (_icdoOrganization.org_type_value.IsNotNullOrEmpty() &&
                    _icdoOrganization.emp_category_value != busConstant.EmployerCategoryState)
                {
                    _icdoOrganization.org_code = GetNewOrgCodeRangeID();
                    DBFunction.DBNonQuery("cdoOrgCodeByType.UpdateLastEnteredOrgCodeID", new object[2] { 
                                            (Convert.ToInt32(_icdoOrganization.org_code)- 1), Convert.ToInt32(_icdoOrganization.org_code) },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
                if (icdoOrganization.org_code.Length < 6)
                    icdoOrganization.org_code = icdoOrganization.org_code.PadLeft(6, '0');
            }
            return base.PersistChanges();
        }

        public string GetNewOrgCodeRangeID()
        {
            string lstrOrgCodeRangeID = string.Empty;
            DataTable ldtbOrgCodeList = new DataTable();

            if (string.IsNullOrEmpty(icdoOrganization.emp_category_value))
            {
                ldtbOrgCodeList = Select<cdoOrgCodeByType>(
                                    new string[1] { "ORG_TYPE_VALUE" },
                                    new object[1] { icdoOrganization.org_type_value }, null, null);
            }
            else
            {
                ldtbOrgCodeList = Select<cdoOrgCodeByType>(
                                    new string[2] { "ORG_TYPE_VALUE", "EMP_CATEGORY_VALUE" },
                                    new object[2] { icdoOrganization.org_type_value, _icdoOrganization.emp_category_value }, null, null);
            }
            if (ldtbOrgCodeList.Rows.Count > 0)
            {
                DataTable ldtbLastEnteredValue = Select<cdoOrgCodeRange>(
                                                    new string[1] { "ORG_CODE_RANGE_ID" },
                                                    new object[1] { Convert.ToInt32(ldtbOrgCodeList.Rows[0]["ORG_CODE_RANGE_ID"]) }, null, null);
                if (ldtbLastEnteredValue.Rows.Count > 0)
                {
                    int lintLastEnteredRange = Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["LAST_ENTERED_ORG_CODE"]);
                    int lintMaxRangeValue = Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["ORG_CODE_MAX"]);
                    if (lintLastEnteredRange + 1 <= lintMaxRangeValue)
                    {
                        lstrOrgCodeRangeID = Convert.ToString(lintLastEnteredRange + 1);
                    }
                    else
                    {
                        // Org Code exceeds max has lesser possibilities.
                    }
                }
            }
            if (lstrOrgCodeRangeID.Length < 6)
                lstrOrgCodeRangeID = lstrOrgCodeRangeID.PadLeft(6, '0');
            return lstrOrgCodeRangeID;
        }

        //To Check the Status from one value to another status value to permit.
        public bool ChangeOrganizationStatusUpdateMode()
        {
            string lstrStatus = icdoOrganization.ihstOldValues["status_value"].ToString();
            if (((lstrStatus == busConstant.OrganizationStatusPending) &&
                ((icdoOrganization.status_value == busConstant.OrganizationStatusInactive)
                || (icdoOrganization.status_value == busConstant.OrganizationStatusMerged)))
                || ((lstrStatus == busConstant.OrganizationStatusActive) &&
                    (icdoOrganization.status_value == busConstant.OrganizationStatusPending))
                || ((lstrStatus == busConstant.OrganizationStatusInactive) &&
                    ((icdoOrganization.status_value == busConstant.OrganizationStatusPending)
                    || (icdoOrganization.status_value == busConstant.OrganizationStatusDeclined)
                    || (icdoOrganization.status_value == busConstant.OrganizationStatusMerged))))
            {
                return true;
            }
            return false;
        }

        //Visible rule for the Status Declined and Merged.
        public bool CheckVisibleRuleForStatus()
        {
            if ((icdoOrganization.status_value == busConstant.OrganizationStatusDeclined)
                || (icdoOrganization.status_value == busConstant.OrganizationStatusMerged))
            {
                return true;
            }
            return false;
        }

        private Collection<busOrgPlan> _iclbOrgPlan;
        public Collection<busOrgPlan> iclbOrgPlan
        {
            get { return _iclbOrgPlan; }
            set { _iclbOrgPlan = value; }
        }

        /// <summary>
        /// Loads the data associated with the Plan assigned to organization.
        /// </summary>
        public void LoadOrgPlan()
        {

            DataTable ldtbList = Select<cdoOrgPlan>(
                new string[1] { "org_id" },
                new object[1] { _icdoOrganization.org_id }, null, "participation_end_date,participation_start_date desc");

            ////PIR - 13171  eliminating ended plans
            //ldtbList = ldtbList.AsEnumerable().Where(row => row.Field<DateTime?>("Participation_End_Date") == null || row.Field<DateTime?>("Participation_End_Date") > DateTime.Now).AsDataTable();
            //PIR - 13905
            _iclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");          

            foreach (busOrgPlan lobjOrgPlan in _iclbOrgPlan)
            {
                lobjOrgPlan.LoadPlanInfo(lobjOrgPlan.icdoOrgPlan.plan_id);              
            }
        }

        //If Organization having any plan with open end date, then it should not allow to change the status to Inactive,Declined,Merged.
        public bool CheckOrgPlanWithOpenEndDate()
        {
            int Count = (int)DBFunction.DBExecuteScalar("cdoOrganization.GET ORGPLAN WITH OPENENDDATE",
                     new object[1] { icdoOrganization.org_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if ((Count > 0)
                && ((icdoOrganization.status_value == "DECL")
                || (icdoOrganization.status_value == "MERG") || (icdoOrganization.status_value == "IATV")))
            {
                return true;
            }
            return false;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            // PIR 180 
            _icdoOrganization.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(214, _icdoOrganization.status_value);
        }
        public override void BeforePersistChanges()
        {
            iblnIsUpdateAddressClicked = false;
            base.BeforePersistChanges();
        }

        //PIR - 346
        public void LoadNotes()
        {
            DataTable ldtbNotesOrg = busNeoSpinBase.Select("cdoNotes.OrgLookup",
                                          new object[3] { "GENR", 0, _icdoOrganization.org_id });
            _iclbNotes = GetCollection<busNotes>(ldtbNotesOrg, "icdoNotes");
        }

        //related to workflow PIR 428
        private Collection<busBpmActivityInstanceHistory> _iclbWorkflowProcessHistory;
        public Collection<busBpmActivityInstanceHistory> iclbWorkflowProcessHistory
        {
            get { return _iclbWorkflowProcessHistory; }
            set { _iclbWorkflowProcessHistory = value; }
        }

        //--Venkat - Modify query
        public void LoadWorkflowProcessHistory()
        {
            DataTable ldtbAIH = busNeoSpinBase.Select("entSolBpmActivityInstance.LoadProcessInstanceHistoryByOrg", new object[1] { _icdoOrganization.org_id });
            _iclbWorkflowProcessHistory = GetCollection<busBpmActivityInstanceHistory>(ldtbAIH, "icdoBpmActivityInstanceHistory");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busBpmActivityInstanceHistory)
            {
                busBpmActivityInstanceHistory lbusActivityInstanceHistory = (busBpmActivityInstanceHistory)aobjBus;
                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstanceHistory.icdoBpmActivityInstanceHistory.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }
                lbusActivityInstanceHistory.ibusBpmActivityInstance = new busSolBpmActivityInstance();
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity = new busSolBpmActivity();

                if (!Convert.IsDBNull(adtrRow["ACTIVITY_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow["ACTIVITY_NAME"].ToString();
                }

                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance = new busSolBpmProcessInstance();
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = new busSolBpmCaseInstance();
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest = new busSolBpmRequest();
                if (!Convert.IsDBNull(adtrRow["SOURCE"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow["SOURCE"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["contact_ticket_id"]))
                {
                    ((busSolBpmCaseInstance)lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance).contact_ticket_id = Convert.ToInt32(adtrRow["contact_ticket_id"]);
                }

                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = ClassMapper.GetObject<busBpmProcess>();
                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow["Process_Description"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["PROCESS_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow["PROCESS_NAME"].ToString();
                }
            }
        }

        //Load Workflow Image Data
        public Collection<busSolBpmProcessInstanceAttachments> iclbProcessInstanceImageData { get; set; }

        public void LoadWorkflowImageData()
        {
            iclbProcessInstanceImageData = busFileNetHelper.LoadFileNetImagesByOrgCode(_icdoOrganization.org_code);
        }

        public bool CheckOrgCodeRange()
        {
            DataTable ldtbOrgCodeList;
            if (_icdoOrganization.emp_category_value != null)
            {
                ldtbOrgCodeList = Select<cdoOrgCodeByType>(
                    new string[2] { "org_type_value", "EMP_CATEGORY_VALUE" },
                    new object[2] { icdoOrganization.org_type_value, _icdoOrganization.emp_category_value }, null,
                    null);
            }
            else
            {
                ldtbOrgCodeList = Select<cdoOrgCodeByType>(
                    new string[1] { "org_type_value" },
                    new object[1] { icdoOrganization.org_type_value }, null, null);
            }
            if (ldtbOrgCodeList.Rows.Count > 0)
            {
                DataTable ldtbLastEnteredValue = Select<cdoOrgCodeRange>(
                    new string[1] { "org_code_range_id" },
                    new object[1] { Convert.ToInt32(ldtbOrgCodeList.Rows[0]["ORG_CODE_RANGE_ID"]) }, null,
                    null);
                if (ldtbLastEnteredValue.Rows.Count > 0)
                {
                    if ((!String.IsNullOrEmpty(ldtbLastEnteredValue.Rows[0]["org_code_max"].ToString())) && 
                        (!String.IsNullOrEmpty(ldtbLastEnteredValue.Rows[0]["org_code_min"].ToString())))
                    {
                        //UAT PIR - 26
                        //if ((_icdoOrganization.org_type_value == busConstant.OrgTypeEmployer) &&
                        //    (_icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) &&
                        //    (!String.IsNullOrEmpty(_icdoOrganization.org_code)))
                        //{
                        if
                            (Convert.ToInt32(_icdoOrganization.org_code) >
                             Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["org_code_max"]))
                        {
                            return false;
                        }
                        if
                            (Convert.ToInt32(_icdoOrganization.org_code) <
                             Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["org_code_min"]))
                        {
                            return false;
                        }
                        //}
                        //else
                        //{
                        //    if (Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["last_entered_org_code"]) <
                        //        (Convert.ToInt32(ldtbLastEnteredValue.Rows[0]["org_code_max"])))
                        //    {
                        //        return true;
                        //    }
                        //}
                    }
                }
            }
            return true; ;
        }

        // duplicated from IBS batch
        public static int GetJSPlanOrgID(utlPassInfo aobjPassInfo)
        { //?? As of now only one organization provides JS plan 
            //?? It should be retrieved form sgt_org_plan JEEVA
            return Convert.ToInt32(DBFunction.DBExecuteScalar("cdoJsRhicBill.GetJSRHOrgID",  aobjPassInfo.iconFramework,  aobjPassInfo.itrnFramework));
        }

        #region Correspondence

        public override busBase GetCorOrganization()
        {
            return this;
        }

        public string LoggedUserID
        {
            get
            {
                return busGlobalFunctions.ToTitleCase(iobjPassInfo.istrUserID);
            }
        }


        public string UserPhoneNo
        {
            get
            {
                string strPhoneNo = string.Empty;
                busUser lobjUser = new busUser();
                lobjUser.FindUser(iobjPassInfo.iintUserSerialID);
                int lintPersonID = lobjUser.icdoUser.person_id;
                if (lintPersonID != 0)
                {
                    busPerson lobjPerson = new busPerson();
                    lobjPerson.FindPerson(lintPersonID);
                    strPhoneNo = lobjPerson.icdoPerson.work_phone_no;
                }
                return strPhoneNo;
            }
        }

        public string CurrentYear
        {
            get
            {
                return DateTime.Now.ToString("yyyy");
            }
        }

        public string PreviousYear
        {
            get
            {
                return DateTime.Now.AddYears(-1).ToString("yyyy");
            }
        }
        #endregion

        private Collection<busOrgPlan> _iclbOrganizationOfferedPlans;
        public Collection<busOrgPlan> iclbOrganizationOfferedPlans
        {
            get { return _iclbOrganizationOfferedPlans; }
            set { _iclbOrganizationOfferedPlans = value; }
        }
        //PIR 20232
        private bool IsPersonAccountsInEnrSusRetiredStatus(int? aintPersonId)
        {
            DateTime? ldt_Plan_changes_effective_date = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.Plan_changes_effective_date, iobjPassInfo));
            DataTable ldtbPersonAccount = Select<cdoPersonAccount>(
            new string[1] { "person_id"},
            new object[1] { aintPersonId}, null, null);
            Collection<busPersonAccount> lclbPersonAccount = GetCollection<busPersonAccount>(ldtbPersonAccount, "icdoPersonAccount");
            //PIR 22842 
            return lclbPersonAccount.Count(i => (i.icdoPersonAccount.plan_id == busConstant.PlanIdMain || i.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf || i.icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety || //PIR 25729
                                           i.icdoPersonAccount.plan_id == busConstant.PlanIdLE || i.icdoPersonAccount.plan_id == busConstant.PlanIdNG || i.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS)
                                       && (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                           i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended || 
                                           i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired) &&
                                           (i.icdoPersonAccount.start_date < ldt_Plan_changes_effective_date)) > 0;
        }
        //PIR 20232
        public bool IsMemberEnrolledInDCPlan(int? aintPersonId)
        {
            DateTime? ldt_Plan_changes_effective_date = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.Plan_changes_effective_date, iobjPassInfo));
            bool lblnResult = false;
            DataTable ldtbPersonAccount = new DataTable();
            ldtbPersonAccount = Select<cdoPersonAccount>(
        new string[2] { "person_id", "plan_id" },
        new object[2] { aintPersonId, busConstant.PlanIdDC }, null, null);
            Collection<busPersonAccount> lclbPersonAccount = GetCollection<busPersonAccount>(ldtbPersonAccount, "icdoPersonAccount");
            foreach (busPersonAccount lbusPersonAccount in lclbPersonAccount)
            {
                if ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC) &&
                    (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled
                    && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.RetirementPlanParticipationStatusTranToDb) &&
                    (lbusPersonAccount.icdoPersonAccount.start_date < ldt_Plan_changes_effective_date))
                {
                    lblnResult = true;
                    break;
                }
            }
            return lblnResult;
        }


        //PIR 20232
        public bool LoadPlanMainOrMain2020(int aintPlanId, DateTime? adtEmpStartDate, int? aintPersonId)
        {
            DateTime? ldt_Plan_changes_effective_date = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.Plan_changes_effective_date, iobjPassInfo));
            bool lblIsInMain2020Plan = false;
            if (adtEmpStartDate < Convert.ToDateTime(ldt_Plan_changes_effective_date))
            {
                lblIsInMain2020Plan = false;
            }
            else
            {
                if (IsPersonAccountsInEnrSusRetiredStatus(aintPersonId))
                {
                    lblIsInMain2020Plan = false;
                }
                else if (adtEmpStartDate >= Convert.ToDateTime(ldt_Plan_changes_effective_date))
                {
                    lblIsInMain2020Plan = true;
                }
            }
            return lblIsInMain2020Plan;
        }
        //PIR 20232
        public bool LoadPlanDCOrDC2020(int aintPlanId, DateTime? adtEmpStartDate, int? aintPersonId) //PIR 20232 ?code
        {
            DateTime? ldt_Plan_changes_effective_date = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.Plan_changes_effective_date, iobjPassInfo));
            bool lblIsInDC2020Plan = false;
            if (adtEmpStartDate < Convert.ToDateTime(ldt_Plan_changes_effective_date))
            {
                lblIsInDC2020Plan = false;
            }
            else
            {
                if (IsMemberEnrolledInDCPlan(aintPersonId))
                {
                    lblIsInDC2020Plan = false;
                }
                else if (adtEmpStartDate >= Convert.ToDateTime(ldt_Plan_changes_effective_date))
                {
                    lblIsInDC2020Plan = true;
                }
            }
            return lblIsInDC2020Plan;
        }
		//PIR 25920 New Plan DC 2025
        /// <summary>
        /// define the eligiblity for new plan DC 2025 according to org plan and person accounts of previous DB plan
        /// </summary>
        /// <param name="adtEffectiveDate">its a max date or employment detail start date </param>
        /// <param name="adtEmpStartDate">start date of employment header</param>
        /// <param name="aintPersonId">person id</param>
        /// <returns></returns>
        public bool IsEligibleToEnrlDC2025(DateTime adtEffectiveDate, DateTime? adtEmpStartDate, int? aintPersonId) 
        {
            //DateTime? ldt_Plan_changes_effective_date = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.Plan_DC25_changes_effective_date, iobjPassInfo));
            bool lblIsDC25PlanEligibleToEnrl = false;
            //DataTable ldtbOrgPlan = Select<cdoOrgPlan>(new string[1] { "org_id" }, new object[1] {
            //                                    icdoOrganization.org_id }, null, "participation_start_date desc");
            //Collection<busOrgPlan> lclbOrgPlan = GetCollection<busOrgPlan>(ldtbOrgPlan, "icdoOrgPlan");
            if(iclbOrgPlan.IsNull()) LoadOrgPlan();
            if (adtEffectiveDate == DateTime.MaxValue)
            {
                LoadPersonEmployment();
                if (iclbPersonEmployment.IsNotNull() && iclbPersonEmployment.Count > 0)
                {
                    busPersonEmployment lbusPersonEmployment = iclbPersonEmployment.OrderByDescending(objPersonEmp => objPersonEmp.icdoPersonEmployment.person_id == aintPersonId && objPersonEmp.icdoPersonEmployment.end_date == DateTime.MinValue).FirstOrDefault();
                    if (lbusPersonEmployment.IsNotNull())
                        adtEffectiveDate = lbusPersonEmployment.icdoPersonEmployment.start_date;
                }
            }
            DataTable ldtbPersonAccount = new DataTable();
            ldtbPersonAccount = Select<cdoPersonAccount>(new string[1] { "person_id" }, new object[1] { aintPersonId }, null, null);
            Collection<busPersonAccount> lclbPersonAccount = GetCollection<busPersonAccount>(ldtbPersonAccount, "icdoPersonAccount");
            lclbPersonAccount.ForEach(objPerson => objPerson.LoadPlan());

            bool lblnIsMainEnrolledPreviously = lclbPersonAccount.Any(objPersonAccount => (objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                                            objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                                            objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) &&
                                            objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled);

            if (iclbOrgPlan.Any(lobjOrgPlan => lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 &&
                                            busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                        lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                                        lobjOrgPlan.icdoOrgPlan.end_date_no_null)))
            {
                DateTime ldt_Plan_changes_effective_date = iclbOrgPlan.Where(lobjOrgPlan => lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025).FirstOrDefault().icdoOrgPlan.participation_start_date;

                if (adtEmpStartDate < Convert.ToDateTime(ldt_Plan_changes_effective_date) &&
                    !(lclbPersonAccount.Any(lobjPersonAccount =>
                    lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdMain && lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdMain2020
                    && lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC && lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdDC2020
                    && lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement
                    && lobjPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB) && !lblnIsMainEnrolledPreviously
                     )
                    )
                {
                    lblIsDC25PlanEligibleToEnrl = false;
                }
                else if (lblnIsMainEnrolledPreviously)
                {
                    lblIsDC25PlanEligibleToEnrl = false;
                }
                else
                {
                    if (lclbPersonAccount.Any(objPersonAccount => (objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                                             objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                                            objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) &&
                         (objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended ||
                         objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)))
                    {
                        lblIsDC25PlanEligibleToEnrl = false;
                    }
                    else if (lclbPersonAccount.Any(objPersonAccount => (objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                                                 objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 || objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                                            objPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) &&
                         (objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn ||
                         objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled)))
                    {
                        lblIsDC25PlanEligibleToEnrl = true;
                    }
                    else if (lclbPersonAccount.Any(objPersonAccount => objPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB
                            && objPersonAccount.icdoPersonAccount.start_date <= ldt_Plan_changes_effective_date)
                            && !lblnIsMainEnrolledPreviously
                             )
                    {
                        lblIsDC25PlanEligibleToEnrl = true;
                    }
                    else if (adtEmpStartDate >= Convert.ToDateTime(ldt_Plan_changes_effective_date))
                    {
                        lblIsDC25PlanEligibleToEnrl = true;
                    }
                }
            }
            return lblIsDC25PlanEligibleToEnrl;
        }
        public void LoadOrganizationOfferedPlans(DateTime adtEffectiveDate, DateTime? adtEmpStartDate = null, int? aintPersonId = 0)
        {
            _iclbOrganizationOfferedPlans = new Collection<busOrgPlan>();
            DataTable ldtbOrgPlan = Select<cdoOrgPlan>(new string[1] { "org_id" }, new object[1] { 
                                                icdoOrganization.org_id }, null, "participation_start_date desc");
			//PIR 25920 New Plan DC 2025
            bool lblnIsEligibleToEnrlDC2025 = IsEligibleToEnrlDC2025(adtEffectiveDate,adtEmpStartDate, aintPersonId);

            foreach (DataRow dr in ldtbOrgPlan.Rows)
            {
                busOrgPlan lobjOrgPlan = new busOrgPlan();
                lobjOrgPlan.icdoOrgPlan = new cdoOrgPlan();
                lobjOrgPlan.icdoOrgPlan.LoadData(dr);
                if (busGlobalFunctions.CheckDateOverlapping(
                            adtEffectiveDate,
                            lobjOrgPlan.icdoOrgPlan.participation_start_date,
                            lobjOrgPlan.icdoOrgPlan.end_date_no_null))
                {
					//PIR 25920 New Plan DC 2025 eligiblity for DC25 plan
                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025 || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain2020
                        || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020)
                    {
                        if (lblnIsEligibleToEnrlDC2025)
                        {
                            if(lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2025)
                                _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                        }
                        else
                        {
                            //PIR 20232
                            if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain2020)
                            {
                                if (LoadPlanMainOrMain2020(lobjOrgPlan.icdoOrgPlan.plan_id, adtEmpStartDate, aintPersonId))
                                {
                                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain2020)
                                    {
                                        _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                                    }
                                }
                                else
                                {
                                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdMain)
                                    {
                                        _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                                    }
                                }
                            }
                            else if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC || lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020) //PIR 20232
                            {
                                if (LoadPlanMainOrMain2020(lobjOrgPlan.icdoOrgPlan.plan_id, adtEmpStartDate, aintPersonId) && LoadPlanDCOrDC2020(lobjOrgPlan.icdoOrgPlan.plan_id, adtEmpStartDate, aintPersonId))
                                {
                                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC2020)
                                    {
                                        _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                                    }
                                }
                                else
                                {
                                    if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdDC)
                                    {
                                        _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                                    }
                                }
                            }
                        }
                    }
                    else
                        _iclbOrganizationOfferedPlans.Add(lobjOrgPlan);
                }
            }
        }

        public void LoadOrganizationOfferedPlans()
        {
            LoadOrganizationOfferedPlans(DateTime.Now);
        }

        private Collection<busOrgDeductionType> _iclbOrgDeductionType;
        public Collection<busOrgDeductionType> iclbOrgDeductionType
        {
            get { return _iclbOrgDeductionType; }
            set { _iclbOrgDeductionType = value; }
        }

        // Load the list of Payment Item Type(Deduction) ID corresponding to this Vendor.
        public void LoadOrgDeductionTypeByOrgID()
        {
            DataTable ldtbResult = Select<cdoOrgDeductionType>(new string[1] { "org_id" }, new object[1] { _icdoOrganization.org_id }, null, null);
            _iclbOrgDeductionType = GetCollection<busOrgDeductionType>(ldtbResult, "icdoOrgDeductionType");
            foreach (busOrgDeductionType lobjOrgDeductionType in _iclbOrgDeductionType)
            {
                lobjOrgDeductionType.LoadPaymentItemType();
            }
        }

        public bool ValidateIfPaymentOptionACHSelected()
        {
            DataTable ldtbList = Select<cdoOrgBank>(
                  new string[2] { "org_id", "usage_value" },
                  new object[2] { icdoOrganization.org_id, busConstant.BankUsageDirectDeposit }, null, null);
            if (ldtbList.Rows.Count == 0)
            {
                return true;
            }
            return false;
        }

        private Collection<busBenefitApplication> _iclbBenefitApplication;
        public Collection<busBenefitApplication> iclbBenefitApplication
        {
            get { return _iclbBenefitApplication; }
            set { _iclbBenefitApplication = value; }
        }

        //Load All Benefit Applications for this Person
        public void LoadBenefitApplication()
        {
            if (_iclbBenefitApplication == null)
                _iclbBenefitApplication = new Collection<busBenefitApplication>();
            DataTable ldtbBenefitApplication = Select<cdoBenefitApplication>(new string[1] { "payee_org_id" }, new object[1] { icdoOrganization.org_id }, null, null);
            iclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbBenefitApplication, "icdoBenefitApplication");
        }

        //property to payment history details for refund amount
        public Collection<busPaymentHistoryHeader> iclbPaymentDetails { get; set; }
        //property to store total gross amount paid
        public decimal idecGrossAmountPaid { get; set; }
        //property to store total non taxable amount paid
        public decimal idecNonTaxableAmountPaid { get; set; }
        //property to store total taxable amount paid
        public decimal idecTaxableAmountPaid { get; set; }

        /// <summary>
        /// Method to load Payment details 
        /// </summary>
        public void LoadPaymentDetails()
        {
            DataTable ldtPaymentHistory = new DataTable();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                DateTime ldtLastPaymentDate = new DateTime(iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Year,
                    iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Month, 1);
                ldtLastPaymentDate = ldtLastPaymentDate.AddMonths(1).AddDays(-1);
                if (icdoOrganization.org_type_value == busConstant.OrgTypeEmployer)
                {
                    ldtPaymentHistory = Select("cdoPaymentHistoryHeader.LoadRefundAmtForOrg",
                        new object[3] { icdoOrganization.org_id, ldtLastPaymentDate, busConstant.Flag_Yes });
                }
                else if (icdoOrganization.org_type_value == busConstant.OrgTypeProvider || icdoOrganization.org_type_value == busConstant.OrgTypeVendor)
                {
                    ldtPaymentHistory = Select("cdoPaymentHistoryHeader.LoadRefundAmtForOrg",
                        new object[3] { icdoOrganization.org_id, ldtLastPaymentDate, busConstant.Flag_No });
                }
                iclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
                if (iclbPaymentDetails.Count > 0)
                {
                    idecGrossAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.gross_amount).Sum();
                    idecNonTaxableAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.NonTaxable_Amount).Sum();
                    idecTaxableAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.taxable_amount).Sum();
                }
            }
        }

        //Property to contain Payment History of a payee account
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }

        /// <summary>
        /// Method to load Payment History collection for current payee account
        /// </summary>
        public void LoadPaymentHistoryHeader()
        {
            if (iclbPaymentHistoryHeader == null)
                iclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            DataTable ldtPaymentHistory = Select<cdoPaymentHistoryHeader>(new string[1] { "org_id" },
                                                                    new object[1] { icdoOrganization.org_id },
                                                                    null, "PAYMENT_DATE desc");
            iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
        }
        public override void LoadCorresProperties(string astrTemplateName)
        {
            Load75CorrespondenceProperties();
            base.LoadCorresProperties(astrTemplateName);            
        }
        // 75 Correspondence
        public decimal idecCheckAmount { get; set; }
        public string istrChecknumber { get; set; }
        public string istrPaymentMonth { get; set; }
        public void Load75CorrespondenceProperties()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDistribution();
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.Count > 0)
                {
                    idecCheckAmount = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().icdoPaymentHistoryDistribution.net_amount;
                    istrChecknumber = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().icdoPaymentHistoryDistribution.check_number;
                    istrPaymentMonth = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().istrPaymentMonth;
                }
            }
        }
        public decimal idecJudgesTotal { get; set; }
        public DateTime idtNextPaymentDate { get; set; }
        private bool _iblnIsUpdateAddressClicked;
        public bool iblnIsUpdateAddressClicked
        {
            get {
                return _iblnIsUpdateAddressClicked;
            }
            set
            {
                _iblnIsUpdateAddressClicked = value;
            }
            }
        public void LoadLastPaymentDate()
        {
            idtNextPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate();
        }
        public string istrPaymentDate
        {
            get
            {
                if (idtNextPaymentDate == DateTime.MinValue)
                    LoadLastPaymentDate();
                return idtNextPaymentDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public ArrayList UpdateAddressClick()
        {
            ArrayList larrList = new ArrayList(); 
            iblnIsUpdateAddressClicked = true;
            istrSuppressWarning = "N";
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }
        public void LoadPaymentsForJudges(DataTable adtjudges)
        {
            if (idtNextPaymentDate == DateTime.MinValue)
                LoadLastPaymentDate();
            // iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(adtjudges, "icdoPaymentHistoryHeader");
            iclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            foreach (DataRow ldtrJudgesPayment in adtjudges.Rows)
            {
                busPaymentHistoryHeader lobjPaymentHistoryHeader = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
                lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(ldtrJudgesPayment);
                busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lobjBenefitApplication.icdoBenefitApplication.LoadData(ldtrJudgesPayment);
                lobjPaymentHistoryHeader.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPaymentHistoryHeader.ibusPerson.icdoPerson.LoadData(ldtrJudgesPayment);
                lobjPaymentHistoryHeader.CalculateAmounts();
                idecJudgesTotal += lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.taxable_amount +
                    lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.NonTaxable_Amount;
                iclbPaymentHistoryHeader.Add(lobjPaymentHistoryHeader);          
            }
        }

        // PIR 10266
        public int GrantOnlineAccessForEmployer(int aintContactID, string astrQuestionCode, string astrAnswer, string astrNDPERSLoginID, string astrFirstName, string astrLastName, int aintOrgID)
        {
            if (iclbOrgContact == null)
                LoadOrgContact();

            var lbusOrgContact = iclbOrgContact.Where(i => i.icdoOrgContact.contact_id == aintContactID).FirstOrDefault();

            if (lbusOrgContact != null)
            {
                if (ibusOrgPrimaryAddress == null)
                    LoadOrgPrimaryAddress();

                if ((astrQuestionCode == busConstant.ESSQuestionEmployerType && icdoOrganization.emp_category_description.ToLower() == astrAnswer.ToLower()) ||
                   (astrQuestionCode == busConstant.ESSQuestionPrimaryZip && ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code == astrAnswer) ||
                   (astrQuestionCode == busConstant.ESSQuestionContactRole && IsContactRoleExists(aintContactID, astrAnswer)))
                {
                    if (lbusOrgContact.ibusContact == null)
                        lbusOrgContact.LoadContact();
                    if (!IsNDPERSLoginIDDuplicate(astrNDPERSLoginID)) {
                        if (IsContactWellnessCoordAndOther(aintOrgID, aintContactID))
                        {
                            if (lbusOrgContact.ibusContact.icdoContact.first_name.ToLower().Trim() == astrFirstName.ToLower().Trim() &&
                               lbusOrgContact.ibusContact.icdoContact.last_name.ToLower().Trim() == astrLastName.ToLower().Trim()) // PIR 10868 Trimmed the trailing blank spaces
                            {
                                lbusOrgContact.ibusContact.icdoContact.previous_ndpers_login_id = lbusOrgContact.ibusContact.icdoContact.ndpers_login_id;
                                lbusOrgContact.ibusContact.icdoContact.ndpers_login_id = astrNDPERSLoginID;
                                lbusOrgContact.ibusContact.icdoContact.Update();
                                GenerateCorrespondenceForSuccessfulRegistration(lbusOrgContact);
                                return 0;
                            }
                            else
                                return 1;
                        }
                        else
                            return 5;
                    }
                    else
                    {
                        return 6;
                    }
                }
                else
                    return 2;
            }
            else
                return 3;
        }
        //PIR 26305 checking Same NDPERSLoginID
        public bool IsNDPERSLoginIDDuplicate(string astrNDPERSLoginID)
        {
            DataTable NDPERSLoginID = busNeoSpinBase.Select("cdoContact.GetDuplicateNDPERSLoginID", new object[1] {astrNDPERSLoginID});
            if(NDPERSLoginID.Rows.Count> 1)
            {
                return true;
            }
            return false;
        }
        public bool IsContactWellnessCoordAndOther(int aintOrgID, int aintContactID)
        {
            DataTable ldtUserSecurity = busNeoSpinBase.Select("cdoContact.GetRolesByOrgIDandContactID", new object[2] { aintContactID, aintOrgID });
            //var x = ldtUserSecurity.AsEnumerable().Where(row => (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther)
            //    && (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAgent) || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAuthorizedAgent
            //    || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleFinance || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRolePrimaryAuthorizedAgent);
            if (ldtUserSecurity.Rows.Count == 0)
            {
                return false;
            }
            else if (ldtUserSecurity.Rows.Count == 1 && ldtUserSecurity.AsEnumerable().Where(row => row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther).Count() == 1)
            {
                return false;
            }
            else if (ldtUserSecurity.Rows.Count == 2 && ldtUserSecurity.AsEnumerable().Where(row => row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther).Count() == 2)
                return false;
            else
                return true;
        }
        //pir 7708
        public void GenerateCorrespondenceForSuccessfulRegistration(busOrgContact abusOrgContact)
        {
            ArrayList larrlist = new ArrayList();
            ibusOrgContact = abusOrgContact;
            larrlist.Add(this);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("ORG-1502", this, lhstDummyTable);
            Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML(); //PIR 16947 - Change to CorBuilderXML class
            lobjCorBuilder.InstantiateWord();
            lobjCorBuilder.CreateCorrespondenceFromTemplate("ORG-1502", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
            lobjCorBuilder.CloseWord();
        }
        private bool IsContactRoleExists(int aintContactID, string astrAnswer)
        {
            if (iclbOrgContact == null)
                LoadOrgContact();
            return iclbOrgContact.Any(i => i.icdoOrgContact.contact_id == aintContactID && i.icdoContactRole.contact_role_description.ToLower() == astrAnswer.ToLower());
        }

        public void LoadOrgContactByContactID(int aintContactID)
        {
            DataTable ldtbList = Select<cdoOrgContact>(
                new string[2] { "org_id", "contact_id" },
                new object[2] { icdoOrganization.org_id, aintContactID }, null, null);
            _iclbOrgContact = GetCollection<busOrgContact>(ldtbList, "icdoOrgContact");
            foreach (busOrgContact lobjOrgcontact in _iclbOrgContact)
            {
                lobjOrgcontact.LoadOrganization();
                lobjOrgcontact.LoadContact();
                lobjOrgcontact.LoadOrgAndContactPrimaryAddress();
                lobjOrgcontact.LoadPlan();
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {            
            if (this.iblnIsPAA)
            {
                VerifyAddressUsingUSPS();
            }
            base.BeforeValidate(aenmPageMode);
            //if (!iblnHasErrors)
            //{
            //    iblnIsUpdateAddressClicked = false;
            //    EvaluateInitialLoadRules();
            //}
        }
        
        /// <summary>
        /// Verify Address using Web Service and save after validation.
        /// </summary>
        /// <returns></returns>
        ///         
        public void VerifyAddressUsingUSPS()
        {
            ArrayList larrErrors = new ArrayList();

            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            //istrSuppressWarning = "N";
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                //address_validate_flag = busConstant.Flag_Yes;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.address_validate_flag = "Y";
                return;
            }
            //LoadOrgPrimaryAddress();             
            //LoadOrgContactPrimaryAddressByContact(ibusOrgContact.icdoOrgContact.contact_id);
            //ArrayList larrErrors = new ArrayList();
            //utlError iobjerror = new utlError();            
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = _ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = _ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = _ibusOrgPrimaryAddress.icdoOrgContactAddress.city;
            _lobjcdoWebServiceAddress.addr_state_value = _ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = _ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = _ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.city = _lobjcdoWebServiceAddressResult.addr_city;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                _ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            //address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            _ibusOrgPrimaryAddress.icdoOrgContactAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            _ibusOrgPrimaryAddress.icdoOrgContactAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;

            //return larrErrors.Add(this);
        }

        #region UCS - 032

        public bool iblnIsPAA { get; set; }

        public int iintContactID { get; set; }

        public busOrgContact ibusESSPrimaryOrgContact { get; set; }

        public void LoadESSPrimaryAuthorizedContact()
        {
            if (iclbOrgContact == null)
                LoadOrgContact();
            ibusESSPrimaryOrgContact = new busOrgContact { icdoContactRole = new cdoOrgContactRole(), icdoOrgContact = new cdoOrgContact() };
            ibusESSPrimaryOrgContact = iclbOrgContact.Where(o => o.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent &&
                                                                    o.icdoOrgContact.status_value == "ACTV")
                                                        .FirstOrDefault();
            if (ibusESSPrimaryOrgContact == null)
            {
                ibusESSPrimaryOrgContact = new busOrgContact { icdoContactRole = new cdoOrgContactRole(), icdoOrgContact = new cdoOrgContact() };
                ibusESSPrimaryOrgContact.ibusContact = new busContact { icdoContact = new cdoContact() };
            }
        }

        public void CheckFinanceRoleAndLoadOrgBank()
        {
            if (iclbOrgContact == null)
                LoadOrgContact();

            IEnumerable<busOrgContact> lenmOrgContact = iclbOrgContact.Where(o => o.icdoOrgContact.contact_id == iintContactID &&
                                                                                o.icdoContactRole.contact_role_value == busConstant.OrgContactRoleFinance);
            if (lenmOrgContact.Count() > 0 || iblnIsPAA)
                LoadOrgBank();
            else
                iclbOrgBank = new Collection<busOrgBank>();
        }
        #endregion


        //PIR 13171
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            iarrErrors = new ArrayList();
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromESS)
            {
                foreach (utlError lobjErr in iarrErrors)
                {
                    lobjErr.istrErrorID = string.Empty;
                }
            }
        }

        public bool iblnIsFromESS { get; set; }

		//PIR 18503
        public bool IsRoutingNumberAlreadyExists()
        {
            DataTable ldtRoutingNoExists = SelectWithOperator<cdoOrganization>(
                        new string[3] { "routing_no", "status_value", "org_type_value" },
                        new string[3] { "=" ,"=" , "="},
                        new object[3] { icdoOrganization.routing_no, busConstant.OrgBankStatusActive, busConstant.OrganizationTypeBank}, null);

            return ldtRoutingNoExists.Rows.Count > 0;
        }
		
        public bool FindOrganizationByRoutingNumber(string AintRoutingNumber)
        {
            _icdoOrganization = new cdoOrganization();
            DataTable ldtbOrganization = Select("cdoOrganization.GetBankWithActiveOrPendingStatus", new object[1] { AintRoutingNumber });
            if (ldtbOrganization.Rows.Count > 0)
            {
                _icdoOrganization.LoadData(ldtbOrganization.Rows[0]);
                return true;
            }
            return false;
        }

        //PIR 24663
        //start
        public Collection<cdoCodeValue> LoadEmployerStatusValueDropdown()
        {
            Collection<cdoCodeValue> lclbParticipationStatusValue = new Collection<cdoCodeValue>();
            DateTime ldtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            if (iclbOrgPlan == null)
                LoadOrgPlan();
            bool lblnPlanExists = iclbOrgPlan.Where(o => o.icdoOrgPlan.end_date_no_null > ldtBatchDate &&
                                                                     o.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement).Any();
            DataTable ldtbResult = Select<cdoCodeValue>(new string[1] { "CODE_ID" }, new object[1] { 310 }, null, null);
            ldtbResult = ldtbResult.AsEnumerable().Where(o => o.Field<string>("DATA1") == "Y").AsDataTable();
            if (ldtbResult.IsNotNull())
            {
                foreach (DataRow ldtr in ldtbResult.Rows)
                {
                    if ((Convert.ToString(ldtr["CODE_VALUE"]) == busConstant.EmploymentStatusContributing) && lblnPlanExists)
                    {
                        cdoCodeValue lcdoCV = new cdoCodeValue();
                        lcdoCV.LoadData(ldtr);
                        lclbParticipationStatusValue.Add(lcdoCV);
                    }
                    else if ((Convert.ToString(ldtr["CODE_VALUE"]) == busConstant.EmploymentStatusNonContributing))
                    {
                        cdoCodeValue lcdoCV = new cdoCodeValue();
                        lcdoCV.LoadData(ldtr);
                        lclbParticipationStatusValue.Add(lcdoCV);
                    }
                }
            }
            return lclbParticipationStatusValue;
        }
        //end
        public Collection<busOrgContact> iclbActiveOrgContactLOB { get; set; }
        public Collection<busOrgContact> iclbInactiveOrgContactLOB { get; set; }

        public Collection<cdoOrgContactRole> iclbOrgContactGroupByRole { get; set; }
        public void LoadActiveOrgContactsLOB()
        {
            if(iclbOrgContact.IsNull())
                LoadOrgContact();
            iclbActiveOrgContactLOB = new System.Collections.ObjectModel.Collection<busOrgContact>();
            List<busOrgContact> llstDistinctActiveOrgContact = iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusActive).ToList();
            llstDistinctActiveOrgContact = llstDistinctActiveOrgContact.GroupBy(CurrentEmployee => CurrentEmployee.icdoOrgContact.contact_id)
                                    .Select(OrgContactRecords => OrgContactRecords.First()).OrderBy(ContactID => ContactID.icdoOrgContact.contact_id).ToList();
            List <busOrgContact> llstActiveOrgContactSorted = iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusActive).OrderBy(i => i.icdoOrgContact.contact_id).ThenByDescending(i=>i.ibusPlan?.icdoPlan?.benefit_type_value).ToList();

            foreach (busOrgContact lobjOrgContactDistinct in llstDistinctActiveOrgContact)
            {
                lobjOrgContactDistinct.istrPlans = string.Empty;
                lobjOrgContactDistinct.istrContactRole = string.Empty;
                lobjOrgContactDistinct.istrStatusValue = string.Empty;
                lobjOrgContactDistinct.iintPAAG = 0;

                lobjOrgContactDistinct.ibusContact.LoadContactPrimaryAddress();
                lobjOrgContactDistinct.iintContactId = lobjOrgContactDistinct.ibusContact.icdoContact.contact_id;
                foreach (busOrgContact lobjOrgContact in llstActiveOrgContactSorted)
                {
                    if (lobjOrgContactDistinct.icdoOrgContact.contact_id == lobjOrgContact.icdoOrgContact.contact_id && lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusActive)
                    {
                        if (lobjOrgContact.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent && lobjOrgContactDistinct.iintPAAG != 1)
                            lobjOrgContactDistinct.iintPAAG = 1;
                        lobjOrgContact.ibusPlan.icdoPlan.plan_name = lobjOrgContact.ibusPlan.icdoPlan.plan_name.IsNullOrEmpty() ? string.Empty : lobjOrgContact.ibusPlan.icdoPlan.plan_name;
                        lobjOrgContact.icdoContactRole.contact_role_description = lobjOrgContact.icdoContactRole.contact_role_description.IsNullOrEmpty() ? string.Empty : lobjOrgContact.icdoContactRole.contact_role_description;
                        lobjOrgContact.icdoOrgContact.status_description = lobjOrgContact.icdoOrgContact.status_description.IsNullOrEmpty() ? string.Empty : lobjOrgContact.icdoOrgContact.status_description;

                        lobjOrgContactDistinct.istrPlans += lobjOrgContactDistinct.istrPlans.Contains(Convert.ToString(lobjOrgContact.ibusPlan.icdoPlan.plan_name)) ? "" : Convert.ToString(lobjOrgContact.ibusPlan.icdoPlan.plan_name) + ", ";
                        if(lobjOrgContact.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent)
                            lobjOrgContactDistinct.istrContactRole = lobjOrgContactDistinct.istrContactRole.Contains(Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description)) ? lobjOrgContactDistinct.istrContactRole : Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description) + ", " + lobjOrgContactDistinct.istrContactRole;
                        else 
                            lobjOrgContactDistinct.istrContactRole += lobjOrgContactDistinct.istrContactRole.Contains(Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description)) ? "" : Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description) + ", ";
                        lobjOrgContactDistinct.istrStatusValue += lobjOrgContactDistinct.istrStatusValue.Contains(Convert.ToString(lobjOrgContact.icdoOrgContact.status_description)) ? "" : Convert.ToString(lobjOrgContact.icdoOrgContact.status_description) + ", ";
                    }
                }
                //string removed last index of comma
                if (lobjOrgContactDistinct.istrPlans.Length > 0)
                    lobjOrgContactDistinct.istrPlans = lobjOrgContactDistinct.istrPlans.Remove(lobjOrgContactDistinct.istrPlans.LastIndexOf(','));
                if (lobjOrgContactDistinct.istrContactRole.Length > 0)
                    lobjOrgContactDistinct.istrContactRole = lobjOrgContactDistinct.istrContactRole.Remove(lobjOrgContactDistinct.istrContactRole.LastIndexOf(','));
                if (lobjOrgContactDistinct.istrStatusValue.Length > 0)
                    lobjOrgContactDistinct.istrStatusValue = lobjOrgContactDistinct.istrStatusValue.Remove(lobjOrgContactDistinct.istrStatusValue.LastIndexOf(','));

                iclbActiveOrgContactLOB.Add(lobjOrgContactDistinct);
            }
            iclbActiveOrgContactLOB = iclbActiveOrgContactLOB.OrderByDescending(i => i.iintPAAG).ThenBy(i => i.ibusContact.icdoContact.full_name).ToList().ToCollection();
        }
        public void LoadInactiveOrgContactsLOB()
        {
            if (iclbOrgContact.IsNull())
                LoadOrgContact();
            iclbInactiveOrgContactLOB = new System.Collections.ObjectModel.Collection<busOrgContact>();
            List<busOrgContact> llstDistinctInactiveOrgContact = iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusInActive).ToList();
            llstDistinctInactiveOrgContact = llstDistinctInactiveOrgContact.GroupBy(CurrentEmployee => CurrentEmployee.icdoOrgContact.contact_id)
                                    .Select(OrgContactRecords => OrgContactRecords.First()).OrderBy(ContactID => ContactID.icdoOrgContact.contact_id).ToList();
            List<busOrgContact> llstInactiveOrgContactSorted = iclbOrgContact.Where(i => i.icdoOrgContact.status_value == busConstant.OrgContactStatusInActive).OrderBy(i => i.icdoOrgContact.contact_id).ThenByDescending(i => i.ibusPlan?.icdoPlan?.benefit_type_value).ToList();

            foreach (busOrgContact lobjOrgContactDistinct in llstDistinctInactiveOrgContact)
            {
                lobjOrgContactDistinct.iintContactId = lobjOrgContactDistinct.ibusContact.icdoContact.contact_id;

                if (iclbActiveOrgContactLOB.IsNull())
                    iclbActiveOrgContactLOB = new System.Collections.ObjectModel.Collection<busOrgContact>();
                //skip the contacts which already include in Active tab
                if (iclbActiveOrgContactLOB.Any(objActiveOrg => objActiveOrg.iintContactId == lobjOrgContactDistinct.iintContactId))
                    continue;
                lobjOrgContactDistinct.istrPlans = string.Empty;
                lobjOrgContactDistinct.istrContactRole = string.Empty;
                lobjOrgContactDistinct.istrStatusValue = string.Empty;
                lobjOrgContactDistinct.iintPAAG = 0;

                lobjOrgContactDistinct.ibusContact.LoadContactPrimaryAddress();
                
                foreach (busOrgContact lobjOrgContact in llstInactiveOrgContactSorted)
                {
                    if (lobjOrgContactDistinct.icdoOrgContact.contact_id == lobjOrgContact.icdoOrgContact.contact_id && lobjOrgContact.icdoOrgContact.status_value == busConstant.OrgContactStatusInActive)
                    {
                        if (lobjOrgContact.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent && lobjOrgContactDistinct.iintPAAG != 1)
                            lobjOrgContactDistinct.iintPAAG = 1;
                        lobjOrgContact.ibusPlan.icdoPlan.plan_name = lobjOrgContact.ibusPlan.icdoPlan.plan_name.IsNullOrEmpty() ? string.Empty : lobjOrgContact.ibusPlan.icdoPlan.plan_name;
                        lobjOrgContact.icdoContactRole.contact_role_description = lobjOrgContact.icdoContactRole.contact_role_description.IsNullOrEmpty() ? string.Empty : lobjOrgContact.icdoContactRole.contact_role_description;
                        lobjOrgContact.icdoOrgContact.status_description = lobjOrgContact.icdoOrgContact.status_description.IsNullOrEmpty() ? string.Empty : lobjOrgContact.icdoOrgContact.status_description;

                        lobjOrgContactDistinct.istrPlans += lobjOrgContactDistinct.istrPlans.Contains(Convert.ToString(lobjOrgContact.ibusPlan.icdoPlan.plan_name)) ? "" : Convert.ToString(lobjOrgContact.ibusPlan.icdoPlan.plan_name) + ", ";
                        if (lobjOrgContact.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent)
                            lobjOrgContactDistinct.istrContactRole = lobjOrgContactDistinct.istrContactRole.Contains(Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description)) ? lobjOrgContactDistinct.istrContactRole : Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description) + ", " + lobjOrgContactDistinct.istrContactRole;
                        else
                            lobjOrgContactDistinct.istrContactRole += lobjOrgContactDistinct.istrContactRole.Contains(Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description)) ? "" : Convert.ToString(lobjOrgContact.icdoContactRole.contact_role_description) + ", ";
                        lobjOrgContactDistinct.istrStatusValue += lobjOrgContactDistinct.istrStatusValue.Contains(Convert.ToString(lobjOrgContact.icdoOrgContact.status_description)) ? "" : Convert.ToString(lobjOrgContact.icdoOrgContact.status_description) + ", ";
                    }
                }
                //string removed last index of comma
                if (lobjOrgContactDistinct.istrPlans.Length > 0)
                    lobjOrgContactDistinct.istrPlans = lobjOrgContactDistinct.istrPlans.Remove(lobjOrgContactDistinct.istrPlans.LastIndexOf(','));
                if (lobjOrgContactDistinct.istrContactRole.Length > 0)
                    lobjOrgContactDistinct.istrContactRole = lobjOrgContactDistinct.istrContactRole.Remove(lobjOrgContactDistinct.istrContactRole.LastIndexOf(','));
                if (lobjOrgContactDistinct.istrStatusValue.Length > 0)
                    lobjOrgContactDistinct.istrStatusValue = lobjOrgContactDistinct.istrStatusValue.Remove(lobjOrgContactDistinct.istrStatusValue.LastIndexOf(','));
                
                iclbInactiveOrgContactLOB.Add(lobjOrgContactDistinct);
            }
            iclbInactiveOrgContactLOB = iclbInactiveOrgContactLOB.OrderByDescending(i => i.iintPAAG).ThenBy(i => i.ibusContact.icdoContact.full_name).ToList().ToCollection();
        }
    }
}
