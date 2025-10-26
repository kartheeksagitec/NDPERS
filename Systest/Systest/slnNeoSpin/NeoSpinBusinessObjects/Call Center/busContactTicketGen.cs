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
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
    public partial class busContactTicket : busExtendBase
	{
		public busContactTicket()
		{

		}
        
		private cdoContactTicket _icdoContactTicket;
		public cdoContactTicket icdoContactTicket
		{
			get
			{
				return _icdoContactTicket;
			}

			set
			{
				_icdoContactTicket = value;
			}
		}
        private busContactTicketHistory _ibusContactTicketHistory;
        public busContactTicketHistory ibusContactTicketHistory
        {
            get
            {
                return _ibusContactTicketHistory;
            }

            set
            {
                _ibusContactTicketHistory = value;
            }
        }
        
		private Collection<busContactTicketHistory> _iclbContactTicketHistory;
		public Collection<busContactTicketHistory> iclbContactTicketHistory
		{
			get
			{
                return _iclbContactTicketHistory;
			}

			set
			{
                _iclbContactTicketHistory = value;
			}
		}
       
		public bool FindContactTicket(int Aintcontactticketid)
		{
			bool lblnResult = false;
			if (_icdoContactTicket == null)
			{
				_icdoContactTicket = new cdoContactTicket();
			}
			if (_icdoContactTicket.SelectRow(new object[1] { Aintcontactticketid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        public void LoadContactTicketHistory()
		{            
			DataTable ldtbList = Select<cdoContactTicketHistory>(
				new string[1] { "contact_ticket_id" },
				new object[1] { icdoContactTicket.contact_ticket_id }, null,"contact_ticket_history_id");
            _iclbContactTicketHistory = GetCollection<busContactTicketHistory>(ldtbList, "icdoContactTicketHistory");            
            foreach (busContactTicketHistory lobjbusContactTicketHistory in _iclbContactTicketHistory)
            {
                busContactTicketHistory lobjContactTicketHistory = lobjbusContactTicketHistory;
                lobjContactTicketHistory.LoadUser();
            }
        }

        //venkat update entity - no reference in entity found
        private busBpmCaseInstance _ibusProcessInstance;
        public busBpmCaseInstance ibusProcessInstance
        {
            get
            {
                return _ibusProcessInstance;
            }
            set
            {
                _ibusProcessInstance = value;
            }
        }

        public void LoadProcessInstance()
        {
            if (_ibusProcessInstance == null)
            {
                _ibusProcessInstance = ClassMapper.GetObject<busBpmCaseInstance>();
            }
            ((busSolBpmCaseInstance)_ibusProcessInstance).FindProcessInstanceByContactTicket(_icdoContactTicket.contact_ticket_id);
        }
        
	}
}
