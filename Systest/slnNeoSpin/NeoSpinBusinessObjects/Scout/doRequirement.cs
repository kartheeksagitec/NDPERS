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
    public class doRequirement : doBase
    {
         
         public doRequirement() : base()
         {
         }
		private int _requirement_id;
		public int requirement_id
		{
			get
			{
				return _requirement_id;
			}

			set
			{
				_requirement_id = value;
			}
		}

		private string _requirement_key;
		public string requirement_key
		{
			get
			{
				return _requirement_key;
			}

			set
			{
				_requirement_key = value;
			}
		}

		private int _parent_requirement_id;
		public int parent_requirement_id
		{
			get
			{
				return _parent_requirement_id;
			}

			set
			{
				_parent_requirement_id = value;
			}
		}

		private string _requirement_short_desc;
		public string requirement_short_desc
		{
			get
			{
				return _requirement_short_desc;
			}

			set
			{
				_requirement_short_desc = value;
			}
		}

		private string _requirement_desc;
		public string requirement_desc
		{
			get
			{
				return _requirement_desc;
			}

			set
			{
				_requirement_desc = value;
			}
		}

		private int _requirement_type_id;
		public int requirement_type_id
		{
			get
			{
				return _requirement_type_id;
			}

			set
			{
				_requirement_type_id = value;
			}
		}

		private string _requirement_type_description;
		public string requirement_type_description
		{
			get
			{
				return _requirement_type_description;
			}

			set
			{
				_requirement_type_description = value;
			}
		}

		private string _requirement_type_value;
		public string requirement_type_value
		{
			get
			{
				return _requirement_type_value;
			}

			set
			{
				_requirement_type_value = value;
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

		private int _category_id;
		public int category_id
		{
			get
			{
				return _category_id;
			}

			set
			{
				_category_id = value;
			}
		}

		private string _category_description;
		public string category_description
		{
			get
			{
				return _category_description;
			}

			set
			{
				_category_description = value;
			}
		}

		private string _category_value;
		public string category_value
		{
			get
			{
				return _category_value;
			}

			set
			{
				_category_value = value;
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

		private int _priority;
		public int priority
		{
			get
			{
				return _priority;
			}

			set
			{
				_priority = value;
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

