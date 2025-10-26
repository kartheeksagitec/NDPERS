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
	public class doProcessLog : doBase
	{
		public doProcessLog() : base()
		{
		}
		private int _process_log_id;
		public int process_log_id
		{
			get
			{
				return _process_log_id;
			}

			set
			{
				_process_log_id = value;
			}
		}

		private int _cycle_no;
		public int cycle_no
		{
			get
			{
				return _cycle_no;
			}

			set
			{
				_cycle_no = value;
			}
		}

		private string _process_name;
		public string process_name
		{
			get
			{
				return _process_name;
			}

			set
			{
				_process_name = value;
			}
		}

		private int _message_type_id;
		public int message_type_id
		{
			get
			{
				return _message_type_id;
			}

			set
			{
				_message_type_id = value;
			}
		}

		private string _message_type_description;
		public string message_type_description
		{
			get
			{
				return _message_type_description;
			}

			set
			{
				_message_type_description = value;
			}
		}

		private string _message_type_value;
		public string message_type_value
		{
			get
			{
				return _message_type_value;
			}

			set
			{
				_message_type_value = value;
			}
		}

		private string _message;
		public string message
		{
			get
			{
				return _message;
			}

			set
			{
				_message = value;
			}
		}

	}
}

