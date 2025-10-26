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
    public class doPaymentSchedule : doBase
    {
         
         public doPaymentSchedule() : base()
         {
         }
		private int _payment_schedule_id;
		public int payment_schedule_id
		{
			get
			{
				return _payment_schedule_id;
			}

			set
			{
				_payment_schedule_id = value;
			}
		}

		private DateTime _payment_date;
		public DateTime payment_date
		{
			get
			{
				return _payment_date;
			}

			set
			{
				_payment_date = value;
			}
		}

		private DateTime _process_date;
		public DateTime process_date
		{
			get
			{
				return _process_date;
			}

			set
			{
				_process_date = value;
			}
		}

		private DateTime _process_start_time;
		public DateTime process_start_time
		{
			get
			{
				return _process_start_time;
			}

			set
			{
				_process_start_time = value;
			}
		}

		private DateTime _process_end_time;
		public DateTime process_end_time
		{
			get
			{
				return _process_end_time;
			}

			set
			{
				_process_end_time = value;
			}
		}

		private string _backup_table_prefix;
		public string backup_table_prefix
		{
			get
			{
				return _backup_table_prefix;
			}

			set
			{
				_backup_table_prefix = value;
			}
		}

		private DateTime _effective_date;
		public DateTime effective_date
		{
			get
			{
				return _effective_date;
			}

			set
			{
				_effective_date = value;
			}
		}

		private int _schedule_type_id;
		public int schedule_type_id
		{
			get
			{
				return _schedule_type_id;
			}

			set
			{
				_schedule_type_id = value;
			}
		}

		private string _schedule_type_description;
		public string schedule_type_description
		{
			get
			{
				return _schedule_type_description;
			}

			set
			{
				_schedule_type_description = value;
			}
		}

		private string _schedule_type_value;
		public string schedule_type_value
		{
			get
			{
				return _schedule_type_value;
			}

			set
			{
				_schedule_type_value = value;
			}
		}

		private int _status_id;
		public int status_id
		{
			get
			{
				return _status_id;
			}

			set
			{
				_status_id = value;
			}
		}

		private string _status_description;
		public string status_description
		{
			get
			{
				return _status_description;
			}

			set
			{
				_status_description = value;
			}
		}

		private string _status_value;
		public string status_value
		{
			get
			{
				return _status_value;
			}

			set
			{
				_status_value = value;
			}
		}

		private int _action_status_id;
		public int action_status_id
		{
			get
			{
				return _action_status_id;
			}

			set
			{
				_action_status_id = value;
			}
		}

		private string _action_status_description;
		public string action_status_description
		{
			get
			{
				return _action_status_description;
			}

			set
			{
				_action_status_description = value;
			}
		}

		private string _action_status_value;
		public string action_status_value
		{
			get
			{
				return _action_status_value;
			}

			set
			{
				_action_status_value = value;
			}
		}

		private string _check_message;
		public string check_message
		{
			get
			{
				return _check_message;
			}

			set
			{
				_check_message = value;
			}
		}

		private string _notes;
		public string notes
		{
			get
			{
				return _notes;
			}

			set
			{
				_notes = value;
			}
		}

    }
}

