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
	public class doCode : doBase
	{
		public doCode() : base()
		{
		}
		private Int32 _code_id;
		public Int32 code_id
		{
			get
			{
				return _code_id;
			}

			set
			{
				_code_id = value;
			}
		}

		private string _description;
		public string description
		{
			get
			{
				return _description;
			}

			set
			{
				_description = value;
			}
		}

		private string _data1_caption;
		public string data1_caption
		{
			get
			{
				return _data1_caption;
			}

			set
			{
				_data1_caption = value;
			}
		}

		private string _data1_type;
		public string data1_type
		{
			get
			{
				return _data1_type;
			}

			set
			{
				_data1_type = value;
			}
		}

		private string _data2_caption;
		public string data2_caption
		{
			get
			{
				return _data2_caption;
			}

			set
			{
				_data2_caption = value;
			}
		}

		private string _data2_type;
		public string data2_type
		{
			get
			{
				return _data2_type;
			}

			set
			{
				_data2_type = value;
			}
		}

		private string _data3_caption;
		public string data3_caption
		{
			get
			{
				return _data3_caption;
			}

			set
			{
				_data3_caption = value;
			}
		}

		private string _data3_type;
		public string data3_type
		{
			get
			{
				return _data3_type;
			}

			set
			{
				_data3_type = value;
			}
		}

		private string _first_lookup_item;
		public string first_lookup_item
		{
			get
			{
				return _first_lookup_item;
			}

			set
			{
				_first_lookup_item = value;
			}
		}

		private string _first_maintenance_item;
		public string first_maintenance_item
		{
			get
			{
				return _first_maintenance_item;
			}

			set
			{
				_first_maintenance_item = value;
			}
		}

		private string _comments;
		public string comments
		{
			get
			{
				return _comments;
			}

			set
			{
				_comments = value;
			}
		}

		private string _legacy_code_id;
		public string legacy_code_id
		{
			get
			{
				return _legacy_code_id;
			}

			set
			{
				_legacy_code_id = value;
			}
		}

	}
}

