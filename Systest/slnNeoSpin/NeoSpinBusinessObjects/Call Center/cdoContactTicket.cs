#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoContactTicket : doContactTicket
	{
		public cdoContactTicket() : base()
		{
        }

        private string _istrOrgCodeID;

        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }

        /// <summary>
        /// Flag to update in History Table.
        /// </summary>
        private bool _NeedHistory;
        public bool NeedHistory
        {
            get { return _NeedHistory; }
            set { _NeedHistory = value; }
        }
        public string istrPublishToWss { get; set; } = "N";  //19351
        public override int Insert()
        {
            int lintResult = base.Insert();
            InsertintoContactTicketHistory();
            return lintResult;
        }
        public override int Update()
        {
            base.Update();
            if (_NeedHistory)
            {
                InsertintoContactTicketHistory();
            }
            return 1;
        }
        
        public void InsertintoContactTicketHistory()
        {
            cdoContactTicketHistory lobjContactTicketHistory = new cdoContactTicketHistory();
            lobjContactTicketHistory.contact_ticket_id = contact_ticket_id;
            lobjContactTicketHistory.person_id = person_id;
            lobjContactTicketHistory.org_id = org_id;
            lobjContactTicketHistory.status_id = status_id;
            lobjContactTicketHistory.status_value = status_value;
            lobjContactTicketHistory.callback_phone = callback_phone;
            lobjContactTicketHistory.callback_phone_2 = callback_phone_2;
            lobjContactTicketHistory.extn = extn;
            lobjContactTicketHistory.extn2 = extn2;
            lobjContactTicketHistory.email = email;
            lobjContactTicketHistory.contact_type_id = contact_type_id;
            lobjContactTicketHistory.contact_type_value = contact_type_value;
            lobjContactTicketHistory.event_type_id = event_type_id;
            lobjContactTicketHistory.event_type_value = event_type_value;
            lobjContactTicketHistory.caller_name = caller_name;
            lobjContactTicketHistory.caller_relationship_id = caller_relationship_id;
            lobjContactTicketHistory.caller_relationship_value = caller_relationship_value;
            lobjContactTicketHistory.contact_method_id = contact_method_id;
            lobjContactTicketHistory.contact_method_value = contact_method_value;
            lobjContactTicketHistory.response_method_id = response_method_id;
            lobjContactTicketHistory.response_method_value = response_method_value;
            lobjContactTicketHistory.assign_to_user_id = assign_to_user_id;
            lobjContactTicketHistory.notes = notes;
            lobjContactTicketHistory.ticket_type_id = ticket_type_id;
            lobjContactTicketHistory.ticket_type_value = ticket_type_value;
            lobjContactTicketHistory.copy_status_flag = copy_status_flag;
            lobjContactTicketHistory.original_contact_ticket_id = original_contact_ticket_id;
            lobjContactTicketHistory.created_by = created_by;
            lobjContactTicketHistory.modified_by = iobjPassInfo.istrUserID;
            lobjContactTicketHistory.created_date = DateTime.Now; //PIR 23792
            lobjContactTicketHistory.modified_date = DateTime.Now;
            lobjContactTicketHistory.publish_wss = istrPublishToWss;
            lobjContactTicketHistory.Insert();
        }

        //Portal Property - Launch Portal Screen based on Contact Ticket Type
        public int PortalLaunchScreenID
        {
            get
            {
                int lintResult = 99;
                switch(contact_type_value)
                {
                    case busConstant.ContactTicketTypeDeath:
                        lintResult = 1;
                        break;
                    case busConstant.ContactTicketTypeAppointment:
                        lintResult = 2;
                        break;
                    case busConstant.ContactTicketTypeRetBenefitEstimate:
                        lintResult = 3;
                        break;
                    case busConstant.ContactTicketTypeRetPurchases:
                        lintResult = 4;
                        break;
                    case busConstant.ContactTicketTypeSeminarAndCounselingOutReach:
                        lintResult = 5;
                        break;
                    case busConstant.ContactTicketTypeOtherProblem:
                    case busConstant.ContactTicketTypeInsuranceProblem:
                    case busConstant.ContactTicketTypeDefCompProblem:
                    case busConstant.ContactTicketTypeRetProblem:
                        lintResult = 6;
                        break;
                }                
                return lintResult;
            }
        }        
    }
}
