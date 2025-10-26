#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
	public class doBatchSchedule : doBase
	{
		public doBatchSchedule() : base()
		{
		}
		private int _batch_schedule_id;
		public int batch_schedule_id
		{
			get
			{
				return _batch_schedule_id;
			}

			set
			{
				_batch_schedule_id = value;
			}
		}

		private int _step_no;
		public int step_no
		{
			get
			{
				return _step_no;
			}

			set
			{
				_step_no = value;
			}
		}

		private string _step_name;
		public string step_name
		{
			get
			{
				return _step_name;
			}

			set
			{
				_step_name = value;
			}
		}

		private string _step_description;
		public string step_description
		{
			get
			{
				return _step_description;
			}

			set
			{
				_step_description = value;
			}
		}

		private int _frequency_in_days;
		public int frequency_in_days
		{
			get
			{
				return _frequency_in_days;
			}

			set
			{
				_frequency_in_days = value;
			}
		}

		private int _frequency_in_months;
		public int frequency_in_months
		{
			get
			{
				return _frequency_in_months;
			}

			set
			{
				_frequency_in_months = value;
			}
		}

		private DateTime _next_run_date;
		public DateTime next_run_date
		{
			get
			{
				return _next_run_date;
			}

			set
			{
				_next_run_date = value;
			}
		}

		private string _step_parameters;
		public string step_parameters
		{
			get
			{
				return _step_parameters;
			}

			set
			{
				_step_parameters = value;
			}
		}

		private string _active_flag;
		public string active_flag
		{
			get
			{
				return _active_flag;
			}

			set
			{
				_active_flag = value;
			}
		}

		private string _requires_transaction_flag;
		public string requires_transaction_flag
		{
			get
			{
				return _requires_transaction_flag;
			}

			set
			{
				_requires_transaction_flag = value;
			}
		}

        private string _email_notification;
        public string email_notification
        {
            get
            {
                return _email_notification;
            }

            set
            {
                _email_notification = value;
            }
        }

        private string _order_no;
        public string order_no
        {
            get
            {
                return _order_no;
            }

            set
            {
                _order_no = value;
            }
        }

        private string _cutoff_start;
        public string cutoff_start
        {
            get
            {
                return _cutoff_start;
            }

            set
            {
                _cutoff_start = value;
            }
        }

        private string _cutoff_end;
        public string cutoff_end
        {
            get
            {
                return _cutoff_end;
            }

            set
            {
                _cutoff_end = value;
            }
        }

        private string _run_in_batch_flag;
        public string run_in_batch_flag
        {
            get
            {
                return _run_in_batch_flag;
            }

            set
            {
                _run_in_batch_flag = value;
            }
        }

        private string _run_in_service_flag;
        public string run_in_service_flag
        {
            get
            {
                return _run_in_service_flag;
            }

            set
            {
                _run_in_service_flag = value;
            }
        }
    }
}

