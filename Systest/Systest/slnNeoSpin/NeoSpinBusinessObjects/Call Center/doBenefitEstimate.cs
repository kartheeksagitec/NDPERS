#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doBenefitEstimate : doBase
    {
         
         public doBenefitEstimate() : base()
         {
         }
		private int _benefit_estimate_id;
		public int benefit_estimate_id
		{
			get
			{
				return _benefit_estimate_id;
			}

			set
			{
				_benefit_estimate_id = value;
			}
		}

		private int _contact_ticket_id;
		public int contact_ticket_id
		{
			get
			{
				return _contact_ticket_id;
			}

			set
			{
				_contact_ticket_id = value;
			}
		}

		private DateTime _termination_date;
		public DateTime termination_date
		{
			get
			{
				return _termination_date;
			}

			set
			{
				_termination_date = value;
			}
		}

		private DateTime _retirement_effective_date;
		public DateTime retirement_effective_date
		{
			get
			{
				return _retirement_effective_date;
			}

			set
			{
				_retirement_effective_date = value;
			}
		}

		private string _convert_unused_sick_leave_flag;
		public string convert_unused_sick_leave_flag
		{
			get
			{
				return _convert_unused_sick_leave_flag;
			}

			set
			{
				_convert_unused_sick_leave_flag = value;
			}
		}

		private string _unused_sick_leave_hours;
		public string unused_sick_leave_hours
		{
			get
			{
				return _unused_sick_leave_hours;
			}

			set
			{
				_unused_sick_leave_hours = value;
			}
		}

		private string _send_ret_kit_with_est_flag;
		public string send_ret_kit_with_est_flag
		{
			get
			{
				return _send_ret_kit_with_est_flag;
			}

			set
			{
				_send_ret_kit_with_est_flag = value;
			}
		}

    }
}

