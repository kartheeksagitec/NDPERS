using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMaritalStatusChangeWeb : busExtendBase
    {
        //public string istrCurrentMaritalStatus { get; set; }
        public busContactTicket ibusDeathNoticeContactTicket { get; set; }
        public Collection<busMSSPersonContact> iclbMSSPersonContact { get; set; }

        public busPerson ibusPerson { get; set; }

        public string istrOldMaritalStatus { get; set; }

        public bool iblnChangeNameOnly { get; set; } // PIR 10956

        public int intEligibleInsurancePlans { get; set; }

        public int intEnrolledInsurancePlans { get; set; }

        //enhancement 6878
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusPerson.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                string lstrConfimation = string.Format(busGlobalFunctions.GetMessageTextByMessageID(8566, iobjPassInfo), luserName, Now);
                return lstrConfimation;
            }
        }

        public void LoadPerson(int aintPersonId)
        {
            if (ibusPerson.IsNull())
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(aintPersonId);
        }

        // PIR 10956
        public bool IsChangeNameOnly()
        {
            if (iblnChangeNameOnly)
                return true;
            else
                return false;
        }

        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            if (astrWizardStepName == "wzsStep")
            {
                if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusWidow)
                    SetDeathNoticeContactTicket();
            }

            if (astrWizardStepName == "wzsStep1")
            {
                if (iclbMSSPersonContact.Count > 0)
                {
                    foreach (busMSSPersonContact lobjPersonContact in iclbMSSPersonContact)
                    {
                        lobjPersonContact.GetPersonId();
                        if (lobjPersonContact.icdoPersonContact.contact_person_id == 0)
                            lobjPersonContact.icdoPersonContact.istrMSSContactName = lobjPersonContact.icdoPersonContact.istrMSSNamePrefix + " " + lobjPersonContact.icdoPersonContact.istrMSSLastName + ", " +
                                                                                     lobjPersonContact.icdoPersonContact.istrMSSFirstName + " " + lobjPersonContact.icdoPersonContact.istrMSSMiddleName + " " +
                                                                                     lobjPersonContact.icdoPersonContact.istrMSSNameSuffix;
                    }
                }
            }

            if (astrWizardStepName == "wzsStep2")
            {
                ibusPerson.icdoPerson.marital_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(306, ibusPerson.icdoPerson.marital_status_value);
            }            
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }
        public override void ProcessWizardData(utlWizardNavigationEventArgs we, string astrWizardName, string astrWizardStepName)
        {
           
                busMSSInsurancePlansWeb lbusInsurancePlans = new busMSSInsurancePlansWeb();
                lbusInsurancePlans.LoadPerson(ibusPerson.icdoPerson.person_id);
                lbusInsurancePlans.LoadEnrolledEligibleInsurancePlans();
                intEligibleInsurancePlans = lbusInsurancePlans.iclbBenefitEligibleInsurancePlans.Count();
                intEnrolledInsurancePlans = lbusInsurancePlans.iclbBenefitEnrolledInsurancePlans.Count();
           

            base.ProcessWizardData(we, astrWizardName, astrWizardStepName);
        }
        public bool IsMaritalStatusNotChanged()
        {
            if (istrOldMaritalStatus == ibusPerson.icdoPerson.marital_status_value)
                return true;
            return false;
        }

        //BR-024-3a
        //is marital status change is valid
        //this is used to launch the next step of wizard
        public bool IsMaritalStatusChangeValid()
        {
            if (istrOldMaritalStatus == busConstant.PersonMaritalStatusMarried)
            {
                if ((ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced)
                    || (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusWidow))
                {
                    return true;
                }
            }
            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PayeeAccountMaritalStatusMarried)
            {
                return true;
            }
            return false;
        }

        //BR-024-3b
        //is marital status change is changed from married to single      
        public bool IsMaritalStatusChangedFromMarriedToSingle()
        {
            if (istrOldMaritalStatus == busConstant.PersonMaritalStatusMarried)
            {
                if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                {
                    return true;
                }
            }
            return false;
        }

        //set the inactive status to contact with relationship as Spouse
        //for Marital status wizard
        private void UpdateContactStatusOfSpouseToInActive()
        {
            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced
                || ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusWidow)
            {
                foreach (busPersonContact lobjPersonContact in iclbMSSPersonContact)
                {
                    if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                        && (lobjPersonContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
                    {
                        lobjPersonContact.icdoPersonContact.relationship_value = busConstant.PersonBeneficiaryRelationshipExSpouse;
                        lobjPersonContact.icdoPersonContact.status_value = busConstant.StatusInActive;
                        lobjPersonContact.icdoPersonContact.Update();
                    }
                }
            }
        }

        //is active spouse added when status changed to married
        public bool IsSpouseExists()
        {
            if (istrOldMaritalStatus == busConstant.PersonMaritalStatusSingle)
            {
                if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                {
                    if (iclbMSSPersonContact.Where(lobjContact => lobjContact.icdoPersonContact.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse
                        && lobjContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive).Count() == 0)
                        return true;
                }
            }
            return false;
        }

        public void LoadMSSPersonContacts()
        {
            iclbMSSPersonContact = new Collection<busMSSPersonContact>();

            if (ibusPerson.icolPersonContact.IsNull())
                ibusPerson.LoadContacts();

            foreach (busPersonContact lobjPersonContact in ibusPerson.icolPersonContact)
            {
                busMSSPersonContact lobjNewPersonContact = new busMSSPersonContact();
                lobjNewPersonContact.icdoPersonContact = new cdoPersonContact();
                lobjNewPersonContact.iobjMainCDO = lobjPersonContact.icdoPersonContact;
                lobjNewPersonContact.icdoPersonContact = lobjPersonContact.icdoPersonContact;
                lobjNewPersonContact.ibusContactPerson = lobjPersonContact.ibusContactPerson;
                if (lobjNewPersonContact.icdoPersonContact.contact_person_id > 0)
                {
                    lobjNewPersonContact.icdoPersonContact.istrMSSFirstName = lobjPersonContact.ibusContactPerson.icdoPerson.first_name;
                    lobjNewPersonContact.icdoPersonContact.istrMSSNamePrefix = lobjPersonContact.ibusContactPerson.icdoPerson.name_prefix_value;
                    lobjNewPersonContact.icdoPersonContact.istrMSSNameSuffix = lobjPersonContact.ibusContactPerson.icdoPerson.name_suffix_value;
                    lobjNewPersonContact.icdoPersonContact.istrMSSLastName = lobjPersonContact.ibusContactPerson.icdoPerson.last_name;
                    lobjNewPersonContact.icdoPersonContact.istrMSSMiddleName = lobjPersonContact.ibusContactPerson.icdoPerson.middle_name;
                    lobjNewPersonContact.icdoPersonContact.istrMSSSSN = lobjPersonContact.ibusContactPerson.icdoPerson.ssn;
                    lobjNewPersonContact.icdoPersonContact.idtMSSContactDOB = lobjPersonContact.ibusContactPerson.icdoPerson.date_of_birth;
                    lobjNewPersonContact.icdoPersonContact.istrMSSGender = lobjPersonContact.ibusContactPerson.icdoPerson.gender_value;
                    lobjNewPersonContact.icdoPersonContact.iintMSSPersonContactId = lobjNewPersonContact.icdoPersonContact.person_contact_id;
                }
                else
                    lobjNewPersonContact.icdoPersonContact.istrMSSFirstName = lobjPersonContact.icdoPersonContact.lstrContactName;
                //to avoid error in updating the grid
                //lobjNewPersonContact.icdoPersonContact.ienuObjectState = ObjectState.None;
                // lobjNewPersonContact.icdoPersonContact.person_contact_id = 0;
                iclbMSSPersonContact.Add(lobjNewPersonContact);
            }
        }

        private void SetDeathNoticeContactTicket()
        {
            ibusDeathNoticeContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
            ibusDeathNoticeContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeDeath;
            ibusDeathNoticeContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
            ibusDeathNoticeContactTicket.ibusDeathNotice = new busDeathNotice { icdoDeathNotice = new cdoDeathNotice() };
            if (ibusPerson.icolPersonContact.IsNull()) ibusPerson.LoadActiveContacts();
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            foreach (busPersonContact lobjContact in ibusPerson.icolPersonContact)
            {
                if (lobjContact.icdoPersonContact.relationship_value == busConstant.FamilyRelationshipSpouse &&
                    lobjContact.icdoPersonContact.contact_person_id > 0)
                {
                    lobjContact.LoadContactPerson();
                    lobjPerson = lobjContact.ibusContactPerson;
                }
            }
            if (lobjPerson.icdoPerson.person_id == 0) lobjPerson = ibusPerson;
            ibusDeathNoticeContactTicket.icdoContactTicket.person_id = lobjPerson.icdoPerson.person_id;
            ibusDeathNoticeContactTicket.ibusPerson = new busPerson();
            ibusDeathNoticeContactTicket.ibusPerson = lobjPerson;
            ibusDeathNoticeContactTicket.icdoContactTicket.contact_method_value = busConstant.ResponseMethodWeb;
            ibusDeathNoticeContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
            ibusDeathNoticeContactTicket.iblnIsFromPortal = true;
            ibusDeathNoticeContactTicket.iblnIsFromMSS = true;
            ibusDeathNoticeContactTicket.icdoContactTicket.is_ticket_created_from_portal_flag = busConstant.Flag_Yes;
            ibusDeathNoticeContactTicket.icdoContactTicket.contact_type_value = busConstant.ContactTicketTypeDeath;
        }

        private void ValidateDeathNoticeContactTicket()
        {
            ibusDeathNoticeContactTicket.BeforeValidate(utlPageMode.New);
            ibusDeathNoticeContactTicket.ValidateHardErrors(utlPageMode.New);
            if (ibusDeathNoticeContactTicket.iarrErrors.Count > 0)
            {
                foreach (utlError lerror in ibusDeathNoticeContactTicket.iarrErrors)
                {
                    iarrErrors.Add(lerror);
                }
            }
        }

        private void ValidatePersonContacts()
        {
            bool lblnErrorFound = false;
            foreach (busPersonContact lobjPersonContact in iclbMSSPersonContact)
            {
                lobjPersonContact.ibusPerson = ibusPerson;
                lobjPersonContact.iblnIsFromMSS = true;
                if (lobjPersonContact.icdoPersonContact.person_contact_id == 0)
                {
                    lobjPersonContact.BeforeValidate(utlPageMode.New);
                    lobjPersonContact.ValidateHardErrors(utlPageMode.New);
                }
                else
                {
                    lobjPersonContact.BeforeValidate(utlPageMode.All);
                    lobjPersonContact.ValidateHardErrors(utlPageMode.All);
                }
                if (lobjPersonContact.iarrErrors.Count > 0)
                {
                    foreach (utlError lerror in lobjPersonContact.iarrErrors)
                    {
                        lblnErrorFound = true;
                        iarrErrors.Add(lerror);
                        break;
                    }
                }
                if (lblnErrorFound)
                    break;
            }
        }

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            if (astrGroupName == "Step2")
                ValidateDeathNoticeContactTicket();
            if (astrGroupName == "Step3")
            {
                ValidatePersonContacts();
                ibusPerson.icdoPerson.PopulateDescriptions();
            }

            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            if (iarrErrors.IsNotNull())
            {
                foreach (utlError lobjError in iarrErrors)
                    lobjError.istrErrorID = string.Empty;
            }
        }

        public override int PersistChanges()
        {
            PersistsPersonChanges();

            PersistsDeathNoticeContactTicket();

            PersistsContacts();

            //update the contact status to in active and relation to ex spouse
            UpdateContactStatusOfSpouseToInActive();

            return 0;
        }

        private void PersistsDeathNoticeContactTicket()
        {
            if (iarrChangeLog.Count > 0)
            {
                foreach (object lobj in iarrChangeLog)
                {
                    if (lobj is cdoDeathNotice)
                    {
                        ibusDeathNoticeContactTicket.BeforePersistChanges();
                        ibusDeathNoticeContactTicket.PersistChanges();
                        ibusDeathNoticeContactTicket.AfterPersistChanges();
                    }
                }
            }
        }

        private void PersistsContacts()
        {
            if (iarrChangeLog.Count > 0)
            {
                foreach (object lobj in iarrChangeLog)
                {
                    if (lobj is cdoPersonContact)
                    {
                        busMSSPersonContact lobjPersonContact = new busMSSPersonContact { icdoPersonContact = new cdoPersonContact() };
                        lobjPersonContact.icdoPersonContact = (cdoPersonContact)lobj;
                        lobjPersonContact.icdoPersonContact.person_id = ibusPerson.icdoPerson.person_id;
                        lobjPersonContact.icdoPersonContact.ienuObjectState = ObjectState.Insert;
                        if (lobjPersonContact.icdoPersonContact.person_contact_id > 0)
                            lobjPersonContact.icdoPersonContact.ienuObjectState = ObjectState.Update;
                        lobjPersonContact.BeforePersistChanges();
                        if (lobjPersonContact.icdoPersonContact.contact_person_id == 0)
                            lobjPersonContact.icdoPersonContact.contact_name = lobjPersonContact.icdoPersonContact.istrMSSNamePrefix + " "
                                                                             + lobjPersonContact.icdoPersonContact.istrMSSLastName + ", " +
                                                                                 lobjPersonContact.icdoPersonContact.istrMSSFirstName + " " +
                                                                                 lobjPersonContact.icdoPersonContact.istrMSSMiddleName + " " +
                                                                                 lobjPersonContact.icdoPersonContact.istrMSSNameSuffix;
                        else
                        {
                            if (lobjPersonContact.ibusContactPerson.IsNull())
                                lobjPersonContact.LoadContactPerson();
                            //set marital status
                            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                            {
                                lobjPersonContact.ibusContactPerson.icdoPerson.marital_status_value = busConstant.PersonMaritalStatusMarried;
                                lobjPersonContact.ibusContactPerson.icdoPerson.Update();
                            }
                        }
                        lobjPersonContact.PersistChanges();
                        lobjPersonContact.AfterPersistChanges();
                    }
                }
            }
        }

        private void PersistsPersonChanges()
        {
            if (iarrChangeLog.Count > 0)
            {
                foreach (object lobj in iarrChangeLog)
                {
                    if (lobj is cdoPerson)
                    {
                        cdoPerson lobjPerson = (cdoPerson)lobj;
                        //PIR 24927 - Update ms_change_batch_flag to Y if there is change in martital status. Skip members with DOD.
                        if (!(string.IsNullOrEmpty(istrOldMaritalStatus)) && istrOldMaritalStatus != lobjPerson.marital_status_value && lobjPerson.date_of_death == DateTime.MinValue
                            && lobjPerson.marital_status_value != busConstant.PersonMaritalStatusWidow)
                        {
                            //PIR 21454 system-generated letter for marital status changes doesn't generate in all instances //ms_change_batch_flag should update from MSS as well
                            busPerson lobjbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lobjbusPerson.icdoPerson = lobjPerson;
                            if (lobjbusPerson.IsMember() || lobjbusPerson.IsRetiree() || lobjbusPerson.IsPayee())
                            {
                                lobjbusPerson.UpdateMSChangeBatchFlag();
                            }
                            lobjPerson.ms_change_batch_flag = lobjbusPerson.icdoPerson.ms_change_batch_flag;
                        }
                        lobjPerson.Update();
                    }
                }
            }
        }

        public bool IsMultipleSpouseExists()
        {
            var lenumSpouseCount = iclbMSSPersonContact.Where(lobjPC => lobjPC.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                && lobjPC.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive).Count();
            if (lenumSpouseCount > 1)
                return true;
            return false;
        }
    }
}