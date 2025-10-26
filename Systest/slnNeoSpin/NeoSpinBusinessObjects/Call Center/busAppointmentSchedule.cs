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
	public partial class busAppointmentSchedule : busExtendBase
    {
        public bool FindAppointmentScheduleByContactTicket(int Aintcontactticketid)
        {
            if (_icdoAppointmentSchedule == null)
            {
                _icdoAppointmentSchedule = new cdoAppointmentSchedule();
            }
            DataTable ldtb = Select<cdoAppointmentSchedule>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoAppointmentSchedule.LoadData(ldtb.Rows[0]);
                return true;
            }
            return false;

        }

        public busContactTicket ibusContactTicket { get; set; }

	}
}
