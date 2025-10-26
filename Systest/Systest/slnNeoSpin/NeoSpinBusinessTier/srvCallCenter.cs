#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Globalization;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvCallCenter : srvNeoSpin
    {
        public srvCallCenter()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public busContactTicket NewContactTicket(int AintContactTicketid, int AintPersonId, string AintOrgCodeId)
        {
            busContactTicket lobjContactTicket = null;

            if (AintContactTicketid == 0)
            {
                lobjContactTicket = new busContactTicket();
                lobjContactTicket.icdoContactTicket = new cdoContactTicket();
                if (AintPersonId != 0)
                {
                    lobjContactTicket.icdoContactTicket.person_id = AintPersonId;
                    lobjContactTicket.LoadPerson();
                }
                if (AintOrgCodeId != String.Empty)
                {
                    lobjContactTicket.icdoContactTicket.istrOrgCodeID = AintOrgCodeId;
                    lobjContactTicket.icdoContactTicket.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(AintOrgCodeId);
                    lobjContactTicket.icdoContactTicket.org_id = lobjContactTicket.icdoContactTicket.org_id;
                    lobjContactTicket.LoadOrganization();
                }
                lobjContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();
                lobjContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule = new cdoAppointmentSchedule();
                lobjContactTicket.ibusBenefitEstimate = new busBenefitEstimate();
                lobjContactTicket.ibusBenefitEstimate.icdoBenefitEstimate = new cdoBenefitEstimate();
                lobjContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();
                lobjContactTicket.ibusDeathNotice = new busDeathNotice();
                lobjContactTicket.ibusDeathNotice.icdoDeathNotice = new cdoDeathNotice();
                lobjContactTicket.ibusNewGroup = new busNewGroup();
                lobjContactTicket.ibusNewGroup.icdoNewGroup = new cdoNewGroup();
                lobjContactTicket.ibusNewGroup.iclcNewGroupPlanType = new utlCollection<cdoNewGroupPlanType>();
                lobjContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase();
                lobjContactTicket.ibusContactMgmtServicePurchase.icdoContactMgmtServicePurchase = new cdoContactMgmtServicePurchase();
                lobjContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
                lobjContactTicket.ibusContactMgmtServicePurchase.LoadSerPurType();
                lobjContactTicket.ibusSeminarSchedule = new busSeminarSchedule();
                lobjContactTicket.ibusSeminarSchedule.icdoSeminarSchedule = new cdoSeminarSchedule();
                lobjContactTicket.EvaluateInitialLoadRules();
            }
            else
            {
                lobjContactTicket = FindContactTicket(AintContactTicketid);
                lobjContactTicket.ibusBenefitEstimate.iclcRetirementType.ForEach(ret => ret.contact_ticket_retirement_type_id = 0);
                if (lobjContactTicket.icdoContactTicket.status_value == busConstant.ContactTicketStatusOpen)
                {
                    //Update the OLD Contact Ticket with the Following Details.
                    lobjContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusClosed;
                    lobjContactTicket.icdoContactTicket.copy_status_flag = "Y";
                    lobjContactTicket.icdoContactTicket.Update();

                    //Creating the New Ticket
                    lobjContactTicket.icdoContactTicket.contact_ticket_id = 0;
                    lobjContactTicket.icdoContactTicket.original_contact_ticket_id = AintContactTicketid;
                    lobjContactTicket.icdoContactTicket.status_value = busConstant.ContactTicketStatusOpen;
                    lobjContactTicket.icdoContactTicket.copy_status_flag = "N";
                    lobjContactTicket.icdoContactTicket.assign_to_user_id = 0;
                    lobjContactTicket.icdoContactTicket.notes = String.Empty;
                    lobjContactTicket.iclbContactTicketHistory = null;
                    lobjContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;

                    if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
                    {
                        if (lobjContactTicket.ibusAppointmentSchedule == null)
                        {
                            lobjContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();
                        }
                        lobjContactTicket.ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(AintContactTicketid);
                        lobjContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.appointment_schedule_id = 0;
                        lobjContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.ienuObjectState = ObjectState.Insert;                        
                    }
                }
                else if (lobjContactTicket.icdoContactTicket.status_value == busConstant.ContactTicketStatusClosed)
                {
                    lobjContactTicket.icdoContactTicket.copy_status_flag = "N";
                    lobjContactTicket.icdoContactTicket.ienuObjectState = ObjectState.Insert;
                }                
                lobjContactTicket.EvaluateInitialLoadRules();
            }
            lobjContactTicket.iblnIsFromInternal = true;
            return lobjContactTicket;
        }
        public busContactTicket FindContactTicket(int Aintcontactticketid)
        {
            busContactTicket lobjContactTicket = new busContactTicket();
            lobjContactTicket.icdoContactTicket = new cdoContactTicket();
            lobjContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule();
            lobjContactTicket.ibusBenefitEstimate = new busBenefitEstimate();
            lobjContactTicket.ibusDeathNotice = new busDeathNotice();
            lobjContactTicket.ibusNewGroup = new busNewGroup();
            lobjContactTicket.ibusSeminarSchedule = new busSeminarSchedule();
            lobjContactTicket.ibusSeminarSchedule.icdoSeminarSchedule = new cdoSeminarSchedule();
            //Creating a Collection Instance in order to correspondence exception
            lobjContactTicket.ibusSeminarSchedule.iclbSeminarAttendeeDetail = new Collection<busSeminarAttendeeDetail>();
            lobjContactTicket.ibusContactMgmtServicePurchase = new busContactMgmtServicePurchase();
            lobjContactTicket.ibusBenefitEstimate.iclcRetirementType = new utlCollection<cdoBenefitEstimateRetirementType>();
            lobjContactTicket.ibusNewGroup.iclcNewGroupPlanType = new utlCollection<cdoNewGroupPlanType>();
            lobjContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();

            if (lobjContactTicket.FindContactTicket(Aintcontactticketid))
            {
                if (lobjContactTicket.icdoContactTicket.org_id != 0)
                {
                    lobjContactTicket.icdoContactTicket.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjContactTicket.icdoContactTicket.org_id);
                }
                lobjContactTicket.icdoContactTicket.notes = String.Empty;
                lobjContactTicket.LoadContactTicketHistory();
                if (lobjContactTicket.icdoContactTicket.person_id != 0)
                {
                    lobjContactTicket.LoadPerson();
                }
                else
                {
                    lobjContactTicket.LoadOrganization();
                }
                //if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
                //{
                    lobjContactTicket.ibusAppointmentSchedule.FindAppointmentScheduleByContactTicket(Aintcontactticketid);
                    //PIR -272
                    lobjContactTicket.LoadAppointmentCounselorName();
                //}
                if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeRetBenefitEstimate)
                {
                    if (lobjContactTicket.ibusBenefitEstimate.FindBenefitEstimateByContactTicket(Aintcontactticketid))
                        lobjContactTicket.ibusBenefitEstimate.LoadRetirementType();
                }
                if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeDeath)
                {
                    lobjContactTicket.ibusDeathNotice.FindDeathNoticeByContactTicket(Aintcontactticketid);
                     //PIR-17314
                    if (lobjContactTicket.ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions != DateTime.MinValue)
                    {
                        lobjContactTicket.ibusDeathNotice.icdoDeathNotice.reporting_Month = lobjContactTicket.ibusDeathNotice.icdoDeathNotice.last_reporting_month_for_retirement_contributions.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    else
                    {
                        lobjContactTicket.ibusDeathNotice.icdoDeathNotice.reporting_Month = String.Empty;
                    }
                }

                //In Order to Avoid Exception we need to load the utlCollection Error for all the contact types
                if (lobjContactTicket.ibusContactMgmtServicePurchase.FindServicePurchaseByContactTicket(Aintcontactticketid))
                {
                    lobjContactTicket.ibusContactMgmtServicePurchase.LoadServiceRollInfo();
                    lobjContactTicket.ibusContactMgmtServicePurchase.LoadSerPurServiceTypeAfterInserting();
                }
                else
                {
                    lobjContactTicket.ibusContactMgmtServicePurchase.iclcSerPurRolloverInfo = new utlCollection<cdoSerPurRolloverInfo>();
                    lobjContactTicket.ibusContactMgmtServicePurchase.LoadServiceRollInfo();
                    lobjContactTicket.ibusContactMgmtServicePurchase.LoadSerPurType();
                }

                //In Order to Avoid Exception we need to load the utlCollection Error for all the contact types
                if (lobjContactTicket.ibusNewGroup.FindNewGroupByContactTicket(Aintcontactticketid))
                {
                    lobjContactTicket.ibusNewGroup.LoadPlanTypes();
                }
                else
                {
                    lobjContactTicket.ibusNewGroup.iclcNewGroupPlanType = new utlCollection<cdoNewGroupPlanType>();
                }
                if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeSeminarAndCounselingOutReach)
                {
                    lobjContactTicket.ibusSeminarSchedule.FindSeminarScheduleByContactTicket(Aintcontactticketid);
                    lobjContactTicket.ibusSeminarSchedule.LoadSeminarAttendeeDetail();
                    lobjContactTicket.ibusSeminarSchedule.LoadGuestSpeakers();
                    lobjContactTicket.LoadSeminarFacilitatorName();
                }

                //If the Contact Type is Workflow , we would need to load the Seminar Objects
                if (lobjContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeWorkflow)
                {
                    lobjContactTicket.ibusSeminarSchedule.FindSeminarScheduleByContactTicket(Aintcontactticketid);
                    lobjContactTicket.ibusSeminarSchedule.LoadSeminarAttendeeDetail();
                    lobjContactTicket.ibusSeminarSchedule.LoadGuestSpeakers();
                    lobjContactTicket.LoadSeminarFacilitatorName();
                }
            }
            lobjContactTicket.iblnIsFromInternal = true;
            return lobjContactTicket;
        }

        public busContactTicketLookup LoadContactTickets(DataTable adtbSearchResult)
        {
            busContactTicketLookup lobjContactTicketLookup = new busContactTicketLookup();
            lobjContactTicketLookup.LoadContactTicket(adtbSearchResult);
            return lobjContactTicketLookup;
        }

        public busContactTicketHistory FindContactTicketHistory(int Aintcontacttickethistoryid)
        {
            busContactTicketHistory lobjContactTicketHistory = new busContactTicketHistory();
            if (lobjContactTicketHistory.FindContactTicketHistory(Aintcontacttickethistoryid))
            {
            }
            return lobjContactTicketHistory;
        }

        public busSeminarAttendeeDetail NewSeminarAttendeeDetail(int AintSeminarScheduleId)
        {
            busSeminarAttendeeDetail lobjSeminarAttendee = new busSeminarAttendeeDetail();
            lobjSeminarAttendee.icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail();
            lobjSeminarAttendee.icdoSeminarAttendeeDetail.seminar_schedule_id = AintSeminarScheduleId;
            lobjSeminarAttendee.LoadSeminarSchedule();
            lobjSeminarAttendee.LoadGuestSpeakers();
            lobjSeminarAttendee.LoadSeminarAttendeeDetail();
            lobjSeminarAttendee.LoadSponsorName();
            lobjSeminarAttendee.LoadDisplayAttendeeName();
            lobjSeminarAttendee.LoadOrganization();
            lobjSeminarAttendee.iclcRetirementType = new utlCollection<cdoSeminarAttendeeDetailRetirementType>();
            return lobjSeminarAttendee;
        }

        public busSeminarAttendeeDetail FindSeminarAttendeeDetail(int Aintseminarattendeedetailid)
        {
            busSeminarAttendeeDetail lobjSeminarAttendeeDetail = new busSeminarAttendeeDetail();
            if (lobjSeminarAttendeeDetail.FindSeminarAttendeeDetail(Aintseminarattendeedetailid))
            {
                lobjSeminarAttendeeDetail.LoadSeminarSchedule();
                lobjSeminarAttendeeDetail.LoadSponsorName();
                lobjSeminarAttendeeDetail.LoadPerson();
                //PIR - 279 - 283
                lobjSeminarAttendeeDetail.LoadContact();
                lobjSeminarAttendeeDetail.ibusContact.LoadContactPrimaryAddress();
                //lobjSeminarAttendeeDetail.LoadOrganization();
                lobjSeminarAttendeeDetail.LoadSeminarAttendeeDetail();
                lobjSeminarAttendeeDetail.LoadGuestSpeakers();
                lobjSeminarAttendeeDetail.LoadDisplayAttendeeName();
                lobjSeminarAttendeeDetail.LoadSeminarFacilitatorName();
                lobjSeminarAttendeeDetail.LoadRetirementType();
                lobjSeminarAttendeeDetail.LoadSeminarAttendeePaymentAllocation();
                lobjSeminarAttendeeDetail.LoadTotalPaidFeeAmount();
                //this will be used for correspondence
                if (lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.contact_id != 0)
                {
                    lobjSeminarAttendeeDetail.ibusContact.LoadContactAddressForDeferredCompAgent(lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.contact_id);
                }

                if (lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.guest_speaker_flag == busConstant.Flag_Yes)
                    lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.guest_speaker_contact_id = lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.contact_id;

                if (lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.org_to_bill_id > 0)
                {
                    lobjSeminarAttendeeDetail.LoadOrgToBillOrganization();
                    lobjSeminarAttendeeDetail.icdoSeminarAttendeeDetail.org_to_bill_org_code = lobjSeminarAttendeeDetail.ibusOrgToBillOrganization.icdoOrganization.org_code;
                }
            }
            return lobjSeminarAttendeeDetail;
        }
    }
}
