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

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public partial class busContactTicketHistory : busExtendBase
    {
		public busContactTicketHistory()
		{

		} 

		private cdoContactTicketHistory _icdoContactTicketHistory;
		public cdoContactTicketHistory icdoContactTicketHistory
		{
			get
			{
				return _icdoContactTicketHistory;
			}

			set
			{
				_icdoContactTicketHistory = value;
			}
		}

        public bool FindContactTicketHistory(int Aintcontacttickethistoryid)
		{
			bool lblnResult = false;
			if (_icdoContactTicketHistory == null)
			{
				_icdoContactTicketHistory = new cdoContactTicketHistory();
			}
            if (_icdoContactTicketHistory.SelectRow(new object[1] { Aintcontacttickethistoryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
