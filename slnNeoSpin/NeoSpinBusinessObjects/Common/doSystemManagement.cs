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
      public class doSystemManagement : doBase
      {
		public doSystemManagement() : base()
		{
		}
		private Int32 _system_management_id;
		public Int32 system_management_id
		{
			get
			{
				return _system_management_id;
			}

			set
			{
				_system_management_id = value;
			}
		}
		private Int32 _current_cycle_no;
		public Int32 current_cycle_no
		{
			get
			{
				return _current_cycle_no;
			}

			set
			{
				_current_cycle_no = value;
			}
		}
		private Int32 _region_id;
		public Int32 region_id
		{
			get
			{
				return _region_id;
			}

			set
			{
				_region_id = value;
			}
		}
		private string _region_description;
		public string region_description
		{
			get
			{
				return _region_description;
			}

			set
			{
				_region_description = value;
			}
		}
		private string _region_value;
		public string region_value
		{
			get
			{
				return _region_value;
			}

			set
			{
				_region_value = value;
			}
		}
		private Int32 _system_availability_id;
		public Int32 system_availability_id
		{
			get
			{
				return _system_availability_id;
			}

			set
			{
				_system_availability_id = value;
			}
		}
		private string _system_availability_description;
		public string system_availability_description
		{
			get
			{
				return _system_availability_description;
			}

			set
			{
				_system_availability_description = value;
			}
		}
		private string _system_availability_value;
		public string system_availability_value
		{
			get
			{
				return _system_availability_value;
			}

			set
			{
				_system_availability_value = value;
			}
		}
		private DateTime _batch_date;
		public DateTime batch_date
		{
			get
			{
				return _batch_date;
			}

			set
			{
				_batch_date = value;
			}
		}
		private string _base_directory;
		public string base_directory
		{
			get
			{
				return _base_directory;
			}

			set
			{
				_base_directory = value;
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

        public string system_flag { get; set; }

        public string data1 { get; set; }

        public string data2 { get; set; }

        public string use_application_date { get; set; }
    }
}

