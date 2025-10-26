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
    public class doUsecaseFlow : doBase
    {
         public doUsecaseFlow() : base()
         {
         }
		private int _flow_id;
		public int flow_id
		{
			get
			{
				return _flow_id;
			}

			set
			{
				_flow_id = value;
			}
		}

		private int _usecase_id;
		public int usecase_id
		{
			get
			{
				return _usecase_id;
			}

			set
			{
				_usecase_id = value;
			}
		}

		private string _flow_key;
		public string flow_key
		{
			get
			{
				return _flow_key;
			}

			set
			{
				_flow_key = value;
			}
		}

		private string _flow_desc;
		public string flow_desc
		{
			get
			{
				return _flow_desc;
			}

			set
			{
				_flow_desc = value;
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

		private int _increment_id;
		public int increment_id
		{
			get
			{
				return _increment_id;
			}

			set
			{
				_increment_id = value;
			}
		}

		private string _increment_description;
		public string increment_description
		{
			get
			{
				return _increment_description;
			}

			set
			{
				_increment_description = value;
			}
		}

		private string _increment_value;
		public string increment_value
		{
			get
			{
				return _increment_value;
			}

			set
			{
				_increment_value = value;
			}
		}

		private int _owner;
		public int owner
		{
			get
			{
				return _owner;
			}

			set
			{
				_owner = value;
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

		private string _file_location;
		public string file_location
		{
			get
			{
				return _file_location;
			}

			set
			{
				_file_location = value;
			}
		}

    }
}

