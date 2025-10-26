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
    public class doNewGroup : doBase
    {
         
         public doNewGroup() : base()
         {
         }
		private int _new_group_id;
		public int new_group_id
		{
			get
			{
				return _new_group_id;
			}

			set
			{
				_new_group_id = value;
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

		private string _correspondence_flag;
		public string correspondence_flag
		{
			get
			{
				return _correspondence_flag;
			}

			set
			{
				_correspondence_flag = value;
			}
		}

		private int _emp_information_id;
		public int emp_information_id
		{
			get
			{
				return _emp_information_id;
			}

			set
			{
				_emp_information_id = value;
			}
		}

		private string _emp_information_description;
		public string emp_information_description
		{
			get
			{
				return _emp_information_description;
			}

			set
			{
				_emp_information_description = value;
			}
		}

		private string _emp_information_value;
		public string emp_information_value
		{
			get
			{
				return _emp_information_value;
			}

			set
			{
				_emp_information_value = value;
			}
		}

		private string _department_name;
		public string department_name
		{
			get
			{
				return _department_name;
			}

			set
			{
				_department_name = value;
			}
		}

		private string _department_no;
		public string department_no
		{
			get
			{
				return _department_no;
			}

			set
			{
				_department_no = value;
			}
		}

		private string _contact_name;
		public string contact_name
		{
			get
			{
				return _contact_name;
			}

			set
			{
				_contact_name = value;
			}
		}

		private string _phone_no;
		public string phone_no
		{
			get
			{
				return _phone_no;
			}

			set
			{
				_phone_no = value;
			}
		}

		private string _address;
		public string address
		{
			get
			{
				return _address;
			}

			set
			{
				_address = value;
			}
		}

		private string _city;
		public string city
		{
			get
			{
				return _city;
			}

			set
			{
				_city = value;
			}
		}

		private int _state_id;
		public int state_id
		{
			get
			{
				return _state_id;
			}

			set
			{
				_state_id = value;
			}
		}

		private string _state_description;
		public string state_description
		{
			get
			{
				return _state_description;
			}

			set
			{
				_state_description = value;
			}
		}

		private string _state_value;
		public string state_value
		{
			get
			{
				return _state_value;
			}

			set
			{
				_state_value = value;
			}
		}

		private string _zip_code;
		public string zip_code
		{
			get
			{
				return _zip_code;
			}

			set
			{
				_zip_code = value;
			}
		}

		private string _group_name;
		public string group_name
		{
			get
			{
				return _group_name;
			}

			set
			{
				_group_name = value;
			}
		}

    }
}

