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
    public class doUsecase : doBase
    {
         
         public doUsecase() : base()
         {
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

		private string _usecase_key;
		public string usecase_key
		{
			get
			{
				return _usecase_key;
			}

			set
			{
				_usecase_key = value;
			}
		}

		private string _usecase_desc;
		public string usecase_desc
		{
			get
			{
				return _usecase_desc;
			}

			set
			{
				_usecase_desc = value;
			}
		}

		private string _reference_id;
		public string reference_id
		{
			get
			{
				return _reference_id;
			}

			set
			{
				_reference_id = value;
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

    }
}

