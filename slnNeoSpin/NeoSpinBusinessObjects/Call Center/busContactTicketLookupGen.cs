#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public partial class busContactTicketLookup  : busMainBase
	{

		private Collection<busContactTicket> _icolContactTicket;
		public Collection<busContactTicket> icolContactTicket
		{
			get
			{
				return _icolContactTicket;
			}

			set
			{
				_icolContactTicket = value;
			}
		}

		public void LoadContactTicket(DataTable adtbSearchResult)
		{
			_icolContactTicket = GetCollection<busContactTicket>(adtbSearchResult, "icdoContactTicket");
		}
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busContactTicket lbusContactTicket = (busContactTicket)aobjBus;
            lbusContactTicket.LoadPerson();
            lbusContactTicket.LoadOrganization();
            lbusContactTicket.icdoContactTicket.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lbusContactTicket.icdoContactTicket.org_id);
        }
	}
}
