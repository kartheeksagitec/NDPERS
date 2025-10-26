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
	public class doUserDefaults : doBase
	{
		public doUserDefaults() : base()
		{
		}
		private int _user_default_id;
		public int user_default_id
		{
			get
			{
				return _user_default_id;
			}

			set
			{
				_user_default_id = value;
			}
		}

		private int _user_serial_id;
		public int user_serial_id
		{
			get
			{
				return _user_serial_id;
			}

			set
			{
				_user_serial_id = value;
			}
		}

		private string _form_name;
		public string form_name
		{
			get
			{
				return _form_name;
			}

			set
			{
				_form_name = value;
			}
		}

		private string _group_control_id;
		public string group_control_id
		{
			get
			{
				return _group_control_id;
			}

			set
			{
				_group_control_id = value;
			}
		}

		private string _data_field;
		public string data_field
		{
			get
			{
				return _data_field;
			}

			set
			{
				_data_field = value;
			}
		}

		private string _default_value;
		public string default_value
		{
			get
			{
				return _default_value;
			}

			set
			{
				_default_value = value;
			}
		}

	}
}
