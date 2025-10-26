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
    public class doMergeEmployerHeader : doBase
    {
         
         public doMergeEmployerHeader() : base()
         {
         }
		private int _merge_employer_header_id;
		public int merge_employer_header_id
		{
			get
			{
				return _merge_employer_header_id;
			}

			set
			{
				_merge_employer_header_id = value;
			}
		}

		private int _from_employer_id;
		public int from_employer_id
		{
			get
			{
				return _from_employer_id;
			}

			set
			{
				_from_employer_id = value;
			}
		}

		private int _to_employer_id;
		public int to_employer_id
		{
			get
			{
				return _to_employer_id;
			}

			set
			{
				_to_employer_id = value;
			}
		}

		private DateTime _effective_date;
		public DateTime effective_date
		{
			get
			{
				return _effective_date;
			}

			set
			{
				_effective_date = value;
			}
		}

		private int _merge_status_id;
		public int merge_status_id
		{
			get
			{
				return _merge_status_id;
			}

			set
			{
				_merge_status_id = value;
			}
		}

		private string _merge_status_description;
		public string merge_status_description
		{
			get
			{
				return _merge_status_description;
			}

			set
			{
				_merge_status_description = value;
			}
		}

		private string _merge_status_value;
		public string merge_status_value
		{
			get
			{
				return _merge_status_value;
			}

			set
			{
				_merge_status_value = value;
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

