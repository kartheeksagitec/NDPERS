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
	public partial class busAppointmentSchedule : busExtendBase
    {
		public busAppointmentSchedule()
		{

		} 

		private cdoAppointmentSchedule _icdoAppointmentSchedule;
		public cdoAppointmentSchedule icdoAppointmentSchedule
		{
			get
			{
				return _icdoAppointmentSchedule;
			}

			set
			{
				_icdoAppointmentSchedule = value;
			}
		}

		public bool FindAppointmentSchedule(int Aintappointmentscheduleid)
		{
			bool lblnResult = false;
			if (_icdoAppointmentSchedule == null)
			{
				_icdoAppointmentSchedule = new cdoAppointmentSchedule();
			}
			if (_icdoAppointmentSchedule.SelectRow(new object[1] { Aintappointmentscheduleid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
