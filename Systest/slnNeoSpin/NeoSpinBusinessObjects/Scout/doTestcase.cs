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
    public class doTestcase : doBase
    {
         
         public doTestcase() : base()
         {
         }
		private int _testcase_id;
		public int testcase_id
		{
			get
			{
				return _testcase_id;
			}

			set
			{
				_testcase_id = value;
			}
		}

		private string _testcase_key;
		public string testcase_key
		{
			get
			{
				return _testcase_key;
			}

			set
			{
				_testcase_key = value;
			}
		}

		private string _testcase_desc;
		public string testcase_desc
		{
			get
			{
				return _testcase_desc;
			}

			set
			{
				_testcase_desc = value;
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

